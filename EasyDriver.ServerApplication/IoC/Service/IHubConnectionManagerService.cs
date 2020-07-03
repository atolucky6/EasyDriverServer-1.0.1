using EasyDriverPlugin;
using EasyDriver.Client.Models;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.AspNet.SignalR.Client.Transports;

namespace EasyScada.ServerApplication
{
    public interface IHubConnectionManagerService
    {
        Dictionary<IStationCore, RemoteStationConnection> ConnectionDictonary { get; }
        void AddConnection(IStationCore stationCore, HubConnection hubConnection = null, IHubProxy hubPorxy = null);
        void RemoveConnection(IStationCore stationCore);
        void ReloadConnection(IStationCore stationCore);
    }

    public class HubConnectionManagerService : IHubConnectionManagerService
    {
        public HubConnectionManagerService()
        {
            ConnectionDictonary = new Dictionary<IStationCore, RemoteStationConnection>();
        }

        public Dictionary<IStationCore, RemoteStationConnection> ConnectionDictonary { get; private set; }

        public async void AddConnection(IStationCore stationCore, HubConnection hubConnection = null, IHubProxy hubPorxy = null)
        {
            await Task.Run(() =>
            {
                if (!ConnectionDictonary.ContainsKey(stationCore) && stationCore != null)
                {
                    ConnectionDictonary[stationCore] = new RemoteStationConnection(stationCore, hubConnection, hubPorxy);
                }
            });
        }

        public Task NotifySubscribeDataChanged(IStationCore stationCore)
        {
            return Task.Run(() =>
            {
                if (ConnectionDictonary.ContainsKey(stationCore))
                    ConnectionDictonary[stationCore].IsSubscribed = true;
            });
        }

        public void ReloadConnection(IStationCore stationCore)
        {
            if (ConnectionDictonary.ContainsKey(stationCore))
            {
                var connection = ConnectionDictonary[stationCore];
                connection.Dispose();
                ConnectionDictonary.Remove(stationCore);
            }

            AddConnection(stationCore);
        }

        public void RemoveConnection(IStationCore stationCore)
        {
            if (ConnectionDictonary.ContainsKey(stationCore))
            {
                var connection = ConnectionDictonary[stationCore];
                connection.Dispose();
                ConnectionDictonary.Remove(stationCore);
            }
        }
    }

    public class RemoteStationConnection : IDisposable
    {
        #region Members

        public IStationCore StationCore { get; set; }
        public string RemoteAddress { get; private set; }
        public ushort Port { get; private set; }
        public bool IsDisposed { get; private set; }
        public bool IsSubscribed { get; set; }

        private Task requestTask;
        private HubConnection hubConnection;
        private IHubProxy hubProxy;

        readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        #endregion

        #region Constructors

        public RemoteStationConnection(IStationCore stationCore, HubConnection hubConnection = null, IHubProxy hubPorxy = null)
        {
            StationCore = stationCore;
            RemoteAddress = StationCore.RemoteAddress;
            Port = StationCore.Port;
            InitializeConnection(hubConnection, hubPorxy);
        }

        ~RemoteStationConnection()
        {
            Dispose();
        }

        #endregion

        #region Methods

        private async void InitializeConnection(HubConnection hubConnection = null, IHubProxy hubPorxy = null)
        {
            try
            {
                if (hubConnection != null && hubPorxy != null && hubConnection.State == ConnectionState.Connected)
                {
                    if (this.hubConnection == null)
                    {
                        this.hubConnection = hubConnection;
                        hubProxy = hubPorxy;
                        this.hubConnection.StateChanged += OnStateChanged;
                        this.hubConnection.Closed += OnDisconnected;
                        this.hubConnection.ConnectionSlow += OnConnectionSlow;
                      
                        hubProxy.On("broadcastSubscribeData", (Action<string>)this.OnReceivedBroadcastMessage);
                        OnStateChanged(new StateChange(ConnectionState.Connecting, this.hubConnection.State));
                    }
                }
                else
                {
                    if (StationCore != null && StationCore.Parent.Contains(StationCore) && !IsDisposed)
                    {
                        if (this.hubConnection != null)
                        {
                            try
                            {
                                this.hubConnection.TransportConnectTimeout = TimeSpan.FromSeconds(1);
                                this.hubConnection.StateChanged -= OnStateChanged;
                                this.hubConnection.Closed -= OnDisconnected;
                                this.hubConnection.ConnectionSlow -= OnConnectionSlow;
                                if (this.hubConnection.State == ConnectionState.Connected)
                                    this.hubConnection.Stop();
                                this.hubConnection?.Dispose();
                                this.hubConnection = null;
                                this.hubProxy = null;
                            }
                            catch { }
                        }
                        this.hubConnection = new HubConnection($"http://{RemoteAddress}:{Port}/easyScada");
                        this.hubConnection.StateChanged += OnStateChanged;
                        this.hubConnection.Closed += OnDisconnected;
                        this.hubConnection.ConnectionSlow += OnConnectionSlow;
                        hubProxy = this.hubConnection.CreateHubProxy("EasyDriverServerHub");
                        hubProxy.On<string>("broadcastSubscribeData", OnReceivedBroadcastMessage);
                        await this.hubConnection.Start(new LongPollingTransport());
                    }
                } 
            }
            catch { }
        }

