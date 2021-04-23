using FaceitStats.Core.DTOs;
using FaceitStats.Core.Interfaces;
using FaceitStats.Core.Models;
using FaceitStats.WPF.Classes;
using FaceitStats.WPF.Interfaces;
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
        private readonly IFaceitService _faceitService;
        private readonly INavigator _navigator;
        private readonly INotifyService _notifyService;

        private int MatchesOnPage { get; set; } = 9;

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
            get => _backCommand ??= new RelayCommand((obj) =>
            {
                _navigator.GoBack();
            });
        }

        private RelayCommand _nextPageCommand;
        public RelayCommand NextPageCommand
        {
            get => _nextPageCommand ??= new RelayCommand((obj) =>
            {
                try
                {
                    SliceOfHistory = GetPage(++Page);
                }
                catch { }
            });
        }

        private RelayCommand _prevPageCommand;
        public RelayCommand PrevPageCommand
        {
            get => _prevPageCommand ??= new RelayCommand((obj) =>
            {
                try
                {
                    SliceOfHistory = GetPage(--Page);
                }
                catch { }
            });
        }

        private RelayCommand _showMatchDetailsCommand;
        public RelayCommand ShowMatchDetailsCommand
        {
            get => _showMatchDetailsCommand ??= new RelayCommand((obj) =>
            {
                int index = (int)obj;
                Match match = Matches.FirstOrDefault((m) => m.Index == index);
                _navigator.Navigate(ViewTypes.Match, match);
            });
        }

        private RelayCommand _showOngoingMatchCommand;
        public RelayCommand ShowOngoingMatchCommand
        {
            get => _showOngoingMatchCommand ??= new RelayCommand((obj) =>
            {
                _navigator.Navigate(ViewTypes.Lobby, OngoingMatchId);
            });
        }

        private RelayCommand _refreshOngoingMatchCommand;
        public RelayCommand RefreshOngoingMatchCommand
        {
            get => _refreshOngoingMatchCommand ??= new RelayCommand(async (obj) =>
            {
                IsRefreshButtonEnabled = false;
                try
                {
                    OngoingMatchId = await _faceitService.GetOngoingMatchIdAsync(CurrentPlayerProfile.Id);
                }
                catch (Exception ex)
                {
                    _notifyService.DisplayError(ex);
                }
                IsRefreshButtonEnabled = true;
            });
        }

        private RelayCommand _changeDisplayablePerfomanceCommand;
        public RelayCommand ChangeDisplayablePerfomanceCommand
        {
            get => _changeDisplayablePerfomanceCommand ??= new RelayCommand((obj) =>
            {
                if (DisplayablePerfomance == LastMatchesPerfomance)
                {
                    if (OverallPerfomance != null)
                    {
                        DisplayablePerfomance = OverallPerfomance;
                    }
                    else
                    {
                        _notifyService.DisplayError(new Exception("Overall perfomance is unavailable"));
                    }
                }
                else
                {
                    DisplayablePerfomance = LastMatchesPerfomance;
                }
            });
        }

        private RelayCommand _openPlayerFaceitCommand;
        public RelayCommand OpenPlayerFaceitCommand
        {
            get => _openPlayerFaceitCommand ??= new RelayCommand((obj) =>
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
                    _notifyService.DisplayError(new Exception("Failed to open link in browser", ex));
                }
            });
        }

        private RelayCommand _openPlayerSteamCommand;
        public RelayCommand OpenPlayerSteamCommand
        {
            get => _openPlayerSteamCommand ??= new RelayCommand((obj) =>
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
                    _notifyService.DisplayError(new Exception("Failed to open link in browser", ex));
                }
            });
        }

        private RelayCommand _addToFavoritesCommand;
        public RelayCommand AddToFavoritesCommand
        {
            get => _addToFavoritesCommand ??= new RelayCommand((obj) =>
            {
                if (!IsFavoritePlayer)
                {
                    SettingsWrapper.Favorites.Add(CurrentPlayerProfile.Nickname);
                    IsFavoritePlayer = true;
                }
                else
                {
                    SettingsWrapper.Favorites.Remove(CurrentPlayerProfile.Nickname);
                    IsFavoritePlayer = false;
                }
                OnPropertyChanged("IsFavoritePlayer");
            });
        }
        #endregion

        public DataViewModel(IFaceitService faceitService, INavigator navigator, INotifyService notifyService, object parameter)
        {
            _faceitService = faceitService;
            _navigator = navigator;
            _notifyService = notifyService;
            _playerName = (string)parameter;
        }

        private List<Match> GetPage(int page)
        {
            try
            {
                if (Matches.Count == 0)
                {
                    return Matches;
                }
                int restOfMatches = Matches.Count - Page * MatchesOnPage;
                if (restOfMatches < MatchesOnPage && restOfMatches > 0)
                    return Matches.GetRange(Page * MatchesOnPage, restOfMatches);
                else if (restOfMatches > 0)
                    return Matches.GetRange(Page * MatchesOnPage, MatchesOnPage);
            }
            catch (Exception)
            {
                if (page != 0)
                    throw;
            }
            return Matches;
        }

        private int CountPages(List<Match> matches)
        {
            int pagesCount = matches.Count / MatchesOnPage;
            if (matches.Count % MatchesOnPage > 0)
                ++pagesCount;
            return pagesCount;
        }

        public override async Task LoadMethod(object obj)
        {
            try
            {
                CurrentPlayerProfile = await _faceitService.GetProfileByNameAsync(_playerName);
                Matches = await _faceitService.GetMatchesAsync(CurrentPlayerProfile.Id, MatchesOnPage * 20);
            }
            catch (Exception ex)
            {
                _navigator.GoBack(ex);
                return;
            }

            try
            {
                OngoingMatchId = await _faceitService.GetOngoingMatchIdAsync(CurrentPlayerProfile.Id);
            }
            catch
            {
                _notifyService.DisplayError(new Exception("Failed to get ongoing match"));
            }

            try
            {
                var playerOverallStats = await _faceitService.GetOverallStatsAsync(CurrentPlayerProfile.Id);
                PlayerMapsStatisticsViewModel = new PlayerMapsStatisticsViewModel(playerOverallStats);
                OverallPerfomance = new AveragePerfomance(playerOverallStats);
            }
            catch
            {
                PlayerMapsStatisticsViewModel = new PlayerMapsStatisticsViewModel(new PlayerOverallStats());
            }

            IsFavoritePlayer = SettingsWrapper.Favorites.Contains(CurrentPlayerProfile.Nickname);
            SliceOfHistory = GetPage(Page);
            LastMatchesPerfomance = new AveragePerfomance(Matches, 20);
            EloChartViewModel = new EloChartViewModel(Matches);
            DisplayablePerfomance = OverallPerfomance ?? LastMatchesPerfomance;

            MatchesViewModel = new MatchesViewModel(SliceOfHistory, MatchesOnPage);
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
