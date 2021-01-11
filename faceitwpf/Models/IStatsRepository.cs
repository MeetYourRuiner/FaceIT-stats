using System.Collections.Generic;
using System.Threading.Tasks;

namespace faceitwpf.Models
{
    interface IStatsRepository
    {
        Task TryToLoadStatsAsync(string nickname);
        List<Match> GetMatches();
        Player GetCurrentPlayer();
        Task<Match> GetMatchDetailsAsync(string matchId);
    }
}
