using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    internal class EasyDriverConnector : IEasyDriverConnector
    {
        #region Private members

        private ConnectionSchema connectionSchema;
        private HubConnection hubConnection;
        private IHubProxy hubProxy;
        private bool firstScan = false;
        private bool isFirstLoad = false;
        private bool isTagFileLoaded = false;

        private MongoDbTagConverter mongoDbTagConverter = new MongoDbTagConverter();
        private Task requestTask;
        private Hashtable channelCache = new Hashtable();
        private Hashtable deviceCache = new Hashtable();
        private Hashtable tagsCache = new Hashtable();
        private SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private List<ITag> subscribedTags = new List<ITag>();
        private ClientTagJsonConverter clientTagJsonConverter = new ClientTagJsonConverter();
        private System.Timers.Timer timeoutTimer;
        #endregion

        #region Public members

        public bool UseMongoDb { get; set; }
        public string MongoDb_ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
        public string StationName { get; set; }
        public int Timeout { get; set; } = 30;

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

        public IEnumerable<ICoreItem> GetAllChannels()
        {
            foreach (var item in channelCache.Values)
            {
                if (item is ICoreItem coreItem)
                    yield return coreItem;
            }
        }

        public ICoreItem GetChannel(string path)
        {
            if (channelCache.ContainsKey(path))
            {
                return channelCache[path] as ICoreItem;
            }
            return null;
        }

        public IEnumerable<ICoreItem> GetAllDevices()
        {
            foreach (var item in deviceCache.Values)
            {
                if (item is ICoreItem coreItem)
                    yield return coreItem;
            }
        }

        public ICoreItem GetDevice(string path)
        {
            if (deviceCache.ContainsKey(path))
            {
                return deviceCache[path] as ICoreItem;
            }
            return null;
        }

        public async Task<ConnectionSchema> GetConnectionSchemaAsync(string ipAddress, ushort port)
        {
            try
            {
                ConnectionSchema ConnectionSchema = null;
                using (HubConnection hubConnection = new HubConnection($"http://{ipAddress}:{port}/easyScada"))
                {
                    IHubProxy hubProxy = hubConnection.CreateHubProxy("EasyDriverServerHub");
                    await hubConnection.Start();
                    if (hubConnection.State == ConnectionState.Connected)
                    {
                        ConnectionSchema = new ConnectionSchema()
                        {
                            CreatedDate = DateTime.Now,
                            Port = port,
                            ServerAddress = ipAddress,
                        };

                        string resJson = await hubProxy.Invoke<string>("getAllElementsAsync");

                        List<ICoreItem> coreItems = new List<ICoreItem>();
                        if (JsonConvert.DeserializeObject(resJson) is JObject obj)
                        {
                            if (obj["Childs"] is JArray jArray)
                            {
                                foreach (var item in jArray)
                                {
                                    ICoreItem coreItem = JsonConvert.DeserializeObject<ICoreItem>(item.ToString(), new ConnectionSchemaJsonConverter());
                                    if (coreItem != null)
                                        coreItems.Add(coreItem);
                                }
                            }
                        }
                        if (coreItems != null)
                            coreItems.ForEach(x => ConnectionSchema.Childs.Add(x));
                    }
                }
                return ConnectionSchema;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ConnectionSchema> GetConnectionSchemaAsync(string url)
        {
            try
            {
                ConnectionSchema ConnectionSchema = null;
                using (HubConnection hubConnection = new HubConnection(url))
                {
                    IHubProxy hubProxy = hubConnection.CreateHubProxy("EasyDriverServerHub");
                    await hubConnection.Start();
                    if (hubConnection.State == ConnectionState.Connected)
                    {
                        ConnectionSchema = new ConnectionSchema()
                        {
                            CreatedDate = DateTime.Now,
                            Url = url,
                        };

                        string resJson = await hubProxy.Invoke<string>("getAllElementsAsync");

                        List<ICoreItem> coreItems = new List<ICoreItem>();
                        if (JsonConvert.DeserializeObject(resJson) is JObject obj)
                        {
                            if (obj["Childs"] is JArray jArray)
                            {
                                foreach (var item in jArray)
                                {
                                    ICoreItem coreItem = JsonConvert.DeserializeObject<ICoreItem>(item.ToString(), new ConnectionSchemaJsonConverter());
                                    if (coreItem != null)
                                        coreItems.Add(coreItem);
                                }
                            }
                        }
                        if (coreItems != null)
                            coreItems.ForEach(x => ConnectionSchema.Childs.Add(x));
                    }
                }
                return ConnectionSchema;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ITag GetTag(string pathToTag)
        {
            if (tagsCache != null && !string.IsNullOrWhiteSpace(pathToTag))
                return tagsCache[pathToTag] as ITag;
            return null;
        }

        public IEnumerable<ITag> GetAllTags()
        {
            if (tagsCache != null)
            {
                foreach (var item in tagsCache.Values)
                {
                    if (item is ITag tag)
                        yield return tag;
                }
            }
        }

        public WriteResponse WriteTag(string pathToTag, string value, WritePiority writePiority)
        {
            //semaphore.Wait();
            try
            {
                if (!string.IsNullOrWhiteSpace(pathToTag) && 
                    hubConnection != null && 
                    hubConnection.State == ConnectionState.Connected)
                {
                    string[] splitAddress = pathToTag.Split('/');
                    string[] prefixArray = new string[splitAddress.Length - 1];
                    Array.Copy(splitAddress, 0, prefixArray, 0, prefixArray.Length);
                    var writeTask = hubProxy.Invoke<WriteResponse>("writeTagValueAsync", new WriteCommand()
                    {
                        Prefix = string.Join("/", prefixArray),
                        TagName = splitAddress[splitAddress.Length - 1],
                        Value = value,
                        SendTime = DateTime.Now,
                        WriteMode = WriteMode.WriteAllValue,
                        WritePiority = writePiority
                    });

                    Task.WaitAll(writeTask);
                    if (writeTask.IsCompleted)
                    {
                        WriteResponse response = writeTask.Result;
                        return response;
                    }
                }
            }
            catch { }
            //finally { semaphore.Release(); }
            return new WriteResponse() { IsSuccess = false, };
        }

        public WriteResponse WriteTag(WriteCommand cmd)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(cmd.Prefix) &&
                    !string.IsNullOrWhiteSpace(cmd.TagName) &&
                    hubConnection != null &&
                    hubConnection.State == ConnectionState.Connected)
                {
                    var writeTask = hubProxy.Invoke<WriteResponse>("writeTagValueAsync", cmd);
                    Task.WaitAll(writeTask);
                    if (writeTask.IsCompleted)
                    {
                        WriteResponse response = writeTask.Result;
                        return response;
                    }
                }
            }
            catch { }
            return new WriteResponse() { IsSuccess = false, };
        }

        public async Task<WriteResponse> WriteTagAsync(string pathToTag, string value, WritePiority writePiority)
        {
            return await Task.Run(() =>
            {
                return WriteTag(pathToTag, value, writePiority);
            });
        }

        public async Task<WriteResponse> WriteTagAsync(WriteCommand cmd)
        {
            return await Task.Run(() =>
            {
                return WriteTag(cmd);
            });
        }

        public List<WriteResponse> WriteMultiTag(List<WriteCommand> writeCommands)
        {
            //semaphore.Wait();
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
            //finally { semaphore.Release(); }
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
                        connectionSchema = JsonConvert.DeserializeObject<ConnectionSchema>(resJson, new ConnectionSchemaJsonConverter());
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


                if (!UseMongoDb)
                {
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
                    if (ServerAddress.StartsWith("http://") || ServerAddress.StartsWith("https://"))
                    {
                        if (ServerAddress.EndsWith("/easyScada"))
                        {
                            hubConnection = new HubConnection($"{ServerAddress}");
                        }
                        else
                        {
                            hubConnection = new HubConnection($"{ServerAddress}/easyScada");
                        }
                    }
                    else
                    {
                        hubConnection = new HubConnection($"http://{ServerAddress}:{Port}/easyScada");
                    }
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
                else
                {
                    if (requestTask == null)
                        requestTask = Task.Factory.StartNew(WatchChangedMongoDb, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                }

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
                if (connectionSchema != null && connectionSchema.Childs != null)
                {
                    SetAllTagBad();
                }
            }
            else if (stateChange.NewState == ConnectionState.Connected && !IsDisposed)
            {
                Debug.WriteLine($"The easy driver connector was connected to server {ServerAddress}:{Port}");
                // Create tag cache and start the refresh timer in the first load
                if (!isFirstLoad)
                {
                    // Create tag cache
                    foreach (var item in connectionSchema.GetAllChildItems())
                    {
                        if (item is ITag tag)
                        {
                            (tag as Tag).quality = Quality.Uncertain;
                            (tag as Tag).value = string.Empty;
                            if (!tagsCache.ContainsKey(tag.Path))
                                tagsCache.Add(tag.Path, tag);
                        }
                        else
                        {
                            if (item.ItemType == ItemType.Channel)
                            {
                                channelCache.Add(item.Path, item);
                            }
                            if (item.ItemType == ItemType.Device)
                            {
                                deviceCache.Add(item.Path, item);
                            }
                        }
                    }
                    // Set a first load flag to determine we are already loaded
                    isFirstLoad = true;
                }

                // Check requires condition before subscribe
                if (isTagFileLoaded && connectionSchema != null && connectionSchema.Childs != null && connectionSchema.Childs.Count > 0)
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
            else if (stateChange.NewState == ConnectionState.Disconnected)
            {
                SetAllTagBad();
            }

            var oldConnectionStatus = ConnectionStatus;
            ConnectionStatus = (ConnectionStatus)Enum.Parse(typeof(ConnectionStatus), stateChange.NewState.ToString());
            ConnectionStatusChaged?.Invoke(this, new ConnectionStatusChangedEventArgs(oldConnectionStatus, ConnectionStatus));
        }

        /// <summary>
        /// Method to subscribe this connection to server
        /// </summary>
        /// <returns></returns>
        bool Subscribe()
        {
            if (connectionSchema != null && connectionSchema.Childs != null)
            {
                subscribedTags = connectionSchema.GetAllTags().ToList();
                List<string> subscribeTagPaths = subscribedTags?.Select(x => x.Path)?.ToList();
                if (subscribeTagPaths != null)
                {
                    string subscribeDataJson = JsonConvert.SerializeObject(subscribeTagPaths);
                    if (hubConnection != null && hubConnection.State == ConnectionState.Connected)
                    {
                        var subTask = hubProxy.Invoke<string>("subscribe", subscribeDataJson, CommunicationMode.ToString(), RefreshRate);
                        Task.WaitAll(subTask);
                        if (subTask.IsCompleted)
                            return subTask.Result == "Ok";
                    }
                }
            }
            return false;
        }

        void RefreshData()
        {
            while (!IsDisposed)
            {
                int delay = 100;
                try
                {
                    // Re subscribe if needed
                    if (!IsSubscribed && hubConnection != null && hubConnection.State == ConnectionState.Connected)
                    {
                        IsSubscribed = Subscribe();
                        delay = 100;
                    }

                    if (connectionSchema != null && connectionSchema.Childs != null && !IsDisposed)
                    {
                        if (CommunicationMode == CommunicationMode.RequestToServer && 
                            connectionSchema.Childs.Count > 0 &&
                            hubConnection.State == ConnectionState.Connected && 
                            tagsCache.Count > 0)
                        {
                            Task<string> getDataTask = null;
                            try
                            {
                                semaphore.Wait();
                                // Get subscribed data from server
                                getDataTask = hubProxy.Invoke<string>("getSubscribedDataAsync");
                                Task.WaitAll(getDataTask);
                            }
                            catch { SetAllTagBad(); }
                            finally { semaphore.Release(); }

                            if (getDataTask.IsCompleted)
                                UpdateTagValue(getDataTask?.Result);
                            else
                                SetAllTagBad();
                        }
                        delay = RefreshRate;
                    }
                }
                catch { SetAllTagBad(); }
                finally
                {
                    Thread.Sleep(delay);
                    // If this is first scan then raise an started event to notify to all controls
                    if (!firstScan && CommunicationMode == CommunicationMode.RequestToServer)
                    {
                        firstScan = true;
                        Started?.Invoke(this, new EventArgs());
                    }
                }
            }

            hubConnection?.Dispose();
        }

        private void OnReceivedBroadcastMessage(string resJson)
        {
            try
            {
                if (connectionSchema != null &&
                    connectionSchema.Childs != null &&
                    connectionSchema.CommunicationMode == CommunicationMode.ReceiveFromServer)
                {
                    UpdateTagValue(resJson);
                    // If this is first scan then raise an started event to notify to all controls
                    if (!firstScan && CommunicationMode == CommunicationMode.ReceiveFromServer)
                    {
                        firstScan = true;
                        Started?.Invoke(this, new EventArgs());
                    }
                }
            }
            catch (Exception)
            {
                SetAllTagBad();
            }
        }

        private void OnStarted(object sender, EventArgs e)
        {
            IsStarted = true;
        }

        private void UpdateTagValue(string resJson)
        {
            Hashtable responseCache = new Hashtable();
            try
            {
                if (JsonConvert.DeserializeObject(resJson) is JArray jArray)
                {
                    foreach (var item in jArray)
                    {
                        try
                        {
                            ClientTag clientTag = JsonConvert.DeserializeObject<ClientTag>(item.ToString(), clientTagJsonConverter);
                            if (clientTag != null)
                                responseCache.Add(clientTag.Path, clientTag);
                        }
                        catch { }
                    }
                }
            }
            catch { }

            foreach (var item in connectionSchema.GetAllTags())
            {
                if (item is Tag tag)
                {
                    if (responseCache.ContainsKey(item.Path))
                    {
                        tag.UpdateValue(responseCache[tag.Path] as ClientTag);
                    }
                    else
                    {
                        tag.UpdateValue(null);
                    }
                }
            } 
        }

        private void SetAllTagBad()
        {
            if (connectionSchema != null)
            {
                foreach (var item in connectionSchema.GetAllTags())
                {
                    if (item is Tag tag)
                    {
                        tag.Quality = Quality.Bad;
                        tag.TimeStamp = DateTime.Now;
                    }
                }
            }
        }

        #endregion

        #region MongoDb
        private void WatchChangedMongoDb()
        {
            while (true)
            {
                try
                {
                    // Create tag cache and start the refresh timer in the first load
                    if (!isFirstLoad)
                    {
                        // Create tag cache
                        foreach (var tag in connectionSchema.GetAllTags())
                        {
                            (tag as Tag).quality = Quality.Uncertain;
                            (tag as Tag).value = string.Empty;
                            if (!tagsCache.ContainsKey(tag.Path))
                                tagsCache.Add(tag.Path, tag);
                        }

                        // Set a first load flag to determine we are already loaded
                        isFirstLoad = true;
                    }

                    if (!IsStarted)
                        Started?.Invoke(this, EventArgs.Empty);

                    var client = new MongoClient(MongoDb_ConnectionString);
                    var database = client.GetDatabase(DatabaseName);
                    var collection = database.GetCollection<BsonDocument>(CollectionName);
                    var filter = Builders<BsonDocument>.Filter.Eq("name", StationName);
                    var pipeline =
                        new EmptyPipelineDefinition<ChangeStreamDocument<BsonDocument>>()
                        .Match(x => x.OperationType == ChangeStreamOperationType.Insert || x.OperationType == ChangeStreamOperationType.Update);

                    var document = collection.Find(filter).FirstOrDefault();
                    UpdateTagViaMongoDb(document);

                    using (var cursor = collection.Watch(pipeline))
                    {
                        try
                        {
                            foreach (var change in cursor.ToEnumerable())
                            {
                                document = collection.Find(filter).FirstOrDefault();
                                UpdateTagViaMongoDb(document);
                            }
                        }
                        catch (Exception ex)
                        {
                            SetAllTagBad();
                        }
                    }
                }
                catch (Exception ex)
                {
                    SetAllTagBad();
                    Thread.Sleep(5000);
                }
            }
        }

        private void UpdateTagViaMongoDb(BsonDocument document)
        {
            bool isValid = false;
            if (document != null && document.ElementCount > 0)
            {
                if (document.Contains("data"))
                {
                    if (document.Contains("name"))
                    {
                        string name = document.Elements.FirstOrDefault(x => x.Name == "name").Value.AsString;

                        string updatedTimeStr = document.Elements.FirstOrDefault(x => x.Name == "updateTime").Value.AsString;

                        if (DateTime.TryParse(updatedTimeStr, out DateTime updatedTime))
                        {
                            if ((DateTime.Now - updatedTime).TotalSeconds < Timeout)
                            {
                                if (name == StationName)
                                {
                                    string data = document.Elements.FirstOrDefault(x => x.Name == "data").Value.AsString;
                                    List<Tag> tags = JsonConvert.DeserializeObject<List<Tag>>(StringCompresser.DecompressString(data), mongoDbTagConverter);

                                    foreach (var item in tags)
                                    {
                                        if (item != null)
                                        {
                                            if (tagsCache.ContainsKey(item.Path))
                                            {
                                                var tag = tagsCache[item.Path];
                                                (tag as Tag).Value = item.Value;
                                                (tag as Tag).Quality = item.Quality;
                                                (tag as Tag).TimeStamp = item.TimeStamp;
                                            }
                                        }
                                    }
                                    isValid = true;
                                }
                            }
                        }
                    }
                }
            }

            if (!isValid)
            {
                SetAllTagBad();
            }
            else
            {

                if (timeoutTimer == null)
                {
                    timeoutTimer = new System.Timers.Timer();
                    timeoutTimer.Interval = Timeout * 1000;
                    timeoutTimer.Elapsed += TimeoutTimer_Elapsed;
                    timeoutTimer.AutoReset = false;
                }
                timeoutTimer.Stop();
                timeoutTimer.Start();
            }
        }

        private void TimeoutTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timeoutTimer.Stop();
            SetAllTagBad();
        }
        #endregion
    }
}
