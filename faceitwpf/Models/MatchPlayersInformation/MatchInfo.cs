using Newtonsoft.Json;

namespace faceitwpf.Models
{
    [JsonConverter(typeof(JsonPathConverter))]
    public class MatchInfo : BaseMatchInfo<PlayerInfo, TeamInfo> { }

    [JsonConverter(typeof(JsonPathConverter))]
    public class PlayerInfo : BasePlayerInfo
    {}

    [JsonConverter(typeof(JsonPathConverter))]
    public class TeamInfo : BaseTeamInfo<PlayerInfo>
    {}
}
