using FaceitStats.Core.Interfaces;
using FaceitStats.Infrastructure.Data;
using FaceitStats.WPF.Classes;
using FaceitStats.WPF.Interfaces;
using FaceitStats.WPF.Services;
using FaceitStats.WPF.ViewModels.Abstractions;
using FaceitStats.WPF.Views.Enums;
using System;
using System.Collections.Generic;

namespace FaceitStats.WPF.ViewModels
{
    class MainWindowViewModel : BaseViewModel, INavigator
    {
        private readonly VMStore _vmStore;
        private readonly FaceitAPIClient _apiClient;
        private readonly IUpdateService _updateService;
        private readonly IFaceitService _faceitRepository;

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

        private Error _error;
        public Error Error
        {
            get { return _error; }
            private set
            {
                _error = value;
                OnPropertyChanged();
            }
        }

        public Stack<BaseViewModel> History { get; } = new Stack<BaseViewModel>();

        public MainWindowViewModel()
        {
            _apiClient = new FaceitAPIClient(Properties.Settings.Default.API_Key, Properties.Settings.Default.User_API_Key);
            _updateService = new UpdateService();
            _faceitRepository = new FaceitService(_apiClient);

            _vmStore = new VMStore();
            _vmStore.Add<SearchViewModel>((parameter) => new SearchViewModel(_updateService, this, parameter));
            _vmStore.Add<DataViewModel>((parameter) => new DataViewModel(_faceitRepository, this, parameter));
            _vmStore.Add<MatchDetailsViewModel>((parameter) => new MatchDetailsViewModel(_faceitRepository, this, parameter));
            _vmStore.Add<LobbyViewModel>((parameter) => new LobbyViewModel(_faceitRepository, this, parameter));
            _vmStore.Add<TeamAnalyzeViewModel>((parameter) => new TeamAnalyzeViewModel(_faceitRepository, this, parameter));

            Navigate(ViewTypes.Search);
        }

        public void Navigate(ViewTypes destination, object parameter = null, bool replace = false)
        {
            if (CurrentViewModel != null && replace == false)
                History.Push(CurrentViewModel);
            switch (destination)
            {
                case ViewTypes.Data:
                    CurrentViewModel = _vmStore.Get<DataViewModel>(parameter);
                    break;
                case ViewTypes.Search:
                    CurrentViewModel = _vmStore.Get<SearchViewModel>(parameter);
                    break;
                case ViewTypes.Match:
                    CurrentViewModel = _vmStore.Get<MatchDetailsViewModel>(parameter);
                    break;
                case ViewTypes.Lobby:
                    CurrentViewModel = _vmStore.Get<LobbyViewModel>(parameter);
                    break;
                case ViewTypes.TeamAnalyze:
                    CurrentViewModel = _vmStore.Get<TeamAnalyzeViewModel>(parameter);
                    break;
            }
        }

        public void DisplayError(Exception exception)
        {
            SetError(exception);
        }

        private void SetError(Exception exception)
        {
            Error = new Error(exception.Message, 3);
            Error.TimerElapsed += (sender, e) =>
            {
                if (sender.Equals(Error))
                    Error = null;
            };
        }

        public void GoBack(Exception exception)
        {
            if (exception != null)
                SetError(exception);
            if (History.Peek() != null)
                CurrentViewModel = History.Pop();
        }

        public void ClearHistory()
        {
            History.Clear();
        }
    }
}
