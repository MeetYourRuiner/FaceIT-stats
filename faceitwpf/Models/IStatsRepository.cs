using System.Collections.Generic;
using System.Threading.Tasks;

namespace faceitwpf.Models
{
    interface IStatsRepository
    {
        Task<List<Match>> GetMatchesAsync(string playerId);
        Task<MatchDetails> GetMatchDetailsAsync(string matchId);
        Task<PlayerProfile> GetPlayerProfileAsync(string playerName);
    }
}
