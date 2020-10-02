using EasyDriver.Core;
using EasyDriver.Opc.Client.Da;
using EasyDriverPlugin;
using Microsoft.AspNet.SignalR.Client;
using System.Collections.Generic;

namespace EasyScada.ServerApplication
{ 
    public class AuthenticateLicenseMessage
    {
        public int LimitTagCount { get; private set; }
        public bool IsAuthenticated { get; private set; }
        public string Message { get; private set; }
        public AuthenticateLicenseMessage(bool isAuthenticated, int limitTagCount, string message)
        {
            IsAuthenticated = isAuthenticated;
            LimitTagCount = limitTagCount;
            Message = message;
        }
    }

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

    public class CreateRemoteStationSuccessMessage
    {
        public HubModel HubModel { get; private set; }
        public HubConnection HubConnection { get; private set; }
        public IHubProxy HubProxy { get; private set; }

        public CreateRemoteStationSuccessMessage(HubModel hubModel, HubConnection hubConnection, IHubProxy hubProxy)
        {
            HubModel = hubModel;
            HubConnection = hubConnection;
            HubProxy = hubProxy;
        }
    }

    public class CreateRemoteOpcDaStationSuccessMessage
    {
        public OpcDaServer OpcServer { get; private set; }
        public IGroupItem Parent { get; private set; }

        public CreateRemoteOpcDaStationSuccessMessage(OpcDaServer opcServer, IGroupItem parent)
        {
            OpcServer = opcServer;
            Parent = parent;
        }
    }

    public class CreateConnectionSchemaSuccessMessage
    {
        public ConnectionSchema ConnectionSchema { get; private set; }
        public string SavePath { get; private set; }
        public CreateConnectionSchemaSuccessMessage(ConnectionSchema connectionSchema, string savePath)
        {
            ConnectionSchema = connectionSchema;
            SavePath = savePath;
        }
    }
}
