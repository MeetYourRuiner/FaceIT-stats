using FaceitStats.Core.Interfaces;
using FaceitStats.Core.Models;
using FaceitStats.WPF.Services;
using FaceitStats.WPF.ViewModels.Abstractions;
using FaceitStats.WPF.ViewModels.Commands;
using FaceitStats.WPF.ViewModels.Controls;
using FaceitStats.WPF.Views.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FaceitStats.WPF.ViewModels
{
    class DataViewModel : LoadableViewModel
    {
        private readonly IFaceitRepository _faceitRepository;
        private readonly INavigator _navigator;

        private const int MATCHES_ON_PAGE = 9;

        private readonly string _playerName;
        private AveragePerfomance LastMatchesPerfomance { get; set; }
        private AveragePerfomance OverallPerfomance { get; set; }

        #region ObservableProperties
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
                OnPropertyChanged("IsPrevEnabled");
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

        private List<Match> _matches;
        public List<Match> Matches
        {
            get => _matches;
            set
            {
                _matches = value;
                PagesCount = CountPages(_matches);
                OnPropertyChanged();
            }
        }

        private AveragePerfomance _displayablePerfomance;
        public AveragePerfomance DisplayablePerfomance
        {
            get { return _displayablePerfomance; }
            set
            {
                _displayablePerfomance = value;
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

        public bool IsNextEnabled
        {
            get { return Page < PagesCount - 1; }
        }

        public bool IsPrevEnabled
        {
            get { return Page > 0; }
        }

        private EloChartViewModel _eloChartViewModel;
        public EloChartViewModel EloChartViewModel
        {
            get { return _eloChartViewModel; }
            private set
            {
                _eloChartViewModel = value;
                OnPropertyChanged();
            }
        }

        private MatchesViewModel _matchesViewModel;
        public MatchesViewModel MatchesViewModel
        {
            get { return _matchesViewModel; }
            private set
            {
                _matchesViewModel = value;
                OnPropertyChanged();
            }
        }

        private PlayerMapsStatisticsViewModel _playerMapsStatisticsViewModel;
        public PlayerMapsStatisticsViewModel PlayerMapsStatisticsViewModel
        {
            get { return _playerMapsStatisticsViewModel; }
            private set
            {
                _playerMapsStatisticsViewModel = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        private RelayCommand _backCommand;
        public RelayCommand BackCommand
        {
            get => _backCommand ?? (_backCommand = new RelayCommand((obj) =>
            {
                _navigator.GoBack();
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

        private RelayCommand _showMatchDetailsCommand;
        public RelayCommand ShowMatchDetailsCommand
        {
            get => _showMatchDetailsCommand ?? (_showMatchDetailsCommand = new RelayCommand((obj) =>
            {
                int index = (int)obj;
                Match match = Matches.FirstOrDefault((m) => m.Index == index);
                _navigator.Navigate(ViewTypes.Match, match);
            }));
        }

        private RelayCommand _showOngoingMatchCommand;
        public RelayCommand ShowOngoingMatchCommand
        {
            get => _showOngoingMatchCommand ?? (_showOngoingMatchCommand = new RelayCommand((obj) =>
            {
                _navigator.Navigate(ViewTypes.Lobby, OngoingMatchId);
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
                    OngoingMatchId = await _faceitRepository.GetOngoingMatchIdAsync(CurrentPlayerProfile.Id);
                }
                catch (Exception ex)
                {
                    _navigator.DisplayError(ex);
                }
                IsRefreshButtonEnabled = true;
            }));
        }

        private RelayCommand _changeDisplayablePerfomanceCommand;
        public RelayCommand ChangeDisplayablePerfomanceCommand
        {
            get => _changeDisplayablePerfomanceCommand ?? (_changeDisplayablePerfomanceCommand = new RelayCommand((obj) =>
            {
                if (DisplayablePerfomance == LastMatchesPerfomance)
                {
                    if (OverallPerfomance != null)
                    {
                        DisplayablePerfomance = OverallPerfomance;
                    }
                    else
                    {
                        _navigator.DisplayError(new Exception("Overall perfomance is unavailable"));
                    }
                }
                else
                {
                    DisplayablePerfomance = LastMatchesPerfomance;
                }
            }));
        }

        private RelayCommand _openPlayerFaceitCommand;
        public RelayCommand OpenPlayerFaceitCommand
        {
            get => _openPlayerFaceitCommand ?? (_openPlayerFaceitCommand = new RelayCommand((obj) =>
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

        private RelayCommand _openPlayerSteamCommand;
        public RelayCommand OpenPlayerSteamCommand
        {
            get => _openPlayerSteamCommand ?? (_openPlayerSteamCommand = new RelayCommand((obj) =>
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
        #endregion

        public DataViewModel(IFaceitRepository faceitRepository, INavigator navigator, object parameter)
        {
            this._navigator = navigator;
            _playerName = (string)parameter;
            this._faceitRepository = faceitRepository;
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

        public override async Task LoadedMethod(object obj)
        {
            try
            {
                CurrentPlayerProfile = await _faceitRepository.GetProfileByNameAsync(_playerName);
                Matches = await _faceitRepository.GetMatchesAsync(CurrentPlayerProfile.Id, MATCHES_ON_PAGE * 20);
                //Matches = await statsRepository.GetMatchesAsync(CurrentPlayerProfile.Id, DateTimeOffset.Now, DateTimeOffset.Now);
                OngoingMatchId = await _faceitRepository.GetOngoingMatchIdAsync(CurrentPlayerProfile.Id);
            }
            catch (Exception ex)
            {
                _navigator.GoBack(ex);
                return;
            }

            try
            {
                var playerOverallStats = await _faceitRepository.GetOverallStatsAsync(CurrentPlayerProfile.Id);
                playerOverallStats.MapOverallStats
                .Sort((m1, m2) =>
                    (m2.WinrateDouble * m2.Matches / playerOverallStats.Matches)
                        .CompareTo(m1.WinrateDouble * m1.Matches / playerOverallStats.Matches)
                );
                PlayerMapsStatisticsViewModel = new PlayerMapsStatisticsViewModel(playerOverallStats);
                OverallPerfomance = new AveragePerfomance(playerOverallStats);
            }
            catch
            {
                PlayerMapsStatisticsViewModel = new PlayerMapsStatisticsViewModel(new PlayerOverallStats());
            }

            IsFavoritePlayer = Properties.Settings.Default.Favorites.Contains(CurrentPlayerProfile.Nickname);
            SliceOfHistory = GetPage(Page);
            LastMatchesPerfomance = new AveragePerfomance(Matches, 20);
            EloChartViewModel = new EloChartViewModel(Matches);
            if (OverallPerfomance != null)
                DisplayablePerfomance = OverallPerfomance;
            else
                DisplayablePerfomance = LastMatchesPerfomance;

            MatchesViewModel = new MatchesViewModel(SliceOfHistory);
            PropertyChanged += (s, args) =>
            {
                if (args.PropertyName == "SliceOfHistory")
                {
                    MatchesViewModel.Matches = SliceOfHistory;
                }
            };
        }

    }
}
