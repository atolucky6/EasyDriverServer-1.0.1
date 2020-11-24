using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace EasyDriverPlugin
{
    public abstract class EasyDriverPluginBase : IEasyDriverPlugin
    {
        public virtual WriteQueue WriteQueue { get; protected set; } = new WriteQueue();

        public virtual List<IDataType> SupportDataTypes { get; protected set; } = new List<IDataType>();

        public virtual event EventHandler Disposed;
        public virtual event EventHandler Refreshed;

        public abstract IChannelCore CreateChannel(IGroupItem parent);
        public abstract IDeviceCore CreateDevice(IGroupItem parent);
        public abstract ITagCore CreateTag(IGroupItem parent);

        public abstract void Dispose();
        public abstract object GetCreateChannelControl(IGroupItem parent, IChannelCore templateItem = null);
        public abstract object GetCreateDeviceControl(IGroupItem parent, IDeviceCore templateItem = null);
        public abstract object GetCreateTagControl(IGroupItem parent, ITagCore templateItem = null);
        public abstract object GetEditChannelControl(IChannelCore channel);
        public abstract object GetEditDeviceControl(IDeviceCore device);
        public abstract object GetEditTagControl(ITagCore tag);
        public abstract bool Start(IChannelCore channel);
        public abstract bool Stop();
    }
}
