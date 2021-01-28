using faceitwpf.Models;
using faceitwpf.Models.Abstractions;
using faceitwpf.Services;
using faceitwpf.ViewModels.Commands;
using faceitwpf.Views.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace faceitwpf.ViewModels
{
    class DataViewModel : BaseViewModel
    {
        private readonly IStatsRepository statsRepository;
        private readonly INavigator navigator;

        private const int MATCHES_ON_PAGE = 9;

        private bool _isLoaded = false;
        private string playerName;
        private int _page = 0;
        private int Page
        {
            get => _page;
            set
            {
                _page = value;
                OnPropertyChanged("PageToDisplay");
                OnPropertyChanged("IsPrevEnabled");
                OnPropertyChanged("IsNextEnabled");
            }
        }

        public int PageToDisplay
        {
            get => Page + 1;
        }

        private int _pagesCount = 1;
        public int PagesCount
        {
            get => _pagesCount;
            set
            {
                _pagesCount = value;
                OnPropertyChanged();
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

        private bool _isFavoritePlayer;
        public bool IsFavoritePlayer
        {
            get 
            {
                return _isFavoritePlayer;
            }
            set
            {
                _isFavoritePlayer = value;
                OnPropertyChanged();
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

        private PlayerProfile _currentPlayerProfile;
        public PlayerProfile CurrentPlayerProfile
        {
            get { return _currentPlayerProfile; }
            set
            {
                _currentPlayerProfile = value;
                OnPropertyChanged();
            }
        }

        private string _ongoingMatchId;
        public string OngoingMatchId
        {
            get { return _ongoingMatchId; }
            private set
            {
                _ongoingMatchId = value;
                OnPropertyChanged("IsInGame");
            }
        }
        public bool IsInGame
        {
            get => OngoingMatchId != null;
        }

        private bool _isRefreshButtonEnabled = true;
        public bool IsRefreshButtonEnabled
        {
            get { return _isRefreshButtonEnabled; }
            set
            {
                _isRefreshButtonEnabled = value;
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
            get { return Page < PagesCount - 1; }
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
                navigator.GoBack();
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
                int index = (int)obj;
                Match match = Matches.FirstOrDefault((m) => m.Index == index);
                navigator.Navigate(ViewTypes.Match, match);
            }));
        }

        private RelayCommand _showOngoingMatchCommand;
        public RelayCommand ShowOngoingMatchCommand
        {
            get => _showOngoingMatchCommand ?? (_showOngoingMatchCommand = new RelayCommand((obj) =>
            {
                navigator.Navigate(ViewTypes.OngoingMatch, OngoingMatchId);
            }));
        }

        private RelayCommand _refreshOngoingMatchCommand;
        public RelayCommand RefreshOngoingMatchCommand
        {
            get => _refreshOngoingMatchCommand ?? (_refreshOngoingMatchCommand = new RelayCommand(async (obj) =>
            {
                IsRefreshButtonEnabled = false;
                try
                {
                    OngoingMatchId = await statsRepository.GetOngoingMatchIdAsync(CurrentPlayerProfile.PlayerId);
                }
                catch (Exception ex)
                {
                    navigator.DisplayError(ex);
                }
                IsRefreshButtonEnabled = true;
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
                    CurrentPlayerProfile = await statsRepository.GetPlayerProfileAsync(playerName);
                    IsFavoritePlayer = Properties.Settings.Default.Favorites.Contains(CurrentPlayerProfile.Nickname);
                    Matches = await statsRepository.GetMatchesAsync(CurrentPlayerProfile.PlayerId, MATCHES_ON_PAGE * 20);
                    OngoingMatchId = await statsRepository.GetOngoingMatchIdAsync(CurrentPlayerProfile.PlayerId);
                }
                catch (Exception ex)
                {
                    navigator.GoBack(ex);
                    return;
                }
                PagesCount = CountPages(Matches);
                SliceOfHistory = GetPage(Page);
                LastMatchesPerfomance = new LastMatchesPerfomance(Matches);
                ChartViewModel = new ChartViewModel(Matches);

                _isLoaded = true;
                IsLoading = false;
            }));
        }

        private RelayCommand _openPlayerFaceit;
        public RelayCommand OpenPlayerFaceit
        {
            get => _openPlayerFaceit ?? (_openPlayerFaceit = new RelayCommand((obj) =>
            {
                try
                {
                    var sInfo = new System.Diagnostics.ProcessStartInfo(CurrentPlayerProfile.FaceitURL)
                    {
                        UseShellExecute = true,
                    };
                    System.Diagnostics.Process.Start(sInfo);
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to open link in browser", ex);
                }
            }));
        }

        private RelayCommand _openPlayerSteam;
        public RelayCommand OpenPlayerSteam
        {
            get => _openPlayerSteam ?? (_openPlayerSteam = new RelayCommand((obj) =>
            {
                try
                {
                    var sInfo = new System.Diagnostics.ProcessStartInfo(CurrentPlayerProfile.SteamURL)
                    {
                        UseShellExecute = true,
                    };
                    System.Diagnostics.Process.Start(sInfo);
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to open link in browser", ex);
                }
            }));
        }

        private RelayCommand _addToFavoritesCommand;
        public RelayCommand AddToFavoritesCommand
        {
            get => _addToFavoritesCommand ?? (_addToFavoritesCommand = new RelayCommand((obj) =>
            {
                System.Collections.Specialized.StringCollection favorites = Properties.Settings.Default.Favorites;
                if (!IsFavoritePlayer)
                {
                    favorites.Add(CurrentPlayerProfile.Nickname);
                    Properties.Settings.Default.Favorites = favorites;
                    Properties.Settings.Default.Save();
                    IsFavoritePlayer = true;
                }
                else
                {
                    favorites.Remove(CurrentPlayerProfile.Nickname);
                    Properties.Settings.Default.Favorites = favorites;
                    Properties.Settings.Default.Save();
                    IsFavoritePlayer = false;
                }
                OnPropertyChanged("IsFavoritePlayer");
            }));
        }

        public DataViewModel(IStatsRepository statsRepository, INavigator navigator, object parameter)
        {
            this.statsRepository = statsRepository;
            this.navigator = navigator;
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
