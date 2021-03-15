using FaceitStats.Core.Models;
using FaceitStats.WPF.ViewModels.Abstractions;

namespace FaceitStats.WPF.ViewModels.Controls
{
    class PlayerMapsStatisticsViewModel : BaseViewModel
    {
        private PlayerOverallStats _currentPlayerStats;
        public PlayerOverallStats CurrentPlayerStats
        {
            get { return _currentPlayerStats; }
            set
            {
                _currentPlayerStats = value;
                OnPropertyChanged();
            }
        }

        public PlayerMapsStatisticsViewModel(PlayerOverallStats playerOverallStats)
        {
            CurrentPlayerStats = playerOverallStats;
        }
    }
}
