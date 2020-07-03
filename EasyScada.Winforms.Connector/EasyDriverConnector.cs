using EasyDriver.Client.Models;
using Microsoft.AspNet.SignalR.Client;
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
    public sealed partial class EasyDriverConnector : Component
    {
        #region Singleton

        static bool isTagFileLoaded = false;
        static bool isFirstLoad = false;
        static bool isSetServerAddress = false;
        static bool isSetPort = false;
        static bool isSetCommunicationMode = false;
        static bool isSetRefreshRate = false;

        static DriverConnector driverConnector;
        static HubConnection hubConnection;
        static IHubProxy hubProxy;
        static bool firstScan = false;
        static System.Threading.Timer refreshTimer;

        static Hashtable tagsCache;
        static EasyDriverConnector instance;
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
        }

        ~EasyDriverConnector()
        {
            Dispose();
        }

        public EasyDriverConnector(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            if (!Site.DesignMode)
                Disposed += OnDisposed;
        }

        #endregion

        #region Public members

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
                if (!Site.DesignMode)
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
                if (!Site.DesignMode)
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
                if (!Site.DesignMode)
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
                if (!Site.DesignMode)
                {
                    if (!isSetRefreshRate)
                    {
                        isSetRefreshRate = true;
                        InitializeConnection();
                    }
                }
            }
        }

        #endregion

        #region Events

        public event EventHandler Started;

        #endregion

        #region Public methods

        public ITag GetTag(string pathToTag)
        {
            if (tagsCache != null)
                return tagsCache[pathToTag] as ITag;
            return null;
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
                    string tagFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\TagFile.json";
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
                        driverConnector = JsonConvert.DeserializeObject<DriverConnector>(resJson);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Some error occurs when deserialize tag file 'tagFilePath'.\nError: {ex.Message}", "Easy Driver Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                // Check all requires property is set
                if (isSetCommunicationMode && isSetPort && isSetServerAddress && isSetRefreshRate)
                {
                    // If hub connection is initialized then unsubscribe all events
                    if (hubConnection != null)
                    {
                        hubConnection.StateChanged -= OnStateChanged;
                        hubConnection.Closed -= OnDisconnected;
                    }

                    // Make a new connection
                    hubConnection = new HubConnection($"http://{serverAddress}:{port}/easyScada");
                    // Subscribe connection events
                    hubConnection.StateChanged += OnStateChanged;
                    hubConnection.Closed += OnDisconnected;
                    // Create proxy
                    hubProxy = hubConnection.CreateHubProxy("EasyDriverServerHub");
                    // Handle all message received from server
                    hubProxy.On<string>("broadcastStations", (x) => { OnReceivedStations(x, driverConnector); });
                    // Start connection
                    await hubConnection.Start();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
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

        private async void OnStateChanged(StateChange stateChange)
        {
            if (stateChange.NewState == ConnectionState.Disconnected)
            {
                Debug.WriteLine($"The easy driver connector was disconnected with server {serverAddress}:{port}");
            }
            else if (stateChange.NewState == ConnectionState.Connected)
            {
                Debug.WriteLine($"The easy driver connector was connected to server {serverAddress}:{port}");
                // Check requires condition before subscribe
                if (isTagFileLoaded && driverConnector != null && driverConnector.Stations != null && driverConnector.Stations.Count > 0)
                {
                    // Delay a little bit before subscribe to server
                    await Task.Delay(100); 
                    // Subscribe to server 
                    var res = await hubProxy.Invoke<string>("subscribe", JsonConvert.SerializeObject(driverConnector.Stations), driverConnector.CommunicationMode.ToString(), RefreshRate);
                    Debug.WriteLine($"Easy driver connector subscribe to server: {res}");
                }

                // Create tag cache and start the refresh timer in the first load
                if (!isFirstLoad)
                {
                    // Create tag cache
                    tagsCache = new Hashtable();
                    foreach (var tag in driverConnector.GetAllTag())
                        tagsCache.Add(tag.Path, tag);

                    // Start a refresh timer 
                    // The timer only work when working mode is RequestToServer
                    // If working mode isn't RequestToServer the timer will turn to ildle state 
                    // and it's will run when working mode change to RequestToServer
                    refreshTimer = new System.Threading.Timer(new TimerCallback(RefreshTimerCallback), driverConnector, 0, refreshRate);

                    // Set a first load flag to determine we are already loaded
                    isFirstLoad = true;
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

        private async void RefreshTimerCallback(object state)
        {
            refreshTimer.Change(Timeout.Infinite, Timeout.Infinite); // Stop timer

            if (!IsDisposed)
            {
                int nextPeriod = RefreshRate;
                // Check requires condition before get data from server
                if (state is DriverConnector driverConnector &&
                    hubConnection != null &&
                    hubProxy != null &&
                    hubConnection.State == ConnectionState.Connected &&
                    driverConnector.CommunicationMode == CommunicationMode.RequestToServer &&
                    driverConnector.Stations != null && driverConnector.Stations.Count > 0)
                {
                    try
                    {
                        // Get data from server
                        string resJson = await hubProxy.Invoke<string>("getSubscribedData");
                        if (!string.IsNullOrWhiteSpace(resJson))
                        {
                            // Deserialize json result
                            List<Station> resStations = JsonConvert.DeserializeObject<List<Station>>(resJson);

                            // Update value 
                            foreach (var station in driverConnector.Stations)
                            {
                                Station sourceStation = resStations?.FirstOrDefault(x => x.Path == station.Path);
                                UpdateStation(station, sourceStation);
                            }
                        }
                    }
                    catch { }
                }
                else { nextPeriod = 100; }

                // If this is first scan then raise an started event to notify to all controls
                if (!firstScan && CommunicationMode == CommunicationMode.RequestToServer)
                {
                    firstScan = true;
                    Started?.Invoke(this, new EventArgs());
                }

                // Start timer again
                refreshTimer.Change(nextPeriod, 0);
            }
        }

        private void OnReceivedStations(string stationsJson, DriverConnector driverConnector)
        {
            if (driverConnector != null && 
                driverConnector.Stations != null &&
                driverConnector.CommunicationMode == CommunicationMode.ReceiveFromServer)
            {
                List<Station> resStations = JsonConvert.DeserializeObject<List<Station>>(stationsJson);
                // Update value 
                foreach (var station in driverConnector.Stations)
                {
                    Station sourceStation = resStations?.FirstOrDefault(x => x.Path == station.Path);
                    UpdateStation(station, sourceStation);
                }
            }

            // If this is first scan then raise an started event to notify to all controls
            if (!firstScan && CommunicationMode == CommunicationMode.ReceiveFromServer)
            {
                firstScan = true;
                Started?.Invoke(this, new EventArgs());
            }
        }

        private void OnDisposed(object sender, EventArgs e)
        {
            try
            {
                IsDisposed = true;
                if (hubConnection != null)
                    hubConnection.Dispose();
                if (refreshTimer != null)
                    refreshTimer.Dispose();
            }
            catch { }
        }

        private void UpdateStation(Station item, Station source)
        {
            if (source != null)
            {
                item.RefreshRate = source.RefreshRate;
                item.LastRefreshTime = source.LastRefreshTime;
                item.RemoteAddress = source.RemoteAddress;
                item.Port = source.Port;
                item.CommunicationMode = source.CommunicationMode;
                item.Error = source.Error;
                item.RefreshRate = source.RefreshRate;
                item.Parameters = source.Parameters;

                if (item.Channels != null && item.Channels.Count > 0 && source.Channels != null && source.Channels.Count > 0)
                {
                    foreach (var channel in item.Channels)
                    {
                        Channel sourceChannel = source.Channels.FirstOrDefault(x => x.Path == channel.Path);
                        UpdateChannel(channel, sourceChannel);
                    }
                }

                if (item.RemoteStations != null && item.RemoteStations.Count > 0 && source.RemoteStations != null && source.RemoteStations.Count > 0)
                {
                    foreach (var station in item.RemoteStations)
                    {
                        Station sourceStation = source.RemoteStations.FirstOrDefault(x => x.Path == station.Path);
                        UpdateStation(station, sourceStation);
                    }
                }
            }
            else
            {
                item.Error = "This station could not be found on server.";
            }
        }

        private void UpdateChannel(Channel item, Channel source)
        {
            if (source != null)
            {
                item.Error = source.Error;
                item.LastRefreshTime = source.LastRefreshTime;
                item.Parameters = source.Parameters;

                if (item.Devices != null && item.Devices.Count > 0 && source.Devices != null && source.Devices.Count > 0)
                {
                    foreach (var device in item.Devices)
                    {
                        Device sourceDevice = source.Devices.FirstOrDefault(x => x.Path == device.Path);
                        UpdateDevice(device, sourceDevice);
                    }
                }
            }
            else
            {
                item.Error = "This channel could not be found on server.";
            }
        }

        private void UpdateDevice(Device item, Device source)
        {
            if (source != null)
            {
                item.LastRefreshTime = source.LastRefreshTime;
                item.Error = source.Error;
                item.LastRefreshTime = source.LastRefreshTime;
                item.Parameters = source.Parameters;

                if (item.Tags != null && item.Tags.Count > 0 && source.Tags != null && source.Tags.Count > 0)
                {
                    foreach (var tag in item.Tags)
                    {
                        Tag tagSource = source.Tags.FirstOrDefault(x => x.Path == tag.Path);
                        UpdateTag(tag, tagSource);
                    }
                }
            }
            else
            {
                item.Error = "This device could not be found on server.";
            }
        }

        private void UpdateTag(Tag item, Tag source)
        {
            if (source != null)
            {
                item.Value = source.Value;
                item.TimeStamp = source.TimeStamp;
                item.Quality = source.Quality;
                item.RefreshInterval = source.RefreshInterval;
                item.RefreshRate = source.RefreshRate;
                item.Address = source.Address;
                item.DataType = source.DataType;
                item.Error = source.Error;
                item.Parameters = source.Parameters;
                item.AccessPermission = source.AccessPermission;
            }
            else
            {
                item.Quality = Quality.Bad;
                item.Error = "This tag could not be found on server.";
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
            debugPath = "\\Debug\\TagFile.json";
            releasePath = "\\Release\\TagFile.json";
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
                    new DesignerActionMethodItem(this, "EditTagFile", "Edit Tag File", "Easy Scada", "Click here to update tag file", true)
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

        private void EditTagFile()
        {
            try
            {
                if (isBusy)
                    return;
                isBusy = true;
                FormTagFile formTagFile = new FormTagFile(this);
                formTagFile.ShowDialog();
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
                        DriverConnector driverConnector = JsonConvert.DeserializeObject<DriverConnector>(resJson);
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
                        DriverConnector driverConnector = JsonConvert.DeserializeObject<DriverConnector>(resJson);
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
