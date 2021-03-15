using System;
using System.Collections.Generic;
using System.Linq;

namespace FaceitStats.Core.Models
{
    public class MapStatistics
    {
        public class PlayerMapStatistics
        {
            public string PlayerName { get; set; }
            public int Matches { get; set; }
            public double Winrate { get; set; }
            public PlayerMapStatistics(string mapName, TeamAnalyzeObject player)
            {
                PlayerName = player.Player.Nickname;
                Match[] matches = player.Matches.Where(m => m.RoundStats.Map == mapName).ToArray();
                Matches = matches.Length;
                if (Matches > 0)
                    Winrate = (double)matches.Where(m => m.PlayerStats.Result == 'W').Count() / Matches;
            }

            public PlayerMapStatistics(string playerName, int matches, double winrate)
            {
                PlayerName = playerName;
                Matches = matches;
                Winrate = winrate;
            }
        }
        public string MapName { get; set; }
        public string MapImage { get => $"/faceitwpf;component/Resources/{MapName}.jpeg"; }
        public List<PlayerMapStatistics> Players { get; set; } = new List<PlayerMapStatistics>();
        public PlayerMapStatistics Average { get; set; }

        private MapStatistics(string mapName, List<TeamAnalyzeObject> playersStats)
        {
            MapName = mapName;
            foreach (TeamAnalyzeObject player in playersStats)
            {
                Players.Add(new PlayerMapStatistics(mapName, player));
            }
            CalculateAverage(Players);
        }

        private void CalculateAverage(List<PlayerMapStatistics> players)
        {
            var playersWithMatches = Players.Where(p => p.Matches != 0);
            Average = new PlayerMapStatistics(
                "Average",
                Convert.ToInt32(Players.Select(p => p.Matches).Average()),
                playersWithMatches.Count() > 0 ? playersWithMatches.Select(p => p.Winrate).Average() : 0
            );
        }

        public static List<MapStatistics> CreateList(string[] mapsNames, List<TeamAnalyzeObject> playersStats)
        {
            List<MapStatistics> mapsStatistics = new List<MapStatistics>();
            foreach (string map in mapsNames)
            {
                mapsStatistics.Add(
                    new MapStatistics(map, playersStats)
                );
            }
            return mapsStatistics;
        }
    }
}
