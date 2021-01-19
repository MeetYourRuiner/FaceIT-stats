using Newtonsoft.Json;

namespace faceitwpf.Models
{
    public class Match : BaseMatch
    {
        public int Index { get; set; }

        [JsonProperty]
        public PlayerStats PlayerStats { get; set; }

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
