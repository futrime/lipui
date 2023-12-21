using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LipUI.Models;

internal static class ServerIcon
{
    [Flags] public enum IconType { None, Empty, Preview, Default, Custom }

    public static async ValueTask<BitmapImage> GetIcon(string? version, string? iconPath)
    {
        IconType type;

        if (iconPath is null)
        {
            if (string.IsNullOrEmpty(version))
                type = IconType.Empty;
            else if (version.Contains("preview"))
                type = IconType.Preview;
            else
                type = IconType.Default;
        }
        else type = IconType.Custom;

        switch (type)
        {
            case IconType.Empty: return GlobalResources.Glass;
            case IconType.Preview: return GlobalResources.Netherrack;
            case IconType.Default: return GlobalResources.GrassBlock;
            case IconType.Custom:
                {
                    if (File.Exists(iconPath))
                    {
                        try
                        {
                            using var file = File.OpenRead(iconPath);
                            var bitmap = new BitmapImage();
                            await bitmap.SetSourceAsync(file.AsRandomAccessStream());
                            return bitmap;
                        }
                        catch (Exception)
                        {
                            return await GetIcon(version, null);
                        }
                    }
                    else return await GetIcon(version, null);

                }

            case IconType.None:
            default: throw new InvalidProgramException("???");

        }
    }

    public static async ValueTask<BitmapImage> GetIcon(ServerInstance? server)
        => server is null ? await GetIcon(null, null) : await GetIcon(server.Version, server.Icon);

}
