using System;
using System.Device.Location;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Caliburn.Micro;

using VChat.Services.Maps;

namespace VChat.Converters
{
    public class GeoToImageConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GeoCoordinate)
            {
                var point = (GeoCoordinate)value;

                return IoC.Get<IMapService>().GetPreview(point.Latitude, point.Longitude);
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}