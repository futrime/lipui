using System;
using System.Globalization;
using System.Windows.Data;

namespace LipUI.Helpers;

internal class StringFormatConverter : IMultiValueConverter
{ 

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length == 0)
        {
            return string.Empty;
        }
        var format = values[0] as string;
        if (string.IsNullOrWhiteSpace(format))
        {
            return string.Empty;
        }
        var args = new object[values.Length - 1];
        Array.Copy(values, 1, args, 0, args.Length);
        return string.Format(format, args);
    }
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        return Array.Empty<object>();
    }
}