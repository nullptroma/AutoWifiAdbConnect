using AutoWifiAdbConnect.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Timers;

namespace AutoWifiAdbConnect.ViewModel
{
    class MainViewModel : BaseViewModel
    {
        private LocalNetworkTools.AdbExecutor _adbExecutor = new LocalNetworkTools.AdbExecutor();
        private Timer _timer = new Timer() { AutoReset = true, Interval = 3000 };
        private WifiAdbConnector _connector;
        public Connectoin SelectedConnection { get; set; }
        public string NewAddress { get; set; }
        public string AdbDir
        {
            get { return _adbExecutor.AdbDir; }
            set
            {
                _adbExecutor.AdbDir = value;
                _connector.Reset();
                OnPropertyChanged(nameof(AdbDir));
                OnPropertyChanged(nameof(AdbCorrect));
                Settings.Instance.AdbPath = value;
            }
        }
        public bool AdbCorrect { get { return _adbExecutor.AdbDirCorrect; } }
        public bool StartWithWindows
        {
            get { return Settings.Instance.RunWithWindows; }
            set
            {
                OnPropertyChanged(nameof(StartWithWindows));
                Settings.Instance.RunWithWindows = value;
            }
        }
        public bool RunHided
        {
            get { return Settings.Instance.RunHided; }
            set
            {
                Settings.Instance.RunHided = value;
                OnPropertyChanged(nameof(StartWithWindows));
            }
        }
        public ObservableCollection<Connectoin> Addresses { get { return _connector.Addresses; } }
        public string CheckInterval
        {
            get { return (_timer.Interval / 1000).ToString(); }
            set
            {
                if (int.TryParse(value, out int o))
                    if (o * 1000 >= 1000)
                    {
                        _timer.Interval = o * 1000;
                        Settings.Instance.CheckInterval = o.ToString();
                    }
            }
        }
        private bool _autoReconnect = false;
        public bool AutoReconnect
        {
            get { return _autoReconnect; }
            set
            {
                _autoReconnect = value;
                if (value)
                    _timer.Elapsed += Reconnect;
                else
                    _timer.Elapsed -= Reconnect;
                Settings.Instance.AutoConnection = value;
            }
        }

        private RelayCommand _deleteSelectedCommand;
        public RelayCommand DeleteSelectedCommand
        {
            get
            {
                return _deleteSelectedCommand ??
                  (_deleteSelectedCommand = new RelayCommand(obj =>
                  {
                      if (SelectedConnection == null)
                          return;
                      _connector.RemoveConnection(SelectedConnection);
                  }));
            }
        }

        private RelayCommand _deleteFailedCommand;
        public RelayCommand DeleteFailedCommand
        {
            get
            {
                return _deleteFailedCommand ??
                  (_deleteFailedCommand = new RelayCommand(obj =>
                  {
                      _connector.RemoveFailed();
                  }));
            }
        }

        private RelayCommand _restartAdbCommand;
        public RelayCommand RestartAdbCommand
        {
            get
            {
                return _restartAdbCommand ??
                  (_restartAdbCommand = new RelayCommand(obj =>
                  {
                      _connector.Reset();
                  }));
            }
        }

        private RelayCommand _reconnectAllCommand;
        public RelayCommand ReconnectAllCommand
        {
            get
            {
                return _reconnectAllCommand ??
                  (_reconnectAllCommand = new RelayCommand(obj =>
                  {
                      Reconnect();
                  }));
            }
        }

        private RelayCommand _addNewCommand;
        public RelayCommand AddNewCommand
        {
            get
            {
                return _addNewCommand ??
                  (_addNewCommand = new RelayCommand(obj =>
                  {
                      AddNew(NewAddress);
                  }));
            }
        }
        private void AddNew(string address)
        {
            if (_connector.Add(address))
                _connector.TryConnect(address);
        }

        private RelayCommand _tryAllCommand;
        public RelayCommand TryAllCommand
        {
            get
            {
                return _tryAllCommand ??
                  (_tryAllCommand = new RelayCommand(obj =>
                  {
                      _connector.TryAllLocalIp();
                  }));
            }
        }

        private RelayCommand _pickAdbDirCommand;
        public RelayCommand PickAdbDirCommand
        {
            get
            {
                return _pickAdbDirCommand ??
                  (_pickAdbDirCommand = new RelayCommand(obj =>
                  {
                      Ookii.Dialogs.Wpf.VistaFolderBrowserDialog fbd = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
                      if (fbd.ShowDialog().Value)
                          AdbDir = fbd.SelectedPath;
                  }));
            }
        }

        private RelayCommand _appShutdownCommand;
        public RelayCommand AppShutdownCommand
        {
            get
            {
                return _appShutdownCommand ??
                  (_appShutdownCommand = new RelayCommand(obj =>
                  {
                      App.Current.Shutdown();
                  }));
            }
        }
        private void Reconnect(object sender = null, EventArgs e = null)
        {
            _connector.ConnectAll();
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
                    Settings.Instance.MacAddresses = macs;
                }
            };
            _connector = new WifiAdbConnector(_adbExecutor);
            _connector.PropertyChanged += (sender, e) => OnPropertyChanged(e.PropertyName);
            _timer.Elapsed += (sender, e) => _connector.UpdateAllStatus();
            _timer.Start();
            Load();
        }

        private void Load()
        {
            var settings = Settings.Instance;
            AdbDir = settings.AdbPath;
            foreach (var mac in settings.MacAddresses)
                AddNew(mac);
            StartWithWindows = settings.RunWithWindows;
            AutoReconnect = settings.AutoConnection;
            CheckInterval = settings.CheckInterval;
        }
    }
}
