using EasyDriverPlugin;
using System;

namespace EasyScada.Core
{
    [Serializable]
    public class Device : GroupItemBase, IDevice
    {
        public Device(IGroupItem parent, bool isReadOnly = false) : base(parent, isReadOnly)
        {
            SyncObject = new object();
            ParameterContainer = new ParameterContainer();
            Tags = new Indexer<ITag>(this);
        }

        public object SyncObject { get; protected set; }

        public Indexer<ITag> Tags { get; protected set; }

        public IParameterContainer ParameterContainer { get; set; }

        public ByteOrder ByteOrder { get; set; }

        public override string GetErrorOfProperty(string propertyName)
        {
            return string.Empty;
        }

        public override void GetErrors(ref IErrorInfo errorInfo)
        {
        }
    }
}
