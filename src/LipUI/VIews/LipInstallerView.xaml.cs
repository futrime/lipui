using CommunityToolkit.WinUI;
using LipUI.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI.VIews;

internal sealed partial class LipInstallerView : UserControl
{
    public LipInstallerView()
    {
        InitializeComponent();
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        InfoText.Text = "lipInstaller$installLip".GetLocalized();
        ProgressRingBorder.Child = new ProgressRing()
        {
            Visibility = Visibility.Collapsed,
            IsActive = false,
            Height = 48,
            Width = 48,
        };
    }

    private event Action<int>? DownloadProgressChanged;

    private record InstallerInfo(string AssetUrl, string Tag, string PublishedTime, long FileSize);

    private static async ValueTask<InstallerInfo> RequestLipInstallerInfo()
    {
        using var client = new HttpClient();

        // Fetch latest release from GitHub API
        client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
        client.DefaultRequestHeaders.Add("User-Agent", "request"); // GitHub requires a User-Agent header
        var response = await client.GetStringAsync(DefaultSettings.LipPortableUrl);
        var data = JsonDocument.Parse(response);
        var rootElement = data.RootElement;

        // Find asset with filename ending with "-windows-amd64-setup.exe"
        var targetFilename = "-windows-amd64.zip";
        string assetUrl = string.Empty, tag = string.Empty, publishedTime = string.Empty;
        long size = 0;

        tag = rootElement.GetProperty("tag_name").GetString()!;
        publishedTime = rootElement.GetProperty("published_at").GetString()!;

        foreach (var asset in rootElement.GetProperty("assets").EnumerateArray())
        {
            var assetFileName = asset.GetProperty("name").GetString()!;
            if (assetFileName.EndsWith(targetFilename))
            {
                assetUrl = asset.GetProperty("browser_download_url").GetString()!;
                size = asset.GetProperty("size").GetInt64();
                break;
            }
        }
        return new(assetUrl, tag, publishedTime, size);
    }

    private async ValueTask<byte[]> DownloadLipPortable(InstallerInfo info)
    {
        var zipFilePath = Path.Combine(Main.WorkingDirectory, "lip.zip");
        var buffer = new byte[1024];
        var output = File.Create(zipFilePath);

        using var client = new HttpClient(
            new HttpClientHandler() { ClientCertificateOptions = ClientCertificateOption.Automatic })
        {
            Timeout = TimeSpan.FromSeconds(2),
            DefaultRequestHeaders = { ExpectContinue = false }
        };

        HttpResponseMessage response;

        try
        {
            response = await client.GetAsync(info.AssetUrl, HttpCompletionOption.ResponseHeadersRead);
        }
        catch
        {
            response = await client.GetAsync($"https://github.moeyy.xyz/{info.AssetUrl}");
        }

        var input = await response.Content.ReadAsStreamAsync();

        int totalBytesRead = 0, bytesRead = 0;
        while (true)
        {
            bytesRead = await input.ReadAsync(buffer);
            if (bytesRead is 0) break;
            totalBytesRead += bytesRead;
            DownloadProgressChanged?.Invoke(totalBytesRead);
            await output.WriteAsync(buffer.AsMemory(0, bytesRead));
        }

        output.Dispose();

        return await File.ReadAllBytesAsync(zipFilePath);
    }

    private void ChangeProgressRingMode(InstallerInfo info)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            var progressRing = (ProgressRing)ProgressRingBorder.Child;
            progressRing.IsIndeterminate = false;
            progressRing.Maximum = 100;
            progressRing.Minimum = 0;

            var size = (double)info.FileSize;

            void ProgressChanged(int val)
            {
                if (View_DownloadProgressChanged(info, val))
                    DownloadProgressChanged -= ProgressChanged;
            }

            DownloadProgressChanged += ProgressChanged;
        });
    }

    private bool View_DownloadProgressChanged(InstallerInfo info, int val)
    {
        var progressRing = (ProgressRing)ProgressRingBorder.Child;
        DispatcherQueue.TryEnqueue(() =>
        {
            progressRing.Value = 100 * val / info.FileSize;
            InfoText.Text = $"{(float)val / 0x100000:F2}MB/{(float)info.FileSize / 0x100000:F2}MB";
        });
        return val is 0;
    }

    private async void InstallButton_Click(object sender, RoutedEventArgs e)
    {
        var progressRing = ((ProgressRing)ProgressRingBorder.Child);
        progressRing.IsActive = true;
        progressRing.Visibility = Visibility.Visible;

        CancelButton.IsEnabled = false;
        InstallButton.IsEnabled = false;
        //ViewGrid.Children.Remove(ButtonGrid);

        var info = await RequestLipInstallerInfo();

        ChangeProgressRingMode(info);

        var bytes = await DownloadLipPortable(info);

        UnzipAndCopyLipEXE(bytes);

        DispatcherQueue.TryEnqueue(() => InfoText.Text = "lipInstaller$installCompleted".GetLocalized());

        await Task.Delay(1000);

        var dialog = Parent as ContentDialog;
        dialog!.Hide();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
        => (Parent as ContentDialog)!.Hide();

    private static void UnzipAndCopyLipEXE(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        using var zip = new ZipArchive(stream, ZipArchiveMode.Read);
        ZipArchiveEntry entry = zip.Entries.First(x => Path.GetFileName(x.Name) == "lip.exe");
        entry.ExtractToFile(Main.Config.Settings.LipPath, true);
    }
}
