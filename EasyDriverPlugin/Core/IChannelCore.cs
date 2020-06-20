using EasyDriver.Client.Models;
using System;

namespace EasyDriverPlugin
{
    public interface IChannelCore : IGroupItem, ISupportParameters, ISupportSynchronization
    {
        ConnectionType ConnectionType { get; set; }
        Indexer<IDeviceCore> Devices { get; }
        string DriverPath { get; set; }
        string CommunicationError { get; set; }
        DateTime LastRefreshTime { get; set; }
    }
}
