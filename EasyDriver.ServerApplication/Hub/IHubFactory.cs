using EasyDriverPlugin;
using Microsoft.AspNet.SignalR.Client;

namespace EasyScada.ServerApplication
{
    public interface IHubFactory
    {
        string HubName { get; }
        HubConnection CreateHubConnection(IStationCore station);
        HubConnection CreateHubConnection(string remoteAddress, ushort port);
        IHubProxy CreateHubProxy(HubConnection hub);
    }

    public class HubFactory : IHubFactory
    {
        public HubFactory(string hubName)
        {
            HubName = hubName;
        }

        public string HubName { get; private set; }

        public HubConnection CreateHubConnection(IStationCore station)
        {
            return CreateHubConnection(station.RemoteAddress, station.Port);
        }

        public HubConnection CreateHubConnection(string remoteAddress, ushort port)
        {
            if (remoteAddress.IsIpAddress())
                return new HubConnection($"http://{remoteAddress}:{port}/easyScada");
            else
                return new HubConnection($"http://{remoteAddress}/easyScada");
        }

        public IHubProxy CreateHubProxy(HubConnection hub)
        {
            return hub.CreateHubProxy(HubName);
        }
    }
}
