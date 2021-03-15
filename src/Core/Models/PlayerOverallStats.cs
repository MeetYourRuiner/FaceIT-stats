using System.Collections.Generic;

namespace FaceitStats.Core.Models
{
    public class PlayerOverallStats
    {
        public string Id { get; set; }
        public int Matches { get; set; }
        public double KDRatio { get; set; }
        public int Winrate { get; set; }
        public int HSPercentage { get; set; }
        public List<MapOverallStats> MapOverallStats { get; set; }
    }

    public class MapOverallStats
    {
        public string MapName { get; set; }
        public string MapImage { get => $"/faceitwpf;component/Resources/{MapName}.jpeg"; }
        public int Matches { get; set; }
        public int Winrate { get; set; }
        public double WinrateDouble { get => (double)Winrate / 100; }
        public double Kills { get; set; }
        public double Deaths { get; set; }
        public double Assists { get; set; }
        public double KDRatio { get; set; }
        public double KRRatio { get; set; }
        public double HSPerMatch { get; set; }
        public int HSPercentage { get; set; }
        public double TriplePerMatch { get; set; }
        public double QuadroPerMatch { get; set; }
        public double AcePerMatch { get; set; }
    }
}
