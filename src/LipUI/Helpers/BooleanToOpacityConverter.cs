using System;
using System.Windows.Data;

namespace LipUI.Helpers;

internal class BooleanToOpacityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is true)
        {
            return parameter;
        }
        return true;
    }

    public object ConvertBack(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}