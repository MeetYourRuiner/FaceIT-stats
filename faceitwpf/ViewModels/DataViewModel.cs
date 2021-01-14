using faceitwpf.Models;
using faceitwpf.Services;
using faceitwpf.ViewModels.Commands;
using faceitwpf.Views.Enums;
using System;
using System.Collections.Generic;

namespace faceitwpf.ViewModels
{
    class DataViewModel : BaseViewModel
    {
        private readonly IStatsRepository statsRepository;
        private readonly INavigationService navigationService;

        private const int MATCHES_ON_PAGE = 9;

        private bool _isLoaded = false;
        private string playerName;
        private int _pagesCount = 1;
        private int _page = 0;
        private int Page
        {
            get => _page;
            set
            {
                _page = value;
                OnPropertyChanged("IsPrevEnabled");
                OnPropertyChanged("IsNextEnabled");
            }
        }

        public bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                OnPropertyChanged("IsNextEnabled");
            }
        }

        private ChartViewModel _chartViewModel;
        public ChartViewModel ChartViewModel 
        { 
            get { return _chartViewModel; }
            set
            {
                _chartViewModel = value;
                OnPropertyChanged();
            }
        }

        private Player _currentPlayer;
        public Player CurrentPlayer
        {
            get { return _currentPlayer; }
            set
            {
                _currentPlayer = value;
                OnPropertyChanged();
            }
        }

        public List<Match> Matches { get; set; }

        private LastMatchesPerfomance _lastMatchesPerfomance;
        public LastMatchesPerfomance LastMatchesPerfomance
        {
            get { return _lastMatchesPerfomance; }
            set
            {
                _lastMatchesPerfomance = value;
                OnPropertyChanged();
            }
        }

        private List<Match> _sliceOfHistory;
        public List<Match> SliceOfHistory
        {
            get { return _sliceOfHistory; }
            set
            {
                _sliceOfHistory = value;
                OnPropertyChanged();
            }
        }

        private bool _isChartVisible = false;
        public bool IsChartVisible
        {
            get { return _isChartVisible; }
            set
            {
                _isChartVisible = value;
                OnPropertyChanged();
            }
        }

        private string _toggleButtonContent = "Chart";
        public string ToggleButtonContent
        {
            get { return _toggleButtonContent; }
            set
            {
                _toggleButtonContent = value;
                OnPropertyChanged();
            }
        }

        public bool IsNextEnabled
        {
            get { return Page < _pagesCount - 1; }
        }

        public bool IsPrevEnabled
        {
            get { return Page > 0; }
        }

        private RelayCommand _backCommand;
        public RelayCommand BackCommand
        {
            get => _backCommand ?? (_backCommand = new RelayCommand((obj) =>
            {
                navigationService.ClearHistory();
                navigationService.Navigate(ViewTypes.Search);
            }));
        }

        private RelayCommand _nextPageCommand;
        public RelayCommand NextPageCommand
        {
            get => _nextPageCommand ?? (_nextPageCommand = new RelayCommand((obj) =>
            {
                try
                {
                    SliceOfHistory = GetPage(++Page);
                }
                catch { }
            }));
        }

        private RelayCommand _prevPageCommand;
        public RelayCommand PrevPageCommand
        {
            get => _prevPageCommand ?? (_prevPageCommand = new RelayCommand((obj) =>
            {
                try
                {
                    SliceOfHistory = GetPage(--Page);
                }
                catch { }
            }));
        }

        private RelayCommand _toggleModeCommand;
        public RelayCommand ToggleModeCommand
        {
            get => _toggleModeCommand ?? (_toggleModeCommand = new RelayCommand((obj) =>
            {
                IsChartVisible = !IsChartVisible;
                ToggleButtonContent = IsChartVisible ? "Table" : "Chart";
            }));
        }

        private RelayCommand _showMatchDetailsCommand;
        public RelayCommand ShowMatchDetailsCommand
        {
            get => _showMatchDetailsCommand ?? (_showMatchDetailsCommand = new RelayCommand((obj) =>
            {
                navigationService.Navigate(ViewTypes.Match);
            }));
        }

        private RelayCommand _loadedCommand;
        public RelayCommand LoadedCommand
        {
            get => _loadedCommand ?? (_loadedCommand = new RelayCommand(async (obj) =>
            {
                if (_isLoaded)
                    return;
                IsLoading = true;
                try
                {
                    CurrentPlayer = await statsRepository.GetPlayerAsync(playerName);
                    Matches = await statsRepository.GetMatchesAsync(CurrentPlayer.PlayerID);
                    _pagesCount = CountPages(Matches);
                    SliceOfHistory = GetPage(Page);
                    LastMatchesPerfomance = new LastMatchesPerfomance(Matches);
                    ChartViewModel = new ChartViewModel(Matches);
                }
                catch(Exception ex)
                {
                    navigationService.GoBack(ex);
                }
                _isLoaded = true;
                IsLoading = false;
            }));
        }

        private RelayCommand _openPlayerFaceit;
        public RelayCommand OpenPlayerFaceit
        {
            get => _openPlayerFaceit ?? (_openPlayerFaceit = new RelayCommand((obj) =>
            {
                var sInfo = new System.Diagnostics.ProcessStartInfo(CurrentPlayer.FaceitURL)
                {
                    UseShellExecute = true,
                };
                System.Diagnostics.Process.Start(sInfo);
            }));
        }

        private RelayCommand _openPlayerSteam;
        public RelayCommand OpenPlayerSteam
        {
            get => _openPlayerSteam ?? (_openPlayerSteam = new RelayCommand((obj) =>
            {
                var sInfo = new System.Diagnostics.ProcessStartInfo(CurrentPlayer.SteamURL)
                {
                    UseShellExecute = true,
                };
                System.Diagnostics.Process.Start(sInfo);
            }));
        }

        public DataViewModel(IStatsRepository statsRepository, INavigationService navigationService, object parameter)
        {
            this.statsRepository = statsRepository;
            this.navigationService = navigationService;
            playerName = (string)parameter;
        }

        private List<Match> GetPage(int page)
        {
            try
            {
                if (Matches.Count == 0)
                {
                    return Matches;
                }
                int restOfMatches = Matches.Count - Page * MATCHES_ON_PAGE;
                if (restOfMatches < MATCHES_ON_PAGE && restOfMatches > 0)
                    return Matches.GetRange(Page * MATCHES_ON_PAGE, restOfMatches);
                else if (restOfMatches > 0)
                    return Matches.GetRange(Page * MATCHES_ON_PAGE, MATCHES_ON_PAGE);
            }
            catch (System.Exception)
            {
                if (page != 0)
                    throw;
            }
            return Matches;
        }

        private int CountPages(List<Match> matches)
        {
            int pagesCount = matches.Count / MATCHES_ON_PAGE;
            if (matches.Count % MATCHES_ON_PAGE > 0)
                ++pagesCount;
            return pagesCount;
        }
    }
}
