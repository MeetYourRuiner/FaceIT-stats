using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace faceitwpf
{
    class API
    {       
        static HttpClient client = new HttpClient();
        public static void APIClient()
        {
            string apikey = "a847d087-70da-4dd0-992d-cc000f257839";
            client.BaseAddress = new Uri("https://api.faceit.com/auth/v1/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apikey);
        }

        public string GetInfo(int i, string id = "0")
        {           
            switch (i)
            {
                case 1:                   
                    var response = client.GetAsync($"https://open.faceit.com/data/v4/players?nickname={id}&game=csgo").Result;
                    if (response.StatusCode != HttpStatusCode.OK) throw new System.ArgumentException();
                    return response.Content.ReadAsStringAsync().Result;
                case 2:
                    response = client.GetAsync($"https://open.faceit.com/data/v4/players/{id}/history?game=csgo&offset=0&limit=10").Result;
                    if (response.StatusCode != HttpStatusCode.OK) throw new System.ArgumentException();
                    return response.Content.ReadAsStringAsync().Result;
                case 3:
                    response = client.GetAsync($"https://open.faceit.com/data/v4/matches/{id}/stats").Result;
                    if (response.StatusCode != HttpStatusCode.OK) throw new System.ArgumentException();
                    return response.Content.ReadAsStringAsync().Result;
                default:
                    return "Error";
            }           
        }
        public Player GetPlayerInfo(string name)
        {
            string PInfo = GetInfo(1, name);
            Player player = new Player
            {
                Nickname = name,
                PId = Regex.Match(PInfo, @"(?<=player_id"".{2}).+?(?="")").Value,
                Matches = Regex.Matches(GetInfo(2, Regex.Match(PInfo, @"(?<=player_id"".{2}).+?(?="")").Value), @"(?<=match_id"".{2}).+?(?="")"),
                Avatar = Regex.Match(PInfo, @"(?<=avatar"".{2}).+?(?="")").Value,
                Level = Regex.Match(PInfo, @"(?<=skill_level"".{1}).+?(?=,)").Value,
                Elo = Regex.Match(PInfo, @"(?<=faceit_elo"".{1}).+?(?=,)").Value,
            };
            return player;
        }
        public Stats[] GetMatchHistory(Player player)
        {
            int i = 0;
            Stats[] stats = new Stats[player.Matches.Count];
            var dreg = new Regex($@"(?<={Regex.Escape(player.PId)}.+Deaths""..).+?(?="")");
            var kreg = new Regex($@"(?<={Regex.Escape(player.PId)}.+Kills""..).+?(?="")");
            try
            { 
            string matchhistory;
            foreach (Match match in player.Matches)
            {
                matchhistory = GetInfo(3, match.Value);
                if (matchhistory == "Error") { i++; continue; }
                stats[i] = new Stats
                {
                    Score = (Regex.Match(matchhistory, @"(?<=Score"".{2}).+?(?="")").Value),
                    Kills = int.Parse(kreg.Match(matchhistory).Value),
                    Map = (Regex.Match(matchhistory, @"(?<=Map"".{2}).+?(?="")").Value),
                    Deaths = int.Parse(dreg.Match(matchhistory).Value)                   
                };
                stats[i].KDRatio = Math.Round((double) stats[i].Kills / stats[i++].Deaths, 2);
            }
            }
            catch (ArgumentException e)
            {
                throw new System.ArgumentException();
            }
            return stats;
        }
        public class Stats
        {
            public string Map { get; set; }
            public string Score { get; set; }          
            public int Kills { get; set; }
            public int Deaths { get; set; }    
            public double KDRatio { get; set; }
        }
        public class Player
        {
            public string Nickname { get; set; }
            public string PId { get; set; }
            public MatchCollection Matches { get; set; }
            public string Avatar { get; set; }
            public string Level { get; set; }
            public string Elo { get; set; }
        }
    }
}