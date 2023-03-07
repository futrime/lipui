using System;
using System.Windows.Data;

namespace LipUI.Helpers;

internal class FeaturedTagConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return value switch
        {
            "featured" => "精华",
            "integration" => "整合",
            "module" => "模块",
            "plugin" => "插件",
            "anti-cheat" => "反作弊",
            _ => value
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}