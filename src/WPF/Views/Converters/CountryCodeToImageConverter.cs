using System;
using System.Globalization;
using System.Windows.Data;

namespace FaceitStats.WPF.Views.Converters
{
    class CountryCodeToImageConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string code = (string)value;
            if (code == "en")
                return $"https://flagcdn.com/h20/gb.png";
            if (code == "zh")
                return $"https://flagcdn.com/h20/cn.png";
            else
                return $"https://flagcdn.com/h20/{code}.png";
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
