using Newtonsoft.Json;

namespace faceitwpf.Models
{
    [JsonConverter(typeof(JsonPathConverter))]
    public class PlayerStats
    {
        [JsonProperty("i14")]
        public int TripleKills { get; set; }

        [JsonProperty("i15")]
        public int QuadroKills { get; set; }

        [JsonProperty("i16")]
        public string PentaKills { get; set; }

        [JsonProperty("c2")]
        public double KDRatio { get; set; }

        [JsonProperty("i9")]
        public int MVPs { get; set; }

        [JsonProperty("c3")]
        public double KRRatio { get; set; }

        [JsonProperty("i6")]
        public int Kills { get; set; }

        [JsonProperty("i7")]
        public int Assists { get; set; }

        [JsonProperty("i8")]
        public int Deaths { get; set; }

        [JsonProperty("i13")]
        public int Headshot { get; set; }

        [JsonProperty("c4")]
        public int HSPercentage { get; set; }

        [JsonProperty("i10")]
        public short _Result { set => Result = value == 1 ? 'W' : 'L'; }
        public char Result { get; set; }
    }
}
