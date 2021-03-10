using FaceitStats.Core.Interfaces;
using FaceitStats.Core.Models;
using FaceitStats.WPF.Services;
using FaceitStats.WPF.ViewModels.Abstractions;
using FaceitStats.WPF.ViewModels.Commands;
using System;
using System.Threading.Tasks;

namespace FaceitStats.WPF.ViewModels
{
    class MatchDetailsViewModel : LoadableViewModel
    {
        private readonly IFaceitRepository _faceitRepository;
        private readonly INavigator _navigator;

        public MatchDetailsViewModel(IFaceitRepository faceitRepository, INavigator navigator, object parameter)
        {
            this._faceitRepository = faceitRepository;
            this._navigator = navigator;
            Match = (Match)parameter;
        }

        public Match Match { get; private set; }

        public Team TeamA
        {
            get => CurrentMatchStats?.Teams[0];
        }
        public Team TeamB
        {
            get => CurrentMatchStats?.Teams[1];
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

        private MatchStats _currentMatchStats;
        public MatchStats CurrentMatchStats
        {
            get => _currentMatchStats;
            set
            {
                _currentMatchStats = value;
                OnPropertyChanged();
                OnPropertyChanged("TeamA");
                OnPropertyChanged("TeamB");
            }
        }
        public override async Task LoadedMethod(object obj)
        {
            try
            {
                CurrentMatchStats = await _faceitRepository.GetMatchStatsAsync(Match.Id);

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

        private RelayCommand _backCommand;
        public RelayCommand BackCommand
        {
            get => _backCommand ?? (_backCommand = new RelayCommand((obj) =>
            {
                _navigator.GoBack();
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
                catch (Exception ex)
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
                _navigator.Navigate(Views.Enums.ViewTypes.Data, player.Nickname);
            }));
        }
    }
}
