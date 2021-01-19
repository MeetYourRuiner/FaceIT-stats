using faceitwpf.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace faceitwpf.Services
{
    interface IAPIService
    {
        Task<PlayerProfile> FetchPlayerProfileAsync(string playerName);
        Task<List<Match>> FetchMatchesAsync(string playerId);
        Task<MatchDetails> FetchMatchDetailsAsync(string matchId);
        Task<MatchOverview> FetchMatchOverviewAsync(string matchId);
    }
}
