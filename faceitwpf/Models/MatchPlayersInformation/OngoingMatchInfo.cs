using faceitwpf.Models.Abstractions;
using Newtonsoft.Json;
using System;

namespace faceitwpf.Models
{
    [JsonConverter(typeof(JsonPathConverter))]
    public class OngoingMatchInfo : BaseMatchInfo<OngoingMatchPlayerInfo, OngoingMatchTeamInfo>
    {

        [JsonProperty("results[0].factions.faction1.score")]
        public int TeamAScore { get; set; }
        [JsonProperty("results[0].factions.faction2.score")]
        public int TeamBScore { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("voting.map.pick[0]")]
        public string Map { get; set; }
        public string MapImage { get => $"/faceitwpf;component/Resources/{Map}.jpeg"; }

        private DateTime date;
        [JsonProperty("createdAt")]
        public DateTime Date { get => date; set => date = value.ToLocalTime(); }
    }

    [JsonConverter(typeof(JsonPathConverter))]
    public class OngoingMatchPlayerInfo : BasePlayerInfo
    {    }

    [JsonConverter(typeof(JsonPathConverter))]
    public class OngoingMatchTeamInfo : BaseTeamInfo<OngoingMatchPlayerInfo>
    {    }
}
