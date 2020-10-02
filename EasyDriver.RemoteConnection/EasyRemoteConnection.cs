using System;
using EasyDriver.ConnectionPlugin;
using EasyDriver.RemoteConnectionManager;

namespace EasyDriver.RemoteConnection
{
    public class EasyRemoteConnection : IRemoteConnectionPlugin
    {
        public EasyRemoteConnection()
        {
            CreateConnectionFunc = new Func<string, IRemoteConnection>(CreateRemoteConnection);
        }

        public Func<string, IRemoteConnection> CreateConnectionFunc { get; private set; }

        private IRemoteConnection CreateRemoteConnection(string stationType)
        {
            return null;
        }
    }
}
