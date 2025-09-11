using ReactiveUI;
using System;
using System.Collections.Generic;

namespace OsuScoreCheck.Models.DB
{
    public class UserOsu : ReactiveObject
    {
        public bool AllMapsSaved { get; set; }

        private int _id;
        private int _osuId;
        private string _username = null!;
        private DateTime _createdAt;
        private int _totalPlayedMaps;
        private string _avatarUrl = null!;
        private int _lastCheckedMapId;
        private bool _isLoading;

        public int Id
        {
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }

        public int OsuId
        {
            get => _osuId;
            set => this.RaiseAndSetIfChanged(ref _osuId, value);
        }

        public string Username
        {
            get => _username;
            set => this.RaiseAndSetIfChanged(ref _username, value);
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set => this.RaiseAndSetIfChanged(ref _createdAt, value);
        }

        public int TotalPlayedMaps
        {
            get => _totalPlayedMaps;
            set => this.RaiseAndSetIfChanged(ref _totalPlayedMaps, value);
        }

        public string AvatarUrl
        {
            get => _avatarUrl;
            set => this.RaiseAndSetIfChanged(ref _avatarUrl, value);
        }

        public int LastCheckedMapId
        {
            get => _lastCheckedMapId;
            set => this.RaiseAndSetIfChanged(ref _lastCheckedMapId, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => this.RaiseAndSetIfChanged(ref _isLoading, value);
        }

        public ICollection<Beatmap> Beatmaps { get; set; } = new List<Beatmap>();
    }
}
