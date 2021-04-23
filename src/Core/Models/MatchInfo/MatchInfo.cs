using System;
using System.Collections.Generic;
using System.Linq;

namespace FaceitStats.Core.Models
{
    public class MatchInfo
    {
        private DateTime _date;
        public string Id { get; set; }
        public string CompetitionName { get; set; }
        public TeamInfo TeamA { get; set; }
        public TeamInfo TeamB { get; set; }
        public string State { get; set; }
        public string Map { get; set; }
        public int BestOf { get; set; }
        public DateTime Date { get => _date; set => _date = value.ToLocalTime(); }
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
