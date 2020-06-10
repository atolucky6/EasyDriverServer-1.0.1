using Microsoft.AspNet.SignalR;

namespace EasyScada.ServerAPI
{
    public class EasyScadaServerHub : Hub<IEasyScadaAPI>, IEasyScadaAPI
    {
        public string GetTagFile()
        {
            return "this is tag file";
        }
    }
}
