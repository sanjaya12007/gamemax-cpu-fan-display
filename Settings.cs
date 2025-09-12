using System.Diagnostics;
using System.IO;

namespace GameMax_CPU_Fan_Display
{
    public static class Settings
    {
        private static readonly string IniPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.ini");

        private const string TaskName = "GameMaxCpuFanDisplay";
        private static readonly string? AppPath =
            Process.GetCurrentProcess().MainModule?.FileName;

        private static readonly Dictionary<string, string> Cache = new();

        static Settings()
        {
            Load();
        }

        private static void Load()
        {
            Cache.Clear();

            if (!File.Exists(IniPath))
            {
                Cache["Interval"] = "1000";
                Cache["Autostart"] = "false";
                Save();
                return;
            }

            foreach (var line in File.ReadAllLines(IniPath))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("[")) continue;

                var parts = line.Split('=', 2);
                if (parts.Length == 2)
                {
                    Cache[parts[0].Trim()] = parts[1].Trim();
                }
            }

            SyncAutostart();
        }

        private static void Save()
        {
            using var sw = new StreamWriter(IniPath, false);
            sw.WriteLine("[General]");
            foreach (var kv in Cache)
                sw.WriteLine($"{kv.Key}={kv.Value}");
        }

        public static int Interval
        {
            get => int.TryParse(Get("Interval", "1000"), out var v) ? v : 1000;
            set
            {
                Cache["Interval"] = value.ToString();
                Save();
            }
        }

        public static bool Autostart
        {
            get => bool.TryParse(Get("Autostart", "false"), out var v) && v;
            set
            {
                Cache["Autostart"] = value.ToString().ToLower();
                Save();
                SyncAutostart();
            }
        }

        private static string Get(string key, string def)
        {
            return Cache.TryGetValue(key, out var v) ? v : def;
        }

        private static void SyncAutostart()
        {
            try
            {
                if (Autostart)
                {
                    CreateTask();
                }
                else
                {
                    DeleteTask();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Autostart sync error: " + ex.Message);
            }
        }

        private static void CreateTask()
        {
            DeleteTask();

            string arguments =
                $"/Create /F /RL HIGHEST /SC ONLOGON /TN \"{TaskName}\" /TR \"\\\"{AppPath}\\\"\"";

            RunSchtasks(arguments);
        }

        private static void DeleteTask()
        {
            string arguments = $"/Delete /F /TN \"{TaskName}\"";
            RunSchtasks(arguments);
        }

        private static void RunSchtasks(string arguments)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "schtasks.exe",
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                Verb = "runas"
            };

            using var proc = Process.Start(psi);
            proc?.WaitForExit();
        }
    }
}
