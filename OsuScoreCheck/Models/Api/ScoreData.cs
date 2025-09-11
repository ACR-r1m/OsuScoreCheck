using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OsuScoreCheck.Models.Api
{
    public class ScoreData
    {
        [JsonPropertyName("rank")]
        public string Rank { get; set; }

        [JsonPropertyName("accuracy")]
        public double Accuracy { get; set; }

        [JsonPropertyName("best_id")]
        public long? BestId { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("max_combo")]
        public int MaxCombo { get; set; }

        [JsonPropertyName("mode")]
        public string Mode { get; set; }

        [JsonPropertyName("mode_int")]
        public int ModeInt { get; set; }

        [JsonPropertyName("mods")]
        public List<string> Mods { get; set; }

        [JsonPropertyName("passed")]
        public bool Passed { get; set; }

        [JsonPropertyName("perfect")]
        public bool Perfect { get; set; }

        [JsonPropertyName("pp")]
        public double? Pp { get; set; }

        [JsonPropertyName("replay")]
        public bool Replay { get; set; }

        [JsonPropertyName("score")]
        public int Score { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("user_id")]
        public long UserId { get; set; }

    }
}
