using FaceitStats.Core.Models;
using FaceitStats.Core.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace FaceitStats.Infrastructure.JSON.Strategies
{
    class MatchInfoParseStrategy : IStrategy
    {
        public object Parse(JToken jToken)
        {
            static TeamInfo ParseTeamInfo(JToken jToken)
            {
                try
                {
                    JToken statsToken = jToken["stats"];
                    bool isStatsNull = statsToken == null;
                    var teamInfo = new TeamInfo
                    {
                        Name = (string)jToken.SelectToken("name"),
                        Players = new List<PlayerInfo>()
                    };
                    if (!isStatsNull)
                    {
                        teamInfo.AverageLevel = (int)(statsToken.SelectToken("skillLevel.average") ?? 0);
                        teamInfo.Rating = (int)(statsToken.SelectToken("rating") ?? 0);
                        teamInfo.WinProbability = (double)(statsToken.SelectToken("winProbability") ?? 0);
                        (int gain, int loss) = EloCalculator.CalculateEloChange(teamInfo.WinProbability);
                        teamInfo.WinElo = gain;
                        teamInfo.LossElo = loss;
                    }
                    JToken playersToken = jToken["roster"];
                    foreach (var player in playersToken)
                    {
                        var playerInfo = new PlayerInfo
                        {
                            Id = (string)player.SelectToken("id"),
                            Avatar = (string)player.SelectToken("avatar"),
                            Nickname = (string)player.SelectToken("nickname"),
                            Level = (int)(player.SelectToken("gameSkillLevel") ?? 0),
                            Elo = (int)(player.SelectToken("elo") ?? 0),
                        };
                        teamInfo.Players.Add(playerInfo);
                    }
                    return teamInfo;
                }
                catch
                {
                    throw;
                }
            }

            try
            {
                var matchInfo = new MatchInfo
                {
                    Id = (string)jToken.SelectToken("id"),
                    CompetitionName = (string)jToken.SelectToken("entity.name"),
                    TeamA = ParseTeamInfo(jToken["teams"]["faction1"]),
                    TeamB = ParseTeamInfo(jToken["teams"]["faction2"]),
                    State = (string)jToken.SelectToken("state"),
                    Map = (string)jToken.SelectToken("voting.map.pick[0]"),
                    BestOf = (int)(jToken.SelectToken("matchCustom.overview.round.to_play") ?? 0),
                    Date = (DateTime)jToken.SelectToken("createdAt"),
                    Parties = jToken["entityCustom"]["parties"]?.ToObject<Dictionary<string, string[]>>(),
                };
                JToken resultsToken = jToken["results"];
                if (resultsToken != null)
                {
                    matchInfo.TeamA.Score = (int)(resultsToken[0].SelectToken("factions.faction1.score") ?? 0);
                    matchInfo.TeamB.Score = (int)(resultsToken[0].SelectToken("factions.faction2.score") ?? 0);
                }
                return matchInfo;
            }
            catch
            {
                throw;
            }
        }
    }
}