        /// <summary>
        /// Method to subscribe this connection to server
        /// </summary>
        /// <returns></returns>
        private bool Subscribe()
        {
            string subscribeDataJson = JsonConvert.SerializeObject(StationCore.Childs.Select(x => x as IStation));
            List<Station> subscribeData = JsonConvert.DeserializeObject<List<Station>>(subscribeDataJson);
            if (subscribeData != null)
            {
                foreach (var station in subscribeData)
                    RemoveFirstStationPath(station, StationCore.Name.Length + 1);
                subscribeDataJson = JsonConvert.SerializeObject(subscribeData);
                if (hubConnection != null && hubConnection.State == ConnectionState.Connected)
                {
                    var subTask = hubProxy.Invoke<string>("subscribe", subscribeDataJson, StationCore.CommunicationMode.ToString(), StationCore.RefreshRate);
                    Task.WaitAll(subTask);
                    if (subTask.IsCompleted)
                        return subTask.Result == "Ok";
                }
            }
            return false;
        }

        public void Dispose()
        {
            IsDisposed = true;
            Task.Factory.StartNew(async () =>
            {
                await semaphore.WaitAsync();
                hubConnection?.Dispose();
                requestTask?.Dispose();
                semaphore.Release();
                semaphore.Dispose();
            });
        }

        #endregion

        #region Connection lifecycle handlers

        private void OnConnectionSlow()
        {

        }


        /// <summary>
        /// The method handler when hub connection is closed
        /// </summary>
        private void OnDisconnected()
        {
            if (!IsDisposed)
            {
                // Delay a little bit before reconnect
                Thread.Sleep(5000);
                // When connection disconnected we will init the new connection and tring to reconnect
                InitializeConnection();
            }
        }

