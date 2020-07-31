using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Owin;
using System.Net;

namespace EasyDriverServer
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
            GlobalHost.Configuration.DefaultMessageBufferSize = 50;
            GlobalHost.Configuration.MaxIncomingWebSocketMessageSize = 20 * 1024;
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
        }
    }
}
