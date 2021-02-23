using faceitwpf.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace faceitwpf.Services
{
    public class APIService : IAPIService
    {
        private readonly HttpClient _client;
        private readonly HttpClient _v1Client;
        public APIService()
        {
            _client = new HttpClient();
            _v1Client = new HttpClient();
            string apikey = Properties.Settings.Default.API_Key;
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apikey);
        }

        public async Task<PlayerProfile> FetchPlayerProfileAsync(string playerName)
        {
            var response = await _client.GetAsync($"https://open.faceit.com/data/v4/players?nickname={playerName}&game=csgo");
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Player is not found");
            string json = await response.Content.ReadAsStringAsync();

            return JObject.Parse(json).ToObject<PlayerProfile>();
        }

        public async Task<PlayerProfile> FetchPlayerProfileByIdAsync(string playerId)
        {
            var response = await _v1Client.GetAsync($"https://api.faceit.com/core/v1/users/{playerId}");
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Player is not found");
            string json = await response.Content.ReadAsStringAsync();
            JObject jObject = JObject.Parse(json);

            var payload = jObject["payload"];
            var playerProfile = payload.ToObject<PlayerProfile>();
            playerProfile.PlayerId = playerId;

            return playerProfile;
        }


        public async Task<PlayerOverallStats> FetchPlayerStatsAsync(string playerId)
        {
            var response = await _v1Client.GetAsync($"https://api.faceit.com/stats/v1/stats/users/{playerId}/games/csgo");
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Failed to get player stats");
            string json = await response.Content.ReadAsStringAsync();
            JObject jObject = JObject.Parse(json);

            var pos = jObject["lifetime"].ToObject<PlayerOverallStats>();
            pos.MapOverallStats = new List<MapOverallStats>();
            var maps = jObject.SelectToken("$.segments[?(@._id.segmentId == 'csgo_map' && @._id.gameMode == '5v5')].segments");
            foreach (JToken map in maps)
            {
                var mapStats = map.First.ToObject<MapOverallStats>();
                mapStats.MapName = ((JProperty)map).Name;
                pos.MapOverallStats.Add(mapStats);
            }
            return pos;
        }

        public async Task<List<Match>> FetchMatchesAsync(string playerId, int size)
        {
            var response = await _v1Client.GetAsync($"https://api.faceit.com/stats/v1/stats/time/users/{playerId}/games/csgo?&size={size}");
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

        public async Task<MatchInfo> FetchMatchInfoAsync(string matchId)
        {
            var response = await _v1Client.GetAsync($"https://api.faceit.com/match/v2/match/{matchId}");
            if (response.StatusCode != HttpStatusCode.OK)
                throw new System.Exception("Failed to get match overview info");
            string json = await response.Content.ReadAsStringAsync();
            JObject jObject = JObject.Parse(json);

            var payload = jObject["payload"];

            if (!payload.HasValues)
                throw new System.Exception("No match");
            var mi = payload.ToObject<MatchInfo>();

            return mi;
        }

        public async Task<MatchStats> FetchMatchStatsAsync(string matchId)
        {
            // V4 https://open.faceit.com/data/v4/matches/{matchId}/stats
            // V1 https://api.faceit.com/stats/v1/stats/matches/{matchId}
            var response = await _v1Client.GetAsync($"https://api.faceit.com/stats/v1/stats/matches/{matchId}");
            if (response.StatusCode != HttpStatusCode.OK)
                throw new System.Exception("Failed to get match details");
            string json = await response.Content.ReadAsStringAsync();
            json = json.Trim(new char[] { '[', ']' });
            JObject jObject;
            try
            {
                jObject = JObject.Parse(json);
            }
            catch
            {
                throw new System.Exception("Only BO1 match stats is available");
            }
            var md = jObject.ToObject<MatchStats>();

            return md;
        }

        public async Task<string> FetchOngoingMatchIdAsync(string playerId)
        {
            var response = await _v1Client.GetAsync($"https://api.faceit.com/match/v1/matches/groupByState?userId={playerId}");
            if (response.StatusCode != HttpStatusCode.OK)
                throw new System.Exception("Failed to get ongoing match id");
            string json = await response.Content.ReadAsStringAsync();
            JObject jObject = JObject.Parse(json);

            var payload = jObject["payload"];

            if (!payload.HasValues)
                return null;
            var dict = payload.ToObject<Dictionary<string, JArray>>();
            foreach (var pair in dict)
            {
                if (pair.Key != "CANCELLED" && pair.Key != "ABORTED")
                    return pair.Value.First["id"].Value<string>();
            }
            return null;
        }
    }
}