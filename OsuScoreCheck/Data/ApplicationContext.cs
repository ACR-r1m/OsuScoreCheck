using Microsoft.EntityFrameworkCore;
using OsuScoreCheck.Models.DB;
using System;
using System.IO;
using System.Linq;

namespace OsuScoreCheck.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<UserOsu> UsersOsu { get; set; } = null!;
        public DbSet<Beatmap> Beatmaps { get; set; } = null!;
        public DbSet<Result> Results { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<GameMode> GameModes { get; set; }
            
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var appDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData");
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            var databasePath = Path.Combine(appDataPath, "appdata.db");
            optionsBuilder.UseSqlite($"Data Source={databasePath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // GameMode
            modelBuilder.Entity<GameMode>().ToTable("game_modes");
            modelBuilder.Entity<GameMode>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name").IsRequired();
            });

            // UserOsu
            modelBuilder.Entity<UserOsu>().ToTable("user_osu");
            modelBuilder.Entity<UserOsu>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.OsuId).HasColumnName("osu_id").IsRequired();
                entity.Property(e => e.Username).HasColumnName("username").IsRequired();
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
                entity.Property(e => e.TotalPlayedMaps).HasColumnName("total_played_maps").IsRequired();
                entity.Property(e => e.AvatarUrl).HasColumnName("avatar_url").IsRequired();
                entity.Property(e => e.LastCheckedMapId).HasColumnName("last_checked_map_id").IsRequired();
                entity.Property(e => e.AllMapsSaved).HasColumnName("all_maps_saved").IsRequired();
                entity.Property(e => e.IsLoading).HasColumnName("is_loading").IsRequired();
            });

            // Result
            modelBuilder.Entity<Result>().ToTable("results");
            modelBuilder.Entity<Result>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name").IsRequired();
                entity.Property(e => e.Order).HasColumnName("order").IsRequired();
            });

            // Category
            modelBuilder.Entity<Category>().ToTable("categories");
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name").IsRequired();
            });

            modelBuilder.Entity<Beatmap>().ToTable("beatmaps");
            modelBuilder.Entity<Beatmap>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.UserOsuId });
                entity.Property(e => e.Id).HasColumnName("id").IsRequired();
                entity.Property(e => e.UserOsuId).HasColumnName("user_osu_id").IsRequired();
                entity.Property(e => e.BeatmapId).HasColumnName("beatmap_id").IsRequired();
                entity.Property(e => e.BeatmapsetId).HasColumnName("beatmapset_id").IsRequired();
                entity.Property(e => e.BeatmapName).HasColumnName("beatmap_name").IsRequired();
                entity.Property(e => e.Difficulty).HasColumnName("difficulty").IsRequired();
                entity.Property(e => e.TimePlayedSeconds).HasColumnName("time_played_seconds").IsRequired();
                entity.Property(e => e.TimePlayed).HasColumnName("time_played").IsRequired();
                entity.Property(e => e.StarRating).HasColumnName("star_rating").IsRequired();
                entity.Property(e => e.PP).HasColumnName("pp").HasDefaultValue(0.0);
                entity.Property(e => e.Mods).HasColumnName("mods");
                entity.Property(e => e.SimplifiedCategoryId).HasColumnName("simplified_category_id").IsRequired();
                entity.Property(e => e.BackgroundImage).HasColumnName("background_image");
                entity.Property(e => e.ResultId).HasColumnName("result_id");
                entity.Property(e => e.GameModeId).HasColumnName("game_mode_id");
                entity.Property(e => e.CategoryId).HasColumnName("category_id").IsRequired();

                entity.HasIndex(e => e.BeatmapName);
                entity.HasIndex(e => e.Difficulty);
                entity.HasIndex(e => e.TimePlayedSeconds);
                entity.HasIndex(e => e.StarRating);
                entity.HasIndex(e => e.PP);
                entity.HasIndex(e => e.Mods);
                entity.HasIndex(e => e.ResultId);
                entity.HasIndex(e => e.CategoryId);
                entity.HasIndex(e => e.SimplifiedCategoryId);

                entity.HasOne(e => e.UserOsu)
                      .WithMany(u => u.Beatmaps)
                      .HasForeignKey(e => e.UserOsuId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.GameMode)
                      .WithMany()
                      .HasForeignKey(e => e.GameModeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Result)
                      .WithMany()
                      .HasForeignKey(e => e.ResultId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Category)
                      .WithMany()
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.SimplifiedCategory)
                      .WithMany()
                      .HasForeignKey(e => e.SimplifiedCategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            base.OnModelCreating(modelBuilder);
        }

        public void EnsureDatabaseCreated()
        {
            Database.EnsureCreated();
            SeedInitialData();
        }

        private void SeedInitialData()
        {
            if (!GameModes.Any())
            {
                GameModes.AddRange(
                    new GameMode { Id = 1, Name = "osu" },
                    new GameMode { Id = 2, Name = "taiko" },
                    new GameMode { Id = 3, Name = "fruits" },
                    new GameMode { Id = 4, Name = "mania" }
                );
            }

            if (!Results.Any())
            {
                Results.AddRange(
                    new Result { Id = 1, Name = "", Order = 0 },
                    new Result { Id = 2, Name = "D", Order = 1 },
                    new Result { Id = 3, Name = "C", Order = 2 },
                    new Result { Id = 4, Name = "B", Order = 3 },
                    new Result { Id = 5, Name = "A", Order = 4 },
                    new Result { Id = 6, Name = "S", Order = 5 },
                    new Result { Id = 7, Name = "SS", Order = 6 }
                );
            }

            if (!Categories.Any())
            {
                Categories.AddRange(
                    new Category { Id = 1, Name = "Ranked" },
                    new Category { Id = 2, Name = "Approved" },
                    new Category { Id = 3, Name = "Qualified" },
                    new Category { Id = 4, Name = "Loved" },
                    new Category { Id = 5, Name = "Pending" },
                    new Category { Id = 6, Name = "WIP" },
                    new Category { Id = 7, Name = "Graveyard" },
                    new Category { Id = 8, Name = "Unranked" }
                );
            }

            SaveChanges();
        }
    }
}
