using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LipUI;

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

public static class Services
{
    public static BitmapImage CreateImageFromBytes(byte[] bytes)
    {
        var image = new BitmapImage();
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
        await ShowInfoBarAsync(ex.GetType().Name, containsStacktrace ? ex.ToString() : ex.Message, severity, interval, barContent);
    }

    public static event Action? WindowClosed;

    internal static void OnWindowClosed() => WindowClosed?.Invoke();
}
