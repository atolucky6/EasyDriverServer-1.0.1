using EasyDriverPlugin;
using EasyDriver.Client.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace EasyDriver.Server.Models
{
    [Serializable]
    public class RemoteStation : GroupItemBase, IStationCore, IStation
    {
        #region IStationCore

        public RemoteStation(IGroupItem parent) : base(parent, true)
        {
            SyncObject = new object();
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
        public DateTime LastRefreshTime { get;set; }

        [JsonIgnore]
        public CommunicationMode CommunicationMode
        {
            get { return GetProperty<CommunicationMode>(); }
            set { SetProperty(value); }
        }

        [field: NonSerialized]
        [JsonIgnore]
        public ConnectionStatus ConnectionStatus { get; set; }

        [Category(PropertyCategory.General), DisplayName("Parameters")]
        [JsonIgnore]
        public IParameterContainer ParameterContainer { get; set; }

        [Browsable(false)]
        [JsonIgnore]
        public object SyncObject { get; private set; }

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
        string IStation.Name => Name;

        [JsonProperty("IsLocalStation")]
        bool IStation.IsLocalStation => IsLocalStation;

        [JsonProperty("RemoteAddress")]
        string IStation.RemoteAddress => RemoteAddress;

        [JsonProperty("Path")]
        string IPath.Path => Path;

        [JsonProperty("Port")]
        ushort IStation.Port => Port;

        [JsonProperty("RefreshRate")]
        int IStation.RefreshRate => RefreshRate;

        [JsonProperty("LastRefreshTime")]
        DateTime IStation.LastRefreshTime => LastRefreshTime;

        [JsonProperty("CommunicationMode")]
        CommunicationMode IStation.CommunicationMode => CommunicationMode;

        [JsonProperty("ConnectionStatus")]
        ConnectionStatus IStation.ConnectionStatus => ConnectionStatus;

        [JsonProperty("Error")]
        string IStation.Error => CommunicationError;

        [JsonProperty("Parameters")]
        Dictionary<string, object> IStation.Parameters => ParameterContainer.Parameters;

        [JsonProperty("Channels")]
        List<IChannel> IStation.Channels
        {
            get { return Childs.Select(x => x as IChannel)?.ToList(); }
        }

        [JsonProperty("RemoteStations")]
        List<IStation> IStation.RemoteStations
        {
            get { return Childs.Select(x => x as IStation)?.ToList(); }
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
