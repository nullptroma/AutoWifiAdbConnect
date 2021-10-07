using AutoWifiAdbConnect.MVVM.ViewModel;
using System.Windows;

namespace AutoWifiAdbConnect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            if (Settings.SettingsObject.RunHided)
                HideWindow();
        }

        private void ShowWindowButton_Click(object sender, RoutedEventArgs e)
        {
            ShowWindow();
        }

        private void HideWindowButton_Click(object sender, RoutedEventArgs e)
        {
            HideWindow();
        }


        private void TaskBarIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (this.Visibility == Visibility.Visible)
                HideWindow();
            else
                ShowWindow();
        }

        private void HideWindow()
        {
            this.Hide();
        }

        private void ShowWindow()
        {
            this.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            HideWindow();
            Settings.Save();
        }
    }
}
