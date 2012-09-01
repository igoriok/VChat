using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace VChat.Converters
{
    public class RelativeColorConverter : IValueConverter
    {
        #region IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Color)value;
            var delta = System.Convert.ToInt32(parameter) / 100D;

            return Color.FromArgb(color.A, (byte)(color.R * delta), (byte)(color.G * delta), (byte)(color.B * delta));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Color)value;
            var delta = 100D / System.Convert.ToInt32(parameter);

            return Color.FromArgb(color.A, (byte)(color.R * delta), (byte)(color.G * delta), (byte)(color.B * delta));
        }

        #endregion
    }
}