using EasyDriver.ServicePlugin;
using Microsoft.Extensions.DependencyInjection;

namespace EasyDriverServer.ModuleInjection
{
    public class ServiceModule : ModuleBase<IEasyServicePlugin>
    {
        public ServiceModule(string moduleDirectory, ServiceCollection services) : base(moduleDirectory, services)
        {

        }
    }
}
