namespace EasyDriver.ServicePlugin
{
    /// <summary>
    /// Đối tượng cơ bản của một dịch vụ
    /// </summary>
    public abstract class EasyServicePluginBase : IEasyServicePlugin
    {
        public EasyServicePluginBase()
        {
        }

        public ServiceContainer ServiceContainer { get => EasyDriver.ServicePlugin.ServiceContainer.Default; }
        public virtual void BeginInit() { }
        public virtual void EndInit() { }
    }
}
