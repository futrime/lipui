#pragma warning disable CA1822

using LipUI.Models.Lip;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace LipUI.Models.Plugin;

public class LipuiServices
{
    public async ValueTask<LipConsole?> CreateLipConsoleAsync(XamlRoot xamlRoot, string workingDir)
    {
        var (success, path) = await Main.TryGetLipConsolePathAsync(xamlRoot);
        if (success)
        {
            return new(path!, workingDir);
        }
        return null;
    }

    public async ValueTask ShowInfoBarAsync(
        string? title,
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
        string? title,
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

    public string? CurrentServerDirectory => Main.Config.SelectedServer?.WorkingDirectory;
}
