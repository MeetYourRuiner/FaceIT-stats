using faceitwpf.Models;
using faceitwpf.Services;
using faceitwpf.ViewModels.Commands;
using System.Linq;

namespace faceitwpf.ViewModels
{
    class MatchDetailsViewModel : BaseViewModel
    {
        private readonly IStatsRepository statsRepository;
        private readonly INavigator navigator;

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

        private MatchDetails _currentMatchDetails;
        public MatchDetails CurrentMatchDetails
        {
            get => _currentMatchDetails;
            set
            {
                _currentMatchDetails = value;
                OnPropertyChanged();
                OnPropertyChanged("TeamA");
                OnPropertyChanged("TeamB");
            }
        }

        private RelayCommand _loadedCommand;
        public RelayCommand LoadedCommand
        {
            get => _loadedCommand ?? (_loadedCommand = new RelayCommand(async (obj) =>
            {
                IsLoading = true;
                CurrentMatchDetails = await statsRepository.GetMatchDetailsAsync(Match.Id);
                if (Match.MatchOverview != null)
                {
                    TeamA.Players.ForEach(p =>
                    {
                        p.PlayerOverview = Match.MatchOverview.TeamA.PlayerOverviews
                            .FirstOrDefault((po) => po.Id == p.PlayerId);
                    });
                    TeamB.Players.ForEach(p =>
                    {
                        p.PlayerOverview = Match.MatchOverview.TeamB.PlayerOverviews
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

        private RelayCommand _openMatchFaceit;
        public RelayCommand OpenMatchFaceit
        {
            get => _openMatchFaceit ?? (_openMatchFaceit = new RelayCommand((obj) =>
            {
                var sInfo = new System.Diagnostics.ProcessStartInfo($"https://www.faceit.com/en/csgo/room/{Match.Id}")
                {
                    UseShellExecute = true,
                };
                System.Diagnostics.Process.Start(sInfo);
            }));
        }

    }
}
