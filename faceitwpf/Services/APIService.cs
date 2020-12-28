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

namespace faceitwpf.Services
{
    public static class APIService
    {
        private readonly static HttpClient _client;
        public static Player CurrentPlayer { get; set; }
        static APIService()
        {
            _client = new HttpClient();
            string apikey = Properties.Settings.Default.API_Key;
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apikey);
        }

        public static async Task<Player> GetPlayerAsync(string playerId)
        {
            var response = await _client.GetAsync($"https://open.faceit.com/data/v4/players?nickname={playerId}&game=csgo");
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Wrong nickname");
            return JObject.Parse(response.Content.ReadAsStringAsync().Result).ToObject<Player>();
        }

        public static async Task<MatchHistory> GetHistoryAsync(string playerId)
        {
            var tempClient = new HttpClient();
            var response = await tempClient.GetAsync($"https://api.faceit.com/stats/v1/stats/time/users/{playerId}/games/csgo?&size=100"); // первая - 0
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Failed to get match history");
            var json = await response.Content.ReadAsStringAsync();
            var matchList = JsonConvert.DeserializeObject<List<Match>>(json);
            for (int i = 0; i < matchList.Count - 1; i++)
            {
                if (matchList[i].ELO != 0 && matchList[i + 1].ELO != 0)
                    matchList[i].ChangeELO = matchList[i].ELO - matchList[i + 1].ELO;
            }
            return new MatchHistory(matchList);
        }

        public static void ClearCurrentPlayer()
        {
            CurrentPlayer = null;
        }
    }
}