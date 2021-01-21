using Newtonsoft.Json;
using System;

namespace faceitwpf.Models
{
    [JsonConverter(typeof(JsonPathConverter))]
    public abstract class BaseMatch
    {
        [JsonProperty("matchId")]
        public string Id { get; set; }

        [JsonProperty]
        public RoundStats Round { get; set; }

        [JsonProperty("date")]
        public long _Date { set => Date = DateTimeOffset.FromUnixTimeMilliseconds(value).ToLocalTime(); }
        public DateTimeOffset Date { get; set; }
    }
}
