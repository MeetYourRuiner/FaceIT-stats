using FaceitStats.Core.Interfaces;
using FaceitStats.Core.Models;
using FaceitStats.WPF.Interfaces;
using FaceitStats.WPF.ViewModels.Abstractions;
using FaceitStats.WPF.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FaceitStats.WPF.ViewModels
{
    class MatchDetailsViewModel : LoadableViewModel
    {
        public class Team : BaseViewModel
        {
            public class Player
            {
                public string Id { get => PlayerInfo.Id; }
                public PlayerStats CurrentPlayerStats { get; set; }
                public PlayerInfo PlayerInfo { get; set; }
                public Player(PlayerInfo playerInfo)
                {
                    PlayerInfo = playerInfo;
                }
            }

            public List<Player> Players { get; set; }

            private TeamStats _currentTeamStats;
            public TeamStats CurrentTeamStats
            { 

                get => _currentTeamStats;
                set
                {
                    _currentTeamStats = value;
                    OnPropertyChanged();
                }
            }
            public TeamInfo TeamInfo { get; set; }
        }

        private readonly IFaceitService _faceitService;
        private readonly INavigator _navigator;

        private int currentRoundNumber = 0;

        public Match Match { get; private set; }
        public List<MatchStats> Rounds { get; private set; }

        private MatchInfo _lobby;
        public MatchInfo Lobby 
        { 
            get => _lobby;
            set
            { 
                _lobby = value;
                OnPropertyChanged();
            }
        }

        private Team _teamA;
        public Team TeamA
        {
            get => _teamA;
            set
            {
                _teamA = value;
                OnPropertyChanged();
            }
        }

        private Team _teamB;
        public Team TeamB
        {
            get => _teamB;
            set
            {
                _teamB = value;
                OnPropertyChanged();
            }
        }

        private int _plusElo;
        public int PlusElo
        {
            get => _plusElo;
            set
            {
                _plusElo = value;
                OnPropertyChanged();
            }
        }

        private int _minusElo;
        public int MinusElo
        {
            get => _minusElo;
            set
            {
                _minusElo = value;
                OnPropertyChanged();
            }
        }

        private MatchStats _currentRound;
        public MatchStats CurrentRound
        {
            get => _currentRound;
            set
            {
                _currentRound = value;
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

        private RelayCommand _openMatchFaceitCommand;
        public RelayCommand OpenMatchFaceitCommand
        {
            get => _openMatchFaceitCommand ??= new RelayCommand((obj) =>
            {
                try
                {
                    var sInfo = new System.Diagnostics.ProcessStartInfo($"https://www.faceit.com/en/csgo/room/{Match.Id}")
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
        
        private RelayCommand _openPlayerStatsCommand;
        public RelayCommand OpenPlayerStatsCommand
        {
            get => _openPlayerStatsCommand ??= new RelayCommand((obj) =>
            {
                Team.Player player = (Team.Player)obj;
                _navigator.Navigate(Views.Enums.ViewTypes.Data, player.PlayerInfo.Nickname);
            });
        }

        public MatchDetailsViewModel(IFaceitService faceitService, INavigator navigator, object parameter)
        {
            _faceitService = faceitService;
            _navigator = navigator;
            Match = (Match)parameter;
        }

        public override async Task LoadMethod(object obj)
        {
            try
            {
                Rounds = await _faceitService.GetMatchStatsAsync(Match.Id);
                Lobby = await _faceitService.GetMatchInfoAsync(Match.Id);
                CurrentRound = Rounds[0];

                TeamA = new Team
                {
                    Players = Lobby.TeamA.Players.Select(p => new Team.Player(p)).ToList(),
                    TeamInfo = Lobby.TeamA
                };
                TeamB = new Team
                {
                    Players = Lobby.TeamB.Players.Select(p => new Team.Player(p)).ToList(),
                    TeamInfo = Lobby.TeamB
                };

                SetCurrentRoundNumber(1);

                if (Match.ChangeELO > 0 && Match.ChangeELO < 50)
                {
                    PlusElo = Match.ChangeELO;
                    MinusElo = Match.ChangeELO - 50;
                }
                else if (Match.ChangeELO < 0 && Match.ChangeELO > -50)
                {
                    MinusElo = Match.ChangeELO;
                    PlusElo = Match.ChangeELO + 50;
                }
                else
                {
                    MinusElo = 0;
                    PlusElo = 0;
                }
            }
            catch (Exception ex)
            {
                _navigator.GoBack(ex);
                return;
            }
        }

        private void SetCurrentRoundNumber(int number)
        {
            static void sortPlayers(List<Team.Player> players)
            {
                players.Sort((p1, p2) =>
                {
                    if (p2.CurrentPlayerStats == null)
                        return -1;
                    if (p1.CurrentPlayerStats == null)
                        return 1;
                    return p2.CurrentPlayerStats.Kills.CompareTo(p1.CurrentPlayerStats.Kills);
                });
            };

            currentRoundNumber = number;
            CurrentRound = Rounds.FirstOrDefault(r => r.RoundStats.RoundNumber == currentRoundNumber);
            TeamA.CurrentTeamStats = CurrentRound.TeamA;
            foreach (var player in TeamA.Players)
            {
                player.CurrentPlayerStats = CurrentRound.TeamA.Players.Find((p) => p.Id == player.Id);
            }
            TeamB.CurrentTeamStats = CurrentRound.TeamB;
            foreach (var player in TeamB.Players)
            {
                player.CurrentPlayerStats = CurrentRound.TeamB.Players.Find((p) => p.Id == player.Id);
            }
            sortPlayers(TeamA.Players);
            sortPlayers(TeamB.Players);
        }
    }
}
