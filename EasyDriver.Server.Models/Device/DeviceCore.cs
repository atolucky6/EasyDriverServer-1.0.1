using EasyDriverPlugin;
using EasyDriver.Client.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyDriver.Server.Models
{
    [Serializable]
    public class DeviceCore : GroupItemBase, IDeviceCore, IDevice
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
        string IDevice.Name => Name;

        [JsonProperty("Path")]
        string IPath.Path => Path;

        [JsonProperty("Parameters")]
        Dictionary<string, object> IDevice.Parameters => ParameterContainer.Parameters;

        [JsonProperty("LastRefreshTime")]
        DateTime IDevice.LastRefreshTime => LastRefreshTime;

        [JsonProperty("Error")]
        string IDevice.Error => CommunicationError;

        [JsonProperty("Tags")]
        List<ITag> IDevice.Tags
        {
            get { return Childs.Select(x => x as ITag)?.ToList(); }
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
