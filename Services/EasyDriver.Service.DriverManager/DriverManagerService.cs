using EasyDriverPlugin;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using EasyDriver.ServicePlugin;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace EasyDriver.Service.DriverManager
{
    [Service(int.MaxValue, true)]
    public class DriverManagerService : EasyServicePluginBase, IDriverManagerService
    {
        public DriverManagerService() : base()
        {
            DriverPoll = new Dictionary<IChannelCore, IEasyDriverPlugin>();
            availableDriver = new List<string>();
            string asmPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            DriverDirectory = Path.GetFullPath(Path.Combine(asmPath, @"..\")) + "\\Drivers\\";
            if (Directory.Exists(DriverDirectory))
            {
                foreach (var item in Directory.GetFiles(DriverDirectory))
                {
                    string driverName = Path.GetFileNameWithoutExtension(item);
                    if (driverName != "EasyDriverPlugin")
                        availableDriver.Add(driverName);
                }
            }
        }

        public string DriverDirectory { get; set; }
        public Dictionary<IChannelCore, IEasyDriverPlugin> DriverPoll { get; private set; }
        public IEnumerable<string> AvailableDrivers { get => availableDriver; }
        List<string> availableDriver;

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
            IEasyDriverPlugin driver = LoadAndCreateInstance<IEasyDriverPlugin>(driverPath);
            return driver;
        }

        public IEasyDriverPlugin AddDriver(IChannelCore channel, string driverPath)
        {
            if (GetDriver(channel) == null)
            {
                IEasyDriverPlugin driver = LoadAndCreateInstance<IEasyDriverPlugin>(driverPath);
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

        /// <summary>
        /// Load assembly and create instance with non argrument
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="driverName"></param>
        /// <returns></returns>
        private T LoadAndCreateInstance<T>(string driverName)
            where T : class
        {
            try
            {
                string driverPath = "";
                if (driverName.EndsWith(".dll"))
                {
                    driverPath = driverName;
                    if (!File.Exists(driverPath))
                        return null;
                }
                else
                {
                    driverPath = DriverDirectory + driverName + ".dll";
                }

                if (driverPath.EndsWith(".dll") && File.Exists(driverPath))
                {
                    Type interfaceType = typeof(T);
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    string fullPath = Path.GetFullPath(driverPath);
                    Assembly loadedAssembly = null;
                    foreach (var assembly in assemblies)
                    {
                        try
                        {
                            if (!assembly.IsDynamic)
                            {
                                if (assembly.Location == fullPath)
                                {
                                    loadedAssembly = assembly;
                                    break;
                                }
                            }
                        }
                        catch { }
                    }
                    if (loadedAssembly == null)
                        loadedAssembly = Assembly.LoadFile(fullPath);
                    Type instanceType = loadedAssembly.GetTypes().FirstOrDefault(t => interfaceType.IsAssignableFrom(t) && t.IsClass);
                    return (T)Activator.CreateInstance(instanceType);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return null;
        }
    }
}
