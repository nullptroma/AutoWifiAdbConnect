using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoWifiAdbConnect
{
    class Settings
    {
        public static readonly string SaveSettingsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Wi-Fi ADB Connector";
        public static Settings Instance { get; private set; } = new Settings();

        public bool AutoConnection { get; set; } = false;
        public bool RunHided { get; set; } = false;
        public string CheckInterval { get; set; } = "3";
        public string AdbPath { get; set; } = "";
        public List<string> MacAddresses { get; set; } = new List<string>();
        private bool _runWithWindows = false;
        public bool RunWithWindows
        {
            get { return _runWithWindows; }
            set
            {
                _runWithWindows = value;
                if (_runWithWindows)
                    AutoRun.RegisterAutoRun();
                else
                    AutoRun.UnRegisterAutoRun();
            }
        }

        static Settings() 
        {
            Load();
        }
        
        private Settings()
        {

        }

        public static void Load()
        {
            try
            {
                Instance = JsonUtilities.Deserialize<Settings>(File.ReadAllText(SaveSettingsDirectory + @"\settings.txt"));
            }
            catch { }
        }

        public static void Save()
        {
            JsonUtilities.SerializeToFile(Instance, SaveSettingsDirectory + @"\settings.txt");
        }
    }
}
