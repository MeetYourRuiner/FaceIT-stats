using Newtonsoft.Json;
using System.Collections.Generic;

namespace faceitwpf.Models
{
    public abstract class BaseMatchInfo<PlayerInfoType, TeamInfoType> 
        where PlayerInfoType : BasePlayerInfo 
        where TeamInfoType : BaseTeamInfo<PlayerInfoType>
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("entity.name")]
        public string CompetitionName { get; set; }
        [JsonProperty("teams.faction1")]
        public TeamInfoType TeamA { get; set; }
        [JsonProperty("teams.faction2")]
        public TeamInfoType TeamB { get; set; }
        [JsonProperty("entityCustom.parties")]
        public Dictionary<string, string[]> Parties { get; set; }
    }

    [JsonConverter(typeof(JsonPathConverter))]
    public abstract class BasePlayerInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("avatar")]
        public string Avatar { get; set; }
        [JsonProperty("nickname")]
        public string Nickname { get; set; }
        [JsonProperty("gameSkillLevel")]
        public int Level { get; set; }
        [JsonProperty("elo")]
        public int Elo { get; set; }
        public string LevelImage { get => $"/faceitwpf;component/Resources/lvl{Level}.png"; }
    }

    [JsonConverter(typeof(JsonPathConverter))]
    public abstract class BaseTeamInfo<PlayerInfoType>
        where PlayerInfoType : BasePlayerInfo
    {
        private string name;
        [JsonProperty("name")]
        public string Name { get => name; set => name = value.Replace("_", "__"); }
        [JsonProperty("roster")]
        public List<PlayerInfoType> Players { get; set; }
        [JsonProperty("stats.skillLevel.average")]
        public int AverageLevel { get; set; }
        [JsonProperty("stats.rating")]
        public int AverageElo { get; set; }
        public string AverageLevelImage { get => $"/faceitwpf;component/Resources/lvl{AverageLevel}.png"; }
    }
}
