using Newtonsoft.Json;

namespace faceitwpf.Models
{
    [JsonConverter(typeof(JsonPathConverter))]
    public class Player
    {
        private readonly int[] Levels =
        {
            0, 0, 801, 951, 1101, 1251, 1401, 1551, 1701, 1851, 2001
        };

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

        public string ToDemote { get => Level == 1 ? "∞" : (Elo - Levels[Level] + 1).ToString(); }

        public string ToPromote { get => Level == 10 ? "∞" : (Levels[Level + 1] - Elo).ToString(); }
    }
}
