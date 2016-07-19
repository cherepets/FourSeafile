using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace FourSeafile.Converters
{
    public class BoolToVisibility : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, string language)
            => System.Convert.ToBoolean(value) ?
                Visibility.Visible :
                Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => (Visibility)value == Visibility.Visible;
    }
}
