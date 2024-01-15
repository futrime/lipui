using LipUI.Assets;
using Microsoft.UI.Xaml.Media.Imaging;


namespace LipUI.Models;

internal static class GlobalIcons
{
    public static readonly BitmapImage GrassBlock = InternalServices.CreateImageFromBytes(Images.grass_block);
    public static readonly BitmapImage Netherrack = InternalServices.CreateImageFromBytes(Images.netherrack);
    public static readonly BitmapImage Glass = InternalServices.CreateImageFromBytes(Images.glass);
}
