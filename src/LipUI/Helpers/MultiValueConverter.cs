using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LipUI.Helpers;
internal class MultiValueConverter : IMultiValueConverter
{
    public object? Convert(object[]? values, Type targetType, object parameter, CultureInfo culture)
    {
        // Check if values are valid
        if (values == null ) return null;
        if (values.Length == 0) return null;
        return values.ToArray();
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        return (object[])value;
    }
}
