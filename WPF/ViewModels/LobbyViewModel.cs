using FaceitStats.Core.Interfaces;
using FaceitStats.Core.Models;
using FaceitStats.WPF.Services;
using FaceitStats.WPF.ViewModels.Abstractions;
using FaceitStats.WPF.ViewModels.Commands;
using FaceitStats.WPF.ViewModels.Controls;
using System;
using System.Threading.Tasks;

namespace FaceitStats.WPF.ViewModels
{
    class LobbyViewModel : LoadableViewModel
    {
        private readonly IFaceitRepository _faceitRepository;
        private readonly INavigator _navigator;

        private string currentMatchId;

        public bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }

        public override async Task LoadedMethod(object obj)
        {
            try
            {
                CurrentMatchInfo = await UpdateMatchInfo();
            }
            catch (Exception ex)
            {
                _navigator.GoBack(ex);
                return;
            }
            TeamAViewModel = new LobbyTeamInfoViewModel(_faceitRepository, _navigator, CurrentMatchInfo.TeamA);
            TeamBViewModel = new LobbyTeamInfoViewModel(_faceitRepository, _navigator, CurrentMatchInfo.TeamB);
        }

        private RelayCommand _backCommand;
        public RelayCommand BackCommand
        {
            get => _backCommand ?? (_backCommand = new RelayCommand((obj) =>
            {
                _navigator.GoBack();
            }));
        }

        private MatchInfo _currentMatchInfo;
        public MatchInfo CurrentMatchInfo
        {
            get => _currentMatchInfo;
            set
            {
                _currentMatchInfo = value;
                OnPropertyChanged();
            }
        }

        private LobbyTeamInfoViewModel _teamAViewModel;
        public LobbyTeamInfoViewModel TeamAViewModel
        {
            get => _teamAViewModel;
            set
            {
                _teamAViewModel = value;
                OnPropertyChanged();
            }
        }

        private LobbyTeamInfoViewModel _teamBViewModel;
        public LobbyTeamInfoViewModel TeamBViewModel
        {
            get => _teamBViewModel;
            set
            {
                _teamBViewModel = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _refreshCommand;
        public RelayCommand RefreshCommand
        {
            get => _refreshCommand ?? (_refreshCommand = new RelayCommand(async (obj) =>
            {
                IsRefreshing = true;
                try
                {
                    CurrentMatchInfo = await UpdateMatchInfo();
                }
                catch (Exception ex)
                {
                    _navigator.DisplayError(ex);
                }
                IsRefreshing = false;
            }));
        }

        private RelayCommand _openPlayerStatsCommand;
        public RelayCommand OpenPlayerStatsCommand
        {
            get => _openPlayerStatsCommand ?? (_openPlayerStatsCommand = new RelayCommand((obj) =>
            {
                PlayerInfo player = (PlayerInfo)obj;
                _navigator.Navigate(Views.Enums.ViewTypes.Data, player.Nickname);
            }));
        }

        private RelayCommand _openMatchFaceitCommand;
        public RelayCommand OpenMatchFaceitCommand
        {
            get => _openMatchFaceitCommand ?? (_openMatchFaceitCommand = new RelayCommand((obj) =>
            {
                try
                {
                    var sInfo = new System.Diagnostics.ProcessStartInfo($"https://www.faceit.com/en/csgo/room/{CurrentMatchInfo.Id}")
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
        public LobbyViewModel(IFaceitRepository faceitRepository, INavigator navigator, object parameter)
        {
            this._faceitRepository = faceitRepository;
            this._navigator = navigator;
            currentMatchId = (string)parameter;
        }

        private async Task<MatchInfo> UpdateMatchInfo()
        {
            return await _faceitRepository.GetMatchInfoAsync(currentMatchId);
        }
    }
}
