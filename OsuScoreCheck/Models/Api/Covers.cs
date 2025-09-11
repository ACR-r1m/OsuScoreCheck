using System.Text.Json.Serialization;

namespace OsuScoreCheck.Models.Api
{
    public class Covers
    {
        [JsonPropertyName("cover")]
        public string Cover { get; set; } = null!;

        [JsonPropertyName("cover@2x")]
        public string Cover2X { get; set; } = null!;

        [JsonPropertyName("card")]
        public string Card { get; set; } = null!;

        [JsonPropertyName("card@2x")]
        public string Card2X { get; set; } = null!;

        [JsonPropertyName("list")]
        public string List { get; set; } = null!;

        [JsonPropertyName("list@2x")]
        public string List2X { get; set; } = null!;

        [JsonPropertyName("slimcover")]
        public string SlimCover { get; set; } = null!;

        [JsonPropertyName("slimcover@2x")]
        public string SlimCover2X { get; set; } = null!;
    }
}
