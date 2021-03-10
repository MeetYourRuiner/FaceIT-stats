using System.Windows;

namespace FaceitStats.WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //Title = "Цифорки " + UpdateService.GetCurrentVersion();
        }
    }
}