        /// <summary>
        /// The method handler when state of hub connection is changed
        /// </summary>
        /// <param name="stateChanged">A change state</param>
        public void OnStateChanged(StateChange stateChanged)
        {
            StationCore.ConnectionStatus = (ConnectionStatus)Enum.Parse(typeof(ConnectionStatus), stateChanged.NewState.ToString());
            if (stateChanged.NewState == ConnectionState.Connected && !IsDisposed)
            {
                // Delay a little bit before subscribe to server
                Thread.Sleep(1000);
                IsSubscribed = true;
                // Initialize the request task if it null
                if (requestTask == null)
                    requestTask = Task.Factory.StartNew(RefreshData, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
        }

        #endregion

        #region Request methods

        /// <summary>
        /// The method callback of refresh timer
        /// </summary>
        /// <param name="state"></param>
        private void RefreshData()
        {
            while (!IsDisposed)
            {
                int delay = 100;
                semaphore.Wait();
                try
                {
                    // Re subscribe if needed
                    if (IsSubscribed && hubConnection != null && hubConnection.State == ConnectionState.Connected)
                    {
                        bool resSub = Subscribe();
                        if (resSub)
                            IsSubscribed = false;
                        delay = 100;
                    }

                    // Check requies condition to start get data from server
                    if (StationCore != null && !IsDisposed && StationCore.Parent.Contains(StationCore))
                    {
                        if (StationCore.CommunicationMode == CommunicationMode.RequestToServer &&
                            StationCore.Childs != null && StationCore.Childs.Count > 0 &&
                            hubConnection.State == ConnectionState.Connected)
                        {
                            try
                            {
                                // Get subscribed data from server
                                var getDataTask = hubProxy.Invoke<string>("getSubscribedDataAsync");
                                Task.WaitAll(getDataTask);
                                if (getDataTask.IsCompleted)
                                {
                                    string resJson = getDataTask.Result;
                                    if (!string.IsNullOrWhiteSpace(resJson))
                                    {
                                        List<Station> resStations = JsonConvert.DeserializeObject<List<Station>>(resJson);
                                        // Update current station with subscribed data we just get
                                        foreach (var item in StationCore.Childs)
                                        {
                                            if (item is IStationCore)
                                            {
                                                Station sourceStation = resStations?.FirstOrDefault(x => x.Path == (item as ICoreItem).Path.Remove(0, StationCore.Name.Length + 1));
                                                UpdateStationCore(sourceStation, item as IStationCore, $"{StationCore.Name}/"); // Method to update 
                                            }
                                        }
                                    }
                                }
                            }
                            catch { }
                            delay = StationCore.RefreshRate;

                        }
                        else { delay = 100; }
                    }
                }

                catch { }
                finally
                {
                    semaphore.Release();
                    Thread.Sleep(delay);
                }
            }
        }

        #endregion

        #region On message handlers

        /// <summary>
        /// The handler when the connection get broadcast message from server
        /// </summary>
        /// <param name="stationsJson"></param>
        private async void OnReceivedBroadcastMessage(string stationsJson)
        {
            await Task.Run(async () =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Restart();
                await semaphore.WaitAsync();
                try
                {
                    // Check requires conditions 
                    if (StationCore != null &&
                        StationCore.Parent.Contains(StationCore) &&
                        !IsDisposed &&
                        StationCore.CommunicationMode == CommunicationMode.ReceiveFromServer)
                    {
                        List<IStationCore> stationCores = StationCore.Childs.Select(x => x as IStationCore).ToList();
                        List<Station> stations = JsonConvert.DeserializeObject<List<Station>>(stationsJson);

                        //byte[] byteBuffer = Convert.FromBase64String(stationsJson);
                        //List<Station> stations;
                        //using (MemoryStream ms = new MemoryStream(byteBuffer))
                        //{
                        //    var formatter = new BinaryFormatter();
                        //    stations = (List<Station>)formatter.Deserialize(ms);
                        //}

                        if (stations != null && stations.Count > 0)
                        {
                            foreach (var station in stations)
                            {
                                if (stationCores.FirstOrDefault(x => x.Name == station.Name) is IStationCore innerStation)
                                {
                                    UpdateStationCore(station, innerStation, $"{StationCore.Path}/");
                                    stationCores.Remove(innerStation);
                                }
                            }
                        }
                        else
                        {
                            stationCores.ForEach(x => x.CommunicationError = "This station could not be found on server.");
                        }

                        StationCore.LastRefreshTime = DateTime.Now;
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    semaphore.Release();
                    sw.Stop();
                    Debug.WriteLine($"Handle broadcast message: {sw.ElapsedMilliseconds}");
                }
            });
        }

        #endregion

        #region Utils

        private void UpdateStationCore(Station station, IStationCore stationCore, string startPath)
        {
            if (station != null && stationCore != null)
            {
                stationCore.RefreshRate = station.RefreshRate;
                stationCore.LastRefreshTime = station.LastRefreshTime;
                stationCore.RemoteAddress = station.RemoteAddress;
                stationCore.Port = station.Port;
                stationCore.CommunicationMode = station.CommunicationMode;
                stationCore.CommunicationError = station.Error;
                stationCore.RefreshRate = station.RefreshRate;
                stationCore.ParameterContainer.Parameters = station.Parameters;

                List<IChannelCore> channelCores = new List<IChannelCore>();
                List<IStationCore> stationCores = new List<IStationCore>();

                foreach (var item in stationCore.Childs)
                {
                    if (item is IChannelCore)
                        channelCores.Add(item as IChannelCore);
                    else if (item is IStationCore)
                        stationCores.Add(item as IStationCore);
                }

                if (channelCores != null && channelCores.Count > 0 && station.Channels != null)
                {
                    foreach (var channel in station.Channels)
                    {
                        if (stationCore.FirstOrDefault(x => x.Path.Remove(0, startPath.Length) == channel.Path) is IChannelCore channelCore)
                        {
                            UpdateChannelCore(channel, channelCore, startPath);
                            channelCores.Remove(channelCore);
                        }
                    }
                }
                channelCores.ForEach(x => x.CommunicationError = "This channel could not be found on server.");

                if (stationCores != null && stationCores.Count > 0 && station.RemoteStations != null)
                {
                    foreach (var remoteStation in station.RemoteStations)
                    {
                        if (stationCores.FirstOrDefault(x => x.Path.Remove(0, startPath.Length) == remoteStation.Path) is IStationCore updateStation)
                        {
                            UpdateStationCore(remoteStation, updateStation, startPath);
                            stationCores.Remove(updateStation);
                        }
                    }
                }
                stationCores.ForEach(x => x.CommunicationError = "This station could not be found on server.");
            }
        }

        private void UpdateChannelCore(Channel channel, IChannelCore channelCore, string startPath)
        {
            if (channel != null && channelCore != null)
            {
                channelCore.CommunicationError = channel.Error;
                channelCore.LastRefreshTime = channel.LastRefreshTime;
                channelCore.ParameterContainer.Parameters = channel.Parameters;

                List<IDeviceCore> deviceCores = channelCore.Childs.Select(x => x as IDeviceCore)?.ToList();
                if (deviceCores != null && deviceCores.Count > 0 && channel.Devices != null)
                {
                    foreach (var device in channel.Devices)
                    {
                        if (deviceCores.FirstOrDefault(x => x.Path.Remove(0, startPath.Length) == device.Path) is IDeviceCore deviceCore)
                        {
                            UpdateDeviceCore(device, deviceCore, startPath);
                            deviceCores.Remove(deviceCore);
                        }
                    }
                }

                deviceCores.ForEach(x => x.CommunicationError = "This device could not be found on server.");
            }
        }

        private void UpdateDeviceCore(Device device, IDeviceCore deviceCore, string startPath)
        {
            if (device != null && deviceCore != null)
            {
                deviceCore.LastRefreshTime = device.LastRefreshTime;
                deviceCore.CommunicationError = device.Error;
                deviceCore.LastRefreshTime = device.LastRefreshTime;
                deviceCore.ParameterContainer.Parameters = device.Parameters;

                List<ITagCore> tagCores = deviceCore.Childs.Select(x => x as ITagCore)?.ToList();
                if (tagCores != null && tagCores.Count > 0 && device.Tags != null)
                {
                    foreach (var tag in device.Tags)
                    {
                        if (tagCores.FirstOrDefault(x => x.Path.Remove(0, startPath.Length) == tag.Path) is ITagCore tagCore)
                        {
                            UpdateTagCore(tag, tagCore, startPath);
                            tagCores.Remove(tagCore);
                        }
                    }
                }

                tagCores.ForEach(x =>
                {
                    x.Quality = Quality.Bad;
                    x.CommunicationError = "This tag could not be found on server.";
                });
            }
        }

        private void UpdateTagCore(Tag tag, ITagCore tagCore, string startPath)
        {
            if (tag != null && tagCore != null)
            {
                tagCore.Value = tag.Value;
                tagCore.TimeStamp = tag.TimeStamp;
                tagCore.Quality = tag.Quality;
                tagCore.RefreshInterval = tag.RefreshInterval;
                tagCore.RefreshRate = tag.RefreshRate;
                tagCore.Address = tag.Address;
                tagCore.DataTypeName = tag.DataType;
                tagCore.CommunicationError = tag.Error;
                tagCore.ParameterContainer.Parameters = tag.Parameters;
                tagCore.AccessPermission = tag.AccessPermission;
            }
        }

        private void RemoveFirstStationPath(Station station, int length)
        {
            if (station != null)
            {
                station.Path = station.Path.Remove(0, length);
                if (station.RemoteStations != null)
                {
                    foreach (var remoteStation in station.RemoteStations)
                        RemoveFirstStationPath(remoteStation, length);
                }

                if (station.Channels != null)
                {
                    foreach (var channel in station.Channels)
                        RemoveFirstStationPath(channel, length);
                }
            }
        }

        private void RemoveFirstStationPath(Channel channel, int length)
        {
            if (channel != null)
            {
                channel.Path = channel.Path.Remove(0, length);
                if (channel.Devices != null)
                {
                    foreach (Device device in channel.Devices)
                    {
                        RemoveFirstStationPath(device, length);
                    }
                }
            }
        }

        private void RemoveFirstStationPath(Device device, int length)
        {
            if (device != null)
            {
                device.Path = device.Path.Remove(0, length);
                if (device.Tags != null)
                {
                    foreach (var tag in device.Tags)
                    {
                        RemoveFirstStationPath(tag, length);
                    }
                }
            }
        }

        private void RemoveFirstStationPath(Tag tag, int length)
        {
            if (tag != null)
                tag.Path = tag.Path.Remove(0, length);
        }

        #endregion
    }
}
