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


    private static Config? config;

    public static Config Config
    {
        get => config ?? throw new NullReferenceException();
        private set
        {
            if (config is not null)
                config.PropertyChanged -= ConfigPropertyChanged;

            config = value;
            config!.PropertyChanged += ConfigPropertyChanged;
        }
    }

    public static string WorkingDirectory { get; private set; }

    public static bool ColorsFirstInitSign = false;


    [MemberNotNull(nameof(Config), nameof(WorkingDirectory))]
    private static void Initialize()
    {
        InitializeWorkingDir();
        InitializeConfig();
        InternalServices.WindowClosed += SaveConfig;
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
            var str = File.ReadAllText(path);
            Config = JsonSerializer.Deserialize<Config>(str) ?? throw new NullReferenceException();
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

    private static bool configChanged = false;
    private static uint configEditCount = 0;
    private static readonly object _lock = new();
    private static bool saving = false;
    private static bool saveRequesting = false;

    private static void ConfigPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        configChanged = true;
        configEditCount++;

        if (configEditCount >= 0xf)
            Task.Run(SaveConfig);
    }

    internal static void SaveConfig()
    {
        if (saving)
        {
            saveRequesting = true;
            return;
        }

        lock (_lock)
        {
            saving = true;
            if (configChanged)
            {
                var path = Path.Combine(WorkingDirectory, DefaultSettings.ConfigFileName);
                if (File.Exists(path)) File.Delete(path);

                using var file = File.Create(path);
                using var writer = new StreamWriter(file);

                writer.Write(Config.Serialize());

                configChanged = false;
                configEditCount = 0;
            }

            if (saveRequesting)
            {
                saveRequesting = false;
                SaveConfig();
            }

            saving = false;
        }
    }
}