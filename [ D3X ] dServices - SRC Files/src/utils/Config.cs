using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static dServices.dServices;

namespace dServices
{
    public static class Config
    {
        private static readonly string configPath = Path.Combine(Instance.ModuleDirectory, "Config.json");
        public static ConfigModel config;
        private static FileSystemWatcher fileWatcher;

        public static void Initialize()
        {
            if (!File.Exists(configPath))
            {
                Instance.Logger.LogInformation("Plik konfiguracyjny nie istnieje. Tworzenie nowego pliku z domyślną konfiguracją.");
                CreateDefaultConfig();
            }

            config = LoadConfig();

            SetupFileWatcher();
        }

        private static void CreateDefaultConfig()
        {
            var defaultConfig = new ConfigModel
            {
                Uslugi = new Dictionary<string, ServiceInfo>
                {
                    ["SVIP"] = new ServiceInfo
                    {
                        Title = "[ ★ CS-Zjarani | Opis SVIPA ★ ]",
                        MenuOptions = new List<string>
                        {
                            "Rezerwacja Slota",
                            "110 HP na start rundy",
                            "Na start rundy +100 Armor",
                            "Na start rundy Hełm",
                            "Prefix \"★SVIP★\"",
                            "na czacie & w tabeli",
                            "4x Granaty na start rundy",
                            "Przywitanie & Pożegnanie",
                            "\"★SVIPA★\"",
                            "Więcej otrzymywanych monet",
                            "Co runde otrzymujesz +750$",
                            "Dostęp do kolorowych Smoków",
                            "Darmowy zestaw rozbrajania",
                            "Dodatkowy Skok",
                            "1x Strzykawkę"
                        }
                    },
                    ["VIP"] = new ServiceInfo
                    {
                        Title = "[ ★ CS-Zjarani | Opis VIPA ★ ]",
                        MenuOptions = new List<string>
                        {
                            "Rezerwacja Slota",
                            "105 HP na start rundy",
                            "Na start rundy +100 Armor",
                            "Prefix \"★VIP★\"",
                            "na czacie & w tabeli",
                            "2x Granaty na start rundy",
                            "Przywitanie & Pożegnanie",
                            "\"★VIPA★\"",
                            "Więcej otrzymywanych monet",
                            "Co runde otrzymujesz +500$",
                            "Dostęp do kolorowych Smoków",
                            "Darmowy zestaw rozbrajania",
                            "Dodatkowy Skok"
                        }
                    }
                }
            };

            SaveConfig(defaultConfig);
        }

        private static ConfigModel LoadConfig()
        {
            try
            {
                string json = File.ReadAllText(configPath);
                var loadedConfig = JsonConvert.DeserializeObject<ConfigModel>(json);

                if (loadedConfig == null || loadedConfig.Uslugi == null || !loadedConfig.Uslugi.Any())
                {
                    Instance.Logger.LogError("Plik konfiguracyjny jest pusty lub ma błędną strukturę.");
                    return null;
                }

                Instance.Logger.LogInformation("Konfiguracja została załadowana poprawnie.");
                return loadedConfig;
            }
            catch (Exception ex)
            {
                Instance.Logger.LogError($"Błąd podczas wczytywania pliku konfiguracyjnego: {ex.Message}");
                return null;
            }
        }

        public static void SaveConfig(ConfigModel config)
        {
            try
            {
                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(configPath, json);
                Instance.Logger.LogInformation("Plik konfiguracyjny został zapisany.");
            }
            catch (Exception ex)
            {
                Instance.Logger.LogError($"Błąd podczas zapisywania pliku konfiguracyjnego: {ex.Message}");
            }
        }

        private static void SetupFileWatcher()
        {
            fileWatcher = new FileSystemWatcher(Path.GetDirectoryName(configPath))
            {
                Filter = Path.GetFileName(configPath),
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
            };

            fileWatcher.Changed += (sender, e) =>
            {
                Thread.Sleep(500);
                var newConfig = LoadConfig();
                if (newConfig != null)
                {
                    config = newConfig;
                    Instance.Logger.LogInformation("Konfiguracja została zaktualizowana po zmianie pliku.");
                }
            };

            fileWatcher.EnableRaisingEvents = true;
        }

        public class ConfigModel
        {
            public Settings Settings { get; set; } = new Settings();
            public Dictionary<string, ServiceInfo> Uslugi { get; set; }
        }

        public class Settings
        {
            public string DisplayMode { get; set; } = "menu"; // menu,chat,both
            public string Services_Commands { get; set; } = "uslugi, services";
            public string Menu_Title { get; set; } = "[ ★ CS-Zjarani | Usługi ★ ]";
            public string Menu_Title_Color { get; set; } = "#29cc94";
        }

        public class ServiceInfo
        {
            public string Title { get; set; }
            public List<string> MenuOptions { get; set; }
        }
    }
}
