﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace GameLauncher.Models
{
    public class SettingsManager
    {
        private static readonly string _settingsFilePath = Path.Combine (Directory.GetCurrentDirectory(), "settings.json");

        private static Settings _settings;

        public static Settings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = LoadSettings ();
                }
                return _settings;
            }
        }

        public static Settings LoadSettings ()
        {
            if (!File.Exists (_settingsFilePath))
            {
                return new Settings ();
            }

            var json = File.ReadAllText (_settingsFilePath);

            return JsonConvert.DeserializeObject<Settings> (json);
        }

        public static void SaveSettings ()
        {
            var json = JsonConvert.SerializeObject (Settings, Formatting.Indented);
            File.WriteAllText (_settingsFilePath, json);
        }

        public static List<GameInfo> LoadGamesInfo ()
        {
            var gamesInfoFilePath = Path.Combine (Directory.GetCurrentDirectory(), "games.json");

            if (!File.Exists (gamesInfoFilePath))
            {
                return new List<GameInfo> ();
            }

            var json = File.ReadAllText (gamesInfoFilePath);

            return JsonConvert.DeserializeObject<List<GameInfo>> (json);
        }

        public static void SaveGamesInfo (List<GameInfo> gamesInfo)
        {
            var gamesInfoFilePath = Path.Combine (Directory.GetCurrentDirectory(), "games.json");

            var json = JsonConvert.SerializeObject (gamesInfo, Formatting.Indented);

            File.WriteAllText (gamesInfoFilePath, json);
        }
    }
}