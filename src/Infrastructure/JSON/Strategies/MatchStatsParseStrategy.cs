using FaceitStats.Core.Models;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace FaceitStats.Infrastructure.JSON.Strategies
{
    class MatchStatsParseStrategy : IStrategy
    {
        public object Parse(JToken jToken)
        {
            var roundStatsParser = new JsonParser(new RoundStatsParseStrategy());
            var teamStatsParser = new JsonParser(new TeamStatsParseStrategy());
            try
            {
                return jToken.Children().Select(token => new MatchStats()
                {
                    Id = (string)token.SelectToken("matchId"),
                    RoundStats = (RoundStats)roundStatsParser.Parse(token),
                    _Date = (long)(token.SelectToken("date") ?? 0),
                    TeamA = (TeamStats)teamStatsParser.Parse(token["teams"][0]),
                    TeamB = (TeamStats)teamStatsParser.Parse(token["teams"][1]),
                }).ToList();
            }
            catch
            {
                throw;
            }
        }
    }
}
