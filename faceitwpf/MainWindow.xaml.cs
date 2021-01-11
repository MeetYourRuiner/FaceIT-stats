using faceitwpf.Services;
using System.Windows;
using System.Windows.Navigation;

namespace faceitwpf
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
