using System;

namespace EasyDriver.ServiceContainer
{
    public interface IServiceContainer
    {
        T Get<T>() where T : class;
        object Get(Type serviceType);
    }
}
