using Avalonia.Platform;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using ReactiveUI;
using Newtonsoft.Json;

namespace OsuScoreCheck.Service
{
    public class Localizer : ReactiveObject
    {
        private const string IndexerName = "Item";
        private const string IndexerArrayName = "Item[]";
        private Dictionary<string, string> m_Strings = null;

        public bool LoadLanguage(string language)
        {
            Language = language;

            Uri uri = new Uri($"avares://OsuScoreCheck/Assets/Localization/{language}.json");
            if (AssetLoader.Exists(uri))
            {
                using (StreamReader sr = new StreamReader(AssetLoader.Open(uri), Encoding.UTF8))
                {
                    m_Strings = JsonConvert.DeserializeObject<Dictionary<string, string>>(sr.ReadToEnd());
                }
                this.RaisePropertyChanged(IndexerName);
                this.RaisePropertyChanged(IndexerArrayName);

                return true;
            }
            return false;
        }

        public string Language { get; private set; }

        public string this[string key]
        {
            get
            {
                if (m_Strings != null && m_Strings.TryGetValue(key, out string res))
                    return res.Replace("\\n", "\n");

                return $"{Language}:{key}";
            }
        }

        public static Localizer Instance { get; set; } = new Localizer();
    }
}