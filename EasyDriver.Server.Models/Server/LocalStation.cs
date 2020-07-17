using EasyDriverPlugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace EasyDriver.Core
{
    [Serializable]
    public class LocalStation : GroupItemBase, IStationCore, IStationClient
    {
        #region IStationCore

        public LocalStation(IGroupItem parent) : base(parent, true)
        {
            Name = "Local Station";
            SyncObject = new object();
            IsLocalStation = true;
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
        public bool IsLocalStation { get; set; }

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

        #region IStation

        [JsonProperty("Name")]
        string IStationClient.Name => Name;

        [JsonProperty("IsLocalStation")]
        bool IStationClient.IsLocalStation => IsLocalStation;

        [JsonProperty("RemoteAddress")]
        string IStationClient.RemoteAddress => RemoteAddress;

        [JsonProperty("RefreshRate")]
        int IStationClient.RefreshRate => RefreshRate;

        [JsonProperty("LastRefreshTime")]
        DateTime IStationClient.LastRefreshTime => LastRefreshTime;

        [JsonProperty("CommunicationMode")]
        CommunicationMode IStationClient.CommunicationMode => CommunicationMode;

        [JsonProperty("ConnectionStatus")]
        ConnectionStatus IStationClient.ConnectionStatus => ConnectionStatus;

        [JsonProperty("Path")]
        string IPath.Path => Path;

        [JsonProperty("Port")]
        ushort IStationClient.Port => Port;

        [JsonProperty("Error")]
        string IStationClient.Error => CommunicationError;

        [JsonProperty("Parameters")]
        Dictionary<string, object> IStationClient.Parameters => ParameterContainer.Parameters;

        [JsonProperty("Channels")]
        List<IChannelClient> IStationClient.Channels
        {
            get { return Childs.Select(x => x as IChannelClient)?.ToList(); }
        }

        [JsonProperty("RemoteStations")]
        List<IStationClient> IStationClient.RemoteStations
        {
            get { return Childs.Select(x => x as IStationClient)?.ToList(); }
        }

        public T GetItem<T>(string pathToObject) where T : class, IPath
        {
            if (string.IsNullOrWhiteSpace(pathToObject))
                return null;
            if (Path == pathToObject)
                return this as T;
            if (pathToObject.StartsWith(Path))
            {
                foreach (var child in Childs)
                {
                    if (child is IPath item)
                    {
                        if (pathToObject.StartsWith(item.Path))
                            return item.GetItem<T>(pathToObject);
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
