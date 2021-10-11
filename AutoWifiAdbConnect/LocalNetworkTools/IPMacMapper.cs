using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoWifiAdbConnect.LocalNetworkTools
{
    public class IPMacMapper
    {
        private static ARP _arp = new ARP();
        private List<IPAndMac> _list;

        public IPMacMapper()
        {
            RefreshList();
        }

        public List<String> GetAllIps()
        {
            return _arp.GetAll().Where(x => !string.IsNullOrEmpty(x))
                .Select(x =>
                {
                    string[] parts = x.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    return parts[0].Trim();
                }).ToList();
        }

        public void RefreshList()
        {
            _list = _arp.GetAll().Where(x => !string.IsNullOrEmpty(x) && x.Trim().StartsWith("192."))
                .Select(x =>
                {
                    string[] parts = x.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    return new IPAndMac { IP = parts[0].Trim(), MAC = parts[1].Trim() };
                }).ToList();
        }

        public string FindIPFromMacAddress(string macAddress)
        {
            IPAndMac item = _list.SingleOrDefault(x => x.MAC == macAddress);
            if (string.IsNullOrEmpty(item.IP))
                return null;
            return item.IP;
        }

        public string FindMacFromIPAddress(string ip)
        {
            IPAndMac item = _list.SingleOrDefault(x => x.IP == ip);
            if (string.IsNullOrEmpty(item.IP))
                return null;
            return item.MAC;
        }

        private struct IPAndMac
        {
            public string IP { get; set; }
            public string MAC { get; set; }
        }
    }
}
