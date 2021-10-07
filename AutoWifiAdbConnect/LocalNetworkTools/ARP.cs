using System.Collections.Generic;
using System.Diagnostics;

namespace AutoWifiAdbConnect.LocalNetworkTools
{
    class ARP
    {
        private void fillCache()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = "cmd";
            startInfo.Arguments = @"/c " + @"for /L %a in (1,1,254) do @start /b ping 192.168.1.%a -n 1 > nul";
            Process process = Process.Start(startInfo);
            process.WaitForExit();

            startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = "cmd";
            startInfo.Arguments = @"/c " + @"for /L %a in (1,1,254) do @start /b ping 192.168.0.%a -n 1 > nul";
            process = Process.Start(startInfo);
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
