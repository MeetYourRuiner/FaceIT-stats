using faceitwpf.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace faceitwpf.Models
{
    class StatsRepository : IStatsRepository
    {
        private readonly IAPIService apiService;
        private List<Match> loadedMatches;
        private Player currentPlayer;

        public StatsRepository(IAPIService apiService)
        {
            this.apiService = apiService;
        }

        public Player GetCurrentPlayer()
        {
            if (currentPlayer != null)
            {
                return currentPlayer;
            }
            else
            {
                throw new Exception("Stats was not preloaded");
            }
        }

        public async Task<Match> GetMatchDetailsAsync(string matchId)
        {
            throw new NotImplementedException();
        }

        public List<Match> GetMatches()
        {
            if (loadedMatches != null)
            {
                return loadedMatches;
            }
            else
            {
                throw new Exception("Stats was not preloaded");
            }
        }

        public async Task TryToLoadStatsAsync(string nickname)
        {
            try
            {
                currentPlayer = await apiService.GetPlayerAsync(nickname);
                loadedMatches = await apiService.GetMatchesAsync(currentPlayer.PlayerID);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
