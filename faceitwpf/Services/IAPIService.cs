using faceitwpf.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace faceitwpf.Services
{
    interface IAPIService
    {
        Task<Player> FetchPlayerAsync(string playerName);
        Task<List<Match>> FetchMatchesAsync(string playerId);
        Task<MatchDetails> FetchMatchDetailsAsync(string matchId);
        Task<List<MatchAvgLevel>> FetchMatchesAvgLevelsAsync(string playerId);
    }
}
