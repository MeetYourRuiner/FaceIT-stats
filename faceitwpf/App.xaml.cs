using System.IO;
using System.Linq;
using System.Windows;

namespace faceitwpf
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Contains("-updated"))
            {
                string path = e.Args[1];
                try
                {
                    File.Delete(path);
                }
                catch { }
            }
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
