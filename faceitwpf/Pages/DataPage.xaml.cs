using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using faceitwpf.Models;

namespace faceitwpf
{
    /// <summary>
    /// Логика взаимодействия для DataPage.xaml
    /// </summary>
    public partial class DataPage : Page
    {
        private List<Match> matches;

        public int Page { get; set; }
        private readonly int MatchesOnPage = 9;

        private enum NavigateTo
        {
            First,
            Next,
            Previous
        }
        public DataPage()
        {
            InitializeComponent();
        }

        public async Task Initialize()
        {
            API api = API.GetInstance();
            Player player = api.CurrentPlayer;
            NickLabel.Content = player.Nickname;
            LevelLabel.Content = player.Level + " Level " + player.Elo + " Elo";
            await LoadMatchesAsync();
            LoadPage(NavigateTo.First);
            try
            {
                avatar.Source = new BitmapImage(new Uri(player.Avatar));
            }
            catch (UriFormatException)
            {
                avatar.Source = new BitmapImage(new Uri("/faceitwpf;component/icon-pheasant-preview-2-268x151.png", UriKind.Relative));
                avatar.Stretch = Stretch.Uniform;
            }
            try
            {
                CoverImage.Fill = new ImageBrush(new BitmapImage(new Uri(player.CoverImage)));
                CoverImage.Stretch = Stretch.Fill;
            }
            catch (UriFormatException)
            {}
        }

        private async Task LoadMatchesAsync()
        {
            API api = API.GetInstance();
            Player player = api.CurrentPlayer;
            try
            {
                var history = await api.GetInfoAsync<MatchHistory>(player.PlayerID);
                matches = history.Matches;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void LoadPage(NavigateTo navigateTo)
        {
            switch (navigateTo)
            {
                case NavigateTo.Next:
                    Page++;
                    break;
                case NavigateTo.Previous:
                    Page--;
                    break;
                case NavigateTo.First:
                    Page = 0;
                    break;
                default:
                    break;
            }
            if (Page == 0)
                Previous.IsEnabled = false;
            else
                Previous.IsEnabled = true;
            try
            {
                if (matches.Count == 0)
                { 
                    Previous.IsEnabled = false;
                    Next.IsEnabled = false;
                }
                var checkCount = matches.Count - Page * MatchesOnPage;
                if (checkCount < MatchesOnPage && checkCount > 0)
                    matchgrid.ItemsSource = matches.GetRange(Page * MatchesOnPage, checkCount);
                else if (checkCount > 0)
                    matchgrid.ItemsSource = matches.GetRange(Page * MatchesOnPage, MatchesOnPage);
                else
                    LoadPage(NavigateTo.Previous);
            }
            catch (Exception ex)
            {
                if (navigateTo != NavigateTo.First)
                    throw ex;
            }
        }

        private void Next_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                LoadPage(NavigateTo.Next);
            }
            catch
            {
                throw;
            }
        }

        private void Previous_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Page == 0)
                return;
            try
            {
                LoadPage(NavigateTo.Previous);
            }
            catch
            {
                throw;
            }
        }

        private void Back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            API.GetInstance().CurrentPlayer = null;
            NavigationService.GoBack();
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

    public class ZeroConverter : IValueConverter
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
