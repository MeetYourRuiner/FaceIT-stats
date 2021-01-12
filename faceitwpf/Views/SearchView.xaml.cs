using System;
using System.Windows.Controls;

namespace faceitwpf.Views
{
    /// <summary>
    /// Логика взаимодействия для Name.xaml
    /// </summary>
    public partial class SearchView : UserControl
    {
        public SearchView()
        {
            InitializeComponent();
        }

        private void nameTextBox_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            var textbox = (TextBox)sender;
            textbox.Dispatcher.BeginInvoke(new Action(() => textbox.SelectAll()));
        }
    }
}
