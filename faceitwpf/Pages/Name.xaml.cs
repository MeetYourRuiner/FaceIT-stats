using faceitwpf.Classes;
using faceitwpf.Models;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
            nameTextBox.Text = Properties.Settings.Default.LastNickname;
        }

        private async void CheckUpdates()
        {
            string latestVersion = await UpdateManager.CheckForUpdate();
            if (latestVersion != null)
            {
                this.UpdateLabel.Content = "Update to\n" + latestVersion;
                this.UpdateLabel.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HandleClickOrEnter();
        }

        private void nameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                HandleClickOrEnter();
            }
        }

        // Нажатие Enter или "Search" ведёт к открытию DataPage 
        private async void HandleClickOrEnter()
        {
            if (nameTextBox.Text.Length > 0 && nameTextBox.Text.Length < 30)
            {
                btn.IsEnabled = false;
                nameTextBox.IsEnabled = false;
                Cursor = Cursors.Wait;
                try
                {
                    API api = API.GetInstance();
                    Player player = await api.GetInfoAsync<Player>(nameTextBox.Text);
                    api.CurrentPlayer = player;
                    DataPage datapage = new DataPage();
                    await datapage.Initialize();
                    NavigationService.Navigate(datapage);
                    Properties.Settings.Default.LastNickname = nameTextBox.Text;
                    Properties.Settings.Default.Save();
                }
                catch (Exception ex)
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

        private void nameTextBox_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            var textbox = (TextBox)sender;
            textbox.Dispatcher.BeginInvoke(new Action(() => textbox.SelectAll()));
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            nameTextBox.Focus();
            CheckUpdates();
        }

        private async void UpdateLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                await UpdateManager.Update();
            }
            catch (Exception ex)
            {
                this.IsEnabled = true;
                nameTextBox.Text = ex.Message;
            }
            finally
            {
                this.IsEnabled = true;
            }
        }
    }
}
