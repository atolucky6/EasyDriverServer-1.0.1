using System;

namespace EasyDriverPlugin
{
    public interface IStationCore : IGroupItem, ISupportParameters, ISupportSynchronization
    {
        string RemoteAddress { get; set; }
        ushort Port { get; set; }
        CommunicationMode CommunicationMode { get; set; }
        ConnectionStatus ConnectionStatus { get; set; }
        StationType StationType { get; set; }
        int RefreshRate { get; set; }
        string CommunicationError { get; set; }
        DateTime LastRefreshTime { get; set; }
        string OpcDaServerName { get; set; }
    }
}
