namespace FaceitStats.Core.Models
{
    public class RoundStats
    {
        public string Score { get; set; }
        public string Map { get; set; }
        public string MapImage { get => $"/faceitwpf;component/Resources/{Map}.jpeg"; }
    }
}
