using System;
using System.Windows.Data;

namespace LipUI.Helpers;

internal class BooleanToVisibilityHiddenConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is true)
        {
            return System.Windows.Visibility.Visible;
        }
        return System.Windows.Visibility.Hidden;
    }

    public object ConvertBack(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}