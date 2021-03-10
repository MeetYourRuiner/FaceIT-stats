using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FaceitStats.WPF.Views.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool invertParameter;
            if (parameter != null)
                invertParameter = bool.Parse((string)parameter);
            else
                invertParameter = false;
            bool boolValue = (bool)value;
            if (invertParameter)
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            else
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
