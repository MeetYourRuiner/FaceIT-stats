using System;
using System.Collections.Generic;
using System.Linq;

namespace FaceitStats.Core.Models
{
    public class MatchInfo
    {
        public string Id { get; set; }
        public string CompetitionName { get; set; }
        public TeamInfo TeamA { get; set; }
        public TeamInfo TeamB { get; set; }
        public int TeamAScore { get; set; }
        public int TeamBScore { get; set; }
        public string State { get; set; }
        public string Map { get; set; }
        public string MapImage { get => $"/faceitwpf;component/Resources/{Map}.jpeg"; }
        private DateTime date;
        public DateTime Date { get => date; set => date = value.ToLocalTime(); }
        public Dictionary<string, string[]> Parties { get; set; }

        public void FillPartiesIndices()
        {
            if (Parties == null)
                return;
            var partiesArray = Parties.Values.ToArray();
            PlayerInfo player;
            int i = 0;
            foreach (var party in partiesArray)
            {
                if (party.Length == 1)
                    continue;
                for (int j = 0; j < party.Length; j++)
                {
                    player = TeamA.Players.FirstOrDefault(p => p.Id == party[j]);
                    if (player == null)
                    {
                        player = TeamB.Players.FirstOrDefault(p => p.Id == party[j]);
                    }
                    try
                    {
                        player.PartyIndex = i;
                    }
                    catch { }
                    player = null;
                }
                i++;
            }
        }
    }
}
