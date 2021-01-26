using faceitwpf.Models.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace faceitwpf.Models
{
    public class TeamAnalyzeObject
    {
        public BasePlayerInfo Player { get; set; }
        public List<Match> Matches { get; set; }
        public TeamAnalyzeObject(BasePlayerInfo player, List<Match> matches)
        {
            Player = player;
            Matches = matches;
        }
    }
}
