using Newtonsoft.Json;
using System.Collections.Generic;

namespace faceitwpf.Models
{
    [JsonConverter(typeof(JsonPathConverter))]
    public class MatchOverview
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("entity.name")]
        public string CompetitionName { get; set; }
        [JsonProperty("teams.faction1")]
        public TeamOverview TeamA { get; set; }
        [JsonProperty("teams.faction2")]
        public TeamOverview TeamB { get; set; }
        [JsonProperty("entityCustom.parties")]
        public Dictionary<string, string[]> Parties { get; set; }
    }

    [JsonConverter(typeof(JsonPathConverter))]
    public class PlayerOverview
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("gameSkillLevel")]
        public int Level { get; set; }
        [JsonProperty("elo")]
        public int Elo { get; set; }
        public string LevelImage { get => $"/faceitwpf;component/Resources/lvl{Level}.png"; }
        [JsonProperty("avatar")]
        public string Avatar { get; set; }
    }

    [JsonConverter(typeof(JsonPathConverter))]
    public class TeamOverview
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("roster")]
        public List<PlayerOverview> PlayerOverviews { get; set; }
        [JsonProperty("stats.skillLevel.average")]
        public int AverageLevel { get; set; }
        [JsonProperty("stats.rating")]
        public int AverageElo { get; set; }
        public string AverageLevelImage { get => $"/faceitwpf;component/Resources/lvl{AverageLevel}.png"; }
    }
}
