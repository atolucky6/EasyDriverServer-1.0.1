using EasyDriverPlugin;
using EasyScada.Api.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace EasyScada.Core
{
    [Serializable]
    public class Channel : GroupItemBase, IChannelCore, IChannel
    {
        #region IChannelCore

        public Channel(IGroupItem parent, bool isReadOnly = false) : base(parent, isReadOnly)
        {
            Devices = new Indexer<IDeviceCore>(this);
            SyncObject = new object();
            ParameterContainer = new ParameterContainer();
        }

        [Browsable(false)]
        public object SyncObject { get; private set; }

        [Browsable(false)]
        public ConnectionType ConnectionType { get; set; }

        [Browsable(false)]
        public string DriverPath { get; set; }

        [Browsable(false)]
        public Indexer<IDeviceCore> Devices { get; }

        public ComunicationMode ComunicationMode { get; set; }

        [Category(PropertyCategory.General), DisplayName("Parameters")]
        public IParameterContainer ParameterContainer { get; set; }

        int refreshRate = 10;
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

        string IChannel.Name => Name;

        string IChannel.DriverName => System.IO.Path.GetFileNameWithoutExtension(DriverPath);

        ConnectionType IChannel.ConnectionType => ConnectionType;

        ComunicationMode IChannel.ComunicationMode => ComunicationMode;

        int IChannel.RefreshRate => RefreshRate;

        Dictionary<string, object> IChannel.Parameters => ParameterContainer.Parameters;

        List<IDevice> IChannel.Devices => Childs.Select(x => x as IDevice).ToList();

        #endregion
    }
}
