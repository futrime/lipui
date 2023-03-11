using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LipUI.Helpers
{
    internal class OpacityToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double opacity)
            {
                if (opacity > 0)
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            //if (value is System.Windows.Visibility visibility)
            //{
            //    if (visibility == System.Windows.Visibility.Visible)
            //    {
            //        return 1.0;
            //    }
            //}
            return Binding.DoNothing;
        }
    }
}
