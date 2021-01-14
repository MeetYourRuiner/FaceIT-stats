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
    public class APIService : IAPIService
    {
        private readonly HttpClient _client;
        public APIService()
        {
            _client = new HttpClient();
            string apikey = Properties.Settings.Default.API_Key;
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apikey);
        }

        public async Task<Player> FetchPlayerAsync(string playerName)
        {
            var response = await _client.GetAsync($"https://open.faceit.com/data/v4/players?nickname={playerName}&game=csgo");
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Player is not found");
            string json = await response.Content.ReadAsStringAsync();
            return JObject.Parse(json).ToObject<Player>();
        }

        public async Task<List<Match>> FetchMatchesAsync(string playerId)
        {
            var tempClient = new HttpClient();
            var response = await tempClient.GetAsync($"https://api.faceit.com/stats/v1/stats/time/users/{playerId}/games/csgo?&size=100"); // первая - 0
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Failed to get match history");
            string json = await response.Content.ReadAsStringAsync();
            var matchList = JsonConvert.DeserializeObject<List<Match>>(json);

            for (int i = 0; i < matchList.Count; ++i)
            {
                matchList[i].Index = i;
            }

            return matchList;
        }

        public async Task<List<MatchAvgLevel>> FetchMatchesAvgLevelsAsync(string playerId)
        {
            var response = await _client.GetAsync($"https://open.faceit.com/data/v4/players/{playerId}/history?game=csgo&limit=99");
            if (response.StatusCode != HttpStatusCode.OK) throw new System.Exception("Failed");
            string json = await response.Content.ReadAsStringAsync();
            JObject jObject = JObject.Parse(json);
            var list = jObject["items"].Select(m =>
                new MatchAvgLevel
                {
                    Id = m["match_id"].Value<string>(),
                    AvgLevel = (int)Math.Round(
                        m
                        .SelectTokens("$..skill_level")
                        .Values<double>()
                        .Average()
                    )
                });
            return list.ToList();
        }

        public Task<MatchDetails> FetchMatchDetailsAsync(string matchId)
        {
            throw new NotImplementedException();
        }
    }
}