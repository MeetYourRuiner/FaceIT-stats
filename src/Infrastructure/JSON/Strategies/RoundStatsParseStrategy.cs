using FaceitStats.Core.Models;
using FaceitStats.Infrastructure.Constants;
using Newtonsoft.Json.Linq;

namespace FaceitStats.Infrastructure.JSON.Strategies
{
    class RoundStatsParseStrategy : IStrategy
    {
        public object Parse(JToken jToken)
        {
            try
            {
                return new RoundStats
                {
                    Score = (string)jToken.SelectToken(MatchStatsConstants.Score),
                    Map = (string)jToken.SelectToken(MatchStatsConstants.Map),
                    RoundNumber = (int)(jToken.SelectToken("matchRound") ?? 0)
                };
            }
            catch
            {
                throw;
            }
        }
    }
}
