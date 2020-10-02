using System;
using System.Collections.Generic;
using EasyDriverPlugin;
using Newtonsoft.Json;

namespace EasyDriver.Core
{
    public class GroupCore : GroupItemBase, IClientObject, IHaveTag
    {
        #region Constructors
        public GroupCore(IGroupItem parent, bool haveTags, bool isReadOnly = false) : base(parent, isReadOnly)
        {
            Tags = new TagCollection(this);
            HaveTags = haveTags;
        }
        #endregion

        #region Methods
        public override string GetErrorOfProperty(string propertyName)
        {
            return null;
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
        string IClientObject.Error => "";

        [JsonProperty("ItemType")]
        ItemType IClientObject.ItemType => ItemType.Group;

        [JsonProperty("Childs")]
        List<IClientObject> IClientObject.Childs => this.GetClientObjects();

        [JsonProperty("ConnectionStatus")]
        ConnectionStatus IClientObject.ConnectionStatus
        {
            get
            {
                var parent = this.FindParent<IGroupItem>(x => x is IDeviceCore || x is IChannelCore);
                if (parent is IDeviceCore deviceParent)
                    return deviceParent.ConnectionStatus;
                else if (parent is IChannelCore channelParent)
                    return channelParent.ConnectionStatus;
                return ConnectionStatus.Disconnected;
            }
        }

        [JsonProperty("DisplayInfo")]
        string IClientObject.DisplayInfo => "";

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
