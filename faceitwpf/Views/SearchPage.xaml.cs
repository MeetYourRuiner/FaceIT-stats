using faceitwpf.Models;
using faceitwpf.Services;
using faceitwpf.ViewModels;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace faceitwpf.Views
{
    /// <summary>
    /// Логика взаимодействия для Name.xaml
    /// </summary>
    public partial class SearchPage : Page
    {
        public SearchPage()
        {
            InitializeComponent();
            DataContext = new SearchPageViewModel(this);
            //nameTextBox.Text = Properties.Settings.Default.LastNickname;
        }

        //private void Button_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    var button = (Label)sender;
        //    button.Background = new SolidColorBrush(Color.FromRgb(255, 125, 0));
        //}

        //private void Button_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    var button = (Label)sender;
        //    button.Background = new SolidColorBrush(Color.FromRgb(255, 85, 0));
        //}

        private void nameTextBox_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            var textbox = (TextBox)sender;
            textbox.Dispatcher.BeginInvoke(new Action(() => textbox.SelectAll()));
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            nameTextBox.Focus();
        }
    }
}
