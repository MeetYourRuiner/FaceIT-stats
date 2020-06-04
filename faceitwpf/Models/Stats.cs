using Newtonsoft.Json;

namespace faceitwpf.Models
{
    [JsonConverter(typeof(JsonPathConverter))]
    public class Stats
    {
        [JsonProperty("rounds[0].round_stats.Map")]
        public string Map { get; set; }
        [JsonProperty("rounds[0].round_stats.Score")]
        public string Score { get; set; }
        public char Result { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public double KDRatio { get; set; }
        public double KRRatio { get; set; }
    }
}
