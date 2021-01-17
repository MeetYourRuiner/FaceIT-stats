﻿using Newtonsoft.Json;
using System;

namespace faceitwpf.Models
{
    [JsonConverter(typeof(JsonPathConverter))]
    public class Match
    {
        public int Index { get; set; }

        [JsonProperty("matchId")]
        public string Id { get; set; }

        [JsonProperty("date")]
        public long _Date { set => Date = DateTimeOffset.FromUnixTimeMilliseconds(value).ToLocalTime(); }
        public DateTimeOffset Date { get; set; }

        [JsonProperty("i1")]
        public string Map { get; set; }

        [JsonProperty("i18")]
        public string Score { get; set; }

        [JsonProperty("i10")]
        public short _Result { set => Result = value == 1 ? 'W' : 'L'; }
        public char Result { get; set; }

        [JsonProperty("i6")]
        public int Kills { get; set; }

        [JsonProperty("i8")]
        public int Deaths { get; set; }

        [JsonProperty("c2")]
        public double KDRatio { get; set; }

        [JsonProperty("c3")]
        public double KRRatio { get; set; }

        [JsonProperty("c4")]
        public double HSPercentage { get; set; }

        [JsonProperty("elo")]
        public int ELO { get; set; }
        public int ChangeELO { get; set; } = 0;
        public string ResultELO
        {
            get
            {
                var sign = ChangeELO >= 0 ? "+" : string.Empty;
                return $"{Result}({sign}{ChangeELO})";
            }
        }

        public int AvgLevel { get; set; }
        public MatchOverview MatchOverview { get; set; }
        public string LevelImage { get => $"/faceitwpf;component/Resources/lvl{AvgLevel}.png"; }
    }
}
