using System;
using System.Collections.Generic;
using System.IO;

namespace AutoWifiAdbConnect
{
    static class Settings
    {
        public static readonly string SaveSettingsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Wi-Fi ADB Connector";
        public static SettingsData SettingsObject { get; set; }

        static Settings()
        {
            Load();
        }

        public static void Load()
        {
            try
            {
                SettingsObject = JsonUtilities.Deserialize<SettingsData>(File.ReadAllText(SaveSettingsDirectory + @"\settings.txt"));
            }
            catch { SettingsObject = new SettingsData(); }
        }

        public static void Save()
        {
            JsonUtilities.SerializeToFile(SettingsObject, SaveSettingsDirectory + @"\settings.txt");
        }

        public class SettingsData
        {
            private bool runWithWindows = false;
            public bool RunWithWindows
            {
                get { return runWithWindows; }
                set
                {
                    runWithWindows = value;
                    if (runWithWindows)
                        AutoRun.RegisterAutoRun();
                    else
                        AutoRun.UnRegisterAutoRun();
                }
            }
            public bool AutoConnection { get; set; } = false;
            public bool RunHided { get; set; } = false;
            public string CheckInterval { get; set; } = "3";
            public string AdbPath { get; set; } = "";
            public List<string> MacAddresses { get; set; } = new List<string>();
        }
    }
}
