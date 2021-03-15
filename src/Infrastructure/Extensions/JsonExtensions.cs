using FaceitStats.Core.Models;
using FaceitStats.Infrastructure.Constants;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaceitStats.Infrastructure.Extensions
{
    static class JsonExtensions
    {
        public static PlayerProfile ToPlayerProfile(this JToken jToken)
        {
            var pp = new PlayerProfile
            {
                Id = jToken.Value<string>("player_id") ?? jToken.Value<string>("guid"),
                Nickname = jToken.Value<string>("nickname"),
                Country = jToken.Value<string>("country"),
                FaceitLanguage = jToken["settings"].Value<string>("language"),
                SteamID64 = jToken.Value<string>("steam_id_64"),
                Avatar = jToken.Value<string>("avatar"),
                Level = jToken["games"]["csgo"].Value<int>("skill_level"),
                Elo = jToken["games"]["csgo"].Value<int>("faceit_elo")
            };
            pp.FaceitURL = jToken.Value<string>("faceit_url") ?? $"https://www.faceit.com/en/players/{pp.Nickname}";
            pp.CoverImage = jToken.Value<string>("cover_image") ?? jToken.Value<string>("cover_image_url");
            return pp;
        }

        public static PlayerOverallStats ToPlayerOverallStats(this JToken jToken)
        {
            var lifetimeToken = jToken["lifetime"];
            var playerStats = new PlayerOverallStats
            {
                Id = lifetimeToken["_id"].Value<string>("playerId"),
                Matches = lifetimeToken.Value<int>(OverallStatsConstants.Matches),
                Winrate = lifetimeToken.Value<int>(OverallStatsConstants.Winrate),
                KDRatio = lifetimeToken.Value<double>(OverallStatsConstants.KDRatio),
                HSPercentage = lifetimeToken.Value<int>(OverallStatsConstants.HSPercentage),
                MapOverallStats = new List<MapOverallStats>()
            };

            var maps = jToken.SelectToken("$.segments[?(@._id.segmentId == 'csgo_map' && @._id.gameMode == '5v5')].segments");
            foreach (JToken map in maps)
            {
                JToken mapToken = map.First;
                var mapStats = new MapOverallStats
                {
                    MapName = ((JProperty)map).Name,
                    Matches = mapToken.Value<int>(OverallStatsConstants.Matches),
                    Winrate = mapToken.Value<int>(OverallStatsConstants.Winrate),
                    Kills = mapToken.Value<double>(OverallStatsConstants.Kills),
                    Deaths = mapToken.Value<double>(OverallStatsConstants.Deaths),
                    Assists = mapToken.Value<double>(OverallStatsConstants.Assists),
                    KDRatio = mapToken.Value<double>(OverallStatsConstants.KDRatio),
                    KRRatio = mapToken.Value<double>(OverallStatsConstants.KRRatio),
                    HSPerMatch = mapToken.Value<double>(OverallStatsConstants.HSPerMatch),
                    HSPercentage = mapToken.Value<int>(OverallStatsConstants.HSPercentage),
                    TriplePerMatch = mapToken.Value<double>(OverallStatsConstants.TriplePerMatch),
                    QuadroPerMatch = mapToken.Value<double>(OverallStatsConstants.QuadroPerMatch),
                    AcePerMatch = mapToken.Value<double>(OverallStatsConstants.AcePerMatch)
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
                    Name = jToken.Value<string>("name"),
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
                        Id = player.Value<string>("id"),
                        Avatar = player.Value<string>("avatar"),
                        Nickname = player.Value<string>("nickname"),
                        Level = player.Value<int>("gameSkillLevel"),
                        Elo = player.Value<int>("elo"),
                    };
                    teamInfo.Players.Add(playerInfo);
                }
                return teamInfo;
            }

            var matchInfo = new MatchInfo
            {
                Id = jToken.Value<string>("id"),
                CompetitionName = jToken["entity"]["name"].Value<string>(),
                TeamA = ToTeamInfo(jToken["teams"]["faction1"]),
                TeamB = ToTeamInfo(jToken["teams"]["faction2"]),
                TeamAScore = jToken["results"][0]["factions"]["faction1"]["score"].Value<int>(),
                TeamBScore = jToken["results"][0]["factions"]["faction2"]["score"].Value<int>(),
                State = jToken.Value<string>("state"),
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
                    Id = matchToken.Value<string>("matchId"),
                    _Date = matchToken.Value<long>("date"),
                    ELO = matchToken.Value<int>("elo"),
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
                Score = jToken.Value<string>(MatchStatsConstants.Score),
                Map = jToken.Value<string>(MatchStatsConstants.Map),
                RoundNumber = jToken.Value<int>("matchRound")
            };

        public static TeamStats ToTeamStats(this JToken jToken) =>
           new TeamStats
           {
               Id = jToken.Value<string>("teamId"),
               Name = jToken.Value<string>(MatchStatsConstants.TeamName),
               IsPremade = jToken.Value<bool>("premade"),
               FirstHalfScore = jToken.Value<int>(MatchStatsConstants.FirstHalfScore),
               SecondHalfScore = jToken.Value<int>(MatchStatsConstants.SecondHalfScore),
               FinalScore = jToken.Value<int>(MatchStatsConstants.FinalScore),
               Players = jToken["players"]?.Select(token => token.ToPlayerStats()).ToList()
           };

        public static PlayerStats ToPlayerStats(this JToken jToken) =>
            new PlayerStats
            {
                Id = jToken.Value<string>("playerId"),
                Nickname = jToken.Value<string>("nickname"),
                TripleKills = jToken.Value<int>(MatchStatsConstants.TripleKills),
                QuadroKills = jToken.Value<int>(MatchStatsConstants.QuadroKills),
                PentaKills = jToken.Value<int>(MatchStatsConstants.PentaKills),
                KDRatio = jToken.Value<double>(MatchStatsConstants.KDRatio),
                KRRatio = jToken.Value<double>(MatchStatsConstants.KRRatio),
                MVPs = jToken.Value<int>(MatchStatsConstants.MVPs),
                Kills = jToken.Value<int>(MatchStatsConstants.Kills),
                Assists = jToken.Value<int>(MatchStatsConstants.Assists),
                Deaths = jToken.Value<int>(MatchStatsConstants.Deaths),
                Headshots = jToken.Value<int>(MatchStatsConstants.Headshots),
                HSPercentage = jToken.Value<int>(MatchStatsConstants.HSPercentage),
                _Result = jToken.Value<int>(MatchStatsConstants.Result)
            };

        public static List<MatchStats> ToMatchStatsList(this JToken jToken) =>
            jToken.Children().Select(token => new MatchStats()
            {
                Id = token.Value<string>("matchId"),
                RoundStats = token.ToRoundStats(),
                _Date = token.Value<long>("date"),
                TeamA = token["teams"][0].ToTeamStats(),
                TeamB = token["teams"][1].ToTeamStats(),
            }).ToList();
    }
}
