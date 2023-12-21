using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace ServiceManager
{
    public class BrushColorConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (null == value)
                return null;

            if (ColorConverter.ConvertFromString(value.ToString()) is Color color)
                return new SolidColorBrush(color);

            return DependencyProperty.UnsetValue;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
