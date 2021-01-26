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
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            var response = await _client.GetAsync($"https://open.faceit.com/data/v4/players?nickname={playerName}&game=csgo");
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Player is not found");
            string json = await response.Content.ReadAsStringAsync();

            stopwatch.Stop();
            TimeSpan timeTaken = stopwatch.Elapsed;
            System.Diagnostics.Trace.WriteLine("FetchPlayerProfileAsync:" + timeTaken);
            return JObject.Parse(json).ToObject<PlayerProfile>();
        }

        //public async Task<PlayerProfile> FetchPlayerProfileByIdAsync(string playerId)
        //{
            // https://api.faceit.com/core/v1/users/11fc14fa-092c-4960-a99d-3fad119b124e
        //}
        public async Task<List<Match>> FetchMatchesAsync(string playerId, int size)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            var response = await _v1Client.GetAsync($"https://api.faceit.com/stats/v1/stats/time/users/{playerId}/games/csgo?&size={size}");
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Failed to get match history");
            string json = await response.Content.ReadAsStringAsync();
            var matchList = JsonConvert.DeserializeObject<List<Match>>(json);

            for (int i = 0; i < matchList.Count; ++i)
            {
                matchList[i].Index = i;
            }

            stopwatch.Stop();
            TimeSpan timeTaken = stopwatch.Elapsed;
            System.Diagnostics.Trace.WriteLine("FetchMatchesAsync:" + timeTaken);

            return matchList;
        }

        public async Task<MatchInfo> FetchMatchInfoAsync(string matchId)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            var response = await _v1Client.GetAsync($"https://api.faceit.com/match/v2/match/{matchId}");
            if (response.StatusCode != HttpStatusCode.OK)
                throw new System.Exception("Failed to get match overview info");
            string json = await response.Content.ReadAsStringAsync();
            JObject jObject = JObject.Parse(json);

            var mo = jObject["payload"].ToObject<MatchInfo>();

            stopwatch.Stop();
            TimeSpan timeTaken = stopwatch.Elapsed;
            System.Diagnostics.Trace.WriteLine("FetchMatchInfoAsync:" + timeTaken);
            return mo;
        }

        public async Task<MatchStats> FetchMatchStatsAsync(string matchId)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            // V4 https://open.faceit.com/data/v4/matches/{matchId}/stats
            // V1 https://api.faceit.com/stats/v1/stats/matches/{matchId}
            var response = await _v1Client.GetAsync($"https://api.faceit.com/stats/v1/stats/matches/{matchId}");
            if (response.StatusCode != HttpStatusCode.OK)
                throw new System.Exception("Failed to get match details");
            string json = await response.Content.ReadAsStringAsync();
            json = json.Trim(new char[] { '[', ']' });
            JObject jObject = JObject.Parse(json);
            var md = jObject.ToObject<MatchStats>();

            stopwatch.Stop();
            TimeSpan timeTaken = stopwatch.Elapsed;
            System.Diagnostics.Trace.WriteLine("FetchMatchStatsAsync:" + timeTaken);
            return md;
        }
        public async Task<string> FetchOngoingMatchIdAsync(string playerId)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            var response = await _v1Client.GetAsync($"https://api.faceit.com/match/v1/matches/groupByState?userId={playerId}");
            if (response.StatusCode != HttpStatusCode.OK)
                throw new System.Exception("Failed to get ongoing match id");
            string json = await response.Content.ReadAsStringAsync();
            JObject jObject = JObject.Parse(json);

            var payload = jObject["payload"];

            if (!payload.HasValues)
                throw new System.Exception("No ongoing match");
            string ongoingMatchId = payload.First.First[0]["id"].Value<string>();

            stopwatch.Stop();
            TimeSpan timeTaken = stopwatch.Elapsed;
            System.Diagnostics.Trace.WriteLine("FetchOngoingMatchIdAsync:" + timeTaken);
            return ongoingMatchId;
        }

        public async Task<OngoingMatchInfo> FetchOngoingMatchAsync(string matchId)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            var response = await _v1Client.GetAsync($"https://api.faceit.com/match/v2/match/{matchId}");
            if (response.StatusCode != HttpStatusCode.OK)
                throw new System.Exception("Failed to get ongoing match");
            string json = await response.Content.ReadAsStringAsync();
            JObject jObject = JObject.Parse(json);

            var payload = jObject["payload"];

            if (!payload.HasValues)
                throw new System.Exception("No ongoing match");
            var ongoingMatch = payload.ToObject<OngoingMatchInfo>();

            stopwatch.Stop();
            TimeSpan timeTaken = stopwatch.Elapsed;
            System.Diagnostics.Trace.WriteLine("FetchOngoingMatchAsync:" + timeTaken);
            return ongoingMatch;
        }
    }
}