using AutoWifiAdbConnect.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Timers;

namespace AutoWifiAdbConnect.MVVM.ViewModel
{
    class MainViewModel : BaseViewModel
    {
        private LocalNetworkTools.AdbExecutor adbExecutor = new LocalNetworkTools.AdbExecutor();
        private WifiAdbConnector connector;
        public WifiAdbConnector.Connectoin SelectedConnection { get; set; }
        public string NewAddress { get; set; }
        public string AdbDir
        {
            get { return adbExecutor.AdbDir; }
            set
            {
                adbExecutor.AdbDir = value;
                connector.Reset();
                OnPropertyChanged(nameof(AdbDir));
                OnPropertyChanged(nameof(AdbCorrect));
                Settings.SettingsObject.AdbPath = value;
            }
        }
        public bool AdbCorrect { get { return adbExecutor.AdbDirCorrect; } }
        public bool StartWithWindows
        {
            get { return Settings.SettingsObject.RunWithWindows; }
            set
            {
                OnPropertyChanged(nameof(StartWithWindows));
                Settings.SettingsObject.RunWithWindows = value;
            }
        }
        public bool RunHided
        {
            get { return Settings.SettingsObject.RunHided; }
            set
            {
                Settings.SettingsObject.RunHided = value;
                OnPropertyChanged(nameof(StartWithWindows));
            }
        }
        public ObservableCollection<WifiAdbConnector.Connectoin> Addresses { get { return connector.Addresses; } }
        public string CheckInterval
        {
            get { return (timer.Interval / 1000).ToString(); }
            set
            {
                if (int.TryParse(value, out int o))
                    if (o * 1000 >= 1000)
                    {
                        timer.Interval = o * 1000;
                        Settings.SettingsObject.CheckInterval = o.ToString();
                    }
            }
        }
        private bool autoReconnect = false;
        public bool AutoReconnect
        {
            get { return autoReconnect; }
            set
            {
                autoReconnect = value;
                if (value)
                    timer.Elapsed += Reconnect;
                else
                    timer.Elapsed -= Reconnect;
                Settings.SettingsObject.AutoConnection = value;
            }
        }
        private Timer timer = new Timer() { AutoReset = true, Interval = 3000 };

        private RelayCommand deleteSelectedCommand;
        public RelayCommand DeleteSelectedCommand
        {
            get
            {
                return deleteSelectedCommand ??
                  (deleteSelectedCommand = new RelayCommand(obj =>
                  {
                      if (SelectedConnection == null)
                          return;
                      connector.RemoveConnection(SelectedConnection);
                  }));
            }
        }

        private RelayCommand deleteFailedCommand;
        public RelayCommand DeleteFailedCommand
        {
            get
            {
                return deleteFailedCommand ??
                  (deleteFailedCommand = new RelayCommand(obj =>
                  {
                      connector.RemoveFailed();
                  }));
            }
        }

        private RelayCommand restartAdbCommand;
        public RelayCommand RestartAdbCommand
        {
            get
            {
                return restartAdbCommand ??
                  (restartAdbCommand = new RelayCommand(obj =>
                  {
                      connector.Reset();
                  }));
            }
        }

        private RelayCommand reconnectAllCommand;
        public RelayCommand ReconnectAllCommand
        {
            get
            {
                return reconnectAllCommand ??
                  (reconnectAllCommand = new RelayCommand(obj =>
                  {
                      Reconnect();
                  }));
            }
        }

        private RelayCommand addNewCommand;
        public RelayCommand AddNewCommand
        {
            get
            {
                return addNewCommand ??
                  (addNewCommand = new RelayCommand(obj =>
                  {
                      AddNew(NewAddress);
                  }));
            }
        }
        private void AddNew(string address)
        {
            bool res = connector.Add(address);
            if (res)
                connector.TryConnect(address);
        }

        private RelayCommand tryAllCommand;
        public RelayCommand TryAllCommand
        {
            get
            {
                return tryAllCommand ??
                  (tryAllCommand = new RelayCommand(obj =>
                  {
                      connector.TryAllLocalIp();
                  }));
            }
        }

        private RelayCommand pickAdbDirCommand;
        public RelayCommand PickAdbDirCommand
        {
            get
            {
                return pickAdbDirCommand ??
                  (pickAdbDirCommand = new RelayCommand(obj =>
                  {
                      Ookii.Dialogs.Wpf.VistaFolderBrowserDialog fbd = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
                      if (fbd.ShowDialog().Value)
                          AdbDir = fbd.SelectedPath;
                  }));
            }
        }

        private RelayCommand appShutdownCommand;
        public RelayCommand AppShutdownCommand
        {
            get
            {
                return appShutdownCommand ??
                  (appShutdownCommand = new RelayCommand(obj =>
                  {
                      App.Current.Shutdown();
                  }));
            }
        }
        private void Reconnect(object sender = null, EventArgs e = null)
        {
            connector.ConnectAll();
        }
        public MainViewModel()
        {
            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Addresses))
                {
                    List<string> macs = new List<string>();
                    foreach (var c in Addresses)
                        macs.Add(c.MacAddress);
                    Settings.SettingsObject.MacAddresses = macs;
                }
            };
            connector = new WifiAdbConnector(adbExecutor);
            connector.PropertyChanged += (sender, e) => OnPropertyChanged(e.PropertyName);
            timer.Elapsed += (sender, e) => connector.UpdateAllStatus();
            timer.Start();
            Load();
        }

        private void Load()
        {
            var settings = Settings.SettingsObject;
            AdbDir = settings.AdbPath;
            foreach (var mac in settings.MacAddresses)
                AddNew(mac);
            StartWithWindows = settings.RunWithWindows;
            AutoReconnect = settings.AutoConnection;
            CheckInterval = settings.CheckInterval;
        }
    }
}
