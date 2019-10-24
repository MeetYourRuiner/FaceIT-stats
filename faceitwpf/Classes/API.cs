using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace faceitwpf
{
    enum GetInfo
    {
        Player,
        MatchHistory,
        MatchStats
    }

    class API
    {
        private static API _instance;
        private static HttpClient _client = new HttpClient();
        public Player CurrentPlayer { get; set; }
        public API()
        {
            _client = new HttpClient();
            string apikey = "a847d087-70da-4dd0-992d-cc000f257839";
            _client.BaseAddress = new Uri("https://api.faceit.com/auth/v1/");
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apikey);
        }

        public static API GetInstance()
        {
            if (_instance == null)
                _instance = new API();
            return _instance;
        }

        public async Task<JObject> AsyncGetInfo(GetInfo getInfo, string id = "0", int page = 1)
        {
            switch (getInfo)
            {
                case GetInfo.Player:
                    var response = await _client.GetAsync($"https://open.faceit.com/data/v4/players?nickname={id}&game=csgo");
                    if (response.StatusCode != HttpStatusCode.OK) throw new System.Exception("Wrong nickname");
                    return JObject.Parse(response.Content.ReadAsStringAsync().Result);
                case GetInfo.MatchHistory:
                    response = await _client.GetAsync($"https://open.faceit.com/data/v4/players/{id}/history?game=csgo&offset={10 * (page - 1)}&limit=10");
                    if (response.StatusCode != HttpStatusCode.OK) throw new System.Exception("Failed to get match history");
                    return JObject.Parse(response.Content.ReadAsStringAsync().Result);
                case GetInfo.MatchStats:
                    response = await _client.GetAsync($"https://open.faceit.com/data/v4/matches/{id}/stats");
                    if (response.StatusCode != HttpStatusCode.OK) throw new System.Exception("Failed to get match info");
                    return JObject.Parse(response.Content.ReadAsStringAsync().Result);
                default:
                    return null;
            }
        }

        public async Task<Player> AsyncGetPlayerInfo(string name)
        {
            Player player = (await AsyncGetInfo(GetInfo.Player, name)).ToObject<Player>();
            return player;
        }
        public async Task<MatchHistory> AsyncGetHistory(string id, int page = 1)
        {
            var data = await AsyncGetInfo(GetInfo.MatchHistory, id, page);
            var matchHistory = data.ToObject<MatchHistory>();
            return matchHistory;
        }

        public async Task<Stats> AsyncGetStats(string matchid, string name)
        {
            string path = $"$..players[?(@.nickname == '{name}')].player_stats";
            Stats Stats = new Stats();
            try
            {
                var data = await AsyncGetInfo(GetInfo.MatchStats, matchid);
                Stats = data.ToObject<Stats>();
                Dictionary<string, double> temp = data.SelectToken(path).ToObject<Dictionary<string, double>>();
                Stats.Kills = (int)temp["Kills"];
                Stats.Deaths = (int)temp["Deaths"];
                Stats.KDRatio = temp["K/D Ratio"];
                Stats.KRRatio = temp["K/R Ratio"];
                Stats.Result = (temp["Result"] == 1 ? 'W' : 'L');
            }
            catch (Exception e)
            {
                if (e.Message == "Failed to get match info")
                    Stats.Map = "de_notfound";
                Stats.Kills = 0;
                Stats.Deaths = 0;
                Stats.KDRatio = 0;
                Stats.KRRatio = 0;
                Stats.Result = 'E';
            }
            return Stats;
        }

        [JsonConverter(typeof(JsonPathConverter))]
        public class Player
        {
            [JsonProperty("nickname")]
            public string Nickname { get; set; }
            [JsonProperty("player_id")]
            public string PlayerID { get; set; }
            [JsonProperty("avatar")]
            public string Avatar { get; set; }
            [JsonProperty("games.csgo.skill_level")]
            public int Level { get; set; }
            [JsonProperty("games.csgo.faceit_elo")]
            public int Elo { get; set; }
        }

        public class MatchHistory
        {
            [JsonProperty("items")]
            public Match[] Match { get; set; }
        }

        [JsonConverter(typeof(JsonPathConverter))]
        public partial class Match
        {
            [JsonProperty("match_id")]
            public string Id { get; set; }
            [JsonProperty("started_at")]
            public int _Date { get; set; }
            public DateTimeOffset Date { get; set; }
            public Stats Stats { get; set; }
        }
        [JsonConverter(typeof(JsonPathConverter))]
        public partial class Stats
        {
            [JsonProperty("rounds[0].round_stats.Map")]
            public string Map { get; set; }
            [JsonProperty("rounds[0].round_stats.Score")]
            public string Score { get; set; }
            public char Result { get; set; }
            public int Kills { get; set; }
            public int Deaths { get; set; }
            public double KDRatio { get; set; }
            public double KRRatio { get; set; }
        }
    }
}