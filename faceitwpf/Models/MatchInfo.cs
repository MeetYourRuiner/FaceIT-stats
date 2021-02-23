using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace faceitwpf.Models
{
    [JsonConverter(typeof(JsonPathConverter))]
    public class MatchInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("entity.name")]
        public string CompetitionName { get; set; }
        [JsonProperty("teams.faction1")]
        public TeamInfo TeamA { get; set; }
        [JsonProperty("teams.faction2")]
        public TeamInfo TeamB { get; set; }

        [JsonProperty("results[0].factions.faction1.score")]
        public int TeamAScore { get; set; }
        [JsonProperty("results[0].factions.faction2.score")]
        public int TeamBScore { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("voting.map.pick[0]")]
        public string Map { get; set; }
        public string MapImage { get => $"/faceitwpf;component/Resources/{Map}.jpeg"; }

        private DateTime date;
        [JsonProperty("createdAt")]
        public DateTime Date { get => date; set => date = value.ToLocalTime(); }
        [JsonProperty("entityCustom.parties")]
        public Dictionary<string, string[]> Parties { get; set; }

        public void FillPartiesIndices()
        {
            if (Parties == null)
                return;
            var partiesArray = Parties.Values.ToArray();
            PlayerInfo player;
            int i = 0;
            foreach(var party in partiesArray)
            {
                if (party.Length == 1)
                    continue;
                for (int j = 0; j < party.Length; j++)
                {
                    player = TeamA.Players.FirstOrDefault(p => p.Id == party[j]);
                    if (player == null)
                    {
                        player = TeamB.Players.FirstOrDefault(p => p.Id == party[j]);
                    }
                    try
                    {
                        player.PartyIndex = i;
                    }
                    catch { }
                    player = null;
                }
                i++;
            }
        }
    }

    [JsonConverter(typeof(JsonPathConverter))]
    public class PlayerInfo
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
        public int PartyIndex { get; set; } = -1;
    }

    [JsonConverter(typeof(JsonPathConverter))]
    public class TeamInfo
    {
        private string name;
        [JsonProperty("name")]
        public string Name { get => name; set => name = value.Replace("_", "__"); }
        [JsonProperty("roster")]
        public List<PlayerInfo> Players { get; set; }
        [JsonProperty("stats.skillLevel.average")]
        public int AverageLevel { get; set; }
        [JsonProperty("stats.rating")]
        public int AverageElo { get; set; }
        public string AverageLevelImage { get => $"/faceitwpf;component/Resources/lvl{AverageLevel}.png"; }
        [JsonProperty("stats.winProbability")]
        public double WinProbability { get; set; }
    }
}
