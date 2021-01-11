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

        public async Task<Player> GetPlayerAsync(string playerName)
        {
            var response = await _client.GetAsync($"https://open.faceit.com/data/v4/players?nickname={playerName}&game=csgo");
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Wrong nickname");
            string json = await response.Content.ReadAsStringAsync();
            return JObject.Parse(json).ToObject<Player>();
        }

        public async Task<List<Match>> GetMatchesAsync(string playerId)
        {
            var tempClient = new HttpClient();
            var response = await tempClient.GetAsync($"https://api.faceit.com/stats/v1/stats/time/users/{playerId}/games/csgo?&size=100"); // первая - 0
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Failed to get match history");
            string json = await response.Content.ReadAsStringAsync();
            var matchList = JsonConvert.DeserializeObject<List<Match>>(json);
            List<MatchLvl> matchLevelsList;
            try
            {
                matchLevelsList = await GetMatchesAvgLevelsAsync(playerId);
            }
            catch
            {
                matchLevelsList = new List<MatchLvl>();
            }
            for (int i = 0; i < matchList.Count - 1; ++i)
            {
                if (matchList[i].ELO != 0 && matchList[i + 1].ELO != 0)
                    matchList[i].ChangeELO = matchList[i].ELO - matchList[i + 1].ELO;
                MatchLvl matchLvl = matchLevelsList.FirstOrDefault(m => m.Id == matchList[i].Id);
                if (matchLvl != null)
                    matchList[i].AvgLevel = matchLvl.AvgLevel;
            }
            return matchList;
        }

        private async Task<List<MatchLvl>> GetMatchesAvgLevelsAsync(string playerId)
        {
            var response = await _client.GetAsync($"https://open.faceit.com/data/v4/players/{playerId}/history?game=csgo&limit=99");
            if (response.StatusCode != HttpStatusCode.OK) throw new System.Exception("Failed");
            string json = await response.Content.ReadAsStringAsync();
            JObject jObject = JObject.Parse(json);
            var list = jObject["items"].Select(m =>
                new MatchLvl
                {
                    Id = m["match_id"].Value<string>(),
                    AvgLevel = (int)Math.Round(
                        m
                        .SelectTokens("$..skill_level")
                        .Values<int>()
                        .Average()
                    )
                });
            return list.ToList();
        }
        /* MatchLvl[] matchLevels;
                    try
                    {
                        matchLevels = await GetInfoAsync<MatchLvl[]>(id);
                    }
                    catch
                    {
                        matchLevels = new MatchLvl[0];
                    }
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
                    var array = jObject["items"].Select(m =>
                        new MatchLvl
                        {
                            Id = m["match_id"].Value<string>(),
                            Levels = m
                                .SelectTokens("$..skill_level")
                                .Values<int>()
                                .ToArray()
                        });
                    return array.ToArray() as T;
        */
    }
}