using EasyDriver.ServiceContainer;
using EasyDriver.ServicePlugin;
using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDriver.RemoteConnectionManager
{
    public class RemoteConnectionManagerService : EasyServicePlugin, IRemoteConnectionManagerService
    {
        public RemoteConnectionManagerService(IServiceContainer serviceContainer) : 
            base(serviceContainer)
        {
            _RemoteConnectionDictionary = new Dictionary<IStationCore, IRemoteConnection>();
        }

        public List<Func<string, IRemoteConnection>> CreateRemoteConnectionFuncs { get; set; }

        Dictionary<IStationCore, IRemoteConnection> _RemoteConnectionDictionary;
        public IReadOnlyDictionary<IStationCore, IRemoteConnection> RemoteConnectionDictionary => _RemoteConnectionDictionary;

        public void AddConnection(IStationCore station, IRemoteConnection connection)
        {
            if (GetConnection(station) == null)
                _RemoteConnectionDictionary.Add(station, connection);
        }

        public void AddConnection(IStationCore station)
        {
            if (GetConnection(station) == null)
            {
                var connection = CreateConnection(station);
                if (connection != null)
                    _RemoteConnectionDictionary[station] = connection;
            }
        }

        public void ReloadConnection(IStationCore station)
        {
            GetConnection(station)?.Reload();
        }

        public bool RemoveConnection(IStationCore station)
        {
            if (_RemoteConnectionDictionary.ContainsKey(station))
            {
                var connection = _RemoteConnectionDictionary[station];
                connection.Dispose();
            }
            return _RemoteConnectionDictionary.Remove(station);
        }

        public IRemoteConnection GetConnection(IStationCore station)
        {
            if (_RemoteConnectionDictionary.ContainsKey(station))
                return _RemoteConnectionDictionary[station];
            return null;
        }

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
        }

        public IRemoteConnection CreateConnection(IStationCore station)
        {
            if (CreateRemoteConnectionFuncs.Count > 0)
            {
                foreach (var func in CreateRemoteConnectionFuncs)
                {
                    if (func != null)
                    {
                        var connection = func.Invoke(station.StationType);
                        if (connection != null)
                        {
                            return connection;
                        }
                    }
                }
            }
            return null;
        }
    }
}
