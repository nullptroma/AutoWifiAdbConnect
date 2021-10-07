using Microsoft.Win32;
using System.Diagnostics;
using System.Windows;

namespace AutoWifiAdbConnect
{
    static class AutoRun
    {
        public static void RegisterAutoRun()//включить автозапуск
        {
            const string applicationName = "WiFiADBConnector";
            const string pathRegistryKeyStartup =
                        "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

            using (RegistryKey registryKeyStartup =
                        Registry.CurrentUser.OpenSubKey(pathRegistryKeyStartup, true))
            {
                registryKeyStartup.SetValue(
                    applicationName,
                    string.Format("\"{0}\"", Process.GetCurrentProcess().MainModule.FileName));
            }
        }

        public static void UnRegisterAutoRun()//выключить автозапуск
        {
            const string applicationName = "WiFiADBConnector";
            const string pathRegistryKeyStartup =
                        "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

            using (RegistryKey registryKeyStartup =
                        Registry.CurrentUser.OpenSubKey(pathRegistryKeyStartup, true))
            {
                registryKeyStartup.DeleteValue(applicationName, false);
            }
        }
    }
}
