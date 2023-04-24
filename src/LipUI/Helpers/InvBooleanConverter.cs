using System;
using System.Globalization;
using System.Windows.Data;

namespace LipUI.Helpers;

internal class InvBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is true)
        {
            return false;
        }
        return true;
    }

    public object ConvertBack(object value, Type targetType, object parameter,
        CultureInfo culture)
    {
        if (value is true)
        {
            return false;
        }
        return true;
    }
}