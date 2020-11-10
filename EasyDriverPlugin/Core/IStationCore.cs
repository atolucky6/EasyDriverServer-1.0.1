using System;

namespace EasyDriverPlugin
{
    public interface IStationCore : IGroupItem, ISupportParameters, ISupportSynchronization
    {
        string RemoteAddress { get; set; }
        ushort Port { get; set; }
        string ConnectionString { get; set; }
        CommunicationMode CommunicationMode { get; set; }
        ConnectionStatus ConnectionStatus { get; set; }
        string StationType { get; set; }
        int RefreshRate { get; set; }
        DateTime LastRefreshTime { get; set; }
    }
}
