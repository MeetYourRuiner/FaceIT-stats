using Newtonsoft.Json;
using System;

namespace faceitwpf.Models
{
    [JsonConverter(typeof(JsonPathConverter))]
    public class Match
    {
        [JsonProperty("matchId")]
        public string Id { get; set; }
        public int Index { get; set; }

        [JsonProperty]
        public RoundStats RoundStats { get; set; }

        [JsonProperty]
        public PlayerStats PlayerStats { get; set; }

        [JsonProperty("date")]
        public long _Date { set => Date = DateTimeOffset.FromUnixTimeMilliseconds(value).ToLocalTime(); }
        public DateTimeOffset Date { get; set; }

        [JsonProperty("elo")]
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
