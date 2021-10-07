using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoWifiAdbConnect.LocalNetworkTools
{
    public class IPMacMapper
    {
        private static ARP arp = new ARP();
        private List<IPAndMac> list;

        public IPMacMapper()
        {
            RefreshList();
        }

        public List<String> GetAllIps()
        {
            return arp.GetAll().Where(x => !string.IsNullOrEmpty(x))
                .Select(x =>
                {
                    string[] parts = x.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    return parts[0].Trim();
                }).ToList();
        }

        public void RefreshList()
        {
            list = arp.GetAll().Where(x => !string.IsNullOrEmpty(x) && x.Trim().StartsWith("192."))
                .Select(x =>
                {
                    string[] parts = x.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    return new IPAndMac { IP = parts[0].Trim(), MAC = parts[1].Trim() };
                }).ToList();
        }

        public string FindIPFromMacAddress(string macAddress)
        {
            IPAndMac item = list.SingleOrDefault(x => x.MAC == macAddress);
            if (string.IsNullOrEmpty(item.IP))
                return null;
            return item.IP;
        }

        public string FindMacFromIPAddress(string ip)
        {
            IPAndMac item = list.SingleOrDefault(x => x.IP == ip);
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
