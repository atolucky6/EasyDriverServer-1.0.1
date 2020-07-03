using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Owin;
using System.Net;

namespace EasyScada.ServerApplication
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR("/easyScada", new HubConfiguration());
            GlobalHost.Configuration.DefaultMessageBufferSize = 100 * 1024;
            GlobalHost.Configuration.MaxIncomingWebSocketMessageSize = 100 * 1024;
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
        }
    }
}
