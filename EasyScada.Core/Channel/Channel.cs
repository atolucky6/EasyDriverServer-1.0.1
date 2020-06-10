using EasyDriverPlugin;
using System;
using System.ComponentModel;

namespace EasyScada.Core
{
    [Serializable]
    public class Channel : GroupItemBase, IChannel
    {
        public Channel(IGroupItem parent, bool isReadOnly = false) : base(parent, isReadOnly)
        {
            Devices = new Indexer<IDevice>(this);
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
        public Indexer<IDevice> Devices { get; }

        [Category(PropertyCategory.General), DisplayName("Parameters")]
        public IParameterContainer ParameterContainer { get; set; }

        public override string GetErrorOfProperty(string propertyName)
        {
            return string.Empty;
        }

        public override void GetErrors(ref IErrorInfo errorInfo)
        {
        }
    }
}
