using System;
using System.Globalization;
using System.Windows.Data;

namespace FaceitStats.WPF.Views.Converters
{
    class LevelToImageConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int level = (int)value;
            return $"/FaceitStats.WPF;component/Resources/lvl{level}.png";
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
