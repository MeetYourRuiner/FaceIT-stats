﻿using System;
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
        private int _page = 1;

        public DataPage()
        {           
            InitializeComponent();
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

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            API.GetInstance().CurrentPlayer = null;
            NavigationService.GoBack();
        }

        private async Task GetPage(bool GetNext = true)
        {
            API api = API.GetInstance();
            var player = api.CurrentPlayer;
            _page = GetNext ? _page + 1 : _page - 1;
            try
            {
                API.MatchHistory matchHistory = await api.AsyncGetHistory(player.PlayerID, _page);
                for (int i = 0; i < matchHistory.Match.Length; i++)
                {
                    matchHistory.Match[i].Stats = await api.AsyncGetStats(matchHistory.Match[i].Id, player.Nickname);
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
            if (_page == 1)
                return;
            var backup = matchgrid.ItemsSource;
            this.matchgrid.IsEnabled = false;
            Cursor = Cursors.Wait;
            try
            {
                await GetPage(GetNext:false);
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
