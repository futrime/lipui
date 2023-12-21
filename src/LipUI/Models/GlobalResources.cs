using LipUI.Assets;
using LipUI.Pages;
using Microsoft.UI.Xaml.Media.Imaging;


namespace LipUI.Models;

internal static class GlobalResources
{
    public static readonly BitmapImage GrassBlock = Helpers.CreateImageFromBytes(Images.grass_block);
    public static readonly BitmapImage Netherrack = Helpers.CreateImageFromBytes(Images.netherrack);
    public static readonly BitmapImage Glass = Helpers.CreateImageFromBytes(Images.glass);
}
