using FaceitStats.Core.Models;
using FaceitStats.Infrastructure.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FaceitStats.Infrastructure.Data
{
    public class FaceitAPIClient
    {
        /// <summary>
        /// Faceit API v4 client
        /// </summary>
        private readonly HttpClient _clientWithBearer;
        private readonly HttpClient _client;
        public FaceitAPIClient(string apikey, string userApikey)
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _clientWithBearer = new HttpClient();
            _clientWithBearer.DefaultRequestHeaders.Accept.Clear();
            _clientWithBearer.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _clientWithBearer.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apikey);
        }

        public async Task<PlayerProfile> FetchPlayerProfileAsync(string playerName)
        {
            var response = await _clientWithBearer.GetAsync($"https://open.faceit.com/data/v4/players?nickname={playerName}&game=csgo");
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"Player \"{playerName}\" is not found");
            string json = await response.Content.ReadAsStringAsync();
            JObject jObject = JObject.Parse(json);
            var playerProfile = jObject.ToPlayerProfile();
            return playerProfile;
        }

        public async Task<PlayerProfile> FetchPlayerProfileByIdAsync(string playerId)
        {
            var response = await _client.GetAsync($"https://api.faceit.com/core/v1/users/{playerId}");
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"Player \"{playerId}\" is not found");
            string json = await response.Content.ReadAsStringAsync();
            JObject jObject = JObject.Parse(json);
            var payload = jObject["payload"];
            var playerProfile = payload.ToPlayerProfile();
            return playerProfile;
        }

        public async Task<PlayerOverallStats> FetchPlayerStatsAsync(string playerId)
        {
            var response = await _client.GetAsync($"https://api.faceit.com/stats/v1/stats/users/{playerId}/games/csgo");
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Failed to get player stats");
            string json = await response.Content.ReadAsStringAsync();
            JObject jObject = JObject.Parse(json);
            var pos = jObject.ToPlayerOverallStats();
            return pos;
        }

        public async Task<List<Match>> FetchMatchesAsync(string playerId, int size)
        {
            var response = await _client.GetAsync($"https://api.faceit.com/stats/v1/stats/time/users/{playerId}/games/csgo?&size={size}");
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Failed to get match history");
            string json = await response.Content.ReadAsStringAsync();
            JArray jArray = JArray.Parse(json);
            var matchList = jArray.ToMatchList();
            for (int i = 0; i < matchList.Count; ++i)
            {
                matchList[i].Index = i;
            }
            return matchList;
        }

        public async Task<List<Match>> FetchMatchesAsync(string playerId, DateTimeOffset from, DateTimeOffset to)
        {
            var response = await _clientWithBearer.GetAsync($"https://api.faceit.com/match-history/v5/players/{playerId}/history/?to=2021-02-25T21:43:00+0000&from=2020-02-25T21:43:00+0000&playerId={playerId}");
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Failed to get match history");
            string json = await response.Content.ReadAsStringAsync();
            JObject jObject = JObject.Parse(json);
            var matchList = jObject.ToMatchList();
            //var matchList = JsonConvert.DeserializeObject<List<Match>>(json);

            //for (int i = 0; i < matchList.Count; ++i)
            //{
            //    matchList[i].Index = i;
            //}
            return matchList;
        }

        public async Task<MatchInfo> FetchMatchInfoAsync(string matchId)
        {
            var response = await _client.GetAsync($"https://api.faceit.com/match/v2/match/{matchId}");
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Failed to get match overview info");
            string json = await response.Content.ReadAsStringAsync();
            JObject jObject = JObject.Parse(json);
            var payload = jObject["payload"];
            if (!payload.HasValues)
                throw new Exception("No match");
            var mi = payload.ToMatchInfo();
            return mi;
        }

        public async Task<List<MatchStats>> FetchMatchStatsAsync(string matchId)
        {
            // V4 https://open.faceit.com/data/v4/matches/{matchId}/stats
            // V1 https://api.faceit.com/stats/v1/stats/matches/{matchId}
            var response = await _client.GetAsync($"https://api.faceit.com/stats/v1/stats/matches/{matchId}");
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Failed to get match details");
            string json = await response.Content.ReadAsStringAsync();
            JArray jArray = JArray.Parse(json);
            var ms = jArray.ToMatchStatsList();
            return ms;
        }

        public async Task<string> FetchOngoingMatchIdAsync(string playerId)
        {
            var response = await _client.GetAsync($"https://api.faceit.com/match/v1/matches/groupByState?userId={playerId}");
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Failed to get ongoing match id");
            string json = await response.Content.ReadAsStringAsync();
            JObject jObject = JObject.Parse(json);
            var payload = jObject["payload"];
            if (!payload.HasValues)
                return null;

            var ongoingMatch = jObject.SelectToken("payload").Where(j =>
                {
                    var tokenName = ((JProperty)j).Name;
                    return tokenName != "CANCELLED" && tokenName != "ABORTED";
                }).FirstOrDefault();
            return ongoingMatch?.First.First.Value<string>("id");
        }
    }
}