using faceitwpf.Services;
using System.Windows;
using System.Windows.Navigation;

namespace faceitwpf.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Style = (Style)FindResource(typeof(Window));
        }

        private void NavigationWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Title = "Цифорки " + UpdateService.GetCurrentVersion();
        }
    }
}
