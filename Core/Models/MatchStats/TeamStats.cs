using System.Collections.Generic;

namespace FaceitStats.Core.Models
{
    public class TeamStats
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsPremade { get; set; }
        public int FirstHalfScore { get; set; }
        public int SecondHalfScore { get; set; }
        public int FinalScore { get; set; }
        public int TeamWin { get; set; }
        public List<PlayerStats> Players { get; set; }
    }
}
