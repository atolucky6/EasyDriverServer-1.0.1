using System;
using System.Collections.Generic;

namespace EasyDriverPlugin
{
    public interface IEasyDriverPlugin : IDisposable
    {
        IChannel Channel { get; set; }
        bool Connect();
        bool Disconnect();
        Quality WriteSingle(ITag tag, string value);
        void WriteMulti(ITag[] tags, string[] values);
        IEnumerable<IDataType> GetSupportDataTypes();

        object GetCreateChannelControl();
        object GetCreateDeviceControl();
        object GetCreateTagControl(IDevice parent);
        object GetEditChannelControl(IChannel channel);
        object GetEditDeviceControl(IDevice device);
        object GetEditTagControl(ITag tag);
    }
}
