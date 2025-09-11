using System.Text.Json.Serialization;

namespace OsuScoreCheck.Models.Api
{
    public class BeatmapDetails
    {
        [JsonPropertyName("beatmapset_id")]
        public int BeatmapsetId { get; set; }

        [JsonPropertyName("difficulty_rating")]
        public double DifficultyRating { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("mode")]
        public string Mode { get; set; } = null!;

        [JsonPropertyName("status")]
        public string Status { get; set; } = null!;

        [JsonPropertyName("total_length")]
        public int TotalLength { get; set; }

        [JsonPropertyName("user_id")]
        public int UserId { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; } = null!;
    }
}
