using System.Text.Json.Serialization;

namespace OsuScoreCheck.Models.Api
{
    public class BeatmapsetDetails
    {
        [JsonPropertyName("artist")]
        public string Artist { get; set; } = null!;

        [JsonPropertyName("artist_unicode")]
        public string ArtistUnicode { get; set; } = null!;

        [JsonPropertyName("covers")]
        public Covers Covers { get; set; } = null!;

        [JsonPropertyName("creator")]
        public string Creator { get; set; } = null!;

        [JsonPropertyName("favourite_count")]
        public int FavouriteCount { get; set; }

        [JsonPropertyName("hype")]
        public object? Hype { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nsfw")]
        public bool Nsfw { get; set; }

        [JsonPropertyName("offset")]
        public int Offset { get; set; }

        [JsonPropertyName("play_count")]
        public int PlayCount { get; set; }

        [JsonPropertyName("preview_url")]
        public string PreviewUrl { get; set; } = null!;

        [JsonPropertyName("source")]
        public string Source { get; set; } = null!;

        [JsonPropertyName("spotlight")]
        public bool Spotlight { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = null!;

        [JsonPropertyName("title")]
        public string Title { get; set; } = null!;

        [JsonPropertyName("title_unicode")]
        public string TitleUnicode { get; set; } = null!;

        [JsonPropertyName("track_id")]
        public object? TrackId { get; set; }

        [JsonPropertyName("user_id")]
        public int UserId { get; set; }

        [JsonPropertyName("video")]
        public bool Video { get; set; }
    }
}
