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
                    object param1 = null;
                    object param2 = null;

                    if (parameters != null)
                    {
                        if (parameters.Length > 0)
                            param1 = parameters[0];

                        if (parameters.Length > 1)
                            param2 = parameters[1];
                    }

                    Thread.Sleep(100);
                    switch (stationCore.StationType)
                    {
                        case "OPC_DA":
                            ConnectionDictonary[stationCore] = new OpcDaRemoteStationConnection(stationCore, param1 as OpcDaServer);
                            break;
                        case "Remote":
                            ConnectionDictonary[stationCore] = new RemoteStationConnection(stationCore, param1 as HubConnection, param2 as IHubProxy);
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
