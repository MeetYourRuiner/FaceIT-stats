using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace faceitwpf.Views.Converters
{
    public class WRToColorConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = (double)value;

            if (val >= 0.5) return new SolidColorBrush(Colors.Green);
            else return new SolidColorBrush(Colors.Red);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
