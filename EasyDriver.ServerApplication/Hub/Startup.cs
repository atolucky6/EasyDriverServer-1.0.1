using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Owin;
using System.Net;

namespace AHDScadaServer
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            HubConfiguration configuration = new HubConfiguration()
            {
                EnableDetailedErrors = false,
                EnableJavaScriptProxies = true,
            };
            app.MapSignalR("/easyScada", configuration);
            GlobalHost.Configuration.DefaultMessageBufferSize = 30;
            GlobalHost.Configuration.MaxIncomingWebSocketMessageSize = null;
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
        }
    }
}
