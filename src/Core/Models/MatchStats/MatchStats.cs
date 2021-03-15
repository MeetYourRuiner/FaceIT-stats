using System;

namespace FaceitStats.Core.Models
{
    public class MatchStats
    {
        public string Id { get; set; }
        public RoundStats RoundStats { get; set; }
        public long _Date { set => Date = DateTimeOffset.FromUnixTimeMilliseconds(value).ToLocalTime(); }
        public DateTimeOffset Date { get; set; }
        public string CompetitionName { get; set; }
        public TeamStats TeamA { get; set; }
        public TeamStats TeamB { get; set; }
    }
}
