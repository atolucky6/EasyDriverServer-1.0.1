using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EasyDriver.Service.ApplicationProperties
{
    [Serializable]
    public class ServerConfiguration
    {
        public ushort Port { get; set; }
        public BroadcastMode BroadcastMode { get; set; }
        public int BroadcastRate { get; set; }
        public int MaximumAllowConnection { get; set; }
        public string ServerConfigPath { get; private set; }

        public ServerConfiguration(string path)
        {
            Port = 8800;
            BroadcastMode = BroadcastMode.SendAskedData;
            BroadcastRate = 1000;
            MaximumAllowConnection = 10;
            ServerConfigPath = path;
            Open(ServerConfigPath);
        }

        public void Open(string path = "")
        {
            bool needToCreate = false;
            try
            {
                if (!string.IsNullOrWhiteSpace(path))
                {
                    if (File.Exists(path))
                    {
                        string[] serverConfigs = File.ReadAllLines(path);
                        for (int i = 0; i < serverConfigs.Length; i++)
                        {
                            string line = serverConfigs[i]?.Replace(" ", "")?.Trim();
                            if (line.StartsWith("port"))
                            {
                                if (ushort.TryParse(line.Split('=')[1], out ushort port))
                                    Port = port;
                            }
                            else if (line.StartsWith("mode"))
                            {
                                if (Enum.TryParse(line.Split('=')[1], out BroadcastMode mode))
                                    BroadcastMode = mode;
                            }
                            else if (line.StartsWith("rate"))
                            {
                                if (int.TryParse(line.Split('=')[1], out int rate))
                                    if (rate >= 100)
                                        BroadcastRate = rate;
                            }
                            else if (line.StartsWith("max_connection"))
                            {
                                if (int.TryParse(line.Split('=')[1], out int maxConn))
                                    if (maxConn >= 1)
                                        MaximumAllowConnection = maxConn;
                            }
                        }
                    }
                    else
                    {
                        needToCreate = true;
                    }
                }
            }
            catch { needToCreate = true; }

            if (needToCreate)
                Save();
        }

        public bool Save()
        {
            try
            {
                if (!string.IsNullOrEmpty(ServerConfigPath))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"port = {Port}");
                    sb.AppendLine($"mode = {BroadcastMode.ToString()}");
                    sb.AppendLine($"rate = {BroadcastRate}");
                    sb.AppendLine($"max_connection = {MaximumAllowConnection}");
                    File.WriteAllText(ServerConfigPath, sb.ToString());
                    return true;
                }
                return false;
            }
            catch { return false; }
        }
    }
}
