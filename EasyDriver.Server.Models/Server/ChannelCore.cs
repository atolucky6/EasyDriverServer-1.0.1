using EasyDriverPlugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace EasyDriver.Core
{
    [Serializable]
    public class ChannelCore : GroupItemBase, IChannelCore, IChannelClient
    {
        #region IChannelCore

        public ChannelCore(IGroupItem parent, bool isReadOnly = false) : base(parent, isReadOnly)
        {
            Devices = new Indexer<IDeviceCore>(this);
            SyncObject = new object();
            ParameterContainer = new ParameterContainer();
        }

        [Browsable(false)]
        [JsonIgnore]
        public object SyncObject { get; private set; }

        [Browsable(false)]
        [JsonIgnore]
        public string DriverPath { get; set; }

        [Browsable(false)]
        [JsonIgnore]
        public string DriverName { get { return System.IO.Path.GetFileNameWithoutExtension(DriverPath); } }

        [Browsable(false)]
        [JsonIgnore]
        public Indexer<IDeviceCore> Devices { get; }

        [JsonIgnore]
        public DateTime LastRefreshTime { get; set; }

        [JsonIgnore]
        public string CommunicationError { get; set; }

        [Category(PropertyCategory.General), DisplayName("Parameters")]
        [JsonIgnore]
        public IParameterContainer ParameterContainer { get; set; }

        public override string GetErrorOfProperty(string propertyName)
        {
            return string.Empty;
        }

        public override void GetErrors(ref IErrorInfo errorInfo)
        {
        }

        #endregion

        #region IChannel

        [JsonProperty("Name")]
        string IChannelClient.Name => Name;

        [JsonProperty("Path")]
        string IPath.Path => Path;

        [JsonProperty("DriverName")]
        string IChannelClient.DriverName => DriverName;

        [JsonProperty("LastRefreshTime")]
        DateTime IChannelClient.LastRefreshTime => LastRefreshTime;

        [JsonProperty("Error")]
        string IChannelClient.Error => CommunicationError;

        [JsonProperty("Parameters")]
        Dictionary<string, object> IChannelClient.Parameters => ParameterContainer.Parameters;

        [JsonProperty("Devices")]
        List<IDeviceClient> IChannelClient.Devices
        {
            get { return Childs.Select(x => x as IDeviceClient)?.ToList(); }
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
