using Newtonsoft.Json;

namespace faceitwpf.Models
{
    [JsonConverter(typeof(JsonPathConverter))]
    public class RoundStats
    {
        [JsonProperty("i18")]
        public string Score { get; set; }
        [JsonProperty("i1")]
        public string Map { get; set; }
        public string MapImage { get => $"/faceitwpf;component/Resources/{Map}.jpeg"; }
    }
}
