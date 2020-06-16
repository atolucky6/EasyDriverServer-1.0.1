using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Owin;

namespace EasyScada.ServerApplication
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR("/easyScada", new HubConfiguration());
        }
    }
}
