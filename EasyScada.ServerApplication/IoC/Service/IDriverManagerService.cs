using EasyDriverPlugin;
using EasyScada.Core;
using System.Collections.Generic;

namespace EasyScada.ServerApplication
{
    public interface IDriverManagerService
    {
        Dictionary<IChannelCore, IEasyDriverPlugin> DriverPoll { get; }
        IEasyDriverPlugin GetDriver(ICoreItem item);
        IEasyDriverPlugin AddDriver(IChannelCore channel, IEasyDriverPlugin driver);
        IEasyDriverPlugin AddDriver(IChannelCore channel, string driverPath);
        void RemoveDriver(IChannelCore channel);
    }

    public class DriverManagerService : IDriverManagerService
    {
        public DriverManagerService()
        {
            DriverPoll = new Dictionary<IChannelCore, IEasyDriverPlugin>();
        }

        public Dictionary<IChannelCore, IEasyDriverPlugin> DriverPoll { get; private set; }

        public IEasyDriverPlugin AddDriver(IChannelCore channel, IEasyDriverPlugin driver)
        {
            if (DriverPoll.ContainsKey(channel))
                return DriverPoll[channel];
            else
                DriverPoll[channel] = driver;
            return driver;
        }

        public IEasyDriverPlugin AddDriver(IChannelCore channel, string driverPath)
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
            IChannelCore channel = GetChannel(item);
            if (DriverPoll.ContainsKey(channel))
                return DriverPoll[channel];
            return null;
        }

        public void RemoveDriver(IChannelCore channel)
        {
            IEasyDriverPlugin driver = GetDriver(channel);
            if (driver != null)
            {
                DriverPoll.Remove(channel);
                driver.Dispose();
            }
        }

        private IChannelCore GetChannel(ICoreItem item)
        {
            if (item == null)
                return null;
            if (item is IChannelCore channel)
                return channel;
            return GetChannel(item.Parent);
        }
    }
}
