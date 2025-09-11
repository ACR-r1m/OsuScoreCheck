using System.Text.Json.Serialization;

namespace OsuScoreCheck.Models.Api
{
    public class UserProfile
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; } = null!;

        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; set; } = null!;

        [JsonPropertyName("beatmap_playcounts_count")]
        public int PlayCount { get; set; }
    }
}
