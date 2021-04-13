using FaceitStats.Core.Models;
using FaceitStats.Infrastructure.Constants;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace FaceitStats.Infrastructure.JSON.Strategies
{
    class TeamStatsParseStrategy : IStrategy
    {
        public object Parse(JToken jToken)
        {
            var playerStatsParser = new JsonParser(new PlayerStatsParseStrategy());
            try
            {
                return new TeamStats
                {
                    Id = (string)jToken.SelectToken("teamId"),
                    Name = (string)jToken.SelectToken(MatchStatsConstants.TeamName),
                    IsPremade = (bool)(jToken.SelectToken("premade") ?? false),
                    FirstHalfScore = (int)(jToken.SelectToken(MatchStatsConstants.FirstHalfScore) ?? 0),
                    SecondHalfScore = (int)(jToken.SelectToken(MatchStatsConstants.SecondHalfScore) ?? 0),
                    TeamWin = (int)(jToken.SelectToken(MatchStatsConstants.TeamWin) ?? 0),
                    FinalScore = (int)(jToken.SelectToken(MatchStatsConstants.FinalScore) ?? 0),
                    Players = jToken["players"]?.Select(token => (PlayerStats)playerStatsParser.Parse(token)).ToList()
                };
            }
            catch
            {
                throw;
            }
        }
    }
}
