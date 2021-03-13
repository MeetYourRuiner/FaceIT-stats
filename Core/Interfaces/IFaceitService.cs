using FaceitStats.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FaceitStats.Core.Interfaces
{
    public interface IFaceitService
    {
        Task<List<Match>> GetMatchesAsync(string playerId, int size);
        Task<List<Match>> GetMatchesAsync(string playerId, DateTimeOffset from, DateTimeOffset to);
        Task<MatchStats> GetMatchStatsAsync(string matchId);
        Task<MatchInfo> GetMatchInfoAsync(string matchId);
        Task<string> GetOngoingMatchIdAsync(string playerId);
        Task<PlayerProfile> GetProfileByNameAsync(string playerName);
        Task<PlayerProfile> GetProfileByIdAsync(string playerId);
        Task<PlayerOverallStats> GetOverallStatsAsync(string playerId);
    }
}
