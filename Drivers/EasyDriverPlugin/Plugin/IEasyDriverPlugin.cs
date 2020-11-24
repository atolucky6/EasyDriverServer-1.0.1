using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace EasyDriverPlugin
{
    public interface IEasyDriverPlugin : IDisposable
    {
        WriteQueue WriteQueue { get; }
        List<IDataType> SupportDataTypes { get; }

        bool Start(IChannelCore channel);
        bool Stop();

        object GetCreateChannelControl(IGroupItem parent, IChannelCore templateItem = null);
        object GetCreateDeviceControl(IGroupItem parent, IDeviceCore templateItem = null);
        object GetCreateTagControl(IGroupItem parent, ITagCore templateItem = null);
        object GetEditChannelControl(IChannelCore channel);
        object GetEditDeviceControl(IDeviceCore device);
        object GetEditTagControl(ITagCore tag);

        IChannelCore CreateChannel(IGroupItem parent);
        IDeviceCore CreateDevice(IGroupItem parent);
        ITagCore CreateTag(IGroupItem parent);

        event EventHandler Disposed;
        event EventHandler Refreshed;
    }
}
