using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LipUI.Helpers;
internal class StringNotEmptyToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return Visibility.Collapsed;
            }
        }
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter,
        CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}