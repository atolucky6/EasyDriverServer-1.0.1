using EasyDriverPlugin;
using EasyScada.Api.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyScada.Core
{
    [Serializable]
    public class Device : GroupItemBase, IDeviceCore, IDevice
    {
        #region IDeviceCore

        public Device(IGroupItem parent, bool isReadOnly = false) : base(parent, isReadOnly)
        {
            SyncObject = new object();
            ParameterContainer = new ParameterContainer();
            Tags = new Indexer<ITagCore>(this);
        }

        public object SyncObject { get; protected set; }

        public Indexer<ITagCore> Tags { get; protected set; }

        public IParameterContainer ParameterContainer { get; set; }

        public ByteOrder ByteOrder { get; set; }

        public DateTime LastRefreshTime { get; set; }

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

        string IDevice.Name => Name;

        Dictionary<string, object> IDevice.Parameters => ParameterContainer.Parameters;

        DateTime IDevice.LastRefreshTime => LastRefreshTime;

        string IDevice.Error => CommunicationError;

        List<ITag> IDevice.Tags => Childs.Select(x => x as ITag).ToList();

        #endregion
    }
}
