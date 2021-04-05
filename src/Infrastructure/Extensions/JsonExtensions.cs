using FaceitStats.Core.Models;
using FaceitStats.Core.Utils;
using FaceitStats.Infrastructure.Constants;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FaceitStats.Infrastructure.Extensions
{
    static class JsonExtensions
    {
        public static PlayerProfile ToPlayerProfile(this JToken jToken)
        {
            try
            {
                var pp = new PlayerProfile
                {
                    Id = (string)jToken.SelectToken("player_id") ?? (string)jToken.SelectToken("guid"),
                    Nickname = (string)jToken.SelectToken("nickname"),
                    Country = (string)jToken.SelectToken("country"),
                    FaceitLanguage = (string)jToken.SelectToken("settings.language"),
                    SteamID64 = (string)jToken.SelectToken("steam_id_64"),
                    Avatar = (string)jToken.SelectToken("avatar"),
                    Level = (int)(jToken.SelectToken("games.csgo.skill_level") ?? 0),
                    Elo = (int)(jToken.SelectToken("games.csgo.faceit_elo") ?? 0)
                };
                pp.FaceitURL = (string)jToken.SelectToken("faceit_url") ?? $"https://www.faceit.com/en/players/{pp.Nickname}";
                pp.CoverImage = (string)jToken.SelectToken("cover_image") ?? (string)jToken.SelectToken("cover_image_url");
                return pp;
            }
            catch (Exception ex)
            {
                throw ThrowException(ex);
            }
        }

        public static PlayerOverallStats ToPlayerOverallStats(this JToken jToken)
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
            catch (Exception ex)
            {
                throw ThrowException(ex);
            }
        }

        public static MatchInfo ToMatchInfo(this JToken jToken)
        {
            static TeamInfo ToTeamInfo(JToken jToken)
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
                catch (Exception ex)
                {
                    throw ThrowException(ex);
                }
            }

            try
            {
                var matchInfo = new MatchInfo
                {
                    Id = (string)jToken.SelectToken("id"),
                    CompetitionName = (string)jToken.SelectToken("entity.name"),
                    TeamA = ToTeamInfo(jToken["teams"]["faction1"]),
                    TeamB = ToTeamInfo(jToken["teams"]["faction2"]),
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
            catch (Exception ex)
            {
                throw ThrowException(ex);
            }
        }

        public static List<Match> ToMatchList(this JToken jToken)
        {
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
                        RoundStats = matchToken.ToRoundStats(),
                        PlayerStats = matchToken.ToPlayerStats(),
                        TeamStats = matchToken.ToTeamStats()
                    };
                    matches.Add(match);
                }
                return matches;
            }
            catch (Exception ex)
            {
                throw ThrowException(ex);
            }
        }

        public static RoundStats ToRoundStats(this JToken jToken)
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
            catch (Exception ex)
            {
                throw ThrowException(ex);
            }
        }

        public static TeamStats ToTeamStats(this JToken jToken)
        {
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
                    Players = jToken["players"]?.Select(token => token.ToPlayerStats()).ToList()
                };
            }
            catch (Exception ex)
            {
                throw ThrowException(ex);
            }
        }

        public static PlayerStats ToPlayerStats(this JToken jToken)
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
            catch (Exception ex)
            {
                throw ThrowException(ex);
            }
        }

        public static List<MatchStats> ToMatchStatsList(this JToken jToken)
        {
            try
            {
                return jToken.Children().Select(token => new MatchStats()
                {
                    Id = (string)token.SelectToken("matchId"),
                    RoundStats = token.ToRoundStats(),
                    _Date = (long)(token.SelectToken("date") ?? 0),
                    TeamA = token["teams"][0].ToTeamStats(),
                    TeamB = token["teams"][1].ToTeamStats(),
                }).ToList();
            }
            catch (Exception ex)
            {
                throw ThrowException(ex);
            }
        }

        private static Exception ThrowException(Exception innerException = null, [CallerMemberName] string methodName = "")
        {
            throw new Exception($"Exception occured in {methodName}", innerException);
        }
    }
}
