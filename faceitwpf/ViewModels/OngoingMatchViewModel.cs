using faceitwpf.Models;
using faceitwpf.Models.Abstractions;
using faceitwpf.Services;
using faceitwpf.ViewModels.Abstractions;
using faceitwpf.ViewModels.Commands;
using faceitwpf.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace faceitwpf.ViewModels
{
    class OngoingMatchViewModel : LoadableViewModel
    {
        private readonly IStatsRepository statsRepository;
        private readonly INavigator navigator;

        private string currentMatchId;

        public bool _isRefreshing = false;
        public bool IsRefreshing 
        {
            get => _isRefreshing ;
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
                navigator.GoBack(ex);
                return;
            }
            TeamAViewModel = new OngoingMatchTeamInfoViewModel(navigator, statsRepository, CurrentMatchInfo.TeamA);
            TeamBViewModel = new OngoingMatchTeamInfoViewModel(navigator, statsRepository, CurrentMatchInfo.TeamB);
        }

        private RelayCommand _backCommand;
        public RelayCommand BackCommand
        {
            get => _backCommand ?? (_backCommand = new RelayCommand((obj) =>
            {
                navigator.GoBack();
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

        private OngoingMatchTeamInfoViewModel _teamAViewModel;
        public OngoingMatchTeamInfoViewModel TeamAViewModel
        {
            get => _teamAViewModel;
            set
            {
                _teamAViewModel = value;
                OnPropertyChanged();
            }
        }

        private OngoingMatchTeamInfoViewModel _teamBViewModel;
        public OngoingMatchTeamInfoViewModel TeamBViewModel
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
                catch(Exception ex)
                {
                    navigator.DisplayError(ex);
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
                navigator.Navigate(Views.Enums.ViewTypes.Data, player.Nickname);
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
        public OngoingMatchViewModel(IStatsRepository statsRepository, INavigator navigator, object parameter)
        {
            this.statsRepository = statsRepository;
            this.navigator = navigator;
            currentMatchId = (string)parameter;
        }
    
        private async Task<MatchInfo> UpdateMatchInfo()
        {
            return await statsRepository.GetMatchInfoAsync(currentMatchId);
        }
    }
}
