using EasyDriver.ServiceContainer;
using System;

namespace EasyDriver.ServicePlugin
{
    public abstract class EasyServicePlugin : IEasyServicePlugin
    {
        public EasyServicePlugin(IServiceContainer serviceContainer)
        {
            ServiceContainer = serviceContainer ?? throw new ArgumentNullException("ServiceContainer", "ServiceContainer can't be null");
            IsUnique = true;
        }

        public bool IsUnique { get; protected set; }

        public virtual IServiceContainer ServiceContainer { get; protected set; }

        public abstract void BeginInit();

        public abstract void EndInit();
    }
}
