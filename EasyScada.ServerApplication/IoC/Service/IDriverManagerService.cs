using EasyDriverPlugin;
using EasyScada.Core;
using System.Collections.Generic;

namespace EasyScada.ServerApplication
{
    public interface IDriverManagerService
    {
        Dictionary<IChannel, IEasyDriverPlugin> DriverPoll { get; }
        IEasyDriverPlugin GetDriver(ICoreItem item);
        IEasyDriverPlugin AddDriver(IChannel channel, IEasyDriverPlugin driver);
        IEasyDriverPlugin AddDriver(IChannel channel, string driverPath);
        void RemoveDriver(IChannel channel);
    }

    public class DriverManagerService : IDriverManagerService
    {
        public DriverManagerService()
        {
            DriverPoll = new Dictionary<IChannel, IEasyDriverPlugin>();
        }

        public Dictionary<IChannel, IEasyDriverPlugin> DriverPoll { get; private set; }

        public IEasyDriverPlugin AddDriver(IChannel channel, IEasyDriverPlugin driver)
        {
            if (DriverPoll.ContainsKey(channel))
                return DriverPoll[channel];
            else
                DriverPoll[channel] = driver;
            return driver;
        }

        public IEasyDriverPlugin AddDriver(IChannel channel, string driverPath)
        {
            if (GetDriver(channel) == null)
            {
                IEasyDriverPlugin driver = AssemblyHelper.LoadAndCreateInstance<IEasyDriverPlugin>(driverPath);
                if (driver != null)
                    driver.Channel = channel;
                return AddDriver(channel, driver);
            }
            return null;
        }

        public IEasyDriverPlugin GetDriver(ICoreItem item)
        {
            IChannel channel = GetChannel(item);
            if (DriverPoll.ContainsKey(channel))
                return DriverPoll[channel];
            return null;
        }

        public void RemoveDriver(IChannel channel)
        {
            IEasyDriverPlugin driver = GetDriver(channel);
            if (driver != null)
            {
                DriverPoll.Remove(channel);
                driver.Dispose();
            }
        }

        private IChannel GetChannel(ICoreItem item)
        {
            if (item == null)
                return null;
            if (item is IChannel channel)
                return channel;
            return GetChannel(item.Parent);
        }
    }
}
