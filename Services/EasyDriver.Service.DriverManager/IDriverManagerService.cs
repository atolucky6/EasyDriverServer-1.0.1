using EasyDriver.ServicePlugin;
using EasyDriverPlugin;
using System;
using System.Collections.Generic;

namespace EasyDriver.Service.DriverManager
{
    public interface IDriverManagerService : IEasyServicePlugin
    {
        IEnumerable<string> AvailableDrivers { get; }
        Dictionary<IChannelCore, IEasyDriverPlugin> DriverPoll { get; }
        IEasyDriverPlugin GetDriver(ICoreItem item);
        IEasyDriverPlugin AddDriver(IChannelCore channel, IEasyDriverPlugin driver);
        IEasyDriverPlugin AddDriver(IChannelCore channel, string driverPath);
        IEasyDriverPlugin CreateDriver(string driverPath);
        void RemoveDriver(IChannelCore channel);
        event EventHandler<CommandExecutedEventArgs> WriteCommandExecuted;
    }
}
