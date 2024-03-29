﻿using FaceitStats.Core.DTOs;
using FaceitStats.Core.Interfaces;
using FaceitStats.Core.Models;
using FaceitStats.WPF.Interfaces;
using FaceitStats.WPF.ViewModels.Abstractions;
using FaceitStats.WPF.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FaceitStats.WPF.ViewModels.Controls
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

        private readonly INavigator _navigator;
        private readonly INotifyService _notifyService;
        private readonly IFaceitService _faceitService;

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
            get => _openPlayerStatsCommand ??= new RelayCommand((obj) =>
            {
                LobbyPlayer player = (LobbyPlayer)obj;
                _navigator.Navigate(Views.Enums.ViewTypes.Data, player.PlayerInfo.Nickname);
            });
        }

        private RelayCommand _analyzeCommand;
        public RelayCommand AnalyzeCommand
        {
            get => _analyzeCommand ??= new RelayCommand((obj) =>
            {
                _navigator.Navigate(Views.Enums.ViewTypes.TeamAnalyze, LobbyTeamInfo.Players);
            });
        }

        public LobbyTeamInfoViewModel(IFaceitService faceitService, INavigator navigator, INotifyService notifyService, TeamInfo lobbyTeamInfo)
        {
            _navigator = navigator;
            _notifyService = notifyService;
            LobbyTeamInfo = lobbyTeamInfo;
            _faceitService = faceitService;
        }

        public override async Task LoadMethod(object obj)
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
                        (Func<Task>)(async () =>
                        {
                            PlayerProfile playerProfile;
                            PlayerOverallStats playerStats;
                            List<Match> playerLastMatches;
                            try
                            {
                                playerProfile = await _faceitService.GetProfileByIdAsync(playerInfo.Id);
                            }
                            catch
                            {
                                playerProfile = new PlayerProfile();
                            }

                            try
                            {
                                playerStats = await _faceitService.GetOverallStatsAsync(playerInfo.Id);
                            }
                            catch
                            {
                                playerStats = new PlayerOverallStats();
                            }

                            try
                            {
                                playerLastMatches = await _faceitService.GetMatchesAsync(playerInfo.Id, 20);
                            }
                            catch
                            {
                                playerLastMatches = new List<Match>();
                            }

                            var omp = lobbyPlayers.FirstOrDefault((player) => player.PlayerInfo.Id == playerInfo.Id);
                            omp.PlayerProfile = playerProfile;
                            omp.PlayerOverallStats = playerStats;
                            omp.LastMatches = playerLastMatches;
                        })
                    )
                );
            }
            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _notifyService.DisplayError(ex);
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
