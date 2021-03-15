using System;
using System.Globalization;
using System.Windows.Data;

namespace FaceitStats.WPF.Views.Converters
{
    public class KDRConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double param = (double)parameter;
            double val = (double)value;
            if (param != 1)
                if (val >= 0.9) return 3;
                else if (val >= 0.8) return 2;
                else if (val > 0.65) return 1;
                else return 0;
            else
                return (double)value < 1;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
