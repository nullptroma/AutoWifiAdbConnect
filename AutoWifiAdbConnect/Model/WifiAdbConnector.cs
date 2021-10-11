using AutoWifiAdbConnect.LocalNetworkTools;
using AutoWifiAdbConnect.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace AutoWifiAdbConnect.Model
{
    class WifiAdbConnector : INotifyPropertyChanged
    {
        private ObservableCollection<Connectoin> _macAddresses = new ObservableCollection<Connectoin>();
        public ObservableCollection<Connectoin> Addresses
        {
            get { return _macAddresses; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(Addresses));
                lock (_macAddresses)
                {
                    _macAddresses = value;
                }
            }
        }
        private readonly IPMacMapper _ipMacMapper;
        private readonly AdbExecutor _adbExecutor;

        public WifiAdbConnector(AdbExecutor _adbExecutor, IEnumerable<string> _macAddresses = null)
        {
            _ipMacMapper = new IPMacMapper();
            this._adbExecutor = _adbExecutor;
            this._adbExecutor.Executed += OnExecResult;
            if (_macAddresses != null)
                foreach (var mac in _macAddresses)
                    Addresses.Add(new Connectoin(mac, _ipMacMapper));
            Reset();
        }

        #region Public
        public void Reset()
        {
            _ipMacMapper.RefreshList();
            _adbExecutor.Enqueue("kill-server");
            foreach (Connectoin con in Addresses)
                con.State = Connectoin.ConnectionState.Failed;
            _adbExecutor.Enqueue("start-server");
        }

        public bool Add(string macOrIp)
        {
            return AddMac(macOrIp) || AddIp(macOrIp);
        }

        public bool AddMac(string mac)
        {
            if (string.IsNullOrEmpty(_ipMacMapper.FindIPFromMacAddress(mac)) || Addresses.Any(con => con.MacAddress == mac))
                return false;
            Connectoin macInfo = new Connectoin(mac, _ipMacMapper);
            Addresses.Add(macInfo);
            OnPropertyChanged(nameof(Addresses));
            return true;
        }

        public bool AddIp(string ip)
        {
            string mac = _ipMacMapper.FindMacFromIPAddress(ip);
            if (string.IsNullOrEmpty(mac) || Addresses.Any(con => con.MacAddress == mac))
                return false;
            Connectoin macInfo = new Connectoin(mac, _ipMacMapper);
            Addresses.Add(macInfo);
            OnPropertyChanged(nameof(Addresses));
            return true;
        }

        public void RemoveFailed()
        {
            for (int i = 0; i < Addresses.Count; i++)
                if (Addresses[i].State == Connectoin.ConnectionState.Failed)
                {
                    RemoveConnection(Addresses[i]);
                    i = 0;
                }
        }

        public bool RemoveConnection(Connectoin con)
        {
            bool result = Addresses.Remove(con);
            OnPropertyChanged(nameof(Addresses));
            return result;
        }

        public bool TryConnect(string ipmac)
        {
            return TryConnectMac(ipmac) || TryConnectIp(ipmac);
        }

        public bool TryConnectMac(string mac)
        {
            return TryConnect(Addresses.FirstOrDefault(m => m.MacAddress == mac));
        }

        public bool TryConnectIp(string ip)
        {
            return TryConnect(Addresses.FirstOrDefault(m => m.IpAddress == ip));
        }

        public bool TryConnect(Connectoin con)
        {
            if (con == null)
                return false;
            if (string.IsNullOrEmpty(con.MacAddress))
                throw new ArgumentNullException(nameof(con));
            if (!Addresses.Contains(con))
                throw new Exception(nameof(Addresses) + " has not this mac. Firstly add it.");
            con.State = Connectoin.ConnectionState.Pending;
            _adbExecutor.Enqueue($"connect {con.IpAddress}");
            return true;
        }

        public void ConnectAll()
        {
            foreach (var mac in Addresses.Where(con => con.State == Connectoin.ConnectionState.Failed))
                TryConnect(mac);
        }

        public void TryAllLocalIp()
        {
            var all = _ipMacMapper.GetAllIps();
            foreach (var ip in all)
            {
                try
                {
                    AddIp(ip);
                    TryConnectIp(ip);
                }
                catch { }
            }
        }

        public void UpdateAllStatus()
        {
            _adbExecutor.Enqueue("devices");
        }
        #endregion

        private void OnExecResult(object sender, AdbExecutor.CommandExecutedEventArgs e)
        {
            if (e.Command.StartsWith("connect"))
                WorkConnectResult(e.Command, e.Result);
            else if (e.Command.StartsWith("devices"))
                WorkDevicesResult(e.Result);
        }

        private void WorkConnectResult(string cmd, string adbConnectResult)
        {
            string ip = cmd.Substring("connect ".Length).Trim();
            Connectoin con = Addresses.FirstOrDefault(c => c.IpAddress == ip);
            if (con == null)
                return;
            bool connected = adbConnectResult.Contains("connected to");
            UpdateAddress(con, connected);
        }

        private void WorkDevicesResult(string adbDevicesResult)
        {
            foreach (var str in adbDevicesResult.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!str.Contains(':'))
                    continue;
                string ip = str.Substring(0, str.IndexOf(':')).Trim();
                Connectoin con = Addresses.FirstOrDefault(c => c.IpAddress == ip);
                if (con == null)
                    continue;
                bool connected = str.EndsWith("device");
                UpdateAddress(con, connected);
            }
        }

        private void UpdateAddress(Connectoin connectoin, bool nowConnected) => UpdateAddress(connectoin, nowConnected ? Connectoin.ConnectionState.Successful : Connectoin.ConnectionState.Failed);
        private void UpdateAddress(Connectoin connectoin, Connectoin.ConnectionState nowConnected)
        {
            if (connectoin.State != nowConnected)
            {
                connectoin.State = nowConnected;
                OnPropertyChanged(nameof(Addresses));
            }
        }

        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
