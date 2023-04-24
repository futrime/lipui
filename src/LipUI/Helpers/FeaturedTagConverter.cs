using System;
using System.Globalization;
using System.Windows.Data;

namespace LipUI.Helpers;

internal class FeaturedTagConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            "featured" => Global.I18N.RegistryTagFeatured,
            "integration" => Global.I18N.RegistryTagIntegration,
            "module" => Global.I18N.RegistryTagModule,
            "plugin" => Global.I18N.RegistryTagPlugin,
            "anti-cheat" => Global.I18N.RegistryTagAntiCheat,
            _ => value
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter,
        CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}