using FaceitStats.Core.Interfaces;
using FaceitStats.Core.Models;
using FaceitStats.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FaceitStats.Infrastructure.Services
{
    public class FaceitService : IFaceitService
    {
        private readonly FaceitAPIClient _apiClient;

        public FaceitService(string apikey, string userApikey)
        {
            _apiClient = new FaceitAPIClient(apikey, userApikey);
        }

        public async Task<List<MatchStats>> GetMatchStatsAsync(string matchId)
        {
            List<MatchStats> matchStats = await _apiClient.FetchMatchStatsAsync(matchId);
            return matchStats;
        }

        public async Task<MatchInfo> GetMatchInfoAsync(string matchId)
        {
            MatchInfo matchInfo = await _apiClient.FetchMatchInfoAsync(matchId);
            matchInfo.FillPartiesIndices();
            return matchInfo;
        }

        public async Task<List<Match>> GetMatchesAsync(string playerId, int size)
        {
            List<Match> matches;
            try
            {
                matches = await _apiClient.FetchMatchesAsync(playerId, size);
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
                matches = await _apiClient.FetchMatchesAsync(playerId, from, to);
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

        public async Task<string> GetOngoingMatchIdAsync(string playerId)
        {
            string ongoingMatchId;
            try
            {
                ongoingMatchId = await _apiClient.FetchOngoingMatchIdAsync(playerId);
            }
            catch
            {
                throw;
            }
            return ongoingMatchId;
        }

        public async Task<PlayerProfile> GetProfileByNameAsync(string playerName)
        {
            PlayerProfile player;
            try
            {
                player = await _apiClient.FetchPlayerProfileAsync(playerName);
            }
            catch
            {
                throw;
            }
            return player;
        }

        public async Task<PlayerProfile> GetProfileByIdAsync(string playerId)
        {
            PlayerProfile player;
            try
            {
                player = await _apiClient.FetchPlayerProfileByIdAsync(playerId);
            }
            catch
            {
                throw;
            }
            return player;
        }

        public async Task<PlayerOverallStats> GetOverallStatsAsync(string playerId)
        {
            PlayerOverallStats playerStats;
            try
            {
                playerStats = await _apiClient.FetchPlayerStatsAsync(playerId);
                playerStats.MapOverallStats
                .Sort((m1, m2) =>
                    (m2.WinrateDouble * m2.Matches / playerStats.Matches)
                        .CompareTo(m1.WinrateDouble * m1.Matches / playerStats.Matches)
                );
            }
            catch
            {
                throw;
            }
            return playerStats;
        }
    }
}
