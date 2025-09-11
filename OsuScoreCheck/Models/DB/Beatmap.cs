namespace OsuScoreCheck.Models.DB
{
    public class Beatmap
    {
        public int Id { get; set; }
        public int UserOsuId { get; set; }
        public UserOsu UserOsu { get; set; }
        public int BeatmapsetId { get; set; }
        public int BeatmapId { get; set; }
        public string BeatmapName { get; set; }
        public string Difficulty { get; set; }
        public int TimePlayedSeconds { get; set; }
        public string TimePlayed { get; set; }
        public double StarRating { get; set; }
        public double? PP { get; set; }
        public string? Mods { get; set; }
        public int SimplifiedCategoryId { get; set; }
        public string? BackgroundImage { get; set; }

        public int? ResultId { get; set; }
        public Result Result { get; set; }


        public int? GameModeId { get; set; }
        public GameMode GameMode { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public Category SimplifiedCategory { get; set; }
    }
}
