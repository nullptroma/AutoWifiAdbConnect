using AutoWifiAdbConnect.LocalNetworkTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace AutoWifiAdbConnect.Model
{
    public class Connectoin : INotifyPropertyChanged
    {
        public string MacAddress { get; private set; }
        public string IpAddress { get { return _mapper.FindIPFromMacAddress(MacAddress); } }
        private ConnectionState _state;
        public ConnectionState State
        {
            get => _state;
            set { _state = value; OnPropertyChanged(nameof(State)); }
        }
        private IPMacMapper _mapper;

        public Connectoin(string mac, IPMacMapper _mapper)
        {
            MacAddress = mac;
            this._mapper = _mapper;
            State = ConnectionState.Failed;
        }

        public override string ToString()
        {
            string pattern = "255.255.255.255";
            return $"IP is {IpAddress + string.Join("", Enumerable.Repeat(" ", pattern.Length - IpAddress.Length))}\t MAC is {MacAddress}";
        }

        public enum ConnectionState
        {
            Failed,
            Pending,
            Successful,
        }

        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public override bool Equals(object obj)
        {
            return obj is Connectoin connectoin &&
                   MacAddress == connectoin.MacAddress &&
                   IpAddress == connectoin.IpAddress;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(MacAddress, IpAddress);
        }
    }
}
