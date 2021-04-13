using FaceitStats.Core.Models;
using FaceitStats.Infrastructure.Constants;
using Newtonsoft.Json.Linq;

namespace FaceitStats.Infrastructure.JSON.Strategies
{
    class PlayerStatsParseStrategy : IStrategy
    {
        public object Parse(JToken jToken)
        {
            try
            {
                return new PlayerStats
                {
                    Id = (string)jToken.SelectToken("playerId"),
                    Nickname = (string)jToken.SelectToken("nickname"),
                    TripleKills = (int)(jToken.SelectToken(MatchStatsConstants.TripleKills) ?? 0),
                    QuadroKills = (int)(jToken.SelectToken(MatchStatsConstants.QuadroKills) ?? 0),
                    PentaKills = (int)(jToken.SelectToken(MatchStatsConstants.PentaKills) ?? 0),
                    KDRatio = (double)(jToken.SelectToken(MatchStatsConstants.KDRatio) ?? 0),
                    KRRatio = (double)(jToken.SelectToken(MatchStatsConstants.KRRatio) ?? 0),
                    MVPs = (int)(jToken.SelectToken(MatchStatsConstants.MVPs) ?? 0),
                    Kills = (int)(jToken.SelectToken(MatchStatsConstants.Kills) ?? 0),
                    Assists = (int)(jToken.SelectToken(MatchStatsConstants.Assists) ?? 0),
                    Deaths = (int)(jToken.SelectToken(MatchStatsConstants.Deaths) ?? 0),
                    Headshots = (int)(jToken.SelectToken(MatchStatsConstants.Headshots) ?? 0),
                    HSPercentage = (int)(jToken.SelectToken(MatchStatsConstants.HSPercentage) ?? 0),
                    _Result = (int)(jToken.SelectToken(MatchStatsConstants.Result) ?? 0)
                };
            }
            catch
            {
                throw;
            }
        }
    }
}
