using EasyDriver.Client.Models;
using System.Collections.Generic;
using System.Linq;

namespace EasyScada.ServerApplication
{
    public class HubModel
    {
        public string RemoteAddress { get; set; }
        public string Port { get; set; }
        public string CommunicationMode { get; set; }
        public string StationName { get; set; }
        public string Name => $"{RemoteAddress}:{Port}";
        public bool Checked { get; set; }
        public List<Station> Stations { get; set; }
    }
}
