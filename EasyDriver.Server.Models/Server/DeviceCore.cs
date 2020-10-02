using EasyDriverPlugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace EasyDriver.Core
{
    [Serializable]
    public class DeviceCore : GroupItemBase, IDeviceCore, IClientObject
    {
        #region IDeviceCore

        public DeviceCore(IGroupItem parent, bool isReadOnly = false) : base(parent, isReadOnly)
        {
            SyncObject = new object();
            ParameterContainer = new ParameterContainer();
            HaveTags = true;
            Tags = new TagCollection(this);
        }

        [JsonIgnore]
        public object SyncObject { get; protected set; }

        [JsonIgnore]
        public IParameterContainer ParameterContainer { get; set; }

        [JsonIgnore]
        public ByteOrder ByteOrder { get; set; }

        [JsonIgnore]
        public DateTime LastRefreshTime { get; set; }

        [JsonIgnore]
        public string CommunicationError { get; set; }

        [JsonIgnore]
        public ConnectionStatus ConnectionStatus { get; set; }

        public override string GetErrorOfProperty(string propertyName)
        {
            return string.Empty;
        }

        public override void GetErrors(ref IErrorInfo errorInfo)
        {
        }

        #endregion

        #region IHaveTags

        [JsonIgnore]
        public bool HaveTags { get; set; }

        [JsonIgnore]
        public TagCollection Tags { get; protected set; }

        #endregion

        #region IClientObject

        [JsonProperty("Name")]
        string IClientObject.Name => Name;

        [JsonProperty("Path")]
        string IClientObject.Path => Path;

        [JsonProperty("Description")]
        string IClientObject.Description => Description;

        [JsonProperty("Error")]
        string IClientObject.Error => CommunicationError;

        [JsonProperty("ItemType")]
        ItemType IClientObject.ItemType => ItemType.Device;

        [JsonProperty("Childs")]
        List<IClientObject> IClientObject.Childs => this.GetClientObjects();

        [JsonProperty("DisplayInfo")]
        string IClientObject.DisplayInfo => "";

        [JsonProperty("ConnectionStatus")]
        ConnectionStatus IClientObject.ConnectionStatus => ConnectionStatus;

        [field: NonSerialized]
        private Dictionary<string, string> properties;
        public Dictionary<string, string> Properties
        {
            get
            {
                if (properties == null)
                    properties = new Dictionary<string, string>();
                return properties;
            }
        }

        #endregion
    }

}
