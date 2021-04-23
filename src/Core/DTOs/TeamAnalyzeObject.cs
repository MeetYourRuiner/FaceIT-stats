using FaceitStats.Core.Models;
using System.Collections.Generic;

namespace FaceitStats.Core.DTOs
{
    public class TeamAnalyzeObject
    {
        public PlayerInfo Player { get; set; }
        public List<Match> Matches { get; set; }
        public TeamAnalyzeObject(PlayerInfo player, List<Match> matches)
        {
            Player = player;
            Matches = matches;
        }
    }
}
