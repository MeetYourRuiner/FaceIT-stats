using FaceitStats.Core.Interfaces;
using FaceitStats.Infrastructure.Services;
using FaceitStats.WPF.Classes;
using FaceitStats.WPF.Interfaces;
using FaceitStats.WPF.Properties;
using FaceitStats.WPF.Services;
using FaceitStats.WPF.ViewModels;
using System.IO;
using System.Linq;
using System.Windows;

namespace FaceitStats.WPF
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
                try
                {
                    File.Delete("old.exe");
                }
                catch { }
            }

            var _vmFactory = new VMFactory();

            _vmFactory.AddViewModel<MainWindowViewModel>();
            _vmFactory.AddViewModel<SearchViewModel>();
            _vmFactory.AddViewModel<DataViewModel>();
            _vmFactory.AddViewModel<MatchDetailsViewModel>();
            _vmFactory.AddViewModel<LobbyViewModel>();
            _vmFactory.AddViewModel<TeamAnalyzeViewModel>();

            _vmFactory.AddService<VMFactory>(_vmFactory);
            _vmFactory.AddService<IFaceitService>(new FaceitService(Settings.Default.API_Key, Settings.Default.User_API_Key));
            _vmFactory.AddService<IUpdateService>(new UpdateService());
            _vmFactory.AddService<INotifyService>(new NotifyService());
            _vmFactory.AddService<INavigator, Navigator>();

            MainWindow mainWindow = new MainWindow();
            mainWindow.DataContext = _vmFactory.Create<MainWindowViewModel>();
            mainWindow.Show();
        }
    }
}
