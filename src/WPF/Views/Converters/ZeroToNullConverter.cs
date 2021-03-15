using System;
using System.Globalization;
using System.Windows.Data;

namespace FaceitStats.WPF.Views.Converters
{
    public class ZeroToNullConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value == 0)
                return null;
            else
                return value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
