using EasyScada.Api.Interfaces;

namespace EasyDriverPlugin
{
    public interface IChannelCore : IGroupItem, ISupportParameters, ISupportSynchronization
    {
        ConnectionType ConnectionType { get; set; }
        ComunicationMode ComunicationMode { get; set; }
        Indexer<IDeviceCore> Devices { get; }
        string DriverPath { get; set; }
        int RefreshRate { get; set; }
    }
}
