using Microsoft.UI.Xaml.Media.Imaging;
using System.IO;

namespace LipUI.Pages;

internal static class Helpers
{
    public static BitmapImage CreateImageFromBytes(byte[] bytes)
    {
        var image = new BitmapImage();
        using var stream = new MemoryStream(bytes);
        image.SetSource(stream.AsRandomAccessStream());
        return image;
    }
}
