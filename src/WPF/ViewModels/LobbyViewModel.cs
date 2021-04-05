using FaceitStats.Core.Interfaces;
using FaceitStats.Core.Models;
using FaceitStats.WPF.Interfaces;
using FaceitStats.WPF.ViewModels.Abstractions;
using FaceitStats.WPF.ViewModels.Commands;
using FaceitStats.WPF.ViewModels.Controls;
using System;
using System.Threading.Tasks;

namespace FaceitStats.WPF.ViewModels
{
    class LobbyViewModel : LoadableViewModel
    {
        private readonly IFaceitService _faceitService;
        private readonly INavigator _navigator;

        private readonly string currentMatchId;

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

        private RelayCommand _backCommand;
        public RelayCommand BackCommand
        {
            get => _backCommand ??= new RelayCommand((obj) =>
            {
                _navigator.GoBack();
            });
        }

        private RelayCommand _refreshCommand;
        public RelayCommand RefreshCommand
        {
            get => _refreshCommand ??= new RelayCommand(async (obj) =>
            {
                IsRefreshing = true;
                try
                {
                    CurrentMatchInfo = await UpdateMatchInfo();
                }
                catch (Exception ex)
                {
                    //_navigator.DisplayError(ex);
                }
                IsRefreshing = false;
            });
        }

        private RelayCommand _openPlayerStatsCommand;
        public RelayCommand OpenPlayerStatsCommand
        {
            get => _openPlayerStatsCommand ??= new RelayCommand((obj) =>
            {
                PlayerInfo player = (PlayerInfo)obj;
                _navigator.Navigate(Views.Enums.ViewTypes.Data, player.Nickname);
            });
        }

        private RelayCommand _openMatchFaceitCommand;
        public RelayCommand OpenMatchFaceitCommand
        {
            get => _openMatchFaceitCommand ??= new RelayCommand((obj) =>
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
            });
        }
        
        public LobbyViewModel(IFaceitService faceitService, INavigator navigator, object parameter)
        {
            this._faceitService = faceitService;
            this._navigator = navigator;
            currentMatchId = (string)parameter;
        }

        public override async Task LoadMethod(object obj)
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
            TeamAViewModel = new LobbyTeamInfoViewModel(_faceitService, _navigator, CurrentMatchInfo.TeamA);
            TeamBViewModel = new LobbyTeamInfoViewModel(_faceitService, _navigator, CurrentMatchInfo.TeamB);
        }

        private async Task<MatchInfo> UpdateMatchInfo()
        {
            return await _faceitService.GetMatchInfoAsync(currentMatchId);
        }
    }
}
