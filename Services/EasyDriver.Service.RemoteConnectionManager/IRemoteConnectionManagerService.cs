using EasyDriver.RemoteConnectionPlugin;
using EasyDriver.ServicePlugin;
using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyDriver.Service.RemoteConnectionManager
{
    public interface IRemoteConnectionManagerService : IEasyServicePlugin
    {
        IEnumerable<string> AvailableRemoteConnections { get; }
        Dictionary<IStationCore, IEasyRemoteConnectionPlugin> RemoteConnectionPool { get; }
        IEasyRemoteConnectionPlugin GetRemoteConnection(ICoreItem item);
        IEasyRemoteConnectionPlugin AddRemoteConnection(IStationCore stationCore, IEasyRemoteConnectionPlugin remoteConnectionPlugin);
        IEasyRemoteConnectionPlugin AddRemoteConnection(IStationCore stationCire, string connectionPath);
        IEasyRemoteConnectionPlugin CreateRemoteConnection(string connectionPath);
        void RemoveRemoteConnection(IStationCore stationCore);
        event EventHandler<CommandExecutedEventArgs> WriteCommandExecuted;
    }
}
