namespace FaceitStats.Core.Models
{
    public class PlayerStats
    {
        public int TripleKills { get; set; }

        public int QuadroKills { get; set; }

        public string PentaKills { get; set; }

        public double KDRatio { get; set; }

        public int MVPs { get; set; }

        public double KRRatio { get; set; }

        public int Kills { get; set; }

        public int Assists { get; set; }

        public int Deaths { get; set; }

        public int Headshot { get; set; }

        public int HSPercentage { get; set; }

        public short _Result { set => Result = value == 1 ? 'W' : 'L'; }
        public char Result { get; set; }
    }
}
