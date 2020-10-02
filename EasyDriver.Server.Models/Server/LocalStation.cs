using EasyDriverPlugin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace EasyDriver.Core
{
    [Serializable]
    public class LocalStation : GroupItemBase, IStationCore, IClientObject
    {
        #region IStationCore

        public LocalStation(IGroupItem parent) : base(parent, true)
        {
            Name = "Local Station";
            SyncObject = new object();
            StationType = "Local";
            ParameterContainer = new ParameterContainer();
        }

        [JsonIgnore]
        public string RemoteAddress
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [JsonIgnore]
        public string CommunicationError { get; set; }

        [JsonIgnore]
        public string OpcDaServerName { get; set; }

        [JsonIgnore]
        public CommunicationMode CommunicationMode
        {
            get { return GetProperty<CommunicationMode>(); }
            set { SetProperty(value); }
        }

        [field: NonSerialized]
        ConnectionStatus connectionStatus;
        [JsonIgnore]
        public ConnectionStatus ConnectionStatus
        {
            get => ConnectionStatus.Connected;
            set { connectionStatus = value; }
        }

        [JsonIgnore]
        public string StationType { get; set; }

        [JsonIgnore]
        public ushort Port
        {
            get { return GetProperty<ushort>(); }
            set { SetProperty(value); }
        }

        [JsonIgnore]
        public int RefreshRate { get; set; }

        [JsonIgnore]
        public DateTime LastRefreshTime { get; set; }

        [Category(PropertyCategory.General), DisplayName("Parameters")]
        [JsonIgnore]
        public IParameterContainer ParameterContainer { get; set; }

        [Browsable(false)]
        [JsonIgnore]
        public object SyncObject { get; set; }

        public override string GetErrorOfProperty(string propertyName)
        {
            return string.Empty;
        }

        public override void GetErrors(ref IErrorInfo errorInfo)
        {
        }

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
        ItemType IClientObject.ItemType => ItemType.LocalStation;

        [JsonProperty("Childs")]
        List<IClientObject> IClientObject.Childs => this.GetClientObjects();

        [JsonProperty("ConnectionStatus")]
        ConnectionStatus IClientObject.ConnectionStatus => ConnectionStatus;

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
    
