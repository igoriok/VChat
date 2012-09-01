using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace VChat.Converters
{
    public class BooleanToImageSourceConverter : IValueConverter
    {
        public ImageSource TrueImage { get; set; }

        public ImageSource FalseImage { get; set; }

        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return System.Convert.ToBoolean(value) ? TrueImage : FalseImage;
            }

            return FalseImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}