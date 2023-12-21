using LipUI.Language;
using LipUI.Models.Lip;
using LipUI.VIews;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace LipUI.Models;

internal static class Main
{
    static Main() => Initialize();

    private static readonly object _lock = new();


    public static Config Config { get; private set; }

    //public static I18nInstance I18N { get; private set; }

    public static string WorkingDirectory { get; private set; }


    [MemberNotNull(nameof(Config), /*nameof(I18N),*/ nameof(WorkingDirectory))]
    private static void Initialize()
    {
        InitializeWorkingDir();
        InitializeConfig();
        //I18N = new();
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
        }
    }

    public static async ValueTask<Lip.LipConsole?> CreateLipConsole(XamlRoot xamlRoot)
    {
        var server = Config.SelectedServer;
        if (server is null)
            return null;

        var (success, path) = await TryGetLipConsolePathAsync(xamlRoot);
        if (success is false)
            return null;

        return new Lip.LipConsole(path!, server.WorkingDirectory);
    }

    public static async ValueTask SaveConfigAsync()
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
        await Task.Yield();
    }

    public static async ValueTask<(bool, string)> TryGetLipConsolePathAsync(XamlRoot xamlRoot)
    {
        if (File.Exists(Config.Settings.LipPath))
            return (true, Config.Settings.LipPath);
        else
        {
            var dialog = new ContentDialog()
            {
                XamlRoot = xamlRoot,
                Content = new LipInstallerView()
            };

            await dialog.ShowAsync();


            return File.Exists(Config.Settings.LipPath) ?
                (true, Config.Settings.LipPath) :
                (false, string.Empty);
        }
    }
}
