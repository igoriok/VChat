using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VChat.Converters
{
    public class InvertBooleanToVisibilityConverter : IValueConverter
    {
        #region IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return (bool)value ? Visibility.Collapsed : Visibility.Visible;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
            {
                return (Visibility)value != Visibility.Visible;
            }

            return DependencyProperty.UnsetValue;
        }

        #endregion
    }
}