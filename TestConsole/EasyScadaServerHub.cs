using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace TestConsole
{
    public class EasyScadaServerHub : Hub<IEasyScadaAPI>, IEasyScadaAPI
    {
        public string GetTagFile()
        {
            return "this is tag file";
        }

        public override Task OnConnected()
        {
            Console.WriteLine($"Client connected with id: {Context.ConnectionId}");
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Console.WriteLine($"Client disconnected with id: {Context.ConnectionId}");
            return base.OnDisconnected(stopCalled);
        }
    }
}
