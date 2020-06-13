using System;

namespace EasyDriverPlugin
{
    public interface IDeviceCore : IGroupItem, ISupportParameters, ISupportSynchronization
    {
        Indexer<ITagCore> Tags { get; }
        string CommunicationError { get; set; }
        ByteOrder ByteOrder { get; set; }
        DateTime LastRefreshTime { get; set; }
    }
}
