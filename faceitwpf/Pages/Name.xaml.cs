using faceitwpf.Classes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace faceitwpf
{
    /// <summary>
    /// Логика взаимодействия для Name.xaml
    /// </summary>
    public partial class Name : Page
    {
        public Name()
        {
            InitializeComponent();
            CheckUpdates();
        }

        private async void CheckUpdates()
        {
            bool isOutdated = await UpdateManager.CheckForUpdate();
            if (isOutdated)
                this.UpdateLabel.Visibility = System.Windows.Visibility.Visible;
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HandleClickOrEnter();
        }
        private async void HandleClickOrEnter()
        {
            if (nameTextBox.Text.Length > 0 && nameTextBox.Text.Length < 30)
            {
                btn.IsEnabled = false;
                nameTextBox.IsEnabled = false;
                Cursor = Cursors.Wait;
                try
                {
                    DataPage datapage = new DataPage();                   
                    API api = API.GetInstance();
                    API.Player player = await api.AsyncGetPlayerInfo(nameTextBox.Text);
                    api.CurrentPlayer = player;
                    var tasks = new List<Task<API.Stats>>();
                    API.MatchHistory matchHistory = await api.AsyncGetHistory(player.PlayerID);
                    for (int i = 0; i < matchHistory.Match.Length; i++)
                    {
                        tasks.Add(api.AsyncGetStats(matchHistory.Match[i].Id, player.Nickname));
                    }
                    //
                    System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                    timer.Start();
                    var Stats = await Task.WhenAll(tasks);
                    timer.Stop();
                    System.Diagnostics.Trace.WriteLine($"Загрузка страницы: {timer.Elapsed.TotalSeconds}");
                    //
                    for (int i = 0; i < matchHistory.Match.Length; i++)
                    {
                        matchHistory.Match[i].Stats = Stats[i];
                        matchHistory.Match[i].Date = DateTimeOffset.FromUnixTimeSeconds(matchHistory.Match[i]._Date).ToLocalTime();
                    }
                    datapage.matchgrid.ItemsSource = matchHistory.Match;

                    datapage.NickLabel.Content = player.Nickname;
                    datapage.LevelLabel.Content = player.Level + " Level " + player.Elo + " Elo";

                    try 
                    { 
                        datapage.avatar.Source = new BitmapImage(new Uri(player.Avatar)); 
                    }
                    catch (System.UriFormatException)
                    {
                        datapage.avatar.Source = new BitmapImage(new Uri("/faceitwpf;component/icon-pheasant-preview-2-268x151.png", UriKind.Relative));
                        datapage.avatar.Stretch = Stretch.Uniform;
                    }                    
                    NavigationService.Navigate(datapage);
                }
                catch(Exception ex)
                {
                    btn.IsEnabled = true;
                    nameTextBox.IsEnabled = true;
                    Cursor = Cursors.Arrow;
                    nameTextBox.Text = ex.Message;
                    await nameTextBox.Dispatcher.BeginInvoke(new Action(() => nameTextBox.SelectAll()));
                    nameTextBox.Focus();
                }
            }
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

        private void nameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                HandleClickOrEnter();
            }
        }

        private void nameTextBox_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            var textbox = (TextBox)sender;
            textbox.Dispatcher.BeginInvoke(new Action(() => textbox.SelectAll()));
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            nameTextBox.Focus();
        }

        private async void UpdateLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                await UpdateManager.Update();
            }
            catch
            {
                this.IsEnabled = true;
                nameTextBox.Text = "Update failed";
            }
            finally
            {
                this.IsEnabled = true;
            }
        }
    }
}
