using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestClient
{
    class Program
    {
        static IHubProxy hubProxy;
        static HubConnection hubConnection;
        static bool isConnected;
        static void Main(string[] args)
        {
            hubConnection = new HubConnection("http://localhost:9090/easyScada/");
            hubConnection.StateChanged += HubConnection_StateChanged;
            hubProxy = hubConnection.CreateHubProxy("EasyScadaServerHub");

            hubConnection.Start(new LongPollingTransport());
            Thread.Sleep(1000);
            Start();
            Console.ReadLine();
        }

        private static void HubConnection_StateChanged(StateChange obj)
        {
            if (obj.NewState == ConnectionState.Connected)
            {
                Console.WriteLine("Connected");
                isConnected = true;            

            }
            else if (obj.NewState == ConnectionState.Disconnected)
            {
                Console.WriteLine("Disconnected");
            }
        }

        private static async void Start()
        {
            while (true)
            {
                if (isConnected)
                {
                    string tagFile = await hubProxy.Invoke<string>("GetTagFile");
                    Console.WriteLine(tagFile);

                  
                }
                Thread.Sleep(1000);
            }
        }
    }
}
