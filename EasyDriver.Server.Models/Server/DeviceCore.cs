using EasyDriverPlugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyDriver.Core
{
    [Serializable]
    public class DeviceCore : GroupItemBase, IDeviceCore, IDeviceClient
    {
        #region IDeviceCore

        public DeviceCore(IGroupItem parent, bool isReadOnly = false) : base(parent, isReadOnly)
        {
            SyncObject = new object();
            ParameterContainer = new ParameterContainer();
            Tags = new Indexer<ITagCore>(this);
        }

        [JsonIgnore]
        public object SyncObject { get; protected set; }

        [JsonIgnore]
        public Indexer<ITagCore> Tags { get; protected set; }

        [JsonIgnore]
        public IParameterContainer ParameterContainer { get; set; }

        [JsonIgnore]
        public ByteOrder ByteOrder { get; set; }

        [JsonIgnore]
        public DateTime LastRefreshTime { get; set; }

        [JsonIgnore]
        public string CommunicationError { get; set; }

        public override string GetErrorOfProperty(string propertyName)
        {
            return string.Empty;
        }

        public override void GetErrors(ref IErrorInfo errorInfo)
        {
        }

        #endregion

        #region IDevice

        [JsonProperty("Name")]
        string IDeviceClient.Name => Name;

        [JsonProperty("Path")]
        string IPath.Path => Path;

        [JsonProperty("Parameters")]
        Dictionary<string, object> IDeviceClient.Parameters => ParameterContainer.Parameters;

        [JsonProperty("LastRefreshTime")]
        DateTime IDeviceClient.LastRefreshTime => LastRefreshTime;

        [JsonProperty("Error")]
        string IDeviceClient.Error => CommunicationError;

        [JsonProperty("Tags")]
        List<ITagClient> IDeviceClient.Tags
        {
            get { return Childs.Select(x => x as ITagClient)?.ToList(); }
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
                        if (pathToObject == item.Path)
                            return item as T;
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
