using faceitwpf.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<T> GetInfoAsync<T>(string id) where T : class
        {
            var typeName = typeof(T).Name;
            string json;
            HttpResponseMessage response;
            switch (typeName)
            {
                case "Player":
                    response = await _client.GetAsync($"https://open.faceit.com/data/v4/players?nickname={id}&game=csgo");
                    if (response.StatusCode != HttpStatusCode.OK) throw new System.Exception("Wrong nickname");
                    json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(json);
                case "MatchHistory":
                    var tempClient = new HttpClient();
                    _client.DefaultRequestHeaders.Accept.Clear();
                    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    response = await tempClient.GetAsync($"https://api.faceit.com/stats/v1/stats/time/users/{id}/games/csgo?&size=99");
                    if (response.StatusCode != HttpStatusCode.OK) throw new System.Exception("Failed to get match history");

                    json = await response.Content.ReadAsStringAsync();
                    var matchList = JsonConvert.DeserializeObject<List<Match>>(json);
                    var matchLevels = await GetInfoAsync<MatchLvl[]>(id);
                    for (int i = 0; i < matchList.Count - 1; i++)
                    {
                        if (matchList[i].ELO != 0 && matchList[i + 1].ELO != 0)
                            matchList[i].ChangeELO = matchList[i].ELO - matchList[i + 1].ELO;
                        var matchLvl = matchLevels.Where(m => m.Id == matchList[i].Id).FirstOrDefault();
                        if (matchLvl != null)
                            matchList[i].AvgLevel = (int)Math.Round(matchLvl.Levels.Average());
                    }
                    return new MatchHistory(matchList) as T;
                case "MatchLvl[]":
                    response = await _client.GetAsync($"https://open.faceit.com/data/v4/players/{id}/history?game=csgo&limit=99");
                    if (response.StatusCode != HttpStatusCode.OK) throw new System.Exception("Failed");
                    json = await response.Content.ReadAsStringAsync();
                    JObject jObject = JObject.Parse(json);
                    var array = jObject["items"].Select(t =>
                        new MatchLvl
                        {
                            Id = t["match_id"].Value<string>(),
                            Levels = t
                                .SelectTokens("$..skill_level")
                                .Values<int>()
                                .ToArray()
                        });
                    return array.ToArray() as T;
                default:
                    return null;
            }
        }
    }
}