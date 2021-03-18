namespace FaceitStats.Core.Models
{
    public class PlayerInfo
    {
        public string Id { get; set; }
        public string Avatar { get; set; }
        public string Nickname { get; set; }
        public int Level { get; set; }
        public int Elo { get; set; }
        public int PartyIndex { get; set; } = -1;
    }
}
