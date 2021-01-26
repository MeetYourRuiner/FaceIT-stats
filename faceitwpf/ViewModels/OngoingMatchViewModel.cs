using faceitwpf.Models;
using faceitwpf.Models.Abstractions;
using faceitwpf.Services;
using faceitwpf.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace faceitwpf.ViewModels
{
    class OngoingMatchViewModel : BaseViewModel
    {
        private readonly IStatsRepository statsRepository;
        private readonly INavigator navigator;

        private string currentMatchId;

        private bool _isLoaded = false;

        public bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

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
                    await UpdateMatchInfo();
                }
                catch (Exception ex)
                {
                    navigator.GoBack(ex);
                    return;
                }
                IsLoading = false;
                _isLoaded = true;
            }));
        }

        private RelayCommand _backCommand;
        public RelayCommand BackCommand
        {
            get => _backCommand ?? (_backCommand = new RelayCommand((obj) =>
            {
                navigator.GoBack();
            }));
        }

        private OngoingMatchInfo _currentMatchInfo;
        public OngoingMatchInfo CurrentMatchInfo
        {
            get => _currentMatchInfo;
            set
            {
                _currentMatchInfo = value;
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
                    await UpdateMatchInfo();
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
                OngoingMatchPlayerInfo player = (OngoingMatchPlayerInfo)obj;
                navigator.Navigate(Views.Enums.ViewTypes.Data, player.Nickname);
            }));
        }

        private RelayCommand _analyzeCommand;
        public RelayCommand AnalyzeCommand
        {
            get => _analyzeCommand ?? (_analyzeCommand = new RelayCommand((obj) =>
            {
                string team = (string)obj;
                List<BasePlayerInfo> players;
                if (team == "A")
                    players = CurrentMatchInfo.TeamA.Players.ConvertAll(p => (BasePlayerInfo)p);
                else
                    players = CurrentMatchInfo.TeamB.Players.ConvertAll(p => (BasePlayerInfo)p);
                navigator.Navigate(Views.Enums.ViewTypes.TeamAnalyze, players);
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
    
        private async Task UpdateMatchInfo()
        {
            CurrentMatchInfo = await statsRepository.GetOngoingMatchAsync(currentMatchId);
        }
    }
}
