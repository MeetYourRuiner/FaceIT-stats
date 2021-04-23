namespace FaceitStats.Core.Models
{
    public class PlayerStats
    {
        public string Id { get; set; }
        public string Nickname { get; set; }
        public int TripleKills { get; set; }
        public int QuadroKills { get; set; }
        public int PentaKills { get; set; }

        public double KDRatio { get; set; }

        public int MVPs { get; set; }

        public double KRRatio { get; set; }

        public int Kills { get; set; }

        public int Assists { get; set; }

        public int Deaths { get; set; }

        public int Headshots { get; set; }

        public int HSPercentage { get; set; }

        public int _Result { set => Result = value == 1 ? 'W' : 'L'; }
        public char Result { get; set; }
    }
}
