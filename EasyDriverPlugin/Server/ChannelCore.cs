using EasyDriverPlugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EasyDriverPlugin
{
    [Serializable]
    public class ChannelCore : GroupItemBase, IChannelCore
    {
        #region IChannelCore

        public ChannelCore(IGroupItem parent, bool isReadOnly = false) : base(parent, isReadOnly)
        {
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
        public string DriverName
        {
            get
            {
                if (string.IsNullOrEmpty(DriverPath))
                    return string.Empty;
                return System.IO.Path.GetFileNameWithoutExtension(DriverPath);
            }
        }

        [JsonIgnore]
        public DateTime LastRefreshTime { get; set; }

        [JsonIgnore]
        [Category(PropertyCategory.General), DisplayName("Parameters")]
        public IParameterContainer ParameterContainer { get; set; }

        [JsonIgnore]
        public ConnectionStatus ConnectionStatus { get; set; }

        public override ItemType ItemType { get; set; } = ItemType.Channel;

        public override string GetErrorOfProperty(string propertyName)
        {
            return string.Empty;
        }

        #endregion
    }
}
