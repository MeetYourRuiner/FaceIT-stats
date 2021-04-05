using System.Collections.Generic;

namespace FaceitStats.Core.Models
{
    public class TeamInfo
    {
        private string name;
        public string Name { get => name; set => name = value.Replace("_", "__"); }
        public List<PlayerInfo> Players { get; set; }
        public int Score { get; set; }
        public int WinElo { get; set; }
        public int LossElo { get; set; }
        public int AverageLevel { get; set; }
        public int Rating { get; set; }
        public double WinProbability { get; set; }
    }
}
