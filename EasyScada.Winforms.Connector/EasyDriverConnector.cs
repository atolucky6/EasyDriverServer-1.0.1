using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyScada.Winforms.Connector
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [Designer(typeof(EasyDriverConnectorDesigner))]
    public sealed partial class EasyDriverConnector : Component, IDisposable
    {
        #region Singleton

        static bool isTagFileLoaded = false;
        static bool isFirstLoad = false;
        static bool isSetServerAddress = false;
        static bool isSetPort = false;
        static bool isSetCommunicationMode = false;
        static bool isSetRefreshRate = false;

        static ConnectionSchema connectionSchema;
        static HubConnection hubConnection;
        static IHubProxy hubProxy;
        static bool firstScan = false;

        static Task requestTask;
        static Hashtable tagsCache;
        static Hashtable cache;
        static EasyDriverConnector instance;
        static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        static EasyDriverConnector()
        {
            if (instance == null)
                instance = new EasyDriverConnector();
        }

        #endregion

        #region Constructors

        public EasyDriverConnector()
        {
            InitializeComponent();
            Channels = new TagStore();
        }

        public EasyDriverConnector(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            if (!DesignMode)
                Disposed += OnDisposed;
            Channels = new TagStore();
        }

        #endregion

        #region Public members

        [Browsable(false)]
        public dynamic Channels { get; private set; }

        [Browsable(false)]
        public bool IsDisposed { get; private set; }

        static string serverAddress = "127.0.0.1";
        [Description("Set server address for connector")]
        [Browsable(true), Category("Easy Scada")]
        public string ServerAddress
        {
            get { return serverAddress; }
            set
            {
                serverAddress = value;
                if (!DesignMode)
                {
                    if (!isSetServerAddress)
                    {
                        isSetServerAddress = true;
                        InitializeConnection();
                    }
                }
            }
        }

        static ushort port = 8800;
        [Description("Set port number for connector")]
        [Browsable(true), Category("Easy Scada")]
        public ushort Port
        {
            get { return port; }
            set
            {
                port = value;
                if (!DesignMode)
                {
                    if (!isSetPort)
                    {
                        isSetPort = true;
                        InitializeConnection();
                    }
                }
            }
        }

        static CommunicationMode communicationMode = CommunicationMode.ReceiveFromServer;
        [Description("Set communication mode for connector")]
        [Browsable(true), Category("Easy Scada")]
        public CommunicationMode CommunicationMode
        {
            get { return communicationMode; }
            set
            {
                communicationMode = value;
                if (!DesignMode)
                {
                    if (!isSetCommunicationMode)
                    {
                        isSetCommunicationMode = true;
                        InitializeConnection();
                    }
                }
            }
        }

        static int refreshRate = 1000;
        [Description("Set refresh rate for connector")]
        [Browsable(true), Category("Easy Scada")]
        public int RefreshRate
        {
            get { return refreshRate; }
            set
            {
                refreshRate = value;
                if (!DesignMode)
                {
                    if (!isSetRefreshRate)
                    {
                        isSetRefreshRate = true;
                        InitializeConnection();
                    }
                }
            }
        }

        private ConnectionStatus connectionStatus;
        [Browsable(false)]
        public ConnectionStatus ConnectionStatus
        {
            get { return connectionStatus; }
            set
            {
                if (value != connectionStatus)
                {
                    ConnectionStatus oldValue = connectionStatus;
                    connectionStatus = value;
                    ConnectionStatusChaged?.Invoke(this, new ConnectionStatusChangedEventArgs(oldValue, value));
                }
            }
        }

        [Browsable(false)]
        public bool IsSubscribed { get; private set; }

        [Browsable(false)]
        public bool IsStarted { get; private set; }

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

        public Quality WriteTag(string pathToTag, string value)
        {
            semaphore.Wait();
            try
            {
                if (!string.IsNullOrWhiteSpace(pathToTag) || string.IsNullOrWhiteSpace(value) && hubConnection != null && hubConnection.State == ConnectionState.Connected)
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
                            return Quality.Good;
                    }
                }
            }
            catch { }
            finally { semaphore.Release(); }
            return Quality.Bad;
        }

        public async Task<Quality> WriteTagAsync(string pathToTag, string value)
        {
            try
            {
                await semaphore.WaitAsync();
                WriteResponse response = await hubProxy.Invoke<WriteResponse>("writeTagValueAsync", new WriteCommand()
                {
                    PathToTag = pathToTag,
                    Value = value,
                    SendTime = DateTime.Now,
                    WriteMode = WriteMode.WriteAllValue,
                    WritePiority = WritePiority.Highest
                });
                if (response.IsSuccess)
                    return Quality.Good;
            }
            catch { }
            finally { semaphore.Release(); }
            return Quality.Bad;
        }

        #endregion

        #region Private methods

        private async void InitializeConnection()
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
                        MessageBox.Show($"The tag file doesn't exists. Please create tag file at '{tagFilePath}' and restart the application.", "Easy Driver Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Read tag file
                    try
                    {
                        resJson = File.ReadAllText(tagFilePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Some error occurs when open tag file '{tagFilePath}'.\nError: {ex.Message}", "Easy Driver Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Deserialize tag file to DriverConnector
                    try
                    {
                        connectionSchema = JsonConvert.DeserializeObject<ConnectionSchema>(resJson);
                        refreshRate = connectionSchema.RefreshRate;
                        communicationMode = connectionSchema.CommunicationMode;
                        serverAddress = connectionSchema.ServerAddress;
                        port = connectionSchema.Port;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Some error occurs when deserialize tag file 'tagFilePath'.\nError: {ex.Message}", "Easy Driver Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    Started += OnStarted;
                }

                // Check all requires property is set
                if (isSetCommunicationMode && isSetPort && isSetServerAddress && isSetRefreshRate)
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
                    hubConnection = new HubConnection($"http://{serverAddress}:{port}/easyScada");
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

            }
            catch (Exception) { }
        }

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
                InitializeConnection();
            }
        }

        private void OnStateChanged(StateChange stateChange)
        {
            ConnectionStatus = (ConnectionStatus)Enum.Parse(typeof(ConnectionStatus), stateChange.NewState.ToString());
            if (stateChange.NewState == ConnectionState.Disconnected)
            {
                Debug.WriteLine($"The easy driver connector was disconnected with server {serverAddress}:{port}");
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
                Debug.WriteLine($"The easy driver connector was connected to server {serverAddress}:{port}");
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
                        tag.connector = this;
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
                Debug.WriteLine($"Begin tring to reconnect to server {serverAddress}:{port}");
            }
            else if (stateChange.NewState == ConnectionState.Connecting)
            {

            }
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
            catch { }
        }

        private void OnDisposed(object sender, EventArgs e)
        {
            IsDisposed = true;
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

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    sealed class EasyDriverConnectorDesigner : ComponentDesigner
    {
        #region Members

        DesignerActionListCollection actionListCollection;
        EasyDriverConnectorDesignerActionList actionList;
        EasyDriverConnector EasyDriverConnector { get { return Component as EasyDriverConnector; } }

        #endregion

        #region Methods

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (actionListCollection == null)
                {
                    actionListCollection = new DesignerActionListCollection();
                    actionList = GetActionList();
                    actionListCollection.Add(actionList);
                }
                return actionListCollection;
            }
        }

        private EasyDriverConnectorDesignerActionList GetActionList()
        {
            if (actionList == null)
                actionList = new EasyDriverConnectorDesignerActionList(Component);
            return actionList;
        }

        #endregion
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    sealed class EasyDriverConnectorDesignerActionList : DesignerActionList
    {
        public EasyDriverConnectorDesignerActionList(IComponent component) : base(component)
        {
            designerActionUIservice = GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
            BaseControl = component as EasyDriverConnector;
            projectPath = GetCurrentDesignPath();
            debugPath = "\\Debug\\ConnectionSchema.json";
            releasePath = "\\Release\\ConnectionSchema.json";
        }

        #region Members


        readonly string projectPath;
        readonly string debugPath;
        readonly string releasePath;
        bool isBusy;
        readonly EasyDriverConnector BaseControl;
        DesignerActionItemCollection actionItems;
        readonly DesignerActionUIService designerActionUIservice;

        #endregion

        #region Methods

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            if (actionItems == null)
            {
                actionItems = new DesignerActionItemCollection
                {
                    new DesignerActionPropertyItem("ServerAddress", "Server Addrees", "Easy Scada", "Set server address for connector"),
                    new DesignerActionPropertyItem("Port", "Port", "Easy Scada", "Set port number for connector"),
                    new DesignerActionPropertyItem("CommunicationMode", "Communication Mode", "Easy Scada", "Set communication mode for connector"),
                    new DesignerActionPropertyItem("RefreshRate", "Refresh Rate", "Easy Scada", "Set refresh rate for connector"),
                    new DesignerActionMethodItem(this, "UpdateConnectionSchema", "Update Connection Schema", "Easy Scada", "Click here to update tag file", true)
                };
            }
            return actionItems;
        }

        #endregion

        #region Desginer properties

        [Description("Set server address for connector")]
        [Browsable(true), Category("Easy Scada")]
        public string ServerAddress
        {
            get { return BaseControl.ServerAddress; }
            set { SetValue(BaseControl, value); SaveTagFile(); }
        }

        [Description("Set port number for connector")]
        [Browsable(true), Category("Easy Scada")]
        public ushort Port
        {
            get { return BaseControl.Port; }
            set { SetValue(BaseControl, value); SaveTagFile(); }
        }

        [Description("Set communication mode for connector")]
        [Browsable(true), Category("Easy Scada")]
        public CommunicationMode CommunicationMode
        {
            get { return BaseControl.CommunicationMode; }
            set { SetValue(BaseControl, value); SaveTagFile(); }
        }

        [Description("Set refresh rate for connector")]
        [Browsable(true), Category("Easy Scada")]
        public int RefreshRate
        {
            get { return BaseControl.RefreshRate; }
            set { SetValue(BaseControl, value); SaveTagFile(); }
        }

        #endregion

        #region Action methods

        private void UpdateConnectionSchema()
        {
            try
            {
                if (isBusy)
                    return;
                isBusy = true;
                FormConnectionSchema formConnectionSchema = new FormConnectionSchema(this);
                formConnectionSchema.ShowDialog();
                isBusy = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Easy Driver Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isBusy = false; 
            }
        }

        private void SaveTagFile()
        {
            try
            {
                if (File.Exists(debugPath))
                {
                    string resJson = File.ReadAllText(debugPath);
                    if (!string.IsNullOrEmpty(resJson))
                    {
                        ConnectionSchema driverConnector = JsonConvert.DeserializeObject<ConnectionSchema>(resJson);
                        if (driverConnector != null)
                        {
                            driverConnector.CommunicationMode = CommunicationMode;
                            driverConnector.RefreshRate = RefreshRate;
                            driverConnector.ServerAddress = ServerAddress;
                            driverConnector.Port = Port;

                            File.WriteAllText(debugPath, JsonConvert.SerializeObject(driverConnector));
                        }
                    }
                }

                if (File.Exists(releasePath))
                {
                    string resJson = File.ReadAllText(releasePath);
                    if (!string.IsNullOrEmpty(resJson))
                    {
                        ConnectionSchema driverConnector = JsonConvert.DeserializeObject<ConnectionSchema>(resJson);
                        if (driverConnector != null)
                        {
                            driverConnector.CommunicationMode = CommunicationMode;
                            driverConnector.RefreshRate = RefreshRate;
                            driverConnector.ServerAddress = ServerAddress;
                            driverConnector.Port = Port;

                            File.WriteAllText(releasePath, JsonConvert.SerializeObject(driverConnector));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occurs when save tag file. {ex.Message}", "Easy Driver Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Helper

        private string GetCurrentDesignPath()
        {
            try
            {
                EnvDTE.DTE dte = this.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
                return Path.GetDirectoryName(dte.ActiveDocument.FullName) + "\\bin";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return string.Empty;
            }
        }

        /// <summary>
        /// The method to get a <see cref="PropertyDescriptor"/> of the control by property name
        /// </summary>
        /// <param name="control"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        private PropertyDescriptor GetPropertyByName(object control, [CallerMemberName]string propName = null)
        {
            PropertyDescriptor prop;
            prop = TypeDescriptor.GetProperties(control)[propName];
            if (null == prop)
                throw new ArgumentException($"Matching {propName} property not found!", propName);
            else
                return prop;
        }

        /// <summary>
        /// Set the value for property of the control
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value"></param>
        /// <param name="propName"></param>
        private void SetValue(object control, object value = null, [CallerMemberName]string propName = null)
        {
            GetPropertyByName(control, propName).SetValue(control, value);
        }

        #endregion
    }
}
