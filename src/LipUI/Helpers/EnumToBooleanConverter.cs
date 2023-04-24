using System;
using System.Globalization;
using System.Windows.Data;
using Wpf.Ui.Appearance;

namespace LipUI.Helpers
{
    internal class EnumToBooleanConverter : IValueConverter
    { 

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is not string enumString)
                throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName");

            if (!Enum.IsDefined(typeof(ThemeType), value))
                throw new ArgumentException("ExceptionEnumToBooleanConverterValueMustBeAnEnum");

            var enumValue = Enum.Parse(typeof(ThemeType), enumString);

            return enumValue.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is not string enumString)
                throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName");

            return Enum.Parse(typeof(ThemeType), enumString);
        }
    }
}
