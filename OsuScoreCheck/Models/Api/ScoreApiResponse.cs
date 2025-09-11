using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OsuScoreCheck.Models.Api
{
    public class ScoreApiResponse
    {
        [JsonPropertyName("scores")]
        public List<ScoreData> Scores { get; set; }
    }
}
