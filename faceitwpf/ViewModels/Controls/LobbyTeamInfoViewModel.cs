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
    class LobbyTeamInfoViewModel : LoadableViewModel
    {
        public class LobbyPlayer
        {
            public LobbyPlayer(PlayerInfo playerInfo)
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

        private TeamInfo _lobbyTeamInfo;
        public TeamInfo LobbyTeamInfo
        {
            get { return _lobbyTeamInfo; }
            set
            {
                _lobbyTeamInfo = value;
                OnPropertyChanged();
            }
        }

        private List<LobbyPlayer> _lobbyPlayers;
        public List<LobbyPlayer> LobbyPlayers
        {
            get { return _lobbyPlayers; }
            set
            {
                _lobbyPlayers = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _openPlayerStatsCommand;
        public RelayCommand OpenPlayerStatsCommand
        {
            get => _openPlayerStatsCommand ?? (_openPlayerStatsCommand = new RelayCommand((obj) =>
            {
                LobbyPlayer player = (LobbyPlayer)obj;
                navigator.Navigate(Views.Enums.ViewTypes.Data, player.PlayerInfo.Nickname);
            }));
        }

        private RelayCommand _analyzeCommand;
        public RelayCommand AnalyzeCommand
        {
            get => _analyzeCommand ?? (_analyzeCommand = new RelayCommand((obj) =>
            {
                navigator.Navigate(Views.Enums.ViewTypes.TeamAnalyze, LobbyTeamInfo.Players);
            }));
        }

        public LobbyTeamInfoViewModel(INavigator navigator, IStatsRepository statsRepository, TeamInfo lobbyTeamInfo)
        {
            this.navigator = navigator;
            this.statsRepository = statsRepository;
            LobbyTeamInfo = lobbyTeamInfo;
        }

        public override async Task LoadedMethod(object obj)
        {
            List<LobbyPlayer> lobbyPlayers = new List<LobbyPlayer>();
            foreach (var playerInfo in LobbyTeamInfo.Players)
            {
                lobbyPlayers.Add(new LobbyPlayer(playerInfo));
            }
            LobbyPlayers = lobbyPlayers;

            var tasks = new List<Task>();
            foreach (var playerInfo in LobbyTeamInfo.Players)
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

                            var omp = lobbyPlayers.FirstOrDefault((player) => player.PlayerInfo.Id == playerInfo.Id);
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
            foreach (var playerInfo in lobbyPlayers)
            {
                playerInfo.LastMatchesPerfomance = new AveragePerfomance(playerInfo.LastMatches, 20);
            }
            LobbyPlayers = null;
            LobbyPlayers = lobbyPlayers;
        }
    }
}
