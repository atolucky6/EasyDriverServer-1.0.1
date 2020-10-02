
using EasyDriver.ServiceContainer;

namespace EasyDriver.ServicePlugin
{
    public interface IEasyServicePlugin
    {
        IServiceContainer ServiceContainer { get; }
        bool IsUnique { get; }
        void BeginInit();
        void EndInit();
    }
}
