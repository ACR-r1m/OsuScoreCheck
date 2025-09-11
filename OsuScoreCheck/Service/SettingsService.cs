using Newtonsoft.Json;
using OsuScoreCheck.Models.json;
using System;
using System.IO;

namespace OsuScoreCheck.Service
{
    public class SettingsService
    {
        private static string SettingsFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

        public Settings LoadSettings()
        {
            if (File.Exists(SettingsFilePath))
            {
                var json = File.ReadAllText(SettingsFilePath);
                return JsonConvert.DeserializeObject<Settings>(json);
            }
            else
            {
                return new Settings { Language = "en", ID = "", ClientSecret = "" };
            }
        }

        public void SaveSettings(Settings settings)
        {
            var directoryPath = Path.GetDirectoryName(SettingsFilePath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(SettingsFilePath, json);
        }
    }
}
