﻿using faceitwpf.Models.Abstractions;
using faceitwpf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace faceitwpf.Models
{
    class StatsRepository : IStatsRepository
    {
        private readonly IAPIService apiService;

        public StatsRepository(IAPIService apiService)
        {
            this.apiService = apiService;
        }

        public async Task<MatchStats> GetMatchStatsAsync(string matchId)
        {
            MatchStats matchStats = await apiService.FetchMatchStatsAsync(matchId);
            MatchInfo matchInfo = null;
            try
            {
                matchInfo = await GetMatchInfoAsync(matchId);
            }
            catch { }
            if (matchInfo != null)
            {
                matchStats.CompetitionName = matchInfo.CompetitionName;
                matchStats.Teams[0].Players.ForEach(p =>
                {
                    p.PlayerInfo = matchInfo.TeamA.Players
                        .FirstOrDefault((po) => po.Id == p.Id);
                });
                matchStats.Teams[1].Players.ForEach(p =>
                {
                    p.PlayerInfo = matchInfo.TeamB.Players
                        .FirstOrDefault((po) => po.Id == p.Id);
                });
            }
            else
            {
                matchStats.Teams[0].Players.ForEach(p =>
                {
                    p.PlayerInfo = new PlayerInfo();
                });
                matchStats.Teams[1].Players.ForEach(p =>
                {
                    p.PlayerInfo = new PlayerInfo();
                });
            }
            matchStats.Teams.ForEach((t) =>
            {
                t.Players.Sort((p1, p2) => p2.PlayerStats.Kills.CompareTo(p1.PlayerStats.Kills));
            });
            return matchStats;
        }

        public async Task<MatchInfo> GetMatchInfoAsync(string matchId)
        {
            MatchInfo matchInfo = await apiService.FetchMatchInfoAsync(matchId);
            matchInfo.FillPartiesIndices();
            return matchInfo;
        }

        public async Task<List<Match>> GetMatchesAsync(string playerId, int size)
        {
            List<Match> matches;
            try
            {
                matches = await apiService.FetchMatchesAsync(playerId, size);
            }
            catch { throw; }
            for (int i = 0; i < matches.Count - 1; ++i)
            {
                if (matches[i].ELO != 0)
                {
                    Match nextMatchWithElo = matches.FirstOrDefault(m => m.Index > i && m.ELO != 0);
                    matches[i].ChangeELO = nextMatchWithElo != null ? matches[i].ELO - nextMatchWithElo.ELO : 0;
                }
            }

            return matches;
        }

        public async Task<List<Match>> GetMatchesAsync(string playerId, DateTimeOffset from, DateTimeOffset to)
        {
            List<Match> matches;
            try
            {
                matches = await apiService.FetchMatchesAsync(playerId, from, to);
            }
            catch { throw; }
            for (int i = 0; i < matches.Count - 1; ++i)
            {
                if (matches[i].ELO != 0)
                {
                    Match nextMatchWithElo = matches.FirstOrDefault(m => m.Index > i && m.ELO != 0);
                    matches[i].ChangeELO = nextMatchWithElo != null ? matches[i].ELO - nextMatchWithElo.ELO : 0;
                }
            }

            return matches;
        }

        public async Task<PlayerProfile> GetPlayerProfileAsync(string playerName)
        {
            PlayerProfile player;
            try
            {
                player = await apiService.FetchPlayerProfileAsync(playerName);
            }
            catch
            {
                throw;
            }
            return player;
        }

        public async Task<PlayerProfile> GetPlayerProfileByIdAsync(string playerId)
        {
            PlayerProfile player;
            try
            {
                player = await apiService.FetchPlayerProfileByIdAsync(playerId);
            }
            catch
            {
                throw;
            }
            return player;
        }

        public async Task<PlayerOverallStats> GetPlayerStatsAsync(string playerId)
        {
            PlayerOverallStats playerStats;
            try
            {
                playerStats = await apiService.FetchPlayerStatsAsync(playerId);
            }
            catch
            {
                throw;
            }
            return playerStats;
        }

        public async Task<string> GetOngoingMatchIdAsync(string playerId)
        {
            string ongoingMatchId;
            try
            {
                ongoingMatchId = await apiService.FetchOngoingMatchIdAsync(playerId);
            }
            catch
            {
                throw;
            }
            return ongoingMatchId;
        }
    }
}
