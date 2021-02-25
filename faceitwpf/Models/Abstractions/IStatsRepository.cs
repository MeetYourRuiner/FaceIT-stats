using System.Collections.Generic;
using System.Threading.Tasks;

namespace faceitwpf.Models.Abstractions
{
    interface IStatsRepository
    {
        Task<List<Match>> GetMatchesAsync(string playerId, int size);
        Task<MatchStats> GetMatchStatsAsync(string matchId);
        Task<MatchInfo> GetMatchInfoAsync(string matchId);
        Task<PlayerProfile> GetPlayerProfileAsync(string playerName);
        Task<string> GetOngoingMatchIdAsync(string playerId);
        Task<PlayerOverallStats> GetPlayerStatsAsync(string playerId);
        Task<PlayerProfile> GetPlayerProfileByIdAsync(string playerId);
    }
}
