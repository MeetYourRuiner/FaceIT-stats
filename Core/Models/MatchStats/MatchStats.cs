using System;
using System.Collections.Generic;

namespace FaceitStats.Core.Models
{
    public class MatchStats
    {
        public string Id { get; set; }
        public RoundStats RoundStats { get; set; }
        public long _Date { set => Date = DateTimeOffset.FromUnixTimeMilliseconds(value).ToLocalTime(); }
        public DateTimeOffset Date { get; set; }
        public string CompetitionName { get; set; }
        public List<Team> Teams { get; set; }
    }

    public class Team
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsPremade { get; set; }
        public TeamStats TeamStats { get; set; }
        public List<PlayerDetails> Players { get; set; }
    }

    public class TeamStats
    {
        public int FirstHalfScore { get; set; }
        public int SecondHalfScore { get; set; }
        public int FinalScore { get; set; }
        public int TeamWin { get; set; }
    }

    public class PlayerDetails
    {
        public string Id { get; set; }
        public string Nickname { get; set; }
        public PlayerStats PlayerStats { get; set; }
        public PlayerInfo PlayerInfo { get; set; }
    }
}
