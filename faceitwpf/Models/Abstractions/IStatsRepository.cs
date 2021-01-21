using System.Collections.Generic;
using System.Threading.Tasks;

namespace faceitwpf.Models
{
    interface IStatsRepository
    {
        Task<List<Match>> GetMatchesAsync(string playerId);
        Task<MatchStats> GetMatchStatsAsync(string matchId);
        Task<MatchInfo> GetMatchInfoAsync(string matchId);
        Task<PlayerProfile> GetPlayerProfileAsync(string playerName);
        Task<OngoingMatchInfo> GetOngoingMatchAsync(string matchId);
    }
}
