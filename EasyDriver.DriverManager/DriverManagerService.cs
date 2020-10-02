using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using EasyDriver.Core;
using EasyDriver.ServiceContainer;
using EasyDriver.ServicePlugin;
using EasyDriverPlugin;

namespace EasyDriver.DriverManager
{
    public class DriverManagerService : EasyServicePlugin, IDriverManagerService
    {
        public DriverManagerService(IServiceContainer serviceContainer) : base(serviceContainer)
        {
            _DriverPoll = new Dictionary<IChannelCore, IEasyDriverPlugin>();
            _Drivers = new ObservableCollection<IEasyDriverPlugin>();
            Drivers = new ReadOnlyObservableCollection<IEasyDriverPlugin>(_Drivers);
        }

        readonly ObservableCollection<IEasyDriverPlugin> _Drivers;
        public ReadOnlyObservableCollection<IEasyDriverPlugin> Drivers { get; private set; }

        readonly Dictionary<IChannelCore, IEasyDriverPlugin> _DriverPoll;
        public IReadOnlyDictionary<IChannelCore, IEasyDriverPlugin> DriverPoll { get => _DriverPoll; }

        public IEasyDriverPlugin AddDriver(IChannelCore channel, IEasyDriverPlugin driver)
        {
            if (DriverPoll.ContainsKey(channel))
                return DriverPoll[channel];
            else
            {
                _DriverPoll[channel] = driver;
                _Drivers.Add(driver);
            }
            return driver;
        }

        public IEasyDriverPlugin AddDriver(IChannelCore channel, string driverPath)
        {
            if (GetDriver(channel) == null)
            {
                IEasyDriverPlugin driver = AssemblyHelper.LoadAndCreateInstance<IEasyDriverPlugin>(driverPath);
                if (driver != null)
                {
                    driver.Channel = channel;
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
                IEasyDriverPlugin driver = GetDriver(channel);
                if (driver != null)
                { 
                    _DriverPoll.Remove(channel);
                    _Drivers.Remove(driver);
                    driver.Dispose();
                }
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

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
        }
    }
}
