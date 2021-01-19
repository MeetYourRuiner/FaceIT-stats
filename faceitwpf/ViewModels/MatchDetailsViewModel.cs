using faceitwpf.Models;
using faceitwpf.Services;
using faceitwpf.ViewModels.Commands;
using System;
using System.Linq;

namespace faceitwpf.ViewModels
{
    class MatchDetailsViewModel : BaseViewModel
    {
        private readonly IStatsRepository statsRepository;
        private readonly INavigator navigator;

        private bool _isLoaded = false;

        public MatchDetailsViewModel(IStatsRepository statsRepository, INavigator navigator, object parameter)
        {
            this.statsRepository = statsRepository;
            this.navigator = navigator;
            Match = (Match)parameter;
        }

        public Match Match { get; private set; }

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

        public Team TeamA
        {
            get => CurrentMatchDetails?.Teams[0];
        }
        public Team TeamB
        {
            get => CurrentMatchDetails?.Teams[1];
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

        private MatchDetails _currentMatchDetails;
        public MatchDetails CurrentMatchDetails
        {
            get => _currentMatchDetails;
            set
            {
                _currentMatchDetails = value;
                OnPropertyChanged();
            }
        }

        private MatchOverview _currentMatchOverview;
        public MatchOverview CurrentMatchOverview
        {
            get => _currentMatchOverview;
            set
            {
                _currentMatchOverview = value;
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
                    CurrentMatchDetails = await statsRepository.GetMatchDetailsAsync(Match.Id);

                    try
                    {
                        CurrentMatchOverview = await statsRepository.GetMatchOverviewAsync(Match.Id);
                    }
                    catch (Exception ex)
                    {
                        navigator.DisplayError(ex);
                    }

                    if (Match.ChangeELO > 0)
                    {
                        PlusElo = Match.ChangeELO;
                        MinusElo = Match.ChangeELO - 50;
                    }
                    else if (Match.ChangeELO < 0)
                    {
                        MinusElo = Match.ChangeELO;
                        PlusElo = Match.ChangeELO + 50;
                    }
                    else
                    {
                        MinusElo = 0;
                        PlusElo = 0;
                    }
                    if (CurrentMatchOverview != null)
                    {
                        TeamA.Players.ForEach(p =>
                        {
                            p.PlayerOverview = CurrentMatchOverview.TeamA.PlayerOverviews
                                .FirstOrDefault((po) => po.Id == p.PlayerId);
                        });
                        TeamB.Players.ForEach(p =>
                        {
                            p.PlayerOverview = CurrentMatchOverview.TeamB.PlayerOverviews
                                .FirstOrDefault((po) => po.Id == p.PlayerId);
                        });
                    }
                    else
                    {
                        TeamA.Players.ForEach(p =>
                        {
                            p.PlayerOverview = new PlayerOverview();
                        });
                        TeamB.Players.ForEach(p =>
                        {
                            p.PlayerOverview = new PlayerOverview();
                        });
                    }
                    OnPropertyChanged("TeamA");
                    OnPropertyChanged("TeamB");
                }
                catch (Exception ex)
                {
                    navigator.GoBack(ex);
                }
                _isLoaded = true;
                IsLoading = false;
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

        private RelayCommand _openMatchFaceitCommand;
        public RelayCommand OpenMatchFaceitCommand
        {
            get => _openMatchFaceitCommand ?? (_openMatchFaceitCommand = new RelayCommand((obj) =>
            {
                try
                {
                    var sInfo = new System.Diagnostics.ProcessStartInfo($"https://www.faceit.com/en/csgo/room/{Match.Id}")
                    {
                        UseShellExecute = true,
                    };
                    System.Diagnostics.Process.Start(sInfo);
                }
                catch(Exception ex)
                {
                    throw new Exception("Failed to open link in browser", ex);
                }
            }));
        }

        private RelayCommand _openPlayerStatsCommand;
        public RelayCommand OpenPlayerStatsCommand
        {
            get => _openPlayerStatsCommand ?? (_openPlayerStatsCommand = new RelayCommand((obj) =>
            {
                PlayerDetails player = (PlayerDetails)obj;
                navigator.Navigate(Views.Enums.ViewTypes.Data, player.Nickname);
            }));
        }
    }
}
