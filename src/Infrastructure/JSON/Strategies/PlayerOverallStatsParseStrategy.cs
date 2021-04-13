using FaceitStats.Core.Models;
using FaceitStats.Infrastructure.Constants;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace FaceitStats.Infrastructure.JSON.Strategies
{
    class PlayerOverallStatsParseStrategy : IStrategy
    {
        public object Parse(JToken jToken)
        {
            try
            {
                var lifetimeToken = jToken["lifetime"];
                var playerStats = new PlayerOverallStats
                {
                    Id = (string)lifetimeToken.SelectToken("_id.playerId"),
                    Matches = (int)(lifetimeToken.SelectToken(OverallStatsConstants.Matches) ?? 0),
                    Winrate = (int)(lifetimeToken.SelectToken(OverallStatsConstants.Winrate) ?? 0),
                    KDRatio = (double)(lifetimeToken.SelectToken(OverallStatsConstants.KDRatio) ?? 0),
                    HSPercentage = (int)(lifetimeToken.SelectToken(OverallStatsConstants.HSPercentage) ?? 0),
                    MapOverallStats = new List<MapOverallStats>()
                };

                var maps = jToken.SelectToken("$.segments[?(@._id.segmentId == 'csgo_map' && @._id.gameMode == '5v5')].segments");
                foreach (JToken map in maps)
                {
                    JToken mapToken = map.First;
                    var mapStats = new MapOverallStats
                    {
                        Map = ((JProperty)map).Name,
                        Matches = (int)(mapToken.SelectToken(OverallStatsConstants.Matches) ?? 0),
                        Winrate = (int)(mapToken.SelectToken(OverallStatsConstants.Winrate) ?? 0),
                        Kills = (double)(mapToken.SelectToken(OverallStatsConstants.Kills) ?? 0),
                        Deaths = (double)(mapToken.SelectToken(OverallStatsConstants.Deaths) ?? 0),
                        Assists = (double)(mapToken.SelectToken(OverallStatsConstants.Assists) ?? 0),
                        KDRatio = (double)(mapToken.SelectToken(OverallStatsConstants.KDRatio) ?? 0),
                        KRRatio = (double)(mapToken.SelectToken(OverallStatsConstants.KRRatio) ?? 0),
                        HSPerMatch = (double)(mapToken.SelectToken(OverallStatsConstants.HSPerMatch) ?? 0),
                        HSPercentage = (int)(mapToken.SelectToken(OverallStatsConstants.HSPercentage) ?? 0),
                        TriplePerMatch = (double)(mapToken.SelectToken(OverallStatsConstants.TriplePerMatch) ?? 0),
                        QuadroPerMatch = (double)(mapToken.SelectToken(OverallStatsConstants.QuadroPerMatch) ?? 0),
                        AcePerMatch = (double)(mapToken.SelectToken(OverallStatsConstants.AcePerMatch) ?? 0)
                    };
                    playerStats.MapOverallStats.Add(mapStats);
                }
                return playerStats;
            }
            catch
            {
                throw;
            }
        }
    }
}
