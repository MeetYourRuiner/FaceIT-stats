using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace faceitwpf
{
    /// <summary>
    /// Логика взаимодействия для DataPage.xaml
    /// </summary>
    public partial class DataPage : Page
    {
        private int page;

        public int Page
        {
            get => page;
            set
            {
                page = value;
                if (page == 1)
                {
                    Previous.IsEnabled = false;
                }
                else
                {
                    Previous.IsEnabled = true;
                }
            }
        }
        public DataPage()
        {
            InitializeComponent();
            Page = 1;
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            var button = (Label)sender;
            button.Background = new SolidColorBrush(Color.FromRgb(255, 125, 0));
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            var button = (Label)sender;
            button.Background = new SolidColorBrush(Color.FromRgb(255, 85, 0));
        }

        private void Back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            API.GetInstance().CurrentPlayer = null;
            NavigationService.GoBack();
        }

        private async Task GetPage(bool GetNext = true)
        {
            API api = API.GetInstance();
            var player = api.CurrentPlayer;
            var tasks = new List<Task<API.Stats>>();
            Page = GetNext ? Page + 1 : Page - 1;
            try
            {
                API.MatchHistory matchHistory = await api.AsyncGetHistory(player.PlayerID, Page);
                if (matchHistory.Match.Length == 0)
                {
                    Page--;
                    throw new Exception("Page is empty"); 
                }
                for (int i = 0; i < matchHistory.Match.Length; i++)
                {
                    tasks.Add(api.AsyncGetStats(matchHistory.Match[i].Id, player.Nickname));
                }
                var Stats = await Task.WhenAll(tasks);
                for (int i = 0; i < matchHistory.Match.Length; i++)
                {
                    matchHistory.Match[i].Stats = Stats[i];
                    matchHistory.Match[i].Date = DateTimeOffset.FromUnixTimeSeconds(matchHistory.Match[i]._Date).ToLocalTime();
                }
                this.matchgrid.ItemsSource = matchHistory.Match;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async void Next_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var backup = matchgrid.ItemsSource;
            this.matchgrid.IsEnabled = false;
            Cursor = Cursors.Wait;
            try
            {
                await GetPage();
            }
            catch
            {
                this.matchgrid.ItemsSource = backup;
            }
            finally
            {
                matchgrid.IsEnabled = true;
                Cursor = Cursors.Arrow;
            }
        }

        private async void Previous_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Page == 1)
                return;
            var backup = matchgrid.ItemsSource;
            this.matchgrid.IsEnabled = false;
            Cursor = Cursors.Wait;
            try
            {
                await GetPage(GetNext: false);
            }
            catch
            {
                this.matchgrid.ItemsSource = backup;
            }
            finally
            {
                matchgrid.IsEnabled = true;
                Cursor = Cursors.Arrow;
            }
        }
    }
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
