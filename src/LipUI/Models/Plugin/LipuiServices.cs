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

        if (string.IsNullOrWhiteSpace(workingDir))
            return null;

        return new(path!, workingDir);
    }

    public async ValueTask ShowInfoBarAsync(
        string? title = null,
        string? message = null,
        InfoBarSeverity severity = InfoBarSeverity.Informational,
        TimeSpan interval = default,
        UIElement? barContent = null)
        => await InternalServices.ShowInfoBarAsync(title, message, severity, interval, barContent);

    public async ValueTask ShowInfoBarAsync(
        Exception ex,
        bool containsStacktrace = false,
        InfoBarSeverity severity = InfoBarSeverity.Error,
        TimeSpan interval = default,
        UIElement? barContent = null)
        => await InternalServices.ShowInfoBarAsync(ex, containsStacktrace, severity, interval, barContent);

    public void ShowInfoBar(
        string? title = null,
        string? message = null,
        InfoBarSeverity severity = InfoBarSeverity.Informational,
        TimeSpan interval = default,
        UIElement? barContent = null,
        Action? completed = null)
        => InternalServices.ShowInfoBar(title, message, severity, interval, barContent, completed);

    public void ShowInfoBar(
        Exception ex,
        bool containsStacktrace = false,
        InfoBarSeverity severity = InfoBarSeverity.Error,
        TimeSpan interval = default,
        UIElement? barContent = null,
        Action? completed = null)
        => InternalServices.ShowInfoBar(ex, containsStacktrace, severity, interval, barContent, completed);

    public string GetPluginKey(ILipuiPlugin plugin) => PluginSystem.GetPluginKey(plugin);

    public string? CurrentServerDirectory => Main.Config.SelectedServer?.WorkingDirectory;

    public Config LipuiConfig => Main.Config;

    public DispatcherQueue DispatcherQueue => InternalServices.MainWindow!.DispatcherQueue;
}
