using EasyDriver.Client.Models;
using System;
using System.Collections.Generic;

namespace EasyScada.Winforms.Connector
{
    [Serializable]
    public class DriverConnector
    {
        public string ServerAddress { get; set; }
        public ushort Port { get; set; }
        public CommunicationMode CommunicationMode { get; set; }
        public int RefreshRate { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<Station> Stations { get; set; }
    }
}
