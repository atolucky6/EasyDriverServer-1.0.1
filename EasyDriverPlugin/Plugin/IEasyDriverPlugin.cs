using EasyDriver.Client.Models;
using System;
using System.Collections.Generic;

namespace EasyDriverPlugin
{
    public interface IEasyDriverPlugin : IDisposable
    {
        IChannelCore Channel { get; set; }
        bool Connect();
        bool Disconnect();
        Quality WriteSingle(ITagCore tag, string value);
        void WriteMulti(ITagCore[] tags, string[] values);
        IEnumerable<IDataType> GetSupportDataTypes();

        object GetCreateChannelControl();
        object GetCreateDeviceControl();
        object GetCreateTagControl(IDeviceCore parent);
        object GetEditChannelControl(IChannelCore channel);
        object GetEditDeviceControl(IDeviceCore device);
        object GetEditTagControl(ITagCore tag);
    }
}
