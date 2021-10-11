using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoWifiAdbConnect.LocalNetworkTools
{
    class AdbExecutor
    {
        public string AdbDir { get; set; }
        public bool AdbDirCorrect { get { return File.Exists(Path.Combine(AdbDir, "adb.exe")); } }
        public event CommandExecutedEventHandler Executed;
        private Queue<string> _commands = new Queue<string>();

        public AdbExecutor(string _adbDir = "")
        {
            AdbDir = _adbDir;
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (_commands.Count > 0)
                    {
                        string cmd = _commands.Dequeue();
                        if (!AdbDirCorrect)
                            continue;
                        Task t = Task.Run(() =>
                        {
                            string result = RunAdb(cmd);
                            Executed?.Invoke(this, new CommandExecutedEventArgs(cmd, result));
                        });
                        if (!cmd.StartsWith("connect"))//Команду connect можно выполнять в нескольких потоках одновременно
                            try { t.Wait(); }
                            catch { throw t.Exception; }
                    }
                    Thread.Sleep(50);
                }
            }, TaskCreationOptions.LongRunning);
        }

        public void Enqueue(string cmd)
        {
            _commands.Enqueue(cmd);
        }

        private string RunAdb(string cmd)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.StandardOutputEncoding = Encoding.Default;
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.FileName = Path.Combine(AdbDir, "adb.exe");
            startInfo.Arguments = cmd;
            Process process = Process.Start(startInfo);
            process.WaitForExit();
            return process.StandardOutput.ReadToEnd();
        }

        public delegate void CommandExecutedEventHandler(object sender, CommandExecutedEventArgs e);
        public class CommandExecutedEventArgs : EventArgs
        {
            public string Command { get; }
            public string Result { get; }
            public CommandExecutedEventArgs(string cmd, string res)
            {
                Command = cmd;
                Result = res;
            }
        }
    }
}
