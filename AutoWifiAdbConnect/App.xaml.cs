using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace AutoWifiAdbConnect
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
                Shutdown();
            this.Exit += (sender, e) => Settings.Save();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString(), "Ошибка");
        }
    }
}
