using FaceitStats.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace FaceitStats.Core.DTOs
{
    public class AveragePerfomance
    {
        public int Matches { get; set; }
        public double Kills { get; set; }
        public double KRRatio { get; set; }
        public double HSPercentage { get; set; }
        public double KDRatio { get; set; }
        public double Winrate { get; set; }

        public AveragePerfomance(List<Match> matches, int matchesCount)
        {
            if (matches.Count == 0)
                return;
            var lastMatchesCount = matches.Count > matchesCount ? matchesCount : matches.Count;
            var lastMatchesStats = matches.GetRange(0, lastMatchesCount).Select(m => m.PlayerStats);
            Matches = lastMatchesCount;
            Kills = lastMatchesStats.Select(m => m.Kills).Average();
            HSPercentage = lastMatchesStats.Select(m => m.HSPercentage).Average();
            KRRatio = lastMatchesStats.Select(m => m.KRRatio).Average();
            KDRatio = lastMatchesStats.Select(m => m.KDRatio).Average();
            Winrate = (double)lastMatchesStats.Where(m => m.Result == 'W').Count() / lastMatchesCount;
        }
        public AveragePerfomance(PlayerOverallStats playerOverallStats)
        {
            Matches = playerOverallStats.Matches;
            Kills = playerOverallStats.MapOverallStats.Select(m => m.Kills).Average();
            HSPercentage = playerOverallStats.HSPercentage;
            KRRatio = playerOverallStats.MapOverallStats.Select(m => m.KRRatio).Average();
            KDRatio = playerOverallStats.KDRatio;
            Winrate = (double)playerOverallStats.Winrate / 100;
        }
    }
}
