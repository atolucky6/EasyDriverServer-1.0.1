using EasyDriverPlugin;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using EasyDriver.Core;
using System;

namespace EasyScada.ServerApplication
{
    public interface IDriverManagerService
    {
        Dictionary<IChannelCore, IEasyDriverPlugin> DriverPoll { get; }
        IEasyDriverPlugin GetDriver(ICoreItem item);
        IEasyDriverPlugin AddDriver(IChannelCore channel, IEasyDriverPlugin driver);
        IEasyDriverPlugin AddDriver(IChannelCore channel, string driverPath);
        IEasyDriverPlugin CreateDriver(string driverPath);
        void RemoveDriver(IChannelCore channel);
        event EventHandler<CommandExecutedEventArgs> WriteCommandExecuted;
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
            {
                DriverPoll[channel] = driver;
                driver.WriteQueue.CommandExecuted += OnDriverCommandExecuted;
            }
            return driver;
        }

        public IEasyDriverPlugin CreateDriver(string driverPath)
        {
            IEasyDriverPlugin driver = AssemblyHelper.LoadAndCreateInstance<IEasyDriverPlugin>(driverPath);
            return driver;
        }

        public IEasyDriverPlugin AddDriver(IChannelCore channel, string driverPath)
        {
            if (GetDriver(channel) == null)
            {
                IEasyDriverPlugin driver = AssemblyHelper.LoadAndCreateInstance<IEasyDriverPlugin>(driverPath);
                if (driver != null)
                {
                    return AddDriver(channel, driver);
                }
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

        public async void RemoveDriver(IChannelCore channel)
        {
            await Task.Run(() =>
            {
                try
                {
                    IEasyDriverPlugin driver = GetDriver(channel);
                    if (driver != null)
                    {
                        DriverPoll.Remove(channel);
                        driver.WriteQueue.CommandExecuted -= OnDriverCommandExecuted;
                        driver.Stop();
                        driver.Dispose();
                    }
                }
                catch { }
            });
        }

        private IChannelCore GetChannel(ICoreItem item)
        {
            if (item == null)
                return null;
            if (item is IChannelCore channel)
                return channel;
            return GetChannel(item.Parent);
        }


        private void OnDriverCommandExecuted(object sender, CommandExecutedEventArgs e)
        {
            WriteCommandExecuted?.Invoke(sender, e);
        }

        public event EventHandler<CommandExecutedEventArgs> WriteCommandExecuted;
    }
}
