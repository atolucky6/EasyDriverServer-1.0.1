using System;
using System.Collections.Generic;

namespace EasyDriverPlugin
{
    public interface IEasyDriverPlugin : IDisposable
    {
        IChannelCore Channel { get; set; }
        bool Connect();
        bool Disconnect();
        Quality Write(ITagCore tag, string value);
        IEnumerable<IDataType> GetSupportDataTypes();

        object GetCreateChannelControl(IGroupItem parent, IChannelCore templateItem = null);
        object GetCreateDeviceControl(IGroupItem parent, IDeviceCore templateItem = null);
        object GetCreateTagControl(IGroupItem parent, ITagCore templateItem = null);
        object GetEditChannelControl(IChannelCore channel);
        object GetEditDeviceControl(IDeviceCore device);
        object GetEditTagControl(ITagCore tag);

        event EventHandler Disposed;
        event EventHandler Refreshed;
    }
}
