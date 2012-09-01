using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VChat.Converters
{
    public class BooleanToValueConverter : IValueConverter
    {
        public object TrueValue { get; set; }

        public object FalseValue { get; set; }

        #region IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return System.Convert.ToBoolean(value) ? TrueValue : FalseValue;
            }

            return DependencyProperty.UnsetValue;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}