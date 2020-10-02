using EasyDriverPlugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EasyDriver.Core
{
    [Serializable]
    public class ChannelCore : GroupItemBase, IChannelCore, IClientObject
    {
        #region IChannelCore

        public ChannelCore(IGroupItem parent, bool isReadOnly = false) : base(parent, isReadOnly)
        {
            SyncObject = new object();
            ParameterContainer = new ParameterContainer();
            HaveTags = false;
            Tags = new TagCollection(this);
        }

        [Browsable(false)]
        [JsonIgnore]
        public object SyncObject { get; private set; }

        [Browsable(false)]
        [JsonIgnore]
        public string DriverPath { get; set; }

        [Browsable(false)]
        [JsonIgnore]
        public string DriverName
        {
            get
            {
                if (string.IsNullOrEmpty(DriverPath))
                    return string.Empty;
                return System.IO.Path.GetFileNameWithoutExtension(DriverPath);
            }
        }

        [JsonIgnore]
        public DateTime LastRefreshTime { get; set; }

        [JsonIgnore]
        public string CommunicationError { get; set; }

        [JsonIgnore]
        [Category(PropertyCategory.General), DisplayName("Parameters")]
        public IParameterContainer ParameterContainer { get; set; }

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
        ItemType IClientObject.ItemType => ItemType.Channel;

        [JsonProperty("Childs")]
        List<IClientObject> IClientObject.Childs => this.GetClientObjects();

        [JsonProperty("ConnectionStatus")]
        ConnectionStatus IClientObject.ConnectionStatus => ConnectionStatus;

        [JsonProperty("DisplayInfo")]
        string IClientObject.DisplayInfo => string.IsNullOrEmpty(DriverPath) ? "" : System.IO.Path.GetFileNameWithoutExtension(DriverPath);

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
