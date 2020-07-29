using EasyDriverPlugin;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.AspNet.SignalR.Client.Transports;
using System.IO;
using System.Collections;
using EasyDriver.Core;

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
                    Thread.Sleep(100);
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
        public HubConnection hubConnection;
        public IHubProxy hubProxy;

        private Task requestTask;
        readonly Hashtable cache;
        readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        #endregion

        #region Constructors

        public RemoteStationConnection(IStationCore stationCore, HubConnection hubConnection = null, IHubProxy hubPorxy = null)
        {
            StationCore = stationCore;
            RemoteAddress = StationCore.RemoteAddress;
            Port = StationCore.Port;
            cache = new Hashtable();
            InitializeConnection(hubConnection, hubPorxy);
        }

        ~RemoteStationConnection()
        {
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
                                this.hubConnection.StateChanged -= OnStateChanged;
                                this.hubConnection.Closed -= OnDisconnected;
                                this.hubConnection.ConnectionSlow -= OnConnectionSlow;
                                if (this.hubConnection.State == ConnectionState.Connected)
                                    this.hubConnection.Stop();
                                this.hubConnection?.Dispose();
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
            string subscribeDataJson = JsonConvert.SerializeObject(StationCore.Childs.Select(x => x as IStationClient));
            List<StationClient> subscribeData = JsonConvert.DeserializeObject<List<StationClient>>(subscribeDataJson);
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
        }

        public async Task<WriteResponse> WriteTagValue(WriteCommand writeCommand)
        {
            WriteResponse response = new WriteResponse();
            response.WriteCommand = writeCommand;
            if (hubConnection != null && hubConnection.State == ConnectionState.Connected)
            {
                try
                {
                    await semaphore.WaitAsync();
                    response.SendTime = DateTime.Now;
                    response = await hubProxy.Invoke<WriteResponse>("writeTagValueAsync", writeCommand);
                    return response;
                }
                catch (Exception ex)
                {
                    response.Error = "Some error occur when send write command to remote station.";
                    return response;
                }
                finally { semaphore.Release(); }
            }
            else
            {
                response.Error = "The connection state of remote station was disconnected";
            }
            return response;
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
                IsSubscribed = false;
                // Initialize the request task if it null
                if (requestTask == null)
                    requestTask = Task.Factory.StartNew(RefreshData, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
            else if (stateChanged.NewState == ConnectionState.Disconnected && !IsDisposed)
            {
                if (StationCore != null)
                {
                    foreach (var item in StationCore.Find(x => x is ITagCore, true))
                    {
                        if (item is ITagCore tagCore)
                            tagCore.Quality = Quality.Bad;
                    }
                }
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
                    if (!IsSubscribed && hubConnection != null && hubConnection.State == ConnectionState.Connected)
                    {
                        bool resSub = Subscribe();
                        if (resSub)
                        {
                            IsSubscribed = true;
                            foreach (var item in GetAllChildItems(StationCore))
                            {
                                cache.Add(item.Path.Remove(0, StationCore.Name.Length + 1), item);
                            }
                        }
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

                                    using (StringReader sr = new StringReader(resJson))
                                    {
                                        JsonTextReader reader = new JsonTextReader(sr);

                                        if (reader.Read())
                                        {
                                            if (reader.TokenType == JsonToken.StartArray)
                                            {
                                                while (reader.Read())
                                                {
                                                    if (reader.TokenType == JsonToken.EndArray)
                                                        break;
                                                    else if (reader.TokenType == JsonToken.StartObject)
                                                        UpdateStationManual(reader);
                                                }
                                            }
                                        }
                                    }

                                    StationCore.LastRefreshTime = DateTime.Now;
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

            hubConnection?.Dispose();
            semaphore.Dispose();
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
                        using (StringReader sr = new StringReader(stationsJson))
                        {
                            JsonTextReader reader = new JsonTextReader(sr);

                            if (reader.Read())
                            {
                                if (reader.TokenType == JsonToken.StartArray)
                                {
                                    while (reader.Read())
                                    {
                                        if (reader.TokenType == JsonToken.EndArray)
                                            break;
                                        else if (reader.TokenType == JsonToken.StartObject)
                                            UpdateStationManual(reader);
                                    }
                                }
                            }
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

        private void UpdateStationManual(JsonTextReader reader)
        {
            reader.Read(); // Read property p (Path)
            reader.Read(); // Read value of p
            if (cache[reader.Value] is IStationCore station)
            {
                reader.Read(); // Read property r (LastRefreshTime)
                reader.Read(); // Read value of r
                station.LastRefreshTime = Convert.ToDateTime(reader.Value);

                reader.Read(); // Read property s (ConnectionStatus)
                reader.Read(); // Read value of s
                if (Enum.TryParse((string)reader.Value, out ConnectionStatus status))
                    station.ConnectionStatus = status;

                reader.Read(); // Read property e (CommunicationError)
                reader.Read(); // Read value of e
                station.CommunicationError = (string)reader.Value;

                reader.Read(); // Read property channels 
                reader.Read(); // Read start array
                while (reader.Read()) // Read start array
                {
                    if (reader.TokenType == JsonToken.EndArray)
                        break;
                    else if (reader.TokenType == JsonToken.StartObject)
                        UpdateChannelManual(reader);
                }

                reader.Read(); // Read property stations 
                reader.Read(); // Read start array
                while (reader.Read()) // Read start array
                {
                    if (reader.TokenType == JsonToken.EndArray)
                        break;
                    else if (reader.TokenType == JsonToken.StartObject)
                        UpdateStationManual(reader);
                }

                reader.Read(); // Read end object;
            }
            else
            {
                while (reader.Read())
                    if (reader.TokenType == JsonToken.EndObject)
                        break;
            }
        }

        private void UpdateChannelManual(JsonTextReader reader)
        {
            reader.Read(); // Read property p (Path)
            reader.Read(); // Read value of p

            if (cache[reader.Value] is IChannelCore channel)
            {
                reader.Read(); // Read property r (LastRefreshTime)
                reader.Read(); // Read value of r
                channel.LastRefreshTime = Convert.ToDateTime(reader.Value);

                reader.Read(); // Read property e (CommunicationError)
                reader.Read(); // Read value of e
                channel.CommunicationError = (string)reader.Value;

                reader.Read(); // Read property devices array
                reader.Read(); // Read start array
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.EndArray)
                        break;
                    else if (reader.TokenType == JsonToken.StartObject)
                        UpdateDeviceManual(reader);
                }

                reader.Read(); // Read end object;
            }
            else
            {
                while (reader.Read())
                    if (reader.TokenType == JsonToken.EndObject)
                        break;
            }
        }

        private void UpdateDeviceManual(JsonTextReader reader)
        {
            reader.Read(); // Read property p (Path)
            reader.Read(); // Read value of p

            if (cache[reader.Value] is IDeviceCore device)
            {
                reader.Read(); // Read property r (LastRefreshTime)
                reader.Read(); // Read value of r
                device.LastRefreshTime = Convert.ToDateTime(reader.Value);

                reader.Read(); // Read property e (CommunicationError)
                reader.Read(); // Read value of e
                device.CommunicationError = (string)reader.Value;

                reader.Read(); // Read property tags array
                reader.Read(); // Read start array
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.EndArray)
                        break;
                    else if (reader.TokenType == JsonToken.StartObject)
                        UpdateTagManual(reader);
                }
                reader.Read(); // Read end object
            }
            else
            {
                while (reader.Read())
                    if (reader.TokenType == JsonToken.EndObject)
                        break;
            }

        }

        private void UpdateTagManual(JsonTextReader reader)
        {
            reader.Read(); // Read property p (Path)
            reader.Read(); // Read value of p

            if (cache[reader.Value] is ITagCore tag)
            {
                reader.Read(); // Read property v (Value)
                reader.Read(); // Read value of v
                tag.Value = (string)reader.Value;

                reader.Read(); // Read property q (Quality)
                reader.Read(); // Read value of q
                if (Enum.TryParse(reader.Value.ToString(), out Quality quality))
                    tag.Quality = quality;

                reader.Read(); // Read property t (TimeStamp)
                reader.Read(); // Read value of t
                tag.TimeStamp = Convert.ToDateTime(reader.Value);

                reader.Read(); // Read property e (CommunicationError)
                reader.Read(); // Read value of e
                tag.CommunicationError = (string)reader.Value;

                reader.Read(); // Read end object
            }
            else
            {
                while (reader.Read())
                    if (reader.TokenType == JsonToken.EndObject)
                        break;
            }
        }

        private void UpdateStationCore(StationClient station, IStationCore stationCore, string startPath)
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

        private void UpdateChannelCore(ChannelClient channel, IChannelCore channelCore, string startPath)
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

        private void UpdateDeviceCore(DeviceClient device, IDeviceCore deviceCore, string startPath)
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

        private void UpdateTagCore(TagClient tag, ITagCore tagCore, string startPath)
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

        private void RemoveFirstStationPath(StationClient station, int length)
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

        private void RemoveFirstStationPath(ChannelClient channel, int length)
        {
            if (channel != null)
            {
                channel.Path = channel.Path.Remove(0, length);
                if (channel.Devices != null)
                {
                    foreach (DeviceClient device in channel.Devices)
                    {
                        RemoveFirstStationPath(device, length);
                    }
                }
            }
        }

        private void RemoveFirstStationPath(DeviceClient device, int length)
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

        private void RemoveFirstStationPath(TagClient tag, int length)
        {
            if (tag != null)
                tag.Path = tag.Path.Remove(0, length);
        }

        private IEnumerable<ICoreItem> GetAllChildItems(IGroupItem groupItem)
        {
            if (groupItem != null)
            {
                if (groupItem.Childs != null)
                {
                    foreach (var item in groupItem.Childs)
                    {
                        if (item is IGroupItem childGroup)
                        {
                            yield return item as IGroupItem;
                            foreach (var child in GetAllChildItems(childGroup))
                                yield return child as ICoreItem;
                        }
                    }
                }
            }
        }

        #endregion
    }
}
