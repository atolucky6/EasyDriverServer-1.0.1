using EasyDriverPlugin;
using System;
using System.Collections.Generic;

namespace EasyDriver.RemoteConnectionManager
{
    public interface IRemoteConnectionManagerService
    {
        List<Func<string, IRemoteConnection>> CreateRemoteConnectionFuncs { get; set; }
        IReadOnlyDictionary<IStationCore, IRemoteConnection> RemoteConnectionDictionary { get; }
        void AddConnection(IStationCore station, IRemoteConnection connection);
        void AddConnection(IStationCore station);
        bool RemoveConnection(IStationCore station);
        void ReloadConnection(IStationCore station);
        IRemoteConnection GetConnection(IStationCore station);
        IRemoteConnection CreateConnection(IStationCore station);
    }
}

