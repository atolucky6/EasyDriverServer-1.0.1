using System;
using System.Collections.Generic;

namespace EasyScada.Api.Interfaces
{
    [Serializable]
    public class Station 
    {
        public string Name { get; set; }

        public bool IsLocalStation { get; set; }

        public string RemoteAddress { get; set; }

        public ushort Port { get; set; }

        public string Error { get; set; }

        public CommunicationMode CommunicationMode { get; set; }

        public int RefreshRate { get; set; }

        public Dictionary<string, object> Parameters { get; set; }

        public List<Channel> Channels { get; set; }

        public List<Station> RemoteStations { get; set; }

        public string Path { get; set; }

        public bool Checked { get; set; }
    }
}
