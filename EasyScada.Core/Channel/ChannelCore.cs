using EasyDriverPlugin;
using EasyScada.Api.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace EasyScada.Core
{
    [Serializable]
    public class ChannelCore : GroupItemBase, IChannelCore, IChannel
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
        public ConnectionType ConnectionType { get; set; }

        [Browsable(false)]
        [JsonIgnore]
        public string DriverPath { get; set; }

        [Browsable(false)]
        [JsonIgnore]
        public Indexer<IDeviceCore> Devices { get; }


        [Category(PropertyCategory.General), DisplayName("Parameters")]
        [JsonIgnore]
        public IParameterContainer ParameterContainer { get; set; }

        int refreshRate = 10;
        [JsonIgnore]
        public int RefreshRate
        {
            get
            {
                if (refreshRate < 10)
                    return 10;
                return refreshRate;
            }
            set
            {
                if (refreshRate != value)
                {
                    if (value < 10)
                        refreshRate = 10;
                    else
                        refreshRate = value;
                    RaisePropertyChanged();
                }
            }
        }

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
        string IChannel.Name => Name;

        [JsonProperty("Path")]
        string IPath.Path => Path;

        [JsonProperty("DriverName")]
        string IChannel.DriverName => System.IO.Path.GetFileNameWithoutExtension(DriverPath);

        [JsonProperty("ConnectionType")]
        ConnectionType IChannel.ConnectionType => ConnectionType;

        [JsonProperty("RefreshRate")]
        int IChannel.RefreshRate => RefreshRate;

        [JsonProperty("Parameters")]
        Dictionary<string, object> IChannel.Parameters => ParameterContainer.Parameters;

        [JsonProperty("Devices")]
        List<IDevice> IChannel.Devices
        {
            get
            {
                List<IDevice> result = Childs.Select(x => x as IDevice)?.ToList();
                if (result == null)
                    result = new List<IDevice>();
                return result;
            }
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
