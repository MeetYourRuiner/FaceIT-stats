using FaceitStats.Core.Models;
using Newtonsoft.Json.Linq;

namespace FaceitStats.Infrastructure.JSON.Strategies
{
    class PlayerProfileParseStrategy : IStrategy
    {
        public object Parse(JToken jToken)
        {
            try
            {
                var pp = new PlayerProfile
                {
                    Id = (string)jToken.SelectToken("player_id") ?? (string)jToken.SelectToken("guid"),
                    Nickname = (string)jToken.SelectToken("nickname"),
                    Country = (string)jToken.SelectToken("country"),
                    FaceitLanguage = (string)jToken.SelectToken("settings.language"),
                    SteamID64 = (string)jToken.SelectToken("steam_id_64"),
                    Avatar = (string)jToken.SelectToken("avatar"),
                    Level = (int)(jToken.SelectToken("games.csgo.skill_level") ?? 0),
                    Elo = (int)(jToken.SelectToken("games.csgo.faceit_elo") ?? 0)
                };
                pp.FaceitURL = (string)jToken.SelectToken("faceit_url") ?? $"https://www.faceit.com/en/players/{pp.Nickname}";
                pp.CoverImage = (string)jToken.SelectToken("cover_image") ?? (string)jToken.SelectToken("cover_image_url");
                return pp;
            }
            catch
            {
                throw;
            }
        }
    }
}
