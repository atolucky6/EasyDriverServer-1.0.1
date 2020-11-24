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
        public object SyncObject { get; private set; }

        public override string DisplayInformation { get => DriverPath; set => base.DisplayInformation = value; }

        string driverPath;
        [Browsable(false)]
        public string DriverPath
        {
            get => driverPath;
            set
            {
                if (driverPath != value)
                {
                    driverPath = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(DisplayInformation));
                }
            }
        }

        [Browsable(false)]
        public string DriverName
        {
            get
            {
                if (string.IsNullOrEmpty(DriverPath))
                    return string.Empty;
                return System.IO.Path.GetFileNameWithoutExtension(DriverPath);
            }
        }

        public DateTime LastRefreshTime { get; set; }

        [Category(PropertyCategory.General), DisplayName("Parameters")]
        public IParameterContainer ParameterContainer { get; set; }

        public ConnectionStatus ConnectionStatus { get; set; }

        public override ItemType ItemType { get; set; } = ItemType.Channel;

        public override string GetErrorOfProperty(string propertyName)
        {
            return string.Empty;
        }

        #endregion
    }
}
