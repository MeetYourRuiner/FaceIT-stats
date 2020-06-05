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

        public async Task<T> GetInfoAsync<T>(string id) where T: class
        {
            var typeName = typeof(T).Name;
            HttpResponseMessage response;
            switch (typeName)
            {
                case "Player":
                    response = await _client.GetAsync($"https://open.faceit.com/data/v4/players?nickname={id}&game=csgo");
                    if (response.StatusCode != HttpStatusCode.OK) throw new System.Exception("Wrong nickname");
                    return JObject.Parse(response.Content.ReadAsStringAsync().Result).ToObject<T>();
                case "MatchHistory":
                    var tempClient = new HttpClient();
                    _client.DefaultRequestHeaders.Accept.Clear();
                    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    response = await tempClient.GetAsync($"https://api.faceit.com/stats/v1/stats/time/users/{id}/games/csgo?&size=99"); // первая - 0
                    if (response.StatusCode != HttpStatusCode.OK) throw new System.Exception("Failed to get match history");
                    var json = await response.Content.ReadAsStringAsync();
                    var matchList = JsonConvert.DeserializeObject<List<Match>>(json);
                    for (int i = 0; i < matchList.Count - 1; i++)
                    {
                        if (matchList[i].ELO != 0 && matchList[i + 1].ELO != 0)
                            matchList[i].ChangeELO = matchList[i].ELO - matchList[i + 1].ELO;
                    }
                    return new MatchHistory(matchList) as T;
                default:
                    return null;
            }
        }
    }
}