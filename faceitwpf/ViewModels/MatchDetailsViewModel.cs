using faceitwpf.Models;
using faceitwpf.Services;
using faceitwpf.ViewModels.Commands;

namespace faceitwpf.ViewModels
{
    class MatchDetailsViewModel : BaseViewModel
    {
        private RelayCommand _backCommand;
        private readonly IStatsRepository statsRepository;
        private readonly INavigationService navigationService;
        private readonly string matchId;

        public MatchDetailsViewModel(IStatsRepository statsRepository, INavigationService navigationService, object parameter)
        {
            this.statsRepository = statsRepository;
            this.navigationService = navigationService;
            matchId = (string)parameter;
        }


        public RelayCommand BackCommand
        {
            get => _backCommand ?? (_backCommand = new RelayCommand((obj) =>
            {
                navigationService.GoBack();
            }));
        }

    }
}
