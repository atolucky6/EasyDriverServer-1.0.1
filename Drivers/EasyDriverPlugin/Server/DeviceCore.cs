using EasyDriverPlugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace EasyDriverPlugin
{
    [Serializable]
    public class DeviceCore : GroupItemBase, IDeviceCore
    {
        #region IDeviceCore

        public DeviceCore(IGroupItem parent, bool isReadOnly = false) : base(parent, isReadOnly)
        {
            SyncObject = new object();
            ParameterContainer = new ParameterContainer();
            HaveTags = true;
            Tags = new NotifyCollection(this);
        }

        public object SyncObject { get; protected set; }

        public IParameterContainer ParameterContainer { get; set; }

        public ByteOrder ByteOrder { get; set; }

        public DateTime LastRefreshTime { get; set; }

        public ConnectionStatus ConnectionStatus { get; set; }

        public override ItemType ItemType { get; set; } = ItemType.Device;

        public override string GetErrorOfProperty(string propertyName)
        {
            return string.Empty;
        }

        #endregion

        #region IHaveTags

        [JsonIgnore]
        public bool HaveTags { get; set; }

        [JsonIgnore]
        public NotifyCollection Tags { get; protected set; }

        #endregion
    }

}
