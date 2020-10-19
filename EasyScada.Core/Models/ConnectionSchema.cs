using Newtonsoft.Json;
using System;

namespace EasyScada.Core
{
    [Serializable]
    public class ConnectionSchema : CoreItemBase
    {
        [JsonConstructor]
        public ConnectionSchema() : base()
        {
            Parent = null;
            ItemType = ItemType.ConnectionSchema;
        }

        public string ServerAddress { get; set; }
        public ushort Port { get; set; }
        public CommunicationMode CommunicationMode { get; set; }
        public int RefreshRate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
