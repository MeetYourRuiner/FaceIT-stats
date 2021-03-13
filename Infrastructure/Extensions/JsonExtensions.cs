using FaceitStats.Core.Models;
using FaceitStats.Infrastructure.Constants;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaceitStats.Infrastructure.Extensions
{
    static class JsonExtensions
    {
        public static PlayerProfile ToPlayerProfile(this JToken jToken) =>
            new PlayerProfile
            {
                Id = jToken["player_id"].Value<string>(),
                Nickname = jToken["nickname"].Value<string>(),
                Country = jToken["country"].Value<string>(),
                FaceitLanguage = jToken["settings"]["language"].Value<string>(),
                FaceitURL = jToken["faceit_url"].Value<string>(),
                SteamID64 = jToken["steam_id_64"].Value<string>(),
                Avatar = jToken["avatar"].Value<string>(),
                CoverImage = jToken["cover_image"].Value<string>(),
                Level = jToken["games.csgo.skill_level"].Value<int>(),
                Elo = jToken["games.csgo.faceit_elo"].Value<int>(),
            };

        public static PlayerOverallStats ToPlayerOverallStats(this JToken jToken)
        {
            var playerStats = new PlayerOverallStats
            {
                Id = jToken["_id"]["playerId"].Value<string>(),
                Matches = jToken[OverallStatsConstants.Matches].Value<int>(),
                Winrate = jToken[OverallStatsConstants.Winrate].Value<int>(),
                KDRatio = jToken[OverallStatsConstants.KDRatio].Value<double>(),
                HSPercentage = jToken[OverallStatsConstants.HSPercentage].Value<int>(),
                MapOverallStats = new List<MapOverallStats>()
            };

            var maps = jToken.SelectToken("$.segments[?(@._id.segmentId == 'csgo_map' && @._id.gameMode == '5v5')].segments");
            foreach (JToken map in maps)
            {
                JToken mapToken = map.First;
                var mapStats = new MapOverallStats
                {
                    MapName = ((JProperty)map).Name,
                    Matches = mapToken[OverallStatsConstants.Matches].Value<int>(),
                    Winrate = mapToken[OverallStatsConstants.Winrate].Value<int>(),
                    Kills = mapToken[OverallStatsConstants.Kills].Value<double>(),
                    Deaths = mapToken[OverallStatsConstants.Deaths].Value<double>(),
                    Assists = mapToken[OverallStatsConstants.Assists].Value<double>(),
                    KDRatio = mapToken[OverallStatsConstants.KDRatio].Value<double>(),
                    KRRatio = mapToken[OverallStatsConstants.KRRatio].Value<double>(),
                    HSPerMatch = mapToken[OverallStatsConstants.HSPerMatch].Value<double>(),
                    HSPercentage = mapToken[OverallStatsConstants.HSPercentage].Value<int>(),
                    TriplePerMatch = mapToken[OverallStatsConstants.TriplePerMatch].Value<double>(),
                    QuadroPerMatch = mapToken[OverallStatsConstants.QuadroPerMatch].Value<double>(),
                    AcePerMatch = mapToken[OverallStatsConstants.AcePerMatch].Value<double>()
                };
                playerStats.MapOverallStats.Add(mapStats);
            }
            return playerStats;
        }

        public static MatchInfo ToMatchInfo(this JToken jToken)
        {
            static TeamInfo ToTeamInfo(JToken jToken)
            {
                var teamInfo = new TeamInfo
                {
                    Name = jToken["name"].Value<string>(),
                    AverageLevel = jToken["stats"]["skillLevel"]["average"].Value<int>(),
                    Rating = jToken["stats"]["rating"].Value<int>(),
                    WinProbability = jToken["stats"]["winProbability"].Value<double>(),
                    Players = new List<PlayerInfo>()
                };
                JToken playersToken = jToken["roster"];
                foreach (var player in playersToken)
                {
                    var playerInfo = new PlayerInfo
                    {
                        Id = player["id"].Value<string>(),
                        Avatar = player["avatar"].Value<string>(),
                        Nickname = player["nickname"].Value<string>(),
                        Level = player["gameSkillLevel"].Value<int>(),
                        Elo = player["elo"].Value<int>(),
                    };
                    teamInfo.Players.Add(playerInfo);
                }
                return teamInfo;
            }

            var matchInfo = new MatchInfo
            {
                Id = jToken["id"].Value<string>(),
                CompetitionName = jToken["entity"]["name"].Value<string>(),
                TeamA = ToTeamInfo(jToken["teams"]["faction1"]),
                TeamB = ToTeamInfo(jToken["teams"]["faction2"]),
                TeamAScore = jToken["results"][0]["factions"]["faction1"]["score"].Value<int>(),
                TeamBScore = jToken["results"][0]["factions"]["faction2"]["score"].Value<int>(),
                State = jToken["state"].Value<string>(),
                Map = jToken["voting"]["map"]["pick"][0].Value<string>(),
                Date = jToken["createdAt"].Value<DateTime>(),
                Parties = jToken["entityCustom"]["parties"].ToObject<Dictionary<string, string[]>>(),
            };
            return matchInfo;
        }

        public static List<Match> ToMatchList(this JToken jToken)
        {
            List<Match> matches = new List<Match>();
            foreach(var matchToken in jToken)
            {
                var match = new Match
                {
                    Id = matchToken["matchId"].Value<string>(),
                    _Date = matchToken["date"].Value<long>(),
                    ELO = matchToken["elo"].Value<int>(),
                    RoundStats = matchToken.ToRoundStats(),
                    PlayerStats = matchToken.ToPlayerStats(),
                    TeamStats = matchToken.ToTeamStats()
                };
                matches.Add(match);
            }
            return matches;
        }

        public static RoundStats ToRoundStats(this JToken jToken) =>
            new RoundStats
            {
                Score = jToken[MatchStatsConstants.Score].Value<string>(),
                Map = jToken[MatchStatsConstants.Map].Value<string>()
            };

        public static TeamStats ToTeamStats(this JToken jToken) =>
           new TeamStats
           {
               Id = jToken["teamId"].Value<string>(),
               Name = jToken[MatchStatsConstants.TeamName].Value<string>(),
               IsPremade = jToken["premade"].Value<bool>(),
               FirstHalfScore = jToken[MatchStatsConstants.FirstHalfScore].Value<int>(),
               SecondHalfScore = jToken[MatchStatsConstants.SecondHalfScore].Value<int>(),
               FinalScore = jToken[MatchStatsConstants.FinalScore].Value<int>(),
               Players = jToken["players"].Select(token => token.ToPlayerStats()).ToList()
           };

        public static PlayerStats ToPlayerStats(this JToken jToken) =>
            new PlayerStats
            {
                Id = jToken["playerId"].Value<string>(),
                Nickname = jToken["nickname"].Value<string>(),
                TripleKills = jToken[MatchStatsConstants.TripleKills].Value<int>(),
                QuadroKills = jToken[MatchStatsConstants.QuadroKills].Value<int>(),
                PentaKills = jToken[MatchStatsConstants.PentaKills].Value<int>(),
                KDRatio = jToken[MatchStatsConstants.KDRatio].Value<double>(),
                KRRatio = jToken[MatchStatsConstants.KRRatio].Value<double>(),
                MVPs = jToken[MatchStatsConstants.MVPs].Value<int>(),
                Kills = jToken[MatchStatsConstants.Kills].Value<int>(),
                Assists = jToken[MatchStatsConstants.Assists].Value<int>(),
                Deaths = jToken[MatchStatsConstants.Deaths].Value<int>(),
                Headshots = jToken[MatchStatsConstants.Headshots].Value<int>(),
                HSPercentage = jToken[MatchStatsConstants.HSPercentage].Value<int>(),
                _Result = jToken[MatchStatsConstants.Result].Value<int>()
            };

        public static MatchStats ToMatchStats(this JToken jToken) =>
            new MatchStats()
            {
                Id = jToken["matchId"].Value<string>(),
                RoundStats = jToken.ToRoundStats(),
                _Date = jToken["date"].Value<long>(),
                TeamA = jToken["teams"][0].ToTeamStats(),
                TeamB = jToken["teams"][1].ToTeamStats(),
            };
    }
}
