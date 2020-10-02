using EasyDriver.ServicePlugin;
using EasyDriverPlugin;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EasyDriver.DriverManager
{
    public interface IDriverManagerService : IEasyServicePlugin
    {
        ReadOnlyObservableCollection<IEasyDriverPlugin> Drivers { get; }
        IReadOnlyDictionary<IChannelCore, IEasyDriverPlugin> DriverPoll { get; }
        IEasyDriverPlugin GetDriver(ICoreItem item);
        IEasyDriverPlugin AddDriver(IChannelCore channel, IEasyDriverPlugin driver);
        IEasyDriverPlugin AddDriver(IChannelCore channel, string driverPath);
        void RemoveDriver(IChannelCore channel);
    }
}
