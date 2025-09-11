using OsuScoreCheck.Models.Enums;
using System.Collections.Generic;

namespace OsuScoreCheck.Models.Objects
{
    public class BeatmapFilter
    {
        public int? GameModeId { get; set; }
        public int UserOsuId { get; set; }
        public int? MinTimeSeconds { get; set; }
        public int? MaxTimeSeconds { get; set; }
        public double? MinStarRating { get; set; }
        public double? MaxStarRating { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public double? MinPP { get; set; }
        public double? MaxPP { get; set; }
        public List<string>? Results { get; set; }
        public string? ModsFilter { get; set; }
        public List<string>? SelectedMods { get; set; }
        public string? NameFilter { get; set; }
        public List<string> Statuses { get; set; }
        public SortField SortField { get; set; }
        public SortDirection SortDirection { get; set; }
    }
}
