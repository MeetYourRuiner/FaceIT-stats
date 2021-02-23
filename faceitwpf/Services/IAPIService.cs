using faceitwpf.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace faceitwpf.Services
{
    interface IAPIService
    {
        Task<PlayerProfile> FetchPlayerProfileAsync(string playerName);
        Task<List<Match>> FetchMatchesAsync(string playerId, int size);
        Task<MatchStats> FetchMatchStatsAsync(string matchId);
        Task<MatchInfo> FetchMatchInfoAsync(string matchId);
        Task<string> FetchOngoingMatchIdAsync(string playerId);
        Task<PlayerOverallStats> FetchPlayerStatsAsync(string playerId);
        Task<PlayerProfile> FetchPlayerProfileByIdAsync(string playerId);
    }
}
