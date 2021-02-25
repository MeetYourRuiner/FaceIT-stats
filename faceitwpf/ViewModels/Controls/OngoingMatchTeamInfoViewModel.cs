using faceitwpf.Models;
using faceitwpf.Models.Abstractions;
using faceitwpf.Services;
using faceitwpf.ViewModels.Abstractions;
using faceitwpf.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace faceitwpf.ViewModels.Controls
{
    class OngoingMatchTeamInfoViewModel : LoadableViewModel
    {
        public class OngoingMatchPlayerObject
        {
            public OngoingMatchPlayerObject(PlayerInfo playerInfo)
            {
                PlayerInfo = playerInfo;
            }

            public PlayerInfo PlayerInfo { get; set; }
            public PlayerOverallStats PlayerOverallStats { get; set; }
            public PlayerProfile PlayerProfile { get; set; }
            public List<Match> LastMatches { get; set; }
            public AveragePerfomance LastMatchesPerfomance { get; set; }
        }

        private readonly INavigator navigator;
        private readonly IStatsRepository statsRepository;

        private TeamInfo _ongoingMatchTeamInfo;
        public TeamInfo OngoingMatchTeamInfo
        {
            get { return _ongoingMatchTeamInfo; }
            set
            {
                _ongoingMatchTeamInfo = value;
                OnPropertyChanged();
            }
        }

        private List<OngoingMatchPlayerObject> _ongoingMatchPlayers;
        public List<OngoingMatchPlayerObject> OngoingMatchPlayers
        {
            get { return _ongoingMatchPlayers; }
            set
            {
                _ongoingMatchPlayers = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _openPlayerStatsCommand;
        public RelayCommand OpenPlayerStatsCommand
        {
            get => _openPlayerStatsCommand ?? (_openPlayerStatsCommand = new RelayCommand((obj) =>
            {
                OngoingMatchPlayerObject player = (OngoingMatchPlayerObject)obj;
                navigator.Navigate(Views.Enums.ViewTypes.Data, player.PlayerInfo.Nickname);
            }));
        }

        private RelayCommand _analyzeCommand;
        public RelayCommand AnalyzeCommand
        {
            get => _analyzeCommand ?? (_analyzeCommand = new RelayCommand((obj) =>
            {
                navigator.Navigate(Views.Enums.ViewTypes.TeamAnalyze, OngoingMatchTeamInfo.Players);
            }));
        }

        public OngoingMatchTeamInfoViewModel(INavigator navigator, IStatsRepository statsRepository, TeamInfo ongoingMatchTeamInfo)
        {
            this.navigator = navigator;
            this.statsRepository = statsRepository;
            OngoingMatchTeamInfo = ongoingMatchTeamInfo;
        }

        public override async Task LoadedMethod(object obj)
        {
            List<OngoingMatchPlayerObject> ongoingMatchPlayers = new List<OngoingMatchPlayerObject>();
            foreach (var playerInfo in OngoingMatchTeamInfo.Players)
            {
                ongoingMatchPlayers.Add(new OngoingMatchPlayerObject(playerInfo));
            }
            OngoingMatchPlayers = ongoingMatchPlayers;

            var tasks = new List<Task>();
            foreach (var playerInfo in OngoingMatchTeamInfo.Players)
            {
                tasks.Add(
                    Task.Run(
                        async () =>
                        {
                            PlayerProfile playerProfile;
                            PlayerOverallStats playerStats;
                            List<Match> playerLastMatches;
                            try
                            {
                                playerProfile = await statsRepository.GetPlayerProfileByIdAsync(playerInfo.Id);
                            }
                            catch
                            {
                                playerProfile = new PlayerProfile();
                            }

                            try
                            {
                                playerStats = await statsRepository.GetPlayerStatsAsync(playerInfo.Id);
                            }
                            catch
                            {
                                playerStats = new PlayerOverallStats();
                            }

                            try
                            {
                                playerLastMatches = await statsRepository.GetMatchesAsync(playerInfo.Id, 20);
                            }
                            catch
                            {
                                playerLastMatches = new List<Match>();
                            }

                            var omp = ongoingMatchPlayers?.FirstOrDefault((player) => player.PlayerInfo.Id == playerInfo.Id);
                            omp.PlayerProfile = playerProfile;
                            omp.PlayerOverallStats = playerStats;
                            omp.LastMatches = playerLastMatches;
                        }
                    )
                );
            }
            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                navigator.DisplayError(ex);
            }
            foreach (var playerInfo in ongoingMatchPlayers)
            {
                playerInfo.LastMatchesPerfomance = new AveragePerfomance(playerInfo.LastMatches, 20);
            }
            OngoingMatchPlayers = null;
            OngoingMatchPlayers = ongoingMatchPlayers;
        }
    }
}
