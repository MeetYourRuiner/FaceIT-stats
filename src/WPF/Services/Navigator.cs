using FaceitStats.WPF.Classes;
using FaceitStats.WPF.Interfaces;
using FaceitStats.WPF.ViewModels;
using FaceitStats.WPF.ViewModels.Abstractions;
using FaceitStats.WPF.Views.Enums;
using System;
using System.Collections.Generic;

namespace FaceitStats.WPF.Services
{
    class Navigator : INavigator
    {
        private readonly VMFactory _vmFactory;
        private readonly INotifyService _notifyService;

        public event EventHandler<NavigatedEventArgs> Navigated;

        public Stack<BaseViewModel> History { get; } = new Stack<BaseViewModel>();

        private BaseViewModel _currentViewModel;
        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnNavigated(new NavigatedEventArgs(_currentViewModel));
            }
        }

        public Navigator(VMFactory vmFactory, INotifyService notifyService)
        {
            _vmFactory = vmFactory;
            _notifyService = notifyService;
        }

        protected virtual void OnNavigated(NavigatedEventArgs e)
        {
            Navigated?.Invoke(this, e);
        }

        public void ClearHistory()
        {
            History.Clear();
        }

        public void GoBack(Exception exception)
        {
            if (exception != null)
            {
                _notifyService.DisplayError(exception);
            }
            if (History.Peek() != null)
                CurrentViewModel = History.Pop();
        }

        public void Navigate(ViewTypes destination, object parameter = null, bool replace = false)
        {
            if (CurrentViewModel != null && replace == false)
                History.Push(CurrentViewModel);
            switch (destination)
            {
                case ViewTypes.Data:
                    CurrentViewModel = _vmFactory.Create<DataViewModel>(parameter);
                    break;
                case ViewTypes.Search:
                    CurrentViewModel = _vmFactory.Create<SearchViewModel>(parameter);
                    break;
                case ViewTypes.Match:
                    CurrentViewModel = _vmFactory.Create<MatchDetailsViewModel>(parameter);
                    break;
                case ViewTypes.Lobby:
                    CurrentViewModel = _vmFactory.Create<LobbyViewModel>(parameter);
                    break;
                case ViewTypes.TeamAnalyze:
                    CurrentViewModel = _vmFactory.Create<TeamAnalyzeViewModel>(parameter);
                    break;
            }
        }
    }

    public class NavigatedEventArgs : EventArgs
    {
        public BaseViewModel DestinationViewModel { get; set; }

        public NavigatedEventArgs(BaseViewModel destinationViewModel)
        {
            DestinationViewModel = destinationViewModel;
        }
    }
}
