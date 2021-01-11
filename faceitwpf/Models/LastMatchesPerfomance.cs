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
            var lastMatches = matches.GetRange(0, lastMatchesCount);
            Kills = lastMatches.Select(m => m.Kills).Average();
            HS = lastMatches.Select(m => m.HSPercentage).Average();
            KR = lastMatches.Select(m => m.KRRatio).Average();
            KD = lastMatches.Select(m => m.KDRatio).Average();
            Winrate = (double)lastMatches.Where(m => m.Result == 'W').Count() / lastMatchesCount;
        }
    }
}
