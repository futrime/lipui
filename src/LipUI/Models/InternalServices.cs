using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Windows.ApplicationModel.Resources;
using Windows.UI;

namespace LipUI.Models;

internal static class ResourceExtensions
{
    private static readonly ResourceLoader _resourceLoader = new();

    public static string GetLocalized(this string resourceKey) => _resourceLoader.GetString(resourceKey);
}

internal static class FrameExtensions
{
    public static bool TryGoBack(this Frame frame)
    {
        if (frame.CanGoBack)
        {
            frame.GoBack();
            return true;
        }
        return false;
    }
}

internal static class ColorExtensions
{
    public static Color Invert(this in Color color)
        => Color.FromArgb(color.A, (byte)(255 - color.R), (byte)(255 - color.G), (byte)(255 - color.B));
}

internal static class InternalServices
{
    public static BitmapImage CreateImageFromBytes(byte[] bytes, Action<BitmapImage>? onInit = null)
    {
        var image = new BitmapImage();
        onInit?.Invoke(image);
        using var stream = new MemoryStream(bytes);
        image.SetSource(stream.AsRandomAccessStream());
        return image;
    }

    internal static MainWindow? MainWindow { get; set; }

    public static async ValueTask ShowInfoBarAsync(
        string? title,
        string? message = null,
        InfoBarSeverity severity = InfoBarSeverity.Informational,
        TimeSpan interval = default,
        UIElement? barContent = null)
    {
        if (interval == default)
            interval = TimeSpan.FromSeconds(3);



        if (MainWindow is not null)
            await MainWindow.ShowInfoBarAsync(title, message, severity, interval, barContent);
    }

    public static async ValueTask ShowInfoBarAsync(
        Exception ex,
        bool containsStacktrace = false,
        InfoBarSeverity severity = InfoBarSeverity.Error,
        TimeSpan interval = default,
        UIElement? barContent = null)
    {
        if (interval == default)
            interval = TimeSpan.FromSeconds(5);
        await ShowInfoBarAsync(
            ex.GetType().Name,
            containsStacktrace ? ex.ToString() : ex.Message,
            severity,
            interval,
            barContent);
    }

    public static void ShowInfoBar(
        string? title,
        string? message = null,
        InfoBarSeverity severity = InfoBarSeverity.Informational,
        TimeSpan interval = default,
        UIElement? barContent = null,
        Action? completed = null)
    {
        if (interval == default)
            interval = TimeSpan.FromSeconds(3);

        MainWindow?.ShowInfoBar(title, message, severity, interval, barContent, completed);
    }

    public static void ShowInfoBar(
        Exception ex,
        bool containsStacktrace = false,
        InfoBarSeverity severity = InfoBarSeverity.Error,
        TimeSpan interval = default,
        UIElement? barContent = null,
        Action? completed = null)
    {
        if (interval == default)
            interval = TimeSpan.FromSeconds(5);
        ShowInfoBar(ex.GetType().Name,
            containsStacktrace ? ex.ToString() : ex.Message,
            severity,
            interval,
            barContent,
            completed);
    }

    public static event Action? WindowClosed;

    internal static void OnWindowClosed() => WindowClosed?.Invoke();

    public static ApplicationTheme ApplicationTheme { get; internal set; }
}
