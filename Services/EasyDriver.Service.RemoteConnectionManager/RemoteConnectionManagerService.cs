using EasyDriver.RemoteConnectionPlugin;
using EasyDriver.Service.RemoteConnectionManager;
using EasyDriver.ServicePlugin;
using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EasyRemoteConnection.Service.RemoteConnectionManager
{
    [Service(int.MaxValue, true)]
    public class RemoteConnectionManagerService : EasyServicePluginBase, IRemoteConnectionManagerService
    {
        public RemoteConnectionManagerService() : base()
        {
            RemoteConnectionPool = new Dictionary<IStationCore, IEasyRemoteConnectionPlugin>();
            availableRemoteConnections = new List<string>();

            string asmPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            RemoteConnectionDirectory = Path.GetFullPath(Path.Combine(asmPath, @"..\")) + "\\RemoteConnections\\";
            if (Directory.Exists(RemoteConnectionDirectory))
            {
                foreach (var item in Directory.GetFiles(RemoteConnectionDirectory))
                {
                    string remoteConnectionName = Path.GetFileNameWithoutExtension(item);
                    if (remoteConnectionName != "EasyDriver.RemoteConnectionPlugin")
                        availableRemoteConnections.Add(remoteConnectionName);
                }
            }
        }

        public string RemoteConnectionDirectory { get; set; }
        readonly List<string> availableRemoteConnections;
        public IEnumerable<string> AvailableRemoteConnections { get => availableRemoteConnections; }
        public Dictionary<IStationCore, IEasyRemoteConnectionPlugin> RemoteConnectionPool { get; private set; }

        public IEasyRemoteConnectionPlugin AddRemoteConnection(IStationCore stationCore, IEasyRemoteConnectionPlugin connection)
        {
            if (RemoteConnectionPool.ContainsKey(stationCore))
                return RemoteConnectionPool[stationCore];
            else
            {
                RemoteConnectionPool[stationCore] = connection;
                connection.WriteCommandExecuted += OnWriteCommandExecuted;
            }
            return connection;
        }

        public IEasyRemoteConnectionPlugin CreateRemoteConnection(string connectionPath)
        {
            IEasyRemoteConnectionPlugin driver = LoadAndCreateInstance<IEasyRemoteConnectionPlugin>(connectionPath);
            return driver;
        }

        public IEasyRemoteConnectionPlugin AddRemoteConnection(IStationCore stationCore, string connectionPath)
        {
            if (GetRemoteConnection(stationCore) == null)
            {
                IEasyRemoteConnectionPlugin driver = LoadAndCreateInstance<IEasyRemoteConnectionPlugin>(connectionPath);
                if (driver != null)
                {
                    return AddRemoteConnection(stationCore, driver);
                }
            }
            return null;
        }

        public IEasyRemoteConnectionPlugin GetRemoteConnection(ICoreItem item)
        {
            IStationCore channel = GetStationParent(item);
            if (RemoteConnectionPool.ContainsKey(channel))
                return RemoteConnectionPool[channel];
            return null;
        }

        public async void RemoveRemoteConnection(IStationCore stationCore)
        {
            await Task.Run(() =>
            {
                try
                {
                    IEasyRemoteConnectionPlugin connection = GetRemoteConnection(stationCore);
                    if (connection != null)
                    {
                        RemoteConnectionPool.Remove(stationCore);
                        connection.WriteCommandExecuted -= OnWriteCommandExecuted;
                        connection.Stop();
                        connection.Dispose();
                    }
                }
                catch { }
            });
        }

        private IStationCore GetStationParent(ICoreItem item)
        {
            if (item == null)
                return null;
            if (item is IStationCore station)
                return station;
            return GetStationParent(item.Parent);
        }

        public event EventHandler<CommandExecutedEventArgs> WriteCommandExecuted;

        private void OnWriteCommandExecuted(object sender, CommandExecutedEventArgs e)
        {
            WriteCommandExecuted?.Invoke(sender, e);
        }

        /// <summary>
        /// Load assembly and create instance with non argrument
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        private T LoadAndCreateInstance<T>(string connectionName)
            where T : class
        {
            try
            {
                string driverPath = "";
                if (connectionName.EndsWith(".dll"))
                {
                    driverPath = connectionName;
                    if (!File.Exists(driverPath))
                        return null;
                }
                else
                {
                    driverPath = RemoteConnectionDirectory + connectionName + ".dll";
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
