using EasyDriver.Opc.Client.Da;
using EasyDriverPlugin;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EasyScada.ServerApplication
{
    public interface IRemoteConnectionManagerService
    {
        Dictionary<IStationCore, IRemoteConnection> ConnectionDictonary { get; }
        void AddConnection(IStationCore stationCore, params object[] parameters);
        void RemoveConnection(IStationCore stationCore);
        void ReloadConnection(IStationCore stationCore);
    }

    public class RemoteConnectionManagerService : IRemoteConnectionManagerService
    {
        public RemoteConnectionManagerService()
        {
            ConnectionDictonary = new Dictionary<IStationCore, IRemoteConnection>();
        }

        public Dictionary<IStationCore, IRemoteConnection> ConnectionDictonary { get; private set; }

        public async void AddConnection(IStationCore stationCore, params object[] parameters)
        {
            await Task.Run(() =>
            {
                if (!ConnectionDictonary.ContainsKey(stationCore) && stationCore != null)
                {
                    Thread.Sleep(100);
                    switch (stationCore.StationType)
                    {
                        case "OPC_DA":
                            ConnectionDictonary[stationCore] = new OpcDaRemoteStationConnection(stationCore, parameters[0] as OpcDaServer);
                            break;
                        case "RemoteStation":
                            ConnectionDictonary[stationCore] = new RemoteStationConnection(stationCore, parameters[0] as HubConnection, parameters[1] as IHubProxy);
                            break;
                        default:
                            break;
                    }
                }
            });
        }

        public void ReloadConnection(IStationCore stationCore)
        {
            if (ConnectionDictonary.ContainsKey(stationCore))
            {
                var connection = ConnectionDictonary[stationCore];
                connection.Dispose();
                ConnectionDictonary.Remove(stationCore);
            }

            AddConnection(stationCore);
        }

        public void RemoveConnection(IStationCore stationCore)
        {
            if (ConnectionDictonary.ContainsKey(stationCore))
            {
                var connection = ConnectionDictonary[stationCore];
                connection.Dispose();
                ConnectionDictonary.Remove(stationCore);
            }
        }
    }
}
