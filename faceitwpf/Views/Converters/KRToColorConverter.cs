using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace faceitwpf.Views.Converters
{
    public class KRToColorConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = (double)value;

            if (val >= 0.85) return new SolidColorBrush(Colors.RoyalBlue);
            else if (val >= 0.75) return new SolidColorBrush(Colors.Green);
            else if (val > 0.65) return new SolidColorBrush(Colors.Yellow);
            else return new SolidColorBrush(Colors.Red);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
