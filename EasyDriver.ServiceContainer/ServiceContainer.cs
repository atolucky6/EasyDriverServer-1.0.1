using System;

namespace EasyDriver.ServiceContainer
{
    public class ServiceContainer : IServiceContainer
    {
        public ServiceContainer(Func<Type, object> getServiceFunc)
        {
            this.getServiceFunc = getServiceFunc;
        }

        private Func<Type, object> getServiceFunc;

        public T Get<T>() where T : class
        {
            Type type = typeof(T);
            return Get(type) as T;
        }

        public object Get(Type serviceType)
        {
            return getServiceFunc?.Invoke(serviceType);
        }
    }
}
