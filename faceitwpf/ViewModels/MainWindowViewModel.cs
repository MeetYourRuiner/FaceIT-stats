using faceitwpf.Classes;
using faceitwpf.Models;
using faceitwpf.Services;
using faceitwpf.Views.Enums;
using System;
using System.Collections.Generic;

namespace faceitwpf.ViewModels
{
    class MainWindowViewModel : BaseViewModel, INavigator
    {
        private readonly VMStore _vmStore;
        private readonly IAPIService _apiService;
        private readonly IUpdateService _updateService;
        private readonly IStatsRepository _statsRepository;

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
            _apiService = new APIService();
            _updateService = new UpdateService();
            _statsRepository = new StatsRepository(_apiService);

            _vmStore = new VMStore();
            _vmStore.Add<SearchViewModel>((parameter) => new SearchViewModel(_updateService, this, parameter));
            _vmStore.Add<DataViewModel>((parameter) => new DataViewModel(_statsRepository, this, parameter));
            _vmStore.Add<MatchDetailsViewModel>((parameter) => new MatchDetailsViewModel(_statsRepository, this, parameter));

            Navigate(ViewTypes.Search);
        }

        public void Navigate(ViewTypes destination, object parameter = null)
        {
            if (CurrentViewModel != null)
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
