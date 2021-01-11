using faceitwpf.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace faceitwpf.Services
{
    interface IAPIService
    {
        Task<Player> GetPlayerAsync(string playerName);
        Task<List<Match>> GetMatchesAsync(string playerId);
    }
}
