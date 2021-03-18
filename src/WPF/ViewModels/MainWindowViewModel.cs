using FaceitStats.WPF.Classes;
using FaceitStats.WPF.Interfaces;
using FaceitStats.WPF.ViewModels.Abstractions;
using FaceitStats.WPF.ViewModels.Commands;
using FaceitStats.WPF.Views.Enums;

namespace FaceitStats.WPF.ViewModels
{
    class MainWindowViewModel : BaseViewModel
    {
        private readonly INavigator _navigator;
        private readonly INotifyService _notifyService;

        private Error _notification;
        public Error Notification
        {
            get { return _notification; }
            private set
            {
                _notification = value;
                OnPropertyChanged();
            }
        }

        private BaseViewModel _currentViewModel;
        public BaseViewModel CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _loadedCommand;
        public RelayCommand LoadedCommand
        {
            get => _loadedCommand ??= new RelayCommand((obj) =>
            {
                _navigator.Navigated += (sender, args) =>
                {
                    CurrentViewModel = args.DestinationViewModel;
                };
                _navigator.Navigate(ViewTypes.Search);
            });
        }

        public MainWindowViewModel(INavigator navigator, INotifyService notifyService)
        {
            _navigator = navigator;
            _notifyService = notifyService;
        }
    }
}
