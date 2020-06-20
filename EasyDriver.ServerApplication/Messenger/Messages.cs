using EasyDriver.Client.Models;
using Microsoft.AspNet.SignalR.Client;
using System.Collections.Generic;

namespace EasyScada.ServerApplication
{
    public class ShowPropertiesMessage
    {
        public object Sender { get; private set; }
        public object Item { get; private set; }
        public ShowPropertiesMessage(object sender, object item)

        {
            Sender = sender;
            Item = item;
        }
    }

    public class HidePropertiesMessage
    {
        public object Sender { get; private set; }

        public HidePropertiesMessage(object sender)
        {
            Sender = sender;
        }
    }

    public class CreateRemoteStationSuccess
    {
        public List<Station> SelectedStations { get; private set; }
        public HubConnection HubConnection { get; private set; }
        public IHubProxy HubProxy { get; private set; }

        public CreateRemoteStationSuccess(List<Station> selectedStations, HubConnection hubConnection, IHubProxy hubProxy)
        {
            SelectedStations = selectedStations;
            HubConnection = hubConnection;
            HubProxy = hubProxy;
        }
    }
}
