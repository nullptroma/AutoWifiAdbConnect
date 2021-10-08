using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Threading.Tasks;

namespace AutoWifiAdbConnect.LocalNetworkTools
{
    class ARP
    {
        private void fillCache()
        {
            string myIp;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                myIp = endPoint.Address.ToString();
            }
            string pattern = myIp.Substring(0, myIp.LastIndexOf('.')) + @".%a";
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = $"cmd";
            startInfo.Arguments = @"/c " + $@"for /L %a in (1,1,254) do @start /b ping {pattern} -n 2 > nul";
            Process process = Process.Start(startInfo);
            process.WaitForExit();
        }

        public List<string> GetAll()
        {
            fillCache();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.FileName = "arp";
            startInfo.Arguments = "-a";
            Process process = Process.Start(startInfo);
            var arpStream = process.StandardOutput;
            List<string> result = new List<string>();
            while (!arpStream.EndOfStream)
            {
                var line = arpStream.ReadLine().Trim();
                result.Add(line);
            }
            return result;
        }
    }
}
