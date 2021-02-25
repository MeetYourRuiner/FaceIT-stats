using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace faceitwpf.Models
{
    [JsonConverter(typeof(JsonPathConverter))]
    public class MatchStats
    {
        [JsonProperty("matchId")]
        public string Id { get; set; }
        [JsonProperty]
        public RoundStats RoundStats { get; set; }
        [JsonProperty("date")]
        public long _Date { set => Date = DateTimeOffset.FromUnixTimeMilliseconds(value).ToLocalTime(); }
        public DateTimeOffset Date { get; set; }
        public string CompetitionName { get; set; }
        [JsonProperty("teams")]
        public List<Team> Teams { get; set; }
    }

    [JsonConverter(typeof(JsonPathConverter))]
    public class Team
    {
        [JsonProperty("teamId")]
        public string Id { get; set; }
        [JsonProperty("i5")]
        public string Name { get; set; }
        [JsonProperty("premade")]
        public bool IsPremade { get; set; }
        [JsonProperty]
        public TeamStats TeamStats { get; set; }
        [JsonProperty("players")]
        public List<PlayerDetails> Players { get; set; }
    }

    [JsonConverter(typeof(JsonPathConverter))]
    public class TeamStats
    {
        [JsonProperty("i3")]
        public int FirstHalfScore { get; set; }
        [JsonProperty("i4")]
        public int SecondHalfScore { get; set; }
        [JsonProperty("c5")]
        public int FinalScore { get; set; }
        [JsonProperty("i17")]
        public int TeamWin { get; set; }
    }

    [JsonConverter(typeof(JsonPathConverter))]
    public class PlayerDetails
    {
        [JsonProperty("playerId")]
        public string Id { get; set; }
        [JsonProperty("nickname")]
        public string Nickname { get; set; }
        [JsonProperty]
        public PlayerStats PlayerStats { get; set; }
        public PlayerInfo PlayerInfo { get; set; }
    }
}
