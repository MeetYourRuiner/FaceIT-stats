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
            throw new NotImplementedException();
        }

        public async Task<List<Match>> GetMatchesAsync(string playerId)
        {
            List<Match> matches;
            try
            {
                matches = await apiService.FetchMatchesAsync(playerId);
            }
            catch { throw; }
            try
            {
                List<MatchAvgLevel> matchesAvgLevels = await apiService.FetchMatchesAvgLevelsAsync(playerId);
                matchesAvgLevels.ForEach(mal => {
                    var match = matches.FirstOrDefault(m => m.Id == mal.Id);
                    if (match != null)
                        match.AvgLevel = mal.AvgLevel;
                });
            }
            catch { }

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

        public async Task<Player> GetPlayerAsync(string playerName)
        {
            Player player = await apiService.FetchPlayerAsync(playerName);
            return player;
        }
    }
}
