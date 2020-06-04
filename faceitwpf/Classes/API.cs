using faceitwpf.Models;
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
    class API
    {
        private static API _instance;
        private HttpClient _client;
        public Player CurrentPlayer { get; set; }
        public API()
        {
            _client = new HttpClient();
            string apikey = Properties.Settings.Default.API_Key;
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

        public async Task<T> GetInfoAsync<T>(string id, int page = 1) where T: class
        {
            var typeName = typeof(T).Name;
            switch (typeName)
            {
                case "Player":
                    var response = await _client.GetAsync($"https://open.faceit.com/data/v4/players?nickname={id}&game=csgo");
                    if (response.StatusCode != HttpStatusCode.OK) throw new System.Exception("Wrong nickname");
                    return JObject.Parse(response.Content.ReadAsStringAsync().Result).ToObject<T>();
                case "MatchHistory":
                    response = await _client.GetAsync($"https://open.faceit.com/data/v4/players/{id}/history?game=csgo&offset={10 * (page - 1)}&limit=10");
                    if (response.StatusCode != HttpStatusCode.OK) throw new System.Exception("Failed to get match history");
                    return JObject.Parse(response.Content.ReadAsStringAsync().Result).ToObject<T>();
                case "Stats":
                    response = await _client.GetAsync($"https://open.faceit.com/data/v4/matches/{id}/stats");
                    if (response.StatusCode != HttpStatusCode.OK) throw new System.Exception("Failed to get match info");
                    var jObject = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    Stats stats = jObject.ToObject<Stats>();
                    try
                    {
                        string path = $"$..players[?(@.nickname == '{CurrentPlayer.Nickname}')].player_stats";
                        Dictionary<string, double> temp = jObject.SelectToken(path).ToObject<Dictionary<string, double>>();
                        stats.Kills = (int)temp["Kills"];
                        stats.Deaths = (int)temp["Deaths"];
                        stats.KDRatio = temp["K/D Ratio"];
                        stats.KRRatio = temp["K/R Ratio"];
                        stats.Result = (temp["Result"] == 1 ? 'W' : 'L');
                    }
                    catch (Exception e)
                    {
                        if (e.Message == "Failed to get match info")
                            stats.Map = "de_notfound";
                        stats.Kills = 0;
                        stats.Deaths = 0;
                        stats.KDRatio = 0;
                        stats.KRRatio = 0;
                        stats.Result = 'E';
                    }
                    return stats as T;
                default:
                    return null;
            }
        }
    }
}