using faceitwpf.Models;
using faceitwpf.Models.Abstractions;
using faceitwpf.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace faceitwpf.ViewModels.Controls
{
    class PlayerMapsStatisticsViewModel : BaseViewModel
    {
        private readonly IStatsRepository statsRepository;

        private bool _isLoaded = false;
        private readonly string playerId;

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

        private RelayCommand _loadedCommand;
        public RelayCommand LoadedCommand
        {
            get => _loadedCommand ?? (_loadedCommand = new RelayCommand(async (obj) =>
            {
                if (_isLoaded)
                    return;
                IsLoading = true;

                var playerStats = await statsRepository.GetPlayerStatsAsync(playerId);
                playerStats.MapOverallStats
                .Sort((m1, m2) => 
                    (m2.WinRateDouble * m2.MatchesCount / playerStats.MatchesCount)
                        .CompareTo(m1.WinRateDouble * m1.MatchesCount / playerStats.MatchesCount)
                );
                CurrentPlayerStats = playerStats;

                _isLoaded = true;
                IsLoading = false;
            }));
        }

        public PlayerMapsStatisticsViewModel(IStatsRepository statsRepository, object parameter)
        {
            this.statsRepository = statsRepository;
            playerId = (string)parameter;
        }
    }
}
