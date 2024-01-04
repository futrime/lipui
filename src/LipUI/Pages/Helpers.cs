using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.IO;
using Microsoft.Windows.ApplicationModel.Resources;
using System.Threading.Tasks;

namespace LipUI.Pages;

public static class ResourceExtensions
{
    private static readonly ResourceLoader _resourceLoader = new();

    public static string GetLocalized(this string resourceKey) => _resourceLoader.GetString(resourceKey);
}

internal static class Helpers
{
    public static BitmapImage CreateImageFromBytes(byte[] bytes)
    {
        var image = new BitmapImage();
        using var stream = new MemoryStream(bytes);
        image.SetSource(stream.AsRandomAccessStream());
        return image;
    }

    public static MainWindow? MainWindow { get; internal set; }

    public static void ShowInfoBar(
        string? title,
        string? message,
        InfoBarSeverity severity,
        TimeSpan interval = default,
        UIElement? barContent = null,
        Action? completed = null)
        => MainWindow?.ShowInfoBar(title, message, severity, interval, barContent, completed);

    public static void ShowInfoBar(
        Exception ex,
        TimeSpan interval = default,
        UIElement? barContent = null,
        Action? completed = null)
    {
        if (interval == default)
            interval = TimeSpan.FromSeconds(5);
        ShowInfoBar(ex.GetType().Name, ex.Message, InfoBarSeverity.Error, interval, barContent, completed);
    }

    public static async ValueTask ShowInfoBarAsync(
        string? title,
        string? message,
        InfoBarSeverity severity,
        TimeSpan interval = default,
        UIElement? barContent = null)
    {
        if (MainWindow is not null)
            await MainWindow.ShowInfoBarAsync(title, message, severity, interval, barContent);
    }

    public static async ValueTask ShowInfoBarAsync(
        Exception ex,
        TimeSpan interval = default,
        UIElement? barContent = null)
    {
        if (interval == default)
            interval = TimeSpan.FromSeconds(5);
        await ShowInfoBarAsync(ex.GetType().Name, ex.Message, InfoBarSeverity.Error, interval, barContent);
    }
}
