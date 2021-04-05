using FaceitStats.Core.Constants;

namespace FaceitStats.Core.Models
{
    public class PlayerProfile
    {
        public string Nickname { get; set; }
        public string Id { get; set; }
        public string Country { get; set; }
        public string FaceitLanguage { get; set; }
        public string CountryImage { get => $"https://flagcdn.com/h20/{Country}.png"; }
        public string FaceitLanguageImage
        {
            get
            {
                if (FaceitLanguage == "en")
                    return $"https://flagcdn.com/h20/gb.png";
                if (FaceitLanguage == "zh")
                    return $"https://flagcdn.com/h20/cn.png";
                else
                    return $"https://flagcdn.com/h20/{FaceitLanguage}.png";
            }
        }
        private string _faceitURL;
        public string FaceitURL
        {
            get => _faceitURL;
            set => _faceitURL = value.Replace("{lang}", "en");
        }
        public string SteamID64 { get; set; }
        public string SteamURL { get => $"http://steamcommunity.com/profiles/{SteamID64}"; }
        public string Avatar { get; set; }
        public string CoverImage { get; set; }
        public int Level { get; set; }
        public int Elo { get; set; }

        public string ToDemote { get => Level == 1 ? "∞" : (Elo - FaceitConstants.Levels[Level] + 1).ToString(); }

        public string ToPromote { get => Level == 10 ? "∞" : (FaceitConstants.Levels[Level + 1] - Elo).ToString(); }
    }
}
