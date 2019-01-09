using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace faceitwpf
{
    /// <summary>
    /// Логика взаимодействия для DataPage.xaml
    /// </summary>
    public partial class DataPage : Page
    {
        public DataPage(string name)
        {           
            InitializeComponent();
            API api = new API();
            API.Player player = new API.Player();
            API.Stats[] stats = new API.Stats[10];
            player = api.GetPlayerInfo(name);
            try
            {
                stats = api.GetMatchHistory(player);
                matchgrid.ItemsSource = stats;
                avatar.Source = new BitmapImage(new Uri(player.Avatar));
                NickLabel.Content = player.Nickname;
                LevelLabel.Content = player.Level + " Level " + player.Elo + " Elo";
            }
            catch (ArgumentException e)
            {
                throw new System.ArgumentException();
            }
            
        }

        private void Back_MouseEnter(object sender, MouseEventArgs e)
        {
            Back.Background = new SolidColorBrush(Color.FromRgb(255, 125, 0));
        }

        private void Back_MouseLeave(object sender, MouseEventArgs e)
        {
            Back.Background = new SolidColorBrush(Color.FromRgb(255, 85, 0));
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }
        
    }
    public class KDRConverter : IValueConverter
        {
            object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (double)value <= 1;
            }

            object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
}
