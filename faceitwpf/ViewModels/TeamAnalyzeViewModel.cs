﻿using faceitwpf.Models;
using faceitwpf.Models.Abstractions;
using faceitwpf.Services;
using faceitwpf.ViewModels.Abstractions;
using faceitwpf.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace faceitwpf.ViewModels
{
    class TeamAnalyzeViewModel : LoadableViewModel
    {
        private const int MATCHES_TO_ANALYZE = 200;
        private readonly string[] maps = new string[]
        {
            "de_mirage",
            "de_cache",
            "de_dust2",
            "de_inferno",
            "de_nuke",
            "de_train",
            "de_overpass",
            "de_vertigo"
        };

        private readonly IStatsRepository statsRepository;
        private readonly INavigator navigator;

        private List<TeamAnalyzeObject> playersStats = new List<TeamAnalyzeObject>();
        private List<MapStatistics> mapsStatistics = new List<MapStatistics>();

        private DataTable _dataTable;
        public DataTable DataTable
        {
            get => _dataTable;
            set
            {
                _dataTable = value;
                OnPropertyChanged();
            }
        }

        private List<PlayerInfo> _players;
        public List<PlayerInfo> Players
        {
            get => _players;
            set
            {
                _players = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _backCommand;
        public RelayCommand BackCommand
        {
            get => _backCommand ?? (_backCommand = new RelayCommand((obj) =>
            {
                navigator.GoBack();
            }));
        }

        public override async Task LoadedMethod(object obj)
        {
            try
            {
                foreach (var player in Players)
                {
                    List<Match> matches = await statsRepository.GetMatchesAsync(player.Id, MATCHES_TO_ANALYZE);
                    playersStats.Add(new TeamAnalyzeObject(player, matches));
                }
            }
            catch (Exception ex)
            {
                navigator.GoBack(ex);
                return;
            }
            mapsStatistics = MapStatistics.CreateList(maps, playersStats);
            double matchesCount = mapsStatistics.Select(m => m.Average.Matches).Sum();
            mapsStatistics.Sort((m1, m2) => (m2.Average.Winrate * m2.Average.Matches / matchesCount).CompareTo(m1.Average.Winrate * m1.Average.Matches / matchesCount));
            DataTable = CreateDataTable(mapsStatistics);
        }

        public TeamAnalyzeViewModel(IStatsRepository statsRepository, INavigator navigator, object parameter)
        {
            this.statsRepository = statsRepository;
            this.navigator = navigator;
            Players = (List<PlayerInfo>)parameter;
        }

        private DataTable CreateDataTable(List<MapStatistics> mapsStats)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("map");
            dataTable.Columns.Add("average_matches");
            dataTable.Columns.Add("average_winrate", typeof(double));
            for (int i = 0; i < playersStats.Count; i++)
            {
                dataTable.Columns.Add($"matches_{i}");
                dataTable.Columns.Add($"winrate_{i}", typeof(double));
            }
            foreach (MapStatistics mapStats in mapsStats)
            {
                DataRow row = dataTable.NewRow();
                row["map"] = mapStats.MapImage;
                row["average_matches"] = mapStats.Average.Matches;
                row["average_winrate"] = mapStats.Average.Winrate;
                for (int j = 0; j < mapStats.Players.Count; j++)
                {
                    MapStatistics.PlayerMapStatistics player = mapStats.Players[j];
                    row[$"matches_{j}"] = player.Matches;
                    row[$"winrate_{j}"] = player.Winrate;
                }
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }
    }
}
