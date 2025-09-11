using System.Text.Json.Serialization;

namespace OsuScoreCheck.Models.Api
{
    public class MostPlayedBeatmap
    {
        [JsonPropertyName("beatmap_id")]
        public int BeatmapId { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("beatmap")]
        public BeatmapDetails Beatmap { get; set; } = null!;

        [JsonPropertyName("beatmapset")]
        public BeatmapsetDetails Beatmapset { get; set; } = null!;

    }
}
