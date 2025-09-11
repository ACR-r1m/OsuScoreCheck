using Microsoft.EntityFrameworkCore;
using OsuScoreCheck.Data;
using OsuScoreCheck.Models.DB;
using OsuScoreCheck.Models.Enums;
using OsuScoreCheck.Models.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OsuScoreCheck.Service
{
    public class UserRepository
    {
        private static bool _isInitialized = false;

        public async Task<int> CalculateNextIdForUser(int userOsuId)
        {
            await using var context = new ApplicationContext();
            var maxId = await context.Beatmaps
                .Where(b => b.UserOsuId == userOsuId)
                .MaxAsync(b => (int?)b.Id) ?? 0;

            return maxId + 1;
        }

        public async Task<int> GetOsuIdByIdAsync(int userId)
        {
            await using var context = new ApplicationContext();
            EnsureDatabaseCreated(context);

            var user = await context.UsersOsu
                .Where(u => u.Id == userId)
                .Select(u => u.OsuId)
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task DeleteUserAsync(int userId)
        {
            await using var context = new ApplicationContext();
            var user = await context.UsersOsu.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                context.UsersOsu.Remove(user);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<Beatmap>> GetMapsToCheckAsync(int osuId, int lastCheckedMapId)
        {
            await using var context = new ApplicationContext();

            var userOsuId = await context.UsersOsu
                .Where(u => u.OsuId == osuId)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();


            return await context.Beatmaps
               .Where(map => map.UserOsuId == userOsuId && map.Id > lastCheckedMapId)
               .OrderBy(map => map.Id)
               .ToListAsync();
        }

        public async Task<string> GetGameModeNameAsync(int? gameModeId)
        {
            if (!gameModeId.HasValue)
            {
                throw new InvalidOperationException($"не найден.");
            }

            await using var context = new ApplicationContext();
            EnsureDatabaseCreated(context);

            var gameMode = await context.GameModes
                .Where(gm => gm.Id == gameModeId.Value)
                .Select(gm => gm.Name)
                .FirstOrDefaultAsync();

            return gameMode?.ToLower();
        }

        public async Task AddOrUpdateUserAsync(UserOsu user)
        {
            await using var context = new ApplicationContext();
            EnsureDatabaseCreated(context);

            var existingUser = await context.UsersOsu
                .SingleOrDefaultAsync(u => u.OsuId == user.OsuId);
            if (existingUser == null)
            {
                context.UsersOsu.Add(user);
            }
            else
            {
                existingUser.IsLoading = existingUser.LastCheckedMapId != existingUser.TotalPlayedMaps;
                existingUser.Username = user.Username;
                existingUser.AvatarUrl = user.AvatarUrl;
                existingUser.LastCheckedMapId = user.LastCheckedMapId;
                existingUser.TotalPlayedMaps = user.TotalPlayedMaps;
                existingUser.IsLoading = user.IsLoading;
                existingUser.AllMapsSaved = user.AllMapsSaved;
                context.UsersOsu.Update(existingUser);
            }

            await context.SaveChangesAsync();
        }

        public async Task<List<UserOsu>> GetAllUsersAsync()
        {
            await using var context = new ApplicationContext();
            EnsureDatabaseCreated(context);
            context.EnsureDatabaseCreated();
            return await context.UsersOsu.ToListAsync();
        }

        public async Task AddOrUpdateBeatmapAsync(Beatmap map)
        {
            await using var context = new ApplicationContext();
            EnsureDatabaseCreated(context);

            var existingUser = await context.UsersOsu.SingleOrDefaultAsync(u => u.Id == map.UserOsuId);
            if (existingUser == null)
            {
                throw new InvalidOperationException($"Пользователь с UserOsuId {map.UserOsuId} не найден.");
            }

            var existingMap = await context.Beatmaps
                .SingleOrDefaultAsync(m => m.BeatmapId == map.BeatmapId && m.UserOsuId == map.UserOsuId);

            if (existingMap == null)
            {
                var duplicateCheck = await context.Beatmaps
                .SingleOrDefaultAsync(m => m.Id == map.Id && m.UserOsuId == map.UserOsuId);

                context.Beatmaps.AddAsync(map);
            }
            else
            {
                existingMap.PP = map.PP;
                existingMap.Mods = map.Mods;
                existingMap.Result = map.Result;
            }

            await context.SaveChangesAsync();
        }

        public async Task<int> EnsureUserExistsAndGetId(int osuId)
        {
            await using var context = new ApplicationContext();
            EnsureDatabaseCreated(context);
            var existingUser = await context.UsersOsu.SingleOrDefaultAsync(u => u.OsuId == osuId);
            if (existingUser == null)
            {
                throw new InvalidOperationException("пользователь проблема");
            }
            return existingUser.Id;
        }

        public async Task<Beatmap?> GetBeatmapByIdAndUserOsuId(int beatmapId, int userOsuId)
        {
            await using var context = new ApplicationContext();
            return await context.Beatmaps
                  .Include(b => b.Result)
                  .Include(b => b.SimplifiedCategory)
                  .SingleOrDefaultAsync(m => m.BeatmapId == beatmapId && m.UserOsuId == userOsuId);
        }

        public async Task UpdateBeatmapAsync(Beatmap map)
        {
            await using var context = new ApplicationContext();
            context.Entry(map).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }

        public async Task<List<GameMode>> GetGameModesAsync()
        {
            await using var context = new ApplicationContext();
            return await context.GameModes.ToListAsync();
        }

        public async Task DeleteBeatmapsAfterCountAsync(int osuId, int keepCount)
        {
            await using var context = new ApplicationContext();
            var userOsuId = await context.UsersOsu
                .Where(u => u.OsuId == osuId)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            var beatmapsToDelete = await context.Beatmaps
                .Where(b => b.UserOsuId == userOsuId)
                .OrderBy(b => b.Id)
                .Skip(keepCount)
                .ToListAsync();

            context.Beatmaps.RemoveRange(beatmapsToDelete);
            await context.SaveChangesAsync();
        }

        public async Task<List<Beatmap>> GetMapsByTimeAsync(BeatmapFilter filter)
        {
            await using var context = new ApplicationContext();
            var query = context.Beatmaps
                .Include(b => b.Result)
                .Include(b => b.SimplifiedCategory)
                .Where(b => b.UserOsuId == filter.UserOsuId);

            if (filter.MinTimeSeconds.HasValue)
                query = query.Where(b => b.TimePlayedSeconds >= filter.MinTimeSeconds.Value);
            if (filter.MaxTimeSeconds.HasValue)
                query = query.Where(b => b.TimePlayedSeconds <= filter.MaxTimeSeconds.Value);
            if (filter.MinStarRating.HasValue)
                query = query.Where(b => b.StarRating >= filter.MinStarRating.Value);
            if (filter.MaxStarRating.HasValue)
                query = query.Where(b => b.StarRating <= filter.MaxStarRating.Value);
            if (filter.MinPP.HasValue)
                query = query.Where(b => b.PP.HasValue && b.PP >= (filter.MinPP.Value - 0.5));
            if (filter.MaxPP.HasValue)
                query = query.Where(b => b.PP.HasValue && b.PP <= filter.MaxPP.Value);
            if (filter.Results != null && filter.Results.Any())
                query = query.Where(b => filter.Results.Contains(b.Result.Name));

            if (!string.IsNullOrEmpty(filter.ModsFilter))
            {
                var modsFilter = filter.ModsFilter;
                query = query.Where(b => b.Mods == modsFilter ||
                                        (b.Mods == $"{modsFilter} SD" && !modsFilter.Contains("SD")));
            }

            if (!string.IsNullOrEmpty(filter.NameFilter))
            {
                var lowerFilter = filter.NameFilter.ToLower();
                query = query.Where(b => b.BeatmapName.ToLower().Contains(lowerFilter) ||
                                        b.Difficulty.ToLower().Contains(lowerFilter));
            }

            if (filter.Statuses != null && filter.Statuses.Any())
                query = query.Where(b => filter.Statuses.Contains(b.SimplifiedCategory.Name));

            if (filter.GameModeId.HasValue)
            {
                query = query.Where(b => b.GameModeId == filter.GameModeId.Value);
            }

            if (filter.SortField != SortField.None && filter.SortDirection != SortDirection.None)
            {
                bool isAscending = filter.SortDirection == SortDirection.Ascending;
                switch (filter.SortField)
                {
                    case SortField.Result:
                        query = isAscending
                            ? query.OrderBy(b => b.Result.Order)
                            : query.OrderByDescending(b => b.Result.Order);
                        break;
                    case SortField.Time:
                        query = isAscending
                            ? query.OrderBy(b => b.TimePlayedSeconds)
                            : query.OrderByDescending(b => b.TimePlayedSeconds);
                        break;
                    case SortField.Star:
                        query = isAscending
                            ? query.OrderBy(b => b.StarRating)
                            : query.OrderByDescending(b => b.StarRating);
                        break;
                    case SortField.PP:
                        query = isAscending
                            ? query.OrderBy(b => b.PP)
                            : query.OrderByDescending(b => b.PP);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(b => b.Id);
            }

            return await query
                .Skip(filter.Offset)
                .Take(filter.Limit)
                .ToListAsync();
        }

        private void EnsureDatabaseCreated(ApplicationContext context)
        {
            if (!_isInitialized)
            {
                context.EnsureDatabaseCreated();
                _isInitialized = true;
            }
        }

    }
}
