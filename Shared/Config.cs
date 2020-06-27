using CitizenFX.Core;
using CitizenFX.Core.Native;
using UE_Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace UE_Shared
{
    public static class Config
    {
        private static Dictionary<string, object> _entries { get; set; }

        public static void LoadConfig(string path = "Config.ini")
        {
            Logger.Info("Loading configuration");

            string content = null;

            try
            {
                content = API.LoadResourceFile(API.GetCurrentResourceName(), path);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"An error occurred while loading the config file, error description: {e.Message}.");
            }

            _entries = new Dictionary<string, object>();

            if (content == null || content.Length == 0)
            {
                Logger.Warn("Configuration is EMPTY!");
                return;
            }

            var splitted = content
                .Split('\n')
                .Where((line) => !line.Trim().StartsWith("#"))
                .Select((line) => line.Trim().Split('='))
                .Where((line) => line.Length == 2);

            foreach (var tuple in splitted)
                _entries.Add(tuple[0], tuple[1]);

            Logger.Info("Configuration loaded!");
        }

        public static T Get<T>(string key)
        {
            if (_entries == null)
                throw new Exception("ERROR: Config not loaded!");

            if (_entries.ContainsKey(key))
            {
                var val = _entries[key];

                T output;

                if (val != null)
                {
                    try
                    {
                        output = (T)Convert.ChangeType(val, typeof(T), CultureInfo.InvariantCulture);
                    }
                    catch (InvalidCastException)
                    {
                        output = (T)Convert.ChangeType(val, typeof(T), CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        output = (T)Convert.ChangeType(val, typeof(T), CultureInfo.InvariantCulture);
                    }

                    _entries[key] = val;
                }
                else
                {
                    output = (T)val;
                }

                return output;
            }

            return default(T);
        }
    }
}