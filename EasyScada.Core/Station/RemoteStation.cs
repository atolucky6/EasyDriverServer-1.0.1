using EasyDriverPlugin;
using System;
using System.ComponentModel;

namespace EasyScada.Core
{
    [Serializable]
    public class RemoteStation : GroupItemBase, IStationCore
    {
        public RemoteStation(IGroupItem parent, bool isReadOnly = false) : base(parent, isReadOnly)
        {
        }

        public string RemoteAddress { get; set; }

        public ushort Port { get; set; }

        [Category(PropertyCategory.General), DisplayName("Parameters")]
        public IParameterContainer ParameterContainer { get; set; }

        [Browsable(false)]
        public object SyncObject => throw new NotImplementedException();

        public override string GetErrorOfProperty(string propertyName)
        {
            throw new NotImplementedException();
        }

        public override void GetErrors(ref IErrorInfo errorInfo)
        {
            throw new NotImplementedException();
        }
    }

    public enum RefreshMode
    {
        Boardcast,
        Request
    }
}
