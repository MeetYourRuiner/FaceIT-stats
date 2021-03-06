﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace faceitwpf.Models
{
    [JsonConverter(typeof(JsonPathConverter))]
    public class PlayerOverallStats
    {
        [JsonProperty("_id.playerId")]
        public string Id { get; set; }
        [JsonProperty("m1")]
        public int Matches { get; set; }
        [JsonProperty("k5")]
        public double KDRatio { get; set; }
        [JsonProperty("k6")]
        public int Winrate { get; set; }
        [JsonProperty("k8")]
        public int HSPercentage { get; set; }
        public List<MapOverallStats> MapOverallStats { get; set; }
    }

    public class MapOverallStats
    {
        public string MapName { get; set; }
        public string MapImage { get => $"/faceitwpf;component/Resources/{MapName}.jpeg"; }
        [JsonProperty("m1")]
        public int Matches { get; set; }
        [JsonProperty("k6")]
        public int Winrate { get; set; }
        public double WinrateDouble { get => (double)Winrate / 100; }
        [JsonProperty("k1")]
        public double Kills { get; set; }
        [JsonProperty("k2")]
        public double Deaths { get; set; }
        [JsonProperty("k3")]
        public double Assists { get; set; }
        [JsonProperty("k5")]
        public double KDRatio { get; set; }
        [JsonProperty("k9")]
        public double KRRatio { get; set; }
        [JsonProperty("k7")]
        public double HSPerMatch { get; set; }
        [JsonProperty("k8")]
        public int HSPercentage { get; set; }
        [JsonProperty("k10")]
        public double TriplePerMatch { get; set; }
        [JsonProperty("k11")]
        public double QuadroPerMatch { get; set; }
        [JsonProperty("k12")]
        public double AcePerMatch { get; set; }
    }
}
