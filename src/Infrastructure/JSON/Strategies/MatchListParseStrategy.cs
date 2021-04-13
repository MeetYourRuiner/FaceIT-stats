using FaceitStats.Core.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace FaceitStats.Infrastructure.JSON.Strategies
{
    class MatchListParseStrategy : IStrategy
    {
        public object Parse(JToken jToken)
        {
            var roundStatsParser = new JsonParser(new RoundStatsParseStrategy());
            var playerStatsParser = new JsonParser(new PlayerStatsParseStrategy());
            var teamStatsParser = new JsonParser(new TeamStatsParseStrategy());
            try
            {
                List<Match> matches = new List<Match>();
                foreach (var matchToken in jToken)
                {
                    var match = new Match
                    {
                        Id = (string)matchToken.SelectToken("matchId"),
                        _Date = (long)matchToken.SelectToken("date"),
                        ELO = (int)(matchToken.SelectToken("elo") ?? 0),
                        RoundStats = (RoundStats)roundStatsParser.Parse(matchToken),
                        PlayerStats = (PlayerStats)playerStatsParser.Parse(matchToken),
                        TeamStats = (TeamStats)teamStatsParser.Parse(matchToken)
                    };
                    matches.Add(match);
                }
                return matches;
            }
            catch
            {
                throw;
            }
        }
    }
}
