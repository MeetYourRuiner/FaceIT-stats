using System;

namespace FaceitStats.Core.Models
{
    public class Match
    {
        public string Id { get; set; }
        public int Index { get; set; }
        public RoundStats RoundStats { get; set; }
        public PlayerStats PlayerStats { get; set; }
        public TeamStats TeamStats { get; set; }
        public long _Date { set => Date = DateTimeOffset.FromUnixTimeMilliseconds(value).ToLocalTime(); }
        public DateTimeOffset Date { get; set; }

        public int ELO { get; set; }
        public int ChangeELO { get; set; } = 0;
        public string ResultELO
        {
            get
            {
                var sign = ChangeELO >= 0 ? "+" : string.Empty;
                return $"{PlayerStats.Result}({sign}{ChangeELO})";
            }
        }
    }
}
