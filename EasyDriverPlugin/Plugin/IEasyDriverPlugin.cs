using System;
using System.Collections.Generic;

namespace EasyDriverPlugin
{
    public interface IEasyDriverPlugin : IDisposable
    {
        WriteQueue WriteQueue { get; }
        IChannelCore Channel { get; set; }
        List<IDataType> SupportDataTypes { get; }

        bool Start();
        bool Stop();

        object GetCreateChannelControl(IGroupItem parent, IChannelCore templateItem = null);
        object GetCreateDeviceControl(IGroupItem parent, IDeviceCore templateItem = null);
        object GetCreateTagControl(IGroupItem parent, ITagCore templateItem = null);
        object GetEditChannelControl(IChannelCore channel);
        object GetEditDeviceControl(IDeviceCore device);
        object GetEditTagControl(ITagCore tag);

        IChannelCore ConvertToChannel(IChannelCore baseChannel);
        IDeviceCore ConverrtToDevice(IDeviceCore baseDevice);
        ITagCore ConvertToTag(ITagCore tagCore);

        event EventHandler Disposed;
        event EventHandler Refreshed;
    }
}
