using LipUI.Models.Lip;
using LipUI.Pages.LipExecutionPanel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace LipUI.Models;

internal static class Main
{
    static Main() => Initialize();



    public static Config Config { get; private set; }

    public static string WorkingDirectory { get; private set; }

    public static bool ColorsFirstInitSign = false;


    [MemberNotNull(nameof(Config), nameof(WorkingDirectory))]
    private static void Initialize()
    {
        InitializeWorkingDir();
        InitializeConfig();
    }

    [MemberNotNull(nameof(WorkingDirectory))]
    private static void InitializeWorkingDir()
    {
        var currentDir = new FileInfo(Environment.ProcessPath!).Directory!.FullName;

        var path = Path.Combine(currentDir, DefaultSettings.DataDirectory);
        if (Directory.Exists(path) is false)
        {
            Directory.CreateDirectory(path);
        }
        WorkingDirectory = path;
    }

    [MemberNotNull(nameof(Config))]
    private static void InitializeConfig()
    {
        var path = Path.Combine(WorkingDirectory, DefaultSettings.ConfigFileName);
        if (File.Exists(path))
        {
            using var config = File.OpenRead(path);
            using var reader = new StreamReader(config);
            Config = JsonSerializer.Deserialize<Config>(reader.ReadToEnd()) ?? throw new NullReferenceException();
        }
        else
        {
            Config = new Config();
            Config.PersonalizationSettings.ResetColors = true;
        }
    }

    public static async ValueTask<LipConsole?> CreateLipConsole(XamlRoot xamlRoot, string? workingDir = null)
    {
        var (success, path) = await TryGetLipConsolePathAsync(xamlRoot);
        if (success is false)
            return null;

        var server = Config.SelectedServer;
        if (server is null)
            return null;

        return new LipConsole(path!, workingDir is null ? server.WorkingDirectory : workingDir);
    }


    private static readonly object _lock = new();
    public static async ValueTask SaveConfigAsync()
    {
        await Task.Run(() =>
        {
            lock (_lock)
            {
                var path = Path.Combine(WorkingDirectory, DefaultSettings.ConfigFileName);
                if (File.Exists(path)) File.Delete(path);

                using var file = File.Create(path);
                using var writer = new StreamWriter(file);


                writer.Write(JsonSerializer.Serialize(Config, new JsonSerializerOptions()
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                }));
            }
        });
    }

    public static async ValueTask<(bool, string)> TryGetLipConsolePathAsync(XamlRoot xamlRoot)
    {
        if (File.Exists(Config.GeneralSettings.LipPath))
            return (true, Config.GeneralSettings.LipPath);
        else
        {
            var dialog = new ContentDialog()
            {
                XamlRoot = xamlRoot,
                Content = new LipInstallerView()
            };

            await dialog.ShowAsync();


            return File.Exists(Config.GeneralSettings.LipPath) ?
                (true, Config.GeneralSettings.LipPath) :
                (false, string.Empty);
        }
    }
}
