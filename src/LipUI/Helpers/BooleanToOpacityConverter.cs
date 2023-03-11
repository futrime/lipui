using System;
using System.Globalization;
using System.Windows.Data;

namespace LipUI.Helpers;

internal class BooleanToOpacityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is true)
        {
            if (double.TryParse(parameter.ToString(), out var pd))
            {
                return pd;
            }
            return .7;
        }
        return true;
    }

    public object ConvertBack(object value, Type targetType, object parameter,
        CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}