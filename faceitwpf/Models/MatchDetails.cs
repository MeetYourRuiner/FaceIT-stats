using Newtonsoft.Json;
using System.Collections.Generic;

namespace faceitwpf.Models
{
    [JsonConverter(typeof(JsonPathConverter))]
    public class MatchDetails
    {
        [JsonProperty("round_stats")]
        public RoundStats RoundStats { get; set; }
        [JsonProperty("teams")]
        public List<Team> Teams { get; set; }
    }

    public class RoundStats
    {
        public string Score { get; set; }
        public string Map { get; set; }
        public string MapImage { get => $"/faceitwpf;component/Resources/{Map}.jpeg"; }
    }

    public class TeamStats
    {
        [JsonProperty("First Half Score")]
        public int FirstHalfScore { get; set; }
        public string Team { get; set; }
        //[JsonProperty("Team Headshot")]
        //public string TeamHeadshot { get; set; }
        [JsonProperty("Final Score")]
        public int FinalScore { get; set; }
        [JsonProperty("Second Half Score")]
        public int SecondHalfScore { get; set; }
        [JsonProperty("Team Win")]
        public int TeamWin { get; set; }
        [JsonProperty("Overtime score")]
        public int Overtimescore { get; set; }
    }

    public class PlayerStats
    {
        [JsonProperty("Triple Kills")]
        public int TripleKills { get; set; }
        [JsonProperty("Quadro Kills")]
        public int QuadroKills { get; set; }
        [JsonProperty("K/D Ratio")]
        public double KDRatio { get; set; }
        public int MVPs { get; set; }
        [JsonProperty("K/R Ratio")]
        public double KRRatio { get; set; }
        [JsonProperty("Penta Kills")]
        public string PentaKills { get; set; }
        public int Kills { get; set; }
        public int Assists { get; set; }
        public int Deaths { get; set; }
        public int Headshot { get; set; }
        [JsonProperty("Headshots %")]
        public int HSPercentage { get; set; }
        public string Result { get; set; }
    }

    public class PlayerDetails
    {
        [JsonProperty("player_id")]
        public string PlayerId { get; set; }
        [JsonProperty("nickname")]
        public string Nickname { get; set; }
        [JsonProperty("player_stats")]
        public PlayerStats PlayerStats { get; set; }
        public PlayerOverview PlayerOverview { get; set; }
    }

    public class Team
    {
        [JsonProperty("team_id")]
        public string TeamId { get; set; }
        [JsonProperty("premade")]
        public bool IsPremade { get; set; }
        [JsonProperty("team_stats")]
        public TeamStats TeamStats { get; set; }
        [JsonProperty("players")]
        public List<PlayerDetails> Players { get; set; }
    }
}
