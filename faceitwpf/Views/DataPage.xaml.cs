using faceitwpf.Models;
using faceitwpf.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
namespace faceitwpf.Views
{
    /// <summary>
    /// Логика взаимодействия для DataPage.xaml
    /// </summary>
    public partial class DataPage : Page
    {
        private List<Match> matches;
        private int[] Levels =
        {
            0, 0, 801, 951, 1101, 1251, 1401, 1551, 1701, 1851, 2001
        };

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
            Player player = APIService.CurrentPlayer;

            NickLabel.Content = player.Nickname;

            LevelLabel.Content = player.Level + " Level " + player.Elo + " ELO";

            var toDemote = player.Level == 1 ? "∞" : (player.Elo - Levels[player.Level] + 1).ToString();
            var toPromote = player.Level == 10 ? "∞" : (Levels[player.Level + 1] - player.Elo).ToString();
            EloLeftLabel.Content = $"-{toDemote}/+{toPromote}";

            await LoadMatchesAsync();

            if (matches.Count > 0)
            {
                var lastMatchesCount = matches.Count > 20 ? 20 : matches.Count;
                var lastMatches = matches.GetRange(0, lastMatchesCount);
                KillsLabel.Content = lastMatches.Select(m => m.Kills).Average();
                HSLabel.Content = lastMatches.Select(m => m.HSPercentage).Average();

                var avgKR = lastMatches.Select(m => m.KRRatio).Average();
                KRLabel.Content = avgKR;
                if (avgKR >= 0.9) KRLabel.Foreground = Brushes.RoyalBlue;
                else if (avgKR >= 0.8) KRLabel.Foreground = Brushes.Green;
                else if (avgKR > 0.65) KRLabel.Foreground = Brushes.Yellow;
                else KRLabel.Foreground = Brushes.Red;

                var avgKD = lastMatches.Select(m => m.KDRatio).Average();
                KDLabel.Content = avgKD;
                if (avgKD >= 1) KDLabel.Foreground = Brushes.Green;
                else KDLabel.Foreground = Brushes.Red;

                var avgWR = (double)lastMatches.Where(m => m.Result == 'W').Count() / lastMatchesCount;
                WRLabel.Content = avgWR;
                if (avgWR >= 0.5) WRLabel.Foreground = Brushes.Green;
                else WRLabel.Foreground = Brushes.Red;
            }

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
            { }
        }

        private async Task LoadMatchesAsync()
        {
            Player player = APIService.CurrentPlayer;
            try
            {
                var history = await APIService.GetHistoryAsync(player.PlayerID);
                matches = history.Matches;
                if (matches.Count > 0)
                    EloChart.SetSource(matches.ToArray());
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
                    ChartBtn.IsEnabled = false;
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
            APIService.ClearCurrentPlayer();
            NavigationService.GoBack();
        }

        private void ChartBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (EloChart.Visibility == System.Windows.Visibility.Hidden)
            {
                EloChart.Visibility = System.Windows.Visibility.Visible;
                matchgrid.Visibility = System.Windows.Visibility.Hidden;
                Next.Visibility = System.Windows.Visibility.Hidden;
                Previous.Visibility = System.Windows.Visibility.Hidden;
                ChartBtn.Content = "Table";
            }
            else
            {
                EloChart.Visibility = System.Windows.Visibility.Hidden;
                matchgrid.Visibility =  System.Windows.Visibility.Visible;
                Next.Visibility = System.Windows.Visibility.Visible;
                Previous.Visibility = System.Windows.Visibility.Visible;
                ChartBtn.Content = "Chart";
            }
        }
    }
}
