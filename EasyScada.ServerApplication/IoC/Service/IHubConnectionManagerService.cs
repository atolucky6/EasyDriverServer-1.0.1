using EasyDriverPlugin;
using Microsoft.AspNet.SignalR.Client;
using System.Collections.Generic;

namespace EasyScada.ServerApplication
{
    public interface IHubConnectionManagerService
    {
        Dictionary<IStationCore, HubConnection> HubConnectionDictionary { get; }
        Dictionary<HubConnection, IHubProxy> HubProxyDictionary { get; }
        HubConnection AddHubConnection(IStationCore station, HubConnection hubConnection, IHubProxy hubProxy);
        bool RemoveHubConnection(IStationCore station);
        HubConnection GetHubConnection(IStationCore station);
        IHubProxy GetHubProxy(HubConnection connection);
        IHubProxy GetHubProxy(IStationCore station);
    }

    public class HubConnectionManagerService : IHubConnectionManagerService
    {
        public Dictionary<IStationCore, HubConnection> HubConnectionDictionary { get; protected set; }

        public Dictionary<HubConnection, IHubProxy> HubProxyDictionary { get; protected set; }

        public HubConnectionManagerService()
        {
            HubConnectionDictionary = new Dictionary<IStationCore, HubConnection>();
            HubProxyDictionary = new Dictionary<HubConnection, IHubProxy>();
        }

        public HubConnection AddHubConnection(IStationCore station, HubConnection hubConnection, IHubProxy hubProxy)
        {
            if (HubConnectionDictionary.ContainsKey(station))
            {
                HubConnection oldHub = HubConnectionDictionary[station];
                IHubProxy oldProxy = HubProxyDictionary[oldHub];
                HubConnectionDictionary.Remove(station);
                oldHub.Dispose();
                oldHub.Dispose();
            }
            else
            {
                HubConnectionDictionary[station] = hubConnection;
                HubProxyDictionary[hubConnection] = hubProxy;
            }

            return hubConnection;
        }

        public bool RemoveHubConnection(IStationCore station)
        {
            throw new System.NotImplementedException();
        }

        public HubConnection GetHubConnection(IStationCore station)
        {
            throw new System.NotImplementedException();
        }

        public IHubProxy GetHubProxy(HubConnection connection)
        {
            throw new System.NotImplementedException();
        }

        public IHubProxy GetHubProxy(IStationCore station)
        {
            throw new System.NotImplementedException();
        }
    }
}
