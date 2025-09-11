using DynamicData;
using OsuScoreCheck.Classes.Images;
using OsuScoreCheck.Models.Api;
using OsuScoreCheck.Models.DB;
using OsuScoreCheck.Models.Enums;
using OsuScoreCheck.Models.Objects;
using OsuScoreCheck.Service;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace OsuScoreCheck.ViewModels
{
    public class ResultViewModel : ViewModelBase, IDisposable
    {
        private readonly UserRepository _userRepository;
        private readonly OsuApiService _osuApiService;
        private readonly SettingsService _settingsService;
        private readonly SourceCache<Beatmap, int> _sourceCache = new(beatmap => beatmap.Id);
        private readonly ReadOnlyObservableCollection<Beatmap> _filteredMaps;
        public ReadOnlyObservableCollection<Beatmap> Maps => _filteredMaps;

        #region prop

        private bool _isLoading = false;
        private int _offset = 0;
        private const int PageSize = 20;
        private GameMode _selectedGameMode;

        public int UserId { get; }

        private SortField _sortField = SortField.None;
        public SortField SortField
        {
            get => _sortField;
            set
            {
                this.RaiseAndSetIfChanged(ref _sortField, value);
                ResetAndReload();
            }
        }

        private SortDirection _sortDirection = SortDirection.None;
        public SortDirection SortDirection
        {
            get => _sortDirection;
            set
            {
                this.RaiseAndSetIfChanged(ref _sortDirection, value);
                ResetAndReload();
            }
        }

        private string _currentSortFieldName;
        public string CurrentSortFieldName
        {
            get => _currentSortFieldName;
            set
            {
                this.RaiseAndSetIfChanged(ref _currentSortFieldName, value);
                SortField = value switch
                {
                    "Result" => SortField.Result,
                    "Time" => SortField.Time,
                    "Star" => SortField.Star,
                    "PP" => SortField.PP,
                    _ => SortField.None
                };
            }
        }

        private string _nameFilter;
        public string NameFilter
        {
            get => _nameFilter;
            set
            {
                this.RaiseAndSetIfChanged(ref _nameFilter, value);
                ResetAndReload();
            }
        }

        private string _minTimePlayed;
        public string MinTimePlayed
        {
            get => _minTimePlayed;
            set
            {
                this.RaiseAndSetIfChanged(ref _minTimePlayed, value);
                ResetAndReload();
            }
        }

        private string _maxTimePlayed;
        public string MaxTimePlayed
        {
            get => _maxTimePlayed;
            set
            {
                this.RaiseAndSetIfChanged(ref _maxTimePlayed, value);
                ResetAndReload();
            }
        }


        private string _minStarRating;
        public string MinStarRating
        {
            get => _minStarRating;
            set
            {
                this.RaiseAndSetIfChanged(ref _minStarRating, value);
                ResetAndReload();
            }
        }

        private string _maxStarRating;
        public string MaxStarRating
        {
            get => _maxStarRating;
            set
            {
                this.RaiseAndSetIfChanged(ref _maxStarRating, value);
                ResetAndReload();
            }
        }

        private string _minPP;
        public string MinPP
        {
            get => _minPP;
            set
            {
                this.RaiseAndSetIfChanged(ref _minPP, value);
                ResetAndReload();
            }
        }

        private string _maxPP;
        public string MaxPP
        {
            get => _maxPP;
            set
            {
                this.RaiseAndSetIfChanged(ref _maxPP, value);
                ResetAndReload();
            }
        }

        private bool _isSSChecked;
        public bool IsSSChecked
        {
            get => _isSSChecked;
            set
            {
                this.RaiseAndSetIfChanged(ref _isSSChecked, value);
                ResetAndReload();
            }
        }

        private bool _isSChecked;
        public bool IsSChecked
        {
            get => _isSChecked;
            set
            {
                this.RaiseAndSetIfChanged(ref _isSChecked, value);
                ResetAndReload();
            }
        }

        private bool _isAChecked;
        public bool IsAChecked
        {
            get => _isAChecked;
            set
            {
                this.RaiseAndSetIfChanged(ref _isAChecked, value);
                ResetAndReload();
            }
        }

        private bool _isBChecked;
        public bool IsBChecked
        {
            get => _isBChecked;
            set
            {
                this.RaiseAndSetIfChanged(ref _isBChecked, value);
                ResetAndReload();
            }
        }

        private bool _isCChecked;
        public bool IsCChecked
        {
            get => _isCChecked;
            set
            {
                this.RaiseAndSetIfChanged(ref _isCChecked, value);
                ResetAndReload();
            }
        }

        private bool _isDChecked;
        public bool IsDChecked
        {
            get => _isDChecked;
            set
            {
                this.RaiseAndSetIfChanged(ref _isDChecked, value);
                ResetAndReload();
            }
        }

        private bool _isEZChecked;
        public bool IsEZChecked
        {
            get => _isEZChecked;
            set
            {
                this.RaiseAndSetIfChanged(ref _isEZChecked, value);
                ResetAndReload();
            }
        }

        private bool _isNFChecked;
        public bool IsNFChecked
        {
            get => _isNFChecked;
            set
            {
                this.RaiseAndSetIfChanged(ref _isNFChecked, value);
                ResetAndReload();
            }
        }

        private bool _isHTChecked;
        public bool IsHTChecked
        {
            get => _isHTChecked;
            set
            {
                this.RaiseAndSetIfChanged(ref _isHTChecked, value);
                ResetAndReload();
            }
        }

        private bool _isHRChecked;
        public bool IsHRChecked
        {
            get => _isHRChecked;
            set
            {
                this.RaiseAndSetIfChanged(ref _isHRChecked, value);
                ResetAndReload();
            }
        }

        private bool _isSDChecked;
        public bool IsSDChecked
        {
            get => _isSDChecked;
            set
            {
                this.RaiseAndSetIfChanged(ref _isSDChecked, value);
                ResetAndReload();
            }
        }

        private bool _isDTChecked;
        public bool IsDTChecked
        {
            get => _isDTChecked;
            set
            {
                this.RaiseAndSetIfChanged(ref _isDTChecked, value);
                ResetAndReload();
            }
        }

        private bool _isHDChecked;
        public bool IsHDChecked
        {
            get => _isHDChecked;
            set
            {
                this.RaiseAndSetIfChanged(ref _isHDChecked, value);
                ResetAndReload();
            }
        }

        private bool _isFLChecked;
        public bool IsFLChecked
        {
            get => _isFLChecked;
            set
            {
                this.RaiseAndSetIfChanged(ref _isFLChecked, value);
                ResetAndReload();
            }
        }

        private bool _isRanked = true;
        public bool IsRanked
        {
            get => _isRanked;
            set
            {
                this.RaiseAndSetIfChanged(ref _isRanked, value);
                ResetAndReload();
            }
        }

        private bool _isQualified = true;
        public bool IsQualified
        {
            get => _isQualified;
            set
            {
                this.RaiseAndSetIfChanged(ref _isQualified, value);
                ResetAndReload();
            }
        }

        private bool _isLoved = true;
        public bool IsLoved
        {
            get => _isLoved;
            set
            {
                this.RaiseAndSetIfChanged(ref _isLoved, value);
                ResetAndReload();
            }
        }

        private bool _isUnranked = true;
        public bool IsUnranked
        {
            get => _isUnranked;
            set
            {
                this.RaiseAndSetIfChanged(ref _isUnranked, value);
                ResetAndReload();
            }
        }

        private bool _isContextMenuVisible;
        public bool IsContextMenuVisible
        {
            get => _isContextMenuVisible;
            set => this.RaiseAndSetIfChanged(ref _isContextMenuVisible, value);
        }

        private Beatmap _selectedBeatmap;
        public Beatmap SelectedBeatmap
        {
            get => _selectedBeatmap;
            set => this.RaiseAndSetIfChanged(ref _selectedBeatmap, value);
        }

        public ReactiveCommand<Beatmap, Unit> OpenMapCommand { get; }
        public ReactiveCommand<Beatmap, Unit> ShowContextMenuCommand { get; }
        public ReactiveCommand<Beatmap, Unit> RefreshMapCommand { get; }
        public ReactiveCommand<Unit, Unit> CloseContextMenuCommand { get; }

        #endregion

        public ResultViewModel(int userId, GameMode selectedGameMode)
        {
            _userRepository = new UserRepository();
            _osuApiService = new OsuApiService();
            _settingsService = new SettingsService();
            UserId = userId;
            _selectedGameMode = selectedGameMode;

            OpenMapCommand = ReactiveCommand.Create<Beatmap>(OpenMap);
            ShowContextMenuCommand = ReactiveCommand.Create<Beatmap>(ShowContextMenu);
            RefreshMapCommand = ReactiveCommand.Create<Beatmap>(RefreshMapAsync);
            CloseContextMenuCommand = ReactiveCommand.Create(CloseContextMenu);

            MessageBus.Current.Listen<SortDirectionChangedMessage>()
                      .Subscribe(message =>
                      {
                          CurrentSortFieldName = message.SortField;
                          SortDirection = message.SortDirection;
                      });

            _sourceCache.Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _filteredMaps)
            .Subscribe();

            Task.Run(async () => await LoadMoreMapsAsync());
        }

        private void ShowContextMenu(Beatmap beatmap)
        {
            SelectedBeatmap = beatmap;
            IsContextMenuVisible = true;
        }

        private async Task ReloadSingleMapAsync(int beatmapId)
        {
            try
            {
                var map = await _userRepository.GetBeatmapByIdAndUserOsuId(beatmapId, UserId);
                _sourceCache.Edit(updater =>
                {
                    updater.Remove(beatmapId);
                    updater.AddOrUpdate(map);
                });
                Debug.WriteLine($"Reloaded map {beatmapId}: PP={map.PP}, Mods={map.Mods}, ResultId={map.ResultId}, Result={map.Result?.Name}, SimplifiedCategory={map.SimplifiedCategory?.Name}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error in ReloadSingleMapAsync: {ex.Message}");
            }
        }

        private async void RefreshMapAsync(Beatmap beatmap)
        {
            if (beatmap == null) return;

            try
            {
                var settings = _settingsService.LoadSettings();
                var accessToken = await _osuApiService.GetAccessTokenAsync(settings.ID, settings.ClientSecret);
                if (string.IsNullOrEmpty(accessToken))
                {
                    throw new InvalidOperationException("Failed to get access token");
                }

                var osuId = await _userRepository.GetOsuIdByIdAsync(UserId);
                if (osuId == 0)
                {
                    throw new InvalidOperationException($"User with Id {UserId} not found in UsersOsu");
                }

                var mostPlayedBeatmap = new MostPlayedBeatmap
                {
                    BeatmapId = beatmap.BeatmapId,
                    Beatmap = new BeatmapDetails
                    {
                        Id = beatmap.BeatmapId,
                        Version = beatmap.Difficulty,
                        Mode = _selectedGameMode.Name,
                        DifficultyRating = beatmap.StarRating
                    },
                    Beatmapset = new BeatmapsetDetails
                    {
                        Id = beatmap.BeatmapsetId,
                        Title = beatmap.BeatmapName,
                        Covers = new Covers { Cover = beatmap.BackgroundImage }
                    }
                };

                var scores = await RateLimitedGetScoreDataAsync(beatmap.BeatmapId, osuId, accessToken);
                var score = scores?.OrderByDescending(s => s.Score).FirstOrDefault();
                if (score != null)
                {
                    await UpdateBeatmapWithResultsAsync(mostPlayedBeatmap, score, UserId);
                    await ReloadSingleMapAsync(beatmap.BeatmapId);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error refreshing beatmap {beatmap.BeatmapId}: {ex.Message}");
            }
            finally
            {
                CloseContextMenu();
            }
        }

        private async Task<List<ScoreData>?> RateLimitedGetScoreDataAsync(int beatmapId, int osuId, string accessToken)
        {
            const int requestsPerMinute = 60;
            const int delayMs = 1000 * 60 / requestsPerMinute;

            var response = await _osuApiService.GetBestScoreDataAsync(beatmapId, osuId, accessToken);
            await Task.Delay(delayMs);
            return response;
        }

        private async Task UpdateBeatmapWithResultsAsync(MostPlayedBeatmap map, ScoreData scoreData, int userOsuId)
        {
            if (map.Beatmap == null || map.Beatmapset == null)
            {
                throw new InvalidOperationException($"[ERROR] Некорректные данные карты.");
            }

            var existingMap = await _userRepository.GetBeatmapByIdAndUserOsuId(map.Beatmap.Id, userOsuId);
            if (existingMap == null)
            {
                throw new InvalidOperationException($"Карта с BeatmapId = {map.Beatmap.Id} для пользователя UserOsuId = {userOsuId} не найдена.");
            }

            var modsList = scoreData.Mods?.ToList() ?? new List<string>();
            var normalizedMods = NormalizeMods(modsList);
            string modsString = normalizedMods.Any() ? string.Join(" ", normalizedMods.OrderBy(m => m)) : "";

            string normalizedResult = NormalizeResult(scoreData.Rank);

            int resultId = normalizedResult switch
            {
                "" => 1,
                "D" => 2,
                "C" => 3,
                "B" => 4,
                "A" => 5,
                "S" => 6,
                "SS" => 7,
                _ => 1
            };

            existingMap.PP = scoreData.Pp;
            existingMap.Mods = modsString;
            existingMap.ResultId = resultId;

            await _userRepository.UpdateBeatmapAsync(existingMap);
        }

        private List<string> NormalizeMods(List<string> mods)
        {
            var normalized = new List<string>();
            foreach (var mod in mods)
            {
                switch (mod)
                {
                    case "NC":
                        if (!normalized.Contains("DT")) normalized.Add("DT");
                        break;
                    case "PF":
                        if (!normalized.Contains("SD")) normalized.Add("SD");
                        break;
                    default:
                        if (!normalized.Contains(mod)) normalized.Add(mod);
                        break;
                }
            }
            return normalized;
        }

        private string NormalizeResult(string rank)
        {
            return rank switch
            {
                "X" => "SS",
                "SH" => "S",
                "XH" => "SS",
                _ => rank
            };
        }

        private void ResetAndReload()
        {
            _offset = 0;
            _sourceCache.Clear();
            Task.Run(async () => await LoadMoreMapsAsync());
        }

        private void CloseContextMenu()
        {
            IsContextMenuVisible = false;
            SelectedBeatmap = null;
        }

        public async Task LoadMoreMapsAsync()
        {
            if (_isLoading) return;

            _isLoading = true;
            try
            {
                int? minSeconds = ParseTimeToSeconds(MinTimePlayed);
                int? maxSeconds = ParseTimeToSeconds(MaxTimePlayed);
                double? minStar = ParseDifficulty(MinStarRating);
                double? maxStar = ParseDifficulty(MaxStarRating);
                double? minPP = ParsePP(MinPP);
                double? maxPP = ParsePP(MaxPP);

                var selectedResults = new List<string>();
                if (IsSSChecked) selectedResults.Add("SS");
                if (IsSChecked) selectedResults.Add("S");
                if (IsAChecked) selectedResults.Add("A");
                if (IsBChecked) selectedResults.Add("B");
                if (IsCChecked) selectedResults.Add("C");
                if (IsDChecked) selectedResults.Add("D");

                var selectedMods = new List<string>();
                if (IsEZChecked) selectedMods.Add("EZ");
                if (IsNFChecked) selectedMods.Add("NF");
                if (IsHTChecked) selectedMods.Add("HT");
                if (IsHRChecked) selectedMods.Add("HR");
                if (IsSDChecked) selectedMods.Add("SD");
                if (IsDTChecked) selectedMods.Add("DT");
                if (IsHDChecked) selectedMods.Add("HD");
                if (IsFLChecked) selectedMods.Add("FL");
                string modsFilter = selectedMods.Any() ? string.Join(" ", selectedMods.OrderBy(m => m)) : null;

                var selectedStatuses = new List<string>();
                if (IsRanked) selectedStatuses.Add("Ranked");
                if (IsQualified) selectedStatuses.Add("Qualified");
                if (IsLoved) selectedStatuses.Add("Loved");
                if (IsUnranked) selectedStatuses.Add("Unranked");

                var filter = new BeatmapFilter
                {
                    UserOsuId = UserId,
                    MinTimeSeconds = minSeconds,
                    MaxTimeSeconds = maxSeconds,
                    MinStarRating = minStar,
                    MaxStarRating = maxStar,
                    Offset = _offset,
                    Limit = PageSize,
                    MinPP = minPP,
                    MaxPP = maxPP,
                    Results = selectedResults.Any() ? selectedResults : null,
                    ModsFilter = modsFilter,
                    NameFilter = NameFilter,
                    Statuses = selectedStatuses.Any() ? selectedStatuses : null,
                    SortField = SortField,
                    SortDirection = SortDirection,
                    GameModeId = _selectedGameMode?.Id == -1 ? null : _selectedGameMode?.Id
                };

                var newMaps = await _userRepository.GetMapsByTimeAsync(filter);
                if (newMaps.Any())
                {
                    _sourceCache.Edit(updater => updater.AddOrUpdate(newMaps));
                    _offset += newMaps.Count;
                }
                else
                {
                    Debug.WriteLine("No more maps to load");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in LoadMoreMapsAsync: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
            }
        }

        #region parse

        private static int? Parse(string time, bool isTime = false)
        {
            if (string.IsNullOrWhiteSpace(time))
                return null;

            if (isTime)
            {
                if (int.TryParse(time, out int secondsOnly))
                    return secondsOnly;

                if (TimeSpan.TryParseExact(time, @"m\:ss", null, out var parsedTime) ||
                    TimeSpan.TryParseExact(time, @"h\:mm\:ss", null, out parsedTime))
                    return (int)parsedTime.TotalSeconds;

                return null;
            }

            return null;
        }

        private static double? Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result) ? result : null;
        }

        private static int? ParseTimeToSeconds(string time) => Parse(time, true);
        private static double? ParseDifficulty(string difficulty) => Parse(difficulty);
        private static double? ParsePP(string pp) => Parse(pp);

        #endregion

        private async void OpenMap(Beatmap map)
        {
            string url = await GenerateMapUrl(map);
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to open URL: {ex.Message}");
            }
        }

        private async Task<string> GenerateMapUrl(Beatmap map)
        {
            if (map == null || map.BeatmapId == 0 || map.BeatmapsetId == 0)
            {
                throw new InvalidOperationException("Недостаточно данных для создания URL карты.");
            }

            string baseUrl = "https://osu.ppy.sh/beatmapsets";
            string gameMode = await _userRepository.GetGameModeNameAsync(map.GameModeId);
            string mods = string.IsNullOrEmpty(map.Mods) ? "" : $"?mods={Uri.EscapeDataString(map.Mods)}";

            return $"{baseUrl}/{map.BeatmapsetId}#{gameMode}/{map.BeatmapId}{mods}";
        }

        public void Dispose()
        {
            _sourceCache.Clear();
            if (AsyncImageLoader.ImageLoader.AsyncImageLoader is CustomRamCachedWebImageLoader loader)
            {
                loader.ClearCache();
            }
            _sourceCache.Dispose();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}