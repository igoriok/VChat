using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using VChat.ViewModels.Data;

namespace VChat.Converters
{
    public class AttachmentToDataTemplateConverter : IValueConverter
    {
        public DataTemplate PhotoTemplate { get; set; }

        public DataTemplate AudioTemplate { get; set; }

        public DataTemplate VideoTemplate { get; set; }

        public DataTemplate DocumentTemplate { get; set; }

        public DataTemplate WallTemplate { get; set; }

        #region IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PhotoViewModel)
                return PhotoTemplate;

            if (value is AudioViewModel)
                return AudioTemplate;

            if (value is VideoViewModel)
                return VideoTemplate;

            if (value is DocumentViewModel)
                return DocumentTemplate;

            if (value is WallViewModel)
                return WallTemplate;

            return DependencyProperty.UnsetValue;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}