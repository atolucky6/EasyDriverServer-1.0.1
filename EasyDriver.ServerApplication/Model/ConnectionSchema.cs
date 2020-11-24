using EasyDriver.Core;
using EasyDriverPlugin;
using System;
using System.Collections.Generic;

namespace EasyScada.ServerApplication
{
    [Serializable]
    public class ConnectionSchema
    {
        public ItemType ItemType { get; set; } = ItemType.ConnectionSchema;
        public string ServerAddress { get; set; }
        public ushort Port { get; set; }
        public CommunicationMode CommunicationMode { get; set; }
        public int RefreshRate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<IClientObject> Childs { get; set; } = new List<IClientObject>();
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    }
}
