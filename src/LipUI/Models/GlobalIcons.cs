using LipUI.Assets;
using Microsoft.UI.Xaml.Media.Imaging;


namespace LipUI.Models;

internal static class GlobalIcons
{
    public static readonly BitmapImage GrassBlock = Services.CreateImageFromBytes(Images.grass_block);
    public static readonly BitmapImage Netherrack = Services.CreateImageFromBytes(Images.netherrack);
    public static readonly BitmapImage Glass = Services.CreateImageFromBytes(Images.glass);
}
