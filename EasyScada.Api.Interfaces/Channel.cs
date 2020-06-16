using System;
using System.Collections.Generic;

namespace EasyScada.Api.Interfaces
{
    [Serializable]
    public class Channel
    {
        public string Name { get; set; }

        public string DriverName { get; set; }

        public ConnectionType ConnectionType { get; set; }

        public int RefreshRate { get; set; }

        public string Error { get; set; }

        public List<Device> Devices { get; set; }

        public Dictionary<string, object> Parameters { get; set; }

        public string Path { get; set; }

        public bool Checked { get; set; }
    }
}
