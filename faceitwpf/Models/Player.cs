using Newtonsoft.Json;

namespace faceitwpf.Models
{
    [JsonConverter(typeof(JsonPathConverter))]
    public class Player
    {
        [JsonProperty("nickname")]
        public string Nickname { get; set; }
        [JsonProperty("player_id")]
        public string PlayerID { get; set; }
        [JsonProperty("avatar")]
        public string Avatar { get; set; }
        [JsonProperty("cover_image")]
        public string CoverImage { get; set; }
        [JsonProperty("games.csgo.skill_level")]
        public int Level { get; set; }
        [JsonProperty("games.csgo.faceit_elo")]
        public int Elo { get; set; }
    }
}
