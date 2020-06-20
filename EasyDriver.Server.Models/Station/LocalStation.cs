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
    public class LocalStation : GroupItemBase, IStationCore, IStation
    {
        #region IStationCore

        public LocalStation(IGroupItem parent) : base(parent, true)
        {
            Name = "Local Station";
            IsLocalStation = true;
            ParameterContainer = new ParameterContainer();
        }

        [JsonIgnore]
        public string RemoteAddress { get; set; }

        [JsonIgnore]
        public string CommunicationError { get; set; }

        [JsonIgnore]
        public CommunicationMode CommunicationMode { get; set; }

        [JsonIgnore]
        public bool IsLocalStation { get; set; }

        [JsonIgnore]
        public ushort Port { get; set; }

        [JsonIgnore]
        public int RefreshRate { get; set; }

        [JsonIgnore]
        public DateTime LastRefreshTime { get; set; }

        [Category(PropertyCategory.General), DisplayName("Parameters")]
        [JsonIgnore]
        public IParameterContainer ParameterContainer { get; set; }

        [Browsable(false)]
        [JsonIgnore]
        public object SyncObject => throw new NotImplementedException();

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

        [JsonProperty("RefreshRate")]
        int IStation.RefreshRate => RefreshRate;

        [JsonProperty("LastRefreshTime")]
        DateTime IStation.LastRefreshTime => LastRefreshTime;

        [JsonProperty("CommunicationMode")]
        CommunicationMode IStation.CommunicationMode => CommunicationMode;

        [JsonProperty("Path")]
        string IPath.Path => Path;

        [JsonProperty("Port")]
        ushort IStation.Port => Port;

        [JsonProperty("Error")]
        string IStation.Error => CommunicationError;

        [JsonProperty("Parameters")]
        Dictionary<string, object> IStation.Parameters => ParameterContainer.Parameters;

        [JsonProperty("Channels")]
        List<IChannel> IStation.Channels
        {
            get
            {
                List<IChannel> result = Childs.Select(x => x as IChannel)?.ToList();
                if (result == null)
                    result = new List<IChannel>();
                return result;
            }
        }

        [JsonProperty("RemoteStations")]
        List<IStation> IStation.RemoteStations => new List<IStation>();

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
