using Newtonsoft.Json;
using System;

namespace faceitwpf.Models
{
    [JsonConverter(typeof(JsonPathConverter))]
    public class Match
    {
        [JsonProperty("match_id")]
        public string Id { get; set; }
        [JsonProperty("started_at")]
        public int _Date { get; set; }
        public DateTimeOffset Date { get; set; }
        public Stats Stats { get; set; }
    }

    public class MatchHistory
    {
        [JsonProperty("items")]
        public Match[] Match { get; set; }
    }
}
