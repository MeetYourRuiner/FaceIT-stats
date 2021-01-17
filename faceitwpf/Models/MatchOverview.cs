using Newtonsoft.Json;
using System.Collections.Generic;

namespace faceitwpf.Models
{
    [JsonConverter(typeof(JsonPathConverter))]
    public class MatchOverview
    {
        [JsonProperty("match_id")]
        public string Id { get; set; }
        [JsonProperty("teams.faction1")]
        public TeamOverview TeamA { get; set; }
        [JsonProperty("teams.faction2")]
        public TeamOverview TeamB { get; set; }
    }

    public class PlayerOverview
    {
        [JsonProperty("player_id")]
        public string Id { get; set; }
        [JsonProperty("skill_level")]
        public int Level { get; set; }
        public string LevelImage { get => $"/faceitwpf;component/Resources/lvl{Level}.png"; }
        [JsonProperty("avatar")]
        public string Avatar { get; set; }
    }

    public class TeamOverview
    {
        [JsonProperty("team_id")]
        public string Id { get; set; }
        [JsonProperty("players")]
        public List<PlayerOverview> PlayerOverviews { get; set; }
    }
}
