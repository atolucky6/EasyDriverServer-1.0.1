using System;

namespace EasyDriverPlugin
{
    public interface IChannelCore : IGroupItem, ISupportParameters, ISupportSynchronization, IHaveTag
    {
        ConnectionStatus ConnectionStatus { get; set; }
        string DriverPath { get; set; }
        DateTime LastRefreshTime { get; set; }
    }
}
