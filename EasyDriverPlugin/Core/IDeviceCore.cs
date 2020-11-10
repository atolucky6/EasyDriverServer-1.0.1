using System;

namespace EasyDriverPlugin
{
    public interface IDeviceCore : IGroupItem, ISupportParameters, ISupportSynchronization, IHaveTag
    {
        ConnectionStatus ConnectionStatus { get; set; }
        ByteOrder ByteOrder { get; set; }
        DateTime LastRefreshTime { get; set; }
    }
}
