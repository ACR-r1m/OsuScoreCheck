using OsuScoreCheck.Classes.PropGive;
using OsuScoreCheck.Models.Api;
using OsuScoreCheck.Models.DB;
using OsuScoreCheck.Service;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace OsuScoreCheck.ViewModels
{
    public class UsersViewModel : ViewModelBase
    {
        #region prop
        private OsuApiService _osuApiService = new OsuApiService();
        private readonly UserRepository _userRepository = new UserRepository();
        private SettingsService _settingsService = new SettingsService();

        private ObservableCollection<UserOsu> _users;

        public ObservableCollection<UserOsu> Users
        {
            get => _users;
            set => this.RaiseAndSetIfChanged(ref _users, value);
        }


        private ObservableCollection<GameMode> _gameModes;
        public ObservableCollection<GameMode> GameModes
        {
            get => _gameModes;
            set => this.RaiseAndSetIfChanged(ref _gameModes, value);
        }

        private bool _isUsersEmpty;
        public bool IsUsersEmpty
        {
            get => _isUsersEmpty;
            set => this.RaiseAndSetIfChanged(ref _isUsersEmpty, value);
        }

        private string _userIdInput = string.Empty;
        public string UserIdInput
        {
            get => _userIdInput;
            set => this.RaiseAndSetIfChanged(ref _userIdInput, value);
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        private bool _isErrorVisible;
        public bool IsErrorVisible
        {
            get => !string.IsNullOrWhiteSpace(ErrorMessage);
            set => this.RaiseAndSetIfChanged(ref _isErrorVisible, value);
        }

        private string _selectedGameMode;
        public string SelectedGameMode
        {
            get => _selectedGameMode;
            set => this.RaiseAndSetIfChanged(ref _selectedGameMode, value);
        }

        private bool _isContextMenuVisible;
        public bool IsContextMenuVisible
        {
            get => _isContextMenuVisible;
            set => this.RaiseAndSetIfChanged(ref _isContextMenuVisible, value);
        }

        private UserOsu _selectedUser;
        public UserOsu SelectedUser
        {
            get => _selectedUser;
            set => this.RaiseAndSetIfChanged(ref _selectedUser, value);
        }

        private int _contextMenuUserId;

        public ReactiveCommand<string, Unit> SelectGameModeCommand { get; }
        public ReactiveCommand<int, Unit> SelectUserCommand { get; }
        public ReactiveCommand<int, Unit> UserRightClickCommand { get; }
        public ReactiveCommand<Unit, Unit> AnalyzeUserCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteUserCommand { get; }
        public ReactiveCommand<Unit, Unit> HideContextMenuCommand { get; }
        public ReactiveCommand<Unit, Unit> ReloadRecentMapsCommand { get; }
        public ReactiveCommand<int, Unit> OpenLinkProfileCommand { get; }
        #endregion

        public UsersViewModel()
        {
            SelectGameModeCommand = ReactiveCommand.Create<string>(mode =>
            {
                SelectedGameMode = mode;
                GameModeStateService.LastSelectedGameMode = new GameMode { Name = mode, Id = GetGameModeId(mode) };
            });

            SelectUserCommand = ReactiveCommand.Create<int>(NavigateToUserResults);
            UserRightClickCommand = ReactiveCommand.Create<int>(ShowContextMenu);
            DeleteUserCommand = ReactiveCommand.Create(DeleteUser);
            AnalyzeUserCommand = ReactiveCommand.CreateFromTask(AnalyzeUserAsync);
            HideContextMenuCommand = ReactiveCommand.Create(HideContextMenu);
            OpenLinkProfileCommand = ReactiveCommand.Create<int>(OpenProfile);

            Task.Run(async () => await LoadUsersFromDatabaseAsync());

            GameModeStateService.LastSelectedGameMode = new GameMode { Name = "osu", Id = 1 };
            SelectedGameMode = "osu";
        }

        private int GetGameModeId(string mode) => mode switch
        {
            "osu" => 1,
            "taiko" => 2,
            "fruits" => 3,
            "mania" => 4,
            _ => 1
        };

        public async Task LoadUsersFromDatabaseAsync()
        {
            try
            {
                var usersFromDb = await _userRepository.GetAllUsersAsync();
                Users = new ObservableCollection<UserOsu>(usersFromDb);

                foreach (var user in Users.Where(u => u.IsLoading))
                {
                    _ = Task.Run(async () => await AnalyzeUserMapsAsync(user));
                }

                IsUsersEmpty = Users.Count == 0;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при загрузке пользователей: {ex.Message}";
                IsErrorVisible = true;
            }
        }

        private async Task AnalyzeUserAsync()
        {
            var userProfile = await ValidateUserAndProfile();
            if (userProfile == null)
            {
                return;
            }

            var user = await AddOrUpdateUserInDatabase(userProfile);
            await AnalyzeUserMapsAsync(user);
        }

        private async Task<UserProfile?> ValidateUserAndProfile()
        {
            ErrorMessage = string.Empty;
            IsErrorVisible = false;

            if (string.IsNullOrWhiteSpace(UserIdInput) || !int.TryParse(UserIdInput, out int osuId))
            {
                ErrorMessage = Localizer.Instance["InvalidUserId"];
                IsErrorVisible = true;
                return null;
            }

            var settings = _settingsService.LoadSettings();
            if (settings == null || string.IsNullOrEmpty(settings.ID) || string.IsNullOrEmpty(settings.ClientSecret))
            {
                ErrorMessage = Localizer.Instance["ApiNotSaved"];
                IsErrorVisible = true;
                return null;
            }

            var accessToken = await _osuApiService.GetAccessTokenAsync(settings.ID, settings.ClientSecret);
            if (string.IsNullOrEmpty(accessToken))
            {
                ErrorMessage = Localizer.Instance["AccessTokenError"];
                IsErrorVisible = true;
                return null;
            }

            var userProfile = await _osuApiService.GetUserProfileAsync(osuId, accessToken);
            if (userProfile == null)
            {
                ErrorMessage = Localizer.Instance["UserNotFound"];
                IsErrorVisible = true;
                return null;
            }

            return userProfile;
        }

        private async Task<UserOsu> AddOrUpdateUserInDatabase(UserProfile userProfile)
        {
            var user = new UserOsu
            {
                OsuId = userProfile.Id,
                Username = userProfile.Username,
                AvatarUrl = userProfile.AvatarUrl,
                CreatedAt = DateTime.Now,
                IsLoading = true
            };

            await _userRepository.AddOrUpdateUserAsync(user);
            Users.Add(user);
            IsUsersEmpty = Users.Count == 0;
            return user;
        }

        private async Task AnalyzeUserMapsAsync(UserOsu user)
        {
            var settings = _settingsService.LoadSettings();
            var accessToken = await _osuApiService.GetAccessTokenAsync(settings.ID, settings.ClientSecret);

            var beatmapCount = (await _userRepository.GetMapsToCheckAsync(user.OsuId, 0)).Count;
            if (beatmapCount > user.TotalPlayedMaps)
            {
                await _userRepository.DeleteBeatmapsAfterCountAsync(user.OsuId, user.TotalPlayedMaps);
                if (user.LastCheckedMapId > user.TotalPlayedMaps)
                {
                    user.LastCheckedMapId = user.TotalPlayedMaps;
                    await _userRepository.AddOrUpdateUserAsync(user);
                }
            }


            var channel = Channel.CreateUnbounded<MostPlayedBeatmap>();
            int limit = 100;

            var producerTask = Task.Run(async () =>
            {
                if (!user.AllMapsSaved)
                {
                    int offset = user.TotalPlayedMaps;

                    while (true)
                    {
                        var currentBatch = await _osuApiService.GetPlayedMapsBatchAsync(user.OsuId, accessToken, limit, offset);
                        if (currentBatch == null || currentBatch.Count == 0)
                        {
                            user.AllMapsSaved = true;
                            await _userRepository.AddOrUpdateUserAsync(user);
                            break;
                        }

                        foreach (var map in currentBatch)
                        {
                            bool saved = await SaveBeatmapWithoutResultsAsync(map, user.OsuId);
                            if (saved)
                            {
                                user.TotalPlayedMaps++;
                                await channel.Writer.WriteAsync(map);
                                await _userRepository.AddOrUpdateUserAsync(user);
                            }
                            else
                            {
                                throw new InvalidOperationException($"Failed to save beatmap {map.Beatmap.Id} for user {user.OsuId}");
                            }
                        }

                        offset += currentBatch.Count;
                        await Task.Delay(1000);
                    }
                    channel.Writer.Complete();
                }
                else
                {
                    await Task.Run(async () =>
                    {
                        var mapsToCheck = await _userRepository.GetMapsToCheckAsync(user.OsuId, user.LastCheckedMapId);

                        if (mapsToCheck == null || mapsToCheck.Count == 0)
                        {
                            channel.Writer.Complete();
                            return;
                        }

                        foreach (var map in mapsToCheck)
                        {
                            try
                            {
                                var mostPlayedBeatmap = ConvertToMostPlayedBeatmap(map);
                                await channel.Writer.WriteAsync(mostPlayedBeatmap);
                            }
                            catch (Exception ex)
                            {
                                throw new InvalidOperationException($"Error converting map: {ex.Message}");
                            }
                        }

                        channel.Writer.Complete();
                    });
                }
            });

            var consumerTask = Task.Run(async () =>
            {
                const int batchSize = 6;
                var batch = new List<MostPlayedBeatmap>(batchSize);

                await foreach (var map in channel.Reader.ReadAllAsync())
                {
                    batch.Add(map);

                    if (batch.Count >= batchSize)
                    {
                        await ProcessBatchInParallelAsync(batch, user, accessToken);
                        batch.Clear();
                    }
                }

                if (batch.Count > 0)
                {
                    await ProcessBatchInParallelAsync(batch, user, accessToken);
                }

            });

            await Task.WhenAll(producerTask, consumerTask);

            if (user.LastCheckedMapId >= user.TotalPlayedMaps)
            {
                user.IsLoading = false;
                await _userRepository.AddOrUpdateUserAsync(user);
            }

            LoadUsersFromDatabaseAsync();
        }

        private MostPlayedBeatmap ConvertToMostPlayedBeatmap(Beatmap map)
        {
            return new MostPlayedBeatmap
            {
                BeatmapId = map.BeatmapId,
                Beatmap = new BeatmapDetails
                {
                    Id = map.BeatmapId,
                    BeatmapsetId = map.BeatmapId,
                },
                Beatmapset = new BeatmapsetDetails
                {
                    Id = map.BeatmapsetId,
                }
            };
        }

        private async Task ProcessBatchInParallelAsync(List<MostPlayedBeatmap> batch, UserOsu user, string accessToken)
        {
            var semaphore = new SemaphoreSlim(6);
            var tasks = batch.Select(async map =>
            {
                await semaphore.WaitAsync();
                try
                {
                    var scores = await RateLimitedGetScoreDataAsync(map.Beatmap.Id, user.OsuId, accessToken);
                    var score = scores?.OrderByDescending(s => s.Score).FirstOrDefault();
                    if (score != null)
                    {
                        await UpdateBeatmapWithResultsAsync(map, score, user.OsuId);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error processing beatmap {map.Beatmap.Id} for user {user.OsuId}: {ex.Message}");
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);
            user.LastCheckedMapId += batch.Count;
            await _userRepository.AddOrUpdateUserAsync(user); // Включает SaveChangesAsync
        }

        private async Task<List<ScoreData>?> RateLimitedGetScoreDataAsync(int beatmapId, int osuId, string accessToken)
        {
            const int requestsPerMinute = 60;
            const int delayMs = 1000 * 60 / requestsPerMinute;

            var response = await _osuApiService.GetBestScoreDataAsync(beatmapId, osuId, accessToken);
            await Task.Delay(delayMs);
            return response;
        }

        private async Task<bool> SaveBeatmapWithoutResultsAsync(MostPlayedBeatmap map, int userId)
        {
            if (map.Beatmap == null || map.Beatmapset == null)
            {
                Debug.WriteLine($"[ERROR] Некорректные данные карты: BeatmapId={map.Beatmap?.Id}");
                return false;
            }

            string gameMode = map.Beatmap.Mode ?? "osu";
            int gameModeId = gameMode switch
            {
                "osu" => 1,
                "taiko" => 2,
                "fruits" => 3,
                "mania" => 4,
                _ => 1
            };

            var userOsuId = await _userRepository.EnsureUserExistsAndGetId(userId);
            if (userOsuId == 0)
            {
                return false;
            }

            var existingMap = await _userRepository.GetBeatmapByIdAndUserOsuId(map.Beatmap.Id, userOsuId);
            if (existingMap != null)
            {
                return false;
            }

            var nextId = await _userRepository.CalculateNextIdForUser(userOsuId);

            TimeSpan timePlayed = TimeSpan.FromSeconds(map.Beatmap.TotalLength);
            string formattedTime = timePlayed.Hours > 0
                ? timePlayed.ToString(@"h\:mm\:ss")
                : timePlayed.Minutes > 0
                    ? timePlayed.ToString(@"m\:ss")
                    : timePlayed.Seconds.ToString();

            int categoryId = map.Beatmapset.Status.ToLower() switch
            {
                "ranked" => 1,
                "approved" => 2,
                "qualified" => 3,
                "loved" => 4,
                "pending" => 5,
                "wip" => 6,
                "graveyard" => 7,
                "unranked" => 8,
                _ => 8
            };

            int simplifiedCategoryId = map.Beatmapset.Status.ToLower() switch
            {
                "ranked" => 1,
                "approved" => 1,
                "qualified" => 3,
                "loved" => 4,
                "pending" => 8,
                "wip" => 8,
                "graveyard" => 8,
                _ => 8
            };

            var userPlayedMap = new Beatmap
            {
                Id = nextId,
                UserOsuId = userOsuId,
                BeatmapId = map.Beatmap.Id,
                BeatmapsetId = map.Beatmapset.Id,
                BeatmapName = map.Beatmapset.Title,
                Difficulty = map.Beatmap.Version,
                StarRating = map.Beatmap.DifficultyRating,
                TimePlayed = formattedTime,
                TimePlayedSeconds = (int)timePlayed.TotalSeconds,
                BackgroundImage = map.Beatmapset.Covers.Cover,
                CategoryId = categoryId,
                SimplifiedCategoryId = simplifiedCategoryId,
                GameModeId = gameModeId,
                ResultId = 1
            };

            await _userRepository.AddOrUpdateBeatmapAsync(userPlayedMap);
            return true;
        }

        private async Task UpdateBeatmapWithResultsAsync(MostPlayedBeatmap map, ScoreData scoreData, int userId)
        {
            if (map.Beatmap == null || map.Beatmapset == null)
            {
                throw new InvalidOperationException($"[ERROR] Некорректные данные карты.");
            }

            var userOsuId = await _userRepository.EnsureUserExistsAndGetId(userId);
            if (userOsuId == 0)
            {
                throw new InvalidOperationException($"Не удалось найти или создать пользователя с OsuId {userId}.");
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

            existingMap.PP = scoreData.Pp ?? 0;
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
                        if (!normalized.Contains("DT")) normalized.Add("DT"); // NC → DT
                        break;
                    case "PF":
                        if (!normalized.Contains("SD")) normalized.Add("SD"); // PF → SD
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

        private void NavigateToUserResults(int userId)
        {
            var selectedUser = Users.FirstOrDefault(u => u.Id == userId);
            if (selectedUser == null)
            {
                ErrorMessage = Localizer.Instance["UserNotFound"];
                IsErrorVisible = true;
                return;
            }

            if (selectedUser.IsLoading)
            {
                ErrorMessage = Localizer.Instance["UserIsLoading"];
                IsErrorVisible = true;
                return;
            }

            var selectedGameMode = GameModeStateService.LastSelectedGameMode ?? GameModes.First();
            NavigateTo<ResultViewModel>(true, userId, selectedGameMode);
        }

        private void OpenProfile(int userId)
        {
            var selectedUser = Users.FirstOrDefault(u => u.Id == _contextMenuUserId);
            if (selectedUser != null)
            {
                var url = $"https://osu.ppy.sh/users/{selectedUser.OsuId}";
                var processInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(processInfo);
            }
            else
            {
                ErrorMessage = Localizer.Instance["UserNotFound"];
                IsErrorVisible = true;
            }
        }

        private void ShowContextMenu(int userId)
        {
            var selectedUser = Users.FirstOrDefault(u => u.Id == userId);
            if (selectedUser == null)
            {
                ErrorMessage = Localizer.Instance["UserNotFound"];
                IsErrorVisible = true;
                return;
            }
            SelectedUser = selectedUser;
            _contextMenuUserId = userId;
            IsContextMenuVisible = true;
        }

        private void DeleteUser()
        {
            if (SelectedUser != null)
            {
                Users.Remove(SelectedUser);
                _userRepository.DeleteUserAsync(SelectedUser.Id);
                IsContextMenuVisible = false;
                SelectedUser = null;
            }
        }
        private void HideContextMenu()
        {
            IsContextMenuVisible = false;
            SelectedUser = null;
        }
    }
}