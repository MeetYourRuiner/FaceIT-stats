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

        public async Task<MatchDetails> GetMatchDetailsAsync(string matchId)
        {
            MatchDetails matchDetails = await apiService.FetchMatchDetailsAsync(matchId);
            matchDetails.Teams.ForEach((t) =>
            {
                t.Players.Sort((p1, p2) => p2.PlayerStats.Kills.CompareTo(p1.PlayerStats.Kills));
            });
            return matchDetails;
        }

        public async Task<MatchOverview> GetMatchOverviewAsync(string matchId)
        {
            MatchOverview matchOverview = await apiService.FetchMatchOverviewAsync(matchId);
            return matchOverview;
        }

        public async Task<List<Match>> GetMatchesAsync(string playerId)
        {
            List<Match> matches;
            try
            {
                matches = await apiService.FetchMatchesAsync(playerId);
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
            PlayerProfile player = await apiService.FetchPlayerProfileAsync(playerName);
            return player;
        }
    }
}
