namespace EasyDriverPlugin
{
    public interface IChannel : IGroupItem, ISupportParameters, ISupportSynchronization
    {
        ConnectionType ConnectionType { get; }
        Indexer<IDevice> Devices { get; }
        string DriverPath { get; set; }
    }

    public enum ConnectionType
    {
        Serial,
        Ethernet
    }
}
