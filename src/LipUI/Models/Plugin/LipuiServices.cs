#pragma warning disable CA1822

using LipUI.Models.Lip;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace LipUI.Models.Plugin;

public class LipuiServices
{
    internal static LipuiServices Default { get; } = new();

    public async ValueTask<LipConsole?> CreateLipConsoleAsync(XamlRoot xamlRoot, string? workingDir = null)
    {
        var (success, path) = await Main.TryGetLipConsolePathAsync(xamlRoot);
        if (success is false)
            return null;

        workingDir ??= Main.Config.SelectedServer?.WorkingDirectory;

        if (workingDir is null)
            return null;

        return new(path!, workingDir);
    }

    public async ValueTask ShowInfoBarAsync(
        string? title = null,
        string? message = null,
        InfoBarSeverity severity = InfoBarSeverity.Informational,
        TimeSpan interval = default,
        UIElement? barContent = null,
        CancellationToken cancellationToken = default)
        => await InternalServices.ShowInfoBarAsync(title, message, severity, interval, barContent, cancellationToken);

    public async ValueTask ShowInfoBarAsync(
        Exception ex,
        bool containsStacktrace = false,
        InfoBarSeverity severity = InfoBarSeverity.Error,
        TimeSpan interval = default,
        UIElement? barContent = null,
        CancellationToken cancellationToken = default)
        => await InternalServices.ShowInfoBarAsync(ex, containsStacktrace, severity, interval, barContent, cancellationToken);

    public void ShowInfoBar(
        string? title = null,
        string? message = null,
        InfoBarSeverity severity = InfoBarSeverity.Informational,
        TimeSpan interval = default,
        UIElement? barContent = null,
        Action? completed = null,
        CancellationToken cancellationToken = default)
        => InternalServices.ShowInfoBar(title, message, severity, interval, barContent, completed, cancellationToken);

    public void ShowInfoBar(
        Exception ex,
        bool containsStacktrace = false,
        InfoBarSeverity severity = InfoBarSeverity.Error,
        TimeSpan interval = default,
        UIElement? barContent = null,
        Action? completed = null,
        CancellationToken cancellationToken = default)
        => InternalServices.ShowInfoBar(ex, containsStacktrace, severity, interval, barContent, completed, cancellationToken);

    public string GetPluginKey(IPlugin plugin) => PluginSystem.GetPluginKey(plugin);

    public ConfigCollection GetPluginConfig(IPlugin plugin)
        => PluginConfigManager.GetOrCreatePluginConfig(plugin);

    public string? CurrentServerDirectory => Main.Config.SelectedServer?.WorkingDirectory;

    public Config LipuiConfig => Main.Config;

    public DispatcherQueue DispatcherQueue => InternalServices.MainWindow!.DispatcherQueue;

    public LipuiRuntimeInfo LipuiInfo => new()
    {
        Theme = Main.Config.PersonalizationSettings.ColorTheme,
        ApplicationWindow = InternalServices.MainWindow ?? throw new NullReferenceException()
    };

    public static ApplicationTheme ApplicationTheme => InternalServices.ApplicationTheme;
}
