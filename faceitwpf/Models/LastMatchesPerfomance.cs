using System.Collections.Generic;
using System.Linq;

namespace faceitwpf.Models
{
    class LastMatchesPerfomance
    {
        public double Kills { get; set; }
        public double KR { get; set; }
        public double HS { get; set; }
        public double KD { get; set; }
        public double Winrate { get; set; }

        public LastMatchesPerfomance(List<Match> matches)
        {
            if (matches.Count == 0)
                return;
            var lastMatchesCount = matches.Count > 20 ? 20 : matches.Count;
            var lastMatchesStats = matches.GetRange(0, lastMatchesCount).Select(m => m.PlayerStats);
            Kills = lastMatchesStats.Select(m => m.Kills).Average();
            HS = lastMatchesStats.Select(m => m.HSPercentage).Average();
            KR = lastMatchesStats.Select(m => m.KRRatio).Average();
            KD = lastMatchesStats.Select(m => m.KDRatio).Average();
            Winrate = (double)lastMatchesStats.Where(m => m.Result == 'W').Count() / lastMatchesCount;
        }
    }
}
