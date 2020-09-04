using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace EasyScada.Core
{
    class EasyDriverConnector : IEasyDriverConnector
    {
        #region Private members

        private ConnectionSchema connectionSchema;
        private HubConnection hubConnection;
        private IHubProxy hubProxy;
        private bool firstScan = false;
        private bool isFirstLoad = false;
        private bool isTagFileLoaded = false;

        private Task requestTask;
        private Hashtable tagsCache;
        private Hashtable cache;
        private SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        #endregion

        #region Public members

        public string ServerAddress { get; set; } = "127.0.0.1";
        public ushort Port { get; set; } = 8800;
        public CommunicationMode CommunicationMode { get; set; } = CommunicationMode.ReceiveFromServer;
        public ConnectionStatus ConnectionStatus { get; set; }
        public int RefreshRate { get; set; } = 1000;
        public bool IsSubscribed { get; set; }
        public bool IsStarted { get; set; }
        public bool IsDisposed { get; set; }

        #endregion

        #region Events

        public event EventHandler Started;
        public event EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChaged;
        public event EventHandler ConnectionSlow;

        #endregion

        #region Public methods

        public ITag GetTag(string pathToTag)
        {
            if (tagsCache != null && !string.IsNullOrWhiteSpace(pathToTag))
                return tagsCache[pathToTag] as ITag;
            return null;
        }

        public WriteResponse WriteTag(string pathToTag, string value)
        {
            semaphore.Wait();
            try
            {
                if (!string.IsNullOrWhiteSpace(pathToTag) && 
                    !string.IsNullOrWhiteSpace(value) && 
                    hubConnection != null && 
                    hubConnection.State == ConnectionState.Connected)
                {
                    var writeTask = hubProxy.Invoke<WriteResponse>("writeTagValueAsync", new WriteCommand()
                    {
                        PathToTag = pathToTag,
                        Value = value,
                        SendTime = DateTime.Now,
                        WriteMode = WriteMode.WriteAllValue,
                        WritePiority = WritePiority.Highest
                    });

                    Task.WaitAll(writeTask);
                    if (writeTask.IsCompleted)
                    {
                        WriteResponse response = writeTask.Result;
                        if (response.IsSuccess)
                            return response;
                    }
                }
            }
            catch { }
            finally { semaphore.Release(); }
            return new WriteResponse() { IsSuccess = false, };
        }

        public async Task<WriteResponse> WriteTagAsync(string pathToTag, string value)
        {
            return await Task.Run(() =>
            {
                return WriteTag(pathToTag, value);
            });
        }

        public List<WriteResponse> WriteMultiTag(List<WriteCommand> writeCommands)
        {
            semaphore.Wait();
            try
            {
                if (writeCommands != null &&
                    writeCommands.Count > 0 &&
                    hubConnection != null &&
                    hubConnection.State == ConnectionState.Connected)
                {
                    var writeTask = hubProxy.Invoke<List<WriteResponse>>("writeMultiTagAsync", writeCommands.ToList());
                    Task.WaitAll(writeTask);
                    if (writeTask.IsCompleted)
                    {
                        List<WriteResponse> response = writeTask.Result;
                        return response;
                    }
                }
            }
            catch { }
            finally { semaphore.Release(); }
            List<WriteResponse> result = new List<WriteResponse>();
            for (int i = 0; i < writeCommands.Count; i++)
                result.Add(new WriteResponse() { IsSuccess = false });
            return result;
        }

        public async Task<List<WriteResponse>> WriteMultiTagAsync(List<WriteCommand> writeCommands)
        {
            return await Task.Run(() =>
            {
                return WriteMultiTag(writeCommands);
            });
        }

        public void Dispose()
        {
            IsDisposed = true;
        }

        public async void Start()
        {
            try
            {
                if (!isTagFileLoaded)
                {
                    isTagFileLoaded = true;
                    string resJson = string.Empty;

                    // Get tag file from application path
                    string tagFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\ConnectionSchema.json";
                    if (!File.Exists(tagFilePath))
                    {
                        
                        Debug.WriteLine($"The tag file doesn't exists. Please create tag file at '{tagFilePath}' and restart the application.");
                        return;
                    }

                    // Read tag file
                    try
                    {
                        resJson = File.ReadAllText(tagFilePath);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Some error occurs when open tag file '{tagFilePath}'.\nError: {ex.Message}");
                        return;
                    }

                    // Deserialize tag file to DriverConnector
                    try
                    {
                        connectionSchema = JsonConvert.DeserializeObject<ConnectionSchema>(resJson);
                        connectionSchema.SetParentForChild();
                        RefreshRate = connectionSchema.RefreshRate;
                        CommunicationMode = connectionSchema.CommunicationMode;
                        ServerAddress = connectionSchema.ServerAddress;
                        Port = connectionSchema.Port;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Some error occurs when deserialize tag file 'tagFilePath'.\nError: {ex.Message}");
                        return;
                    }

                    Started += OnStarted;
                }

                // If hub connection is initialized then unsubscribe all events
                if (hubConnection != null)
                {
                    try
                    {
                        hubConnection.StateChanged -= OnStateChanged;
                        hubConnection.Closed -= OnDisconnected;
                        hubConnection.ConnectionSlow -= OnConnectionSlow;
                        if (hubConnection.State == ConnectionState.Connected)
                            await Task.Factory.StartNew(() => hubConnection.Stop());
                        await Task.Factory.StartNew(() => hubConnection.Dispose());
                    }
                    catch { }
                }

                // Make a new connection
                hubConnection = new HubConnection($"http://{ServerAddress}:{Port}/easyScada");
                // Subscribe connection events
                hubConnection.StateChanged += OnStateChanged;
                hubConnection.Closed += OnDisconnected;
                hubConnection.ConnectionSlow += OnConnectionSlow;
                // Create proxy
                hubProxy = hubConnection.CreateHubProxy("EasyDriverServerHub");
                // Handle all message received from server
                hubProxy.On<string>("broadcastSubscribeData", OnReceivedBroadcastMessage);
                // Start connection
                await hubConnection.Start(new LongPollingTransport());

            }
            catch (Exception) { }
        }

        public void Stop()
        {

        }

        #endregion

        #region Private methods

        private void OnConnectionSlow()
        {
            ConnectionSlow?.Invoke(this, EventArgs.Empty);
        }

        private void OnDisconnected()
        {
            if (!IsDisposed)
            {
                // Delay a little bit before reconnect
                Thread.Sleep(5000);
                // When connection disconnected we will init the new connection and tring to reconnect
                Start();
            }
        }

        private void OnStateChanged(StateChange stateChange)
        {
            if (stateChange.NewState == ConnectionState.Disconnected)
            {
                Debug.WriteLine($"The easy driver connector was disconnected with server {ServerAddress}:{Port}");
                if (connectionSchema != null && connectionSchema.Stations != null)
                {
                    foreach (var tag in connectionSchema.GetAllTag())
                    {
                        try
                        {
                            tag.Quality = Quality.Bad;
                        }
                        catch { }
                    }
                }
            }
            else if (stateChange.NewState == ConnectionState.Connected && !IsDisposed)
            {
                Debug.WriteLine($"The easy driver connector was connected to server {ServerAddress}:{Port}");
                // Create tag cache and start the refresh timer in the first load
                if (!isFirstLoad)
                {
                    // Create tag cache
                    tagsCache = new Hashtable();
                    foreach (var tag in connectionSchema.GetAllTag())
                    {
                        tag.quality = Quality.Uncertain;
                        tag.value = string.Empty;
                        tagsCache.Add(tag.Path, tag);
                    }
                    // Set a first load flag to determine we are already loaded
                    isFirstLoad = true;
                }

                // Check requires condition before subscribe
                if (isTagFileLoaded && connectionSchema != null && connectionSchema.Stations != null && connectionSchema.Stations.Count > 0)
                {
                    // Delay a little bit before subscribe to server
                    Thread.Sleep(1000);
                    IsSubscribed = false;
                    if (requestTask == null)
                        requestTask = Task.Factory.StartNew(RefreshData, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                }
            }
            else if (stateChange.NewState == ConnectionState.Reconnecting)
            {
                Debug.WriteLine($"Begin tring to reconnect to server {ServerAddress}:{Port}");
            }
            else if (stateChange.NewState == ConnectionState.Connecting)
            {

            }

            var oldConnectionStatus = ConnectionStatus;
            ConnectionStatus = (ConnectionStatus)Enum.Parse(typeof(ConnectionStatus), stateChange.NewState.ToString());
            ConnectionStatusChaged?.Invoke(this, new ConnectionStatusChangedEventArgs(oldConnectionStatus, ConnectionStatus));
        }

        /// <summary>
        /// Method to subscribe this connection to server
        /// </summary>
        /// <returns></returns>
        private bool Subscribe()
        {
            if (connectionSchema != null && connectionSchema.Stations != null)
            {
                string subscribeDataJson = JsonConvert.SerializeObject(connectionSchema.Stations);
                if (hubConnection != null && hubConnection.State == ConnectionState.Connected)
                {
                    var subTask = hubProxy.Invoke<string>("subscribe", subscribeDataJson, CommunicationMode.ToString(), RefreshRate);
                    Task.WaitAll(subTask);
                    if (subTask.IsCompleted)
                        return subTask.Result == "Ok";
                }
            }
            return false;
        }

        private void RefreshData()
        {
            while (!IsDisposed)
            {
                int delay = 100;
                try
                {
                    // Re subscribe if needed
                    if (!IsSubscribed && hubConnection != null && hubConnection.State == ConnectionState.Connected)
                    {
                        bool resSub = Subscribe();
                        if (resSub)
                        {
                            if (cache == null)
                                cache = new Hashtable();

                            IsSubscribed = true;
                            foreach (var item in GetAllChildItems(connectionSchema))
                            {
                                if (item is IPath pathObj)
                                    cache.Add(pathObj.Path, item);
                            }
                        }
                        delay = 100;
                    }

                    if (connectionSchema != null && connectionSchema.Stations != null && !IsDisposed)
                    {
                        if (CommunicationMode == CommunicationMode.RequestToServer && connectionSchema.Stations.Count > 0 &&
                            hubConnection.State == ConnectionState.Connected && cache != null)
                        {
                            Task<string> getDataTask = null;
                            try
                            {
                                semaphore.Wait();
                                // Get subscribed data from server
                                getDataTask = hubProxy.Invoke<string>("getSubscribedDataAsync");
                                Task.WaitAll(getDataTask);
                            }
                            finally { semaphore.Release(); }

                            try
                            {
                                if (getDataTask != null && getDataTask.IsCompleted)
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
                                }

                                // If this is first scan then raise an started event to notify to all controls
                                if (!firstScan && CommunicationMode == CommunicationMode.RequestToServer)
                                {
                                    firstScan = true;
                                    Started?.Invoke(this, new EventArgs());
                                }
                            }
                            catch { }
                        }
                        delay = RefreshRate;
                    }
                }
                catch { }
                finally { Thread.Sleep(delay); }
            }

            hubConnection?.Dispose();
        }

        private void OnReceivedBroadcastMessage(string resJson)
        {
            try
            {
                if (connectionSchema != null &&
                connectionSchema.Stations != null &&
                connectionSchema.CommunicationMode == CommunicationMode.ReceiveFromServer)
                {
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

                    // If this is first scan then raise an started event to notify to all controls
                    if (!firstScan && CommunicationMode == CommunicationMode.ReceiveFromServer)
                    {
                        firstScan = true;
                        Started?.Invoke(this, new EventArgs());
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void OnStarted(object sender, EventArgs e)
        {
            IsStarted = true;
        }

        private void UpdateStationManual(JsonTextReader reader)
        {
            reader.Read(); // Read property p (Path)
            reader.Read(); // Read value of p
            if (cache[reader.Value] is Station station)
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
                station.Error = (string)reader.Value;

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

            if (cache[reader.Value] is Channel channel)
            {
                reader.Read(); // Read property r (LastRefreshTime)
                reader.Read(); // Read value of r
                channel.LastRefreshTime = Convert.ToDateTime(reader.Value);

                reader.Read(); // Read property e (CommunicationError)
                reader.Read(); // Read value of e
                channel.Error = (string)reader.Value;

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

            if (cache[reader.Value] is Device device)
            {
                reader.Read(); // Read property r (LastRefreshTime)
                reader.Read(); // Read value of r
                device.LastRefreshTime = Convert.ToDateTime(reader.Value);

                reader.Read(); // Read property e (CommunicationError)
                reader.Read(); // Read value of e
                device.Error = (string)reader.Value;

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

            if (cache[reader.Value] is Tag tag)
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
                tag.Error = (string)reader.Value;

                reader.Read(); // Read end object
            }
            else
            {
                while (reader.Read())
                    if (reader.TokenType == JsonToken.EndObject)
                        break;
            }
        }

        private List<object> GetAllChildItems(ConnectionSchema connectionSchema)
        {
            List<object> result = new List<object>();
            if (connectionSchema != null && connectionSchema.Stations != null)
            {
                foreach (var station in connectionSchema.Stations)
                {
                    result.Add(station);
                    AddAllChildItems(result, station as IComposite);
                }
            }
            return result;
        }

        private void AddAllChildItems(List<object> source, IComposite composite)
        {
            if (composite != null)
            {
                if (composite.Childs != null)
                {
                    foreach (var item in composite.Childs)
                    {
                        if (item != null)
                            source.Add(item);
                        if (item is IComposite childComposite)
                        {
                            if (childComposite.Childs.Count > 0)
                                AddAllChildItems(source, childComposite);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
