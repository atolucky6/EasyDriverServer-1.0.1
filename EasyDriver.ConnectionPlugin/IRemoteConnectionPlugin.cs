using EasyDriver.RemoteConnectionManager;
using System;

namespace EasyDriver.ConnectionPlugin
{
    public interface IRemoteConnectionPlugin
    {
        Func<string, IRemoteConnection> CreateConnectionFunc { get; }
    }
}
