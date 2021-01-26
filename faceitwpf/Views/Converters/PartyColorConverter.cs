using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace faceitwpf.Views.Converters
{
    public class PartyColorConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val = (int)value;

            switch (val)
            {
                case -1:
                    return new SolidColorBrush(Colors.Transparent);
                case 0:
                    return new SolidColorBrush(Colors.CornflowerBlue);
                case 1:
                    return new SolidColorBrush(Colors.MediumVioletRed);
                case 2:
                    return new SolidColorBrush(Colors.MediumSeaGreen);
                case 3:
                    return new SolidColorBrush(Colors.MediumPurple);
                case 4:
                    return new SolidColorBrush(Colors.SaddleBrown);
                default:
                    return new SolidColorBrush(Colors.Transparent);
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
