using faceitwpf.Models;
using faceitwpf.Services;
using faceitwpf.ViewModels.Commands;

namespace faceitwpf.ViewModels.Controls
{
    class OngoingMatchTeamInfoViewModel : BaseViewModel
    {
        private readonly INavigator navigator;

        private RelayCommand _openPlayerStatsCommand;
        public RelayCommand OpenPlayerStatsCommand
        {
            get => _openPlayerStatsCommand ?? (_openPlayerStatsCommand = new RelayCommand((obj) =>
            {
                PlayerInfo player = (PlayerInfo)obj;
                navigator.Navigate(Views.Enums.ViewTypes.Data, player.Nickname);
            }));
        }

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

        public OngoingMatchTeamInfoViewModel(INavigator navigator, TeamInfo ongoingMatchTeamInfo)
        {
            this.navigator = navigator;
            OngoingMatchTeamInfo = ongoingMatchTeamInfo;
        }
    }
}
