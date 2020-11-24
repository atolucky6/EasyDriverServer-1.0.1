using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections;
using EasyDriver.Opc.Client.Da;
using EasyDriver.Opc.Client.Common;
using EasyDriver.Opc.Client.Da.Browsing;

namespace EasyScada.ServerApplication
{
    public class OpcDaRemoteStationConnection : IRemoteConnection
    {
        #region Members

        public IStationCore Station { get; set; }
        public string RemoteAddress { get; private set; }
        public ushort Port { get; private set; }
        public string OpcDaServerName { get; set; }
        public bool IsDisposed { get; private set; }
        public bool IsSubscribed { get; set; }
        public OpcDaServer OpcDaServer { get; set; }

        private Task requestTask;
        readonly Hashtable cache;
        readonly Hashtable opcDaItemCache;
        readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private IOpcDaBrowser browser;
        private bool firstScan;

        #endregion

        #region Constructors

        public OpcDaRemoteStationConnection(IStationCore stationCore, OpcDaServer opcDaServer = null)
        {
            Station = stationCore;
            RemoteAddress = Station.RemoteAddress;
            Port = Station.Port;
            cache = new Hashtable();
            opcDaItemCache = new Hashtable();
            InitializeConnection(opcDaServer);
        }

        #endregion

        #region Methods

        private async void InitializeConnection(OpcDaServer opcDaServer = null)
        {
            try
            {
                if (!IsDisposed)
                {
                    if (opcDaServer != null && opcDaServer.IsConnected)
                    {
                        if (OpcDaServer == null)
                        {
                            OpcDaServer = opcDaServer;
                            OpcDaServer.ConnectionStateChanged += OnConnectionStateChanged;
                            OpcDaServer.Shutdown += OnServerShutdown;
                            OpcDaServer.GroupsChanged += OnGroupsChanged;
                            OnConnectionStateChanged(OpcDaServer, new OpcDaServerConnectionStateChangedEventArgs(OpcDaServer.IsConnected));
                        }
                    }
                    else
                    {
                        if (Station != null && Station.Parent.Childs.Contains(Station) && !IsDisposed)
                        {
                            if (OpcDaServer != null)
                            {
                                try
                                {
                                    OpcDaServer.ConnectionStateChanged -= OnConnectionStateChanged;
                                    OpcDaServer.Shutdown -= OnServerShutdown;
                                    OpcDaServer.GroupsChanged -= OnGroupsChanged;
                                    OpcDaServer?.Dispose();
                                    OpcDaServer = null;
                                }
                                catch { }
                            }

                            Uri url = new Uri(Station.ConnectionString);
                            OpcDaServer = new OpcDaServer(url);
                            OpcDaServer.ConnectionStateChanged += OnConnectionStateChanged;
                            OpcDaServer.Shutdown += OnServerShutdown;
                            OpcDaServer.GroupsChanged += OnGroupsChanged;
                            await OpcDaServer.ConnectAsync();
                        }
                    }
                }
            }
            catch
            {
                foreach (var item in Station.GetAllTags())
                {
                    if (item is ITagCore tag)
                    {
                        tag.TimeStamp = DateTime.Now;
                        tag.Quality = Quality.Bad;
                    }
                }

                Station.ConnectionStatus = ConnectionStatus.Disconnected;
                // Delay a little bit before reconnect
                Thread.Sleep(3000);
                // When connection disconnected we will init the new connection and tring to reconnect
                InitializeConnection();
            }
        }

        private void OnGroupsChanged(object sender, OpcDaServerGroupsChangedEventArgs e)
        {
        }

        private void OnServerShutdown(object sender, OpcShutdownEventArgs e)
        {
        }

        private void OnConnectionStateChanged(object sender, OpcDaServerConnectionStateChangedEventArgs e)
        {
            if (!IsDisposed)
            {
                if (e.IsConnected)
                {
                    Station.ConnectionStatus = ConnectionStatus.Connected;
                    // Delay a little bit before subscribe to server
                    Thread.Sleep(1000);
                    IsSubscribed = false;
                    // Initialize the request task if it null
                    if (requestTask == null)
                        requestTask = Task.Factory.StartNew(RefreshData, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                }
                else
                {
                    if (Station != null)
                    {
                        Station.ConnectionStatus = ConnectionStatus.Disconnected;
                        foreach (var item in Station.GetAllTags())
                        {
                            if (item is ITagCore tag)
                            {
                                tag.Quality = Quality.Bad;
                                tag.TimeStamp = DateTime.Now;
                            }
                        }
                    }

                    if (!IsDisposed)
                    {
                        // Delay a little bit before reconnect
                        Thread.Sleep(3000);
                        // When connection disconnected we will init the new connection and tring to reconnect
                        InitializeConnection();
                    }
                }
            }
        }

        private void RefreshData()
        {
            while (!IsDisposed)
            {
                int delay = 100;
                semaphore.Wait();
                try
                {
                    if (!IsSubscribed && OpcDaServer != null && OpcDaServer.IsConnected)
                    {
                        if (OpcDaServer.Groups.Count > 0)
                        {
                            foreach (OpcDaGroup oldGroup in OpcDaServer.Groups.ToArray())
                            {
                                OpcDaServer.RemoveGroup(oldGroup);
                            }
                        }

                        cache.Clear();
                        opcDaItemCache.Clear();

                        OpcDaGroup group = OpcDaServer.AddGroup(Guid.NewGuid().ToString());
                        group.IsActive = true;
                        List<OpcDaItemDefinition> definitions = new List<OpcDaItemDefinition>();

                        foreach (var item in Station.GetAllTags())
                        {
                            if (item is ITagCore tagCore)
                            {
                                string groupName = item.Parent.Path.Remove(0, Station.Name.Length + 1).Replace('/', '.');
                                string itemId = groupName + "." + tagCore.Name;
                                cache.Add(itemId, item);
                                OpcDaItemDefinition itemDefinition = new OpcDaItemDefinition
                                {
                                    ItemId = tagCore.ParameterContainer.Parameters["ItemId"].ToString(),
                                    IsActive = true
                                };
                                definitions.Add(itemDefinition);
                            }
                        }

                        if (definitions.Count > 0)
                        {
                            OpcDaItemResult[] results = group.AddItems(definitions.ToArray());
                        }

                        if (browser == null)
                            browser = new OpcDaBrowserAuto(OpcDaServer);

                        List<string> itemIds = new List<string>();
                        foreach (var item in cache.Keys)
                            itemIds.Add(item.ToString());

                        List<ITagCore> tags = new List<ITagCore>();
                        foreach (var item in cache.Values)
                        {
                            if (item is ITagCore tag)
                                tags.Add(tag);
                        }

                        foreach (var item in group.Items)
                            opcDaItemCache.Add(item.ItemId, item);

                        RefreshTagItemProperties(browser, itemIds, tags);
                        firstScan = false;
                        IsSubscribed = true;
                        delay = 100;
                    }

                    if (IsSubscribed)
                    {
                        if (Station != null && !IsDisposed &&
                            Station.Parent.Childs.Contains(Station) &&
                            OpcDaServer != null && OpcDaServer.IsConnected)
                        {
                            if (Station.CommunicationMode == CommunicationMode.RequestToServer &&
                                Station.Childs != null && Station.Childs.Count > 0 &&
                                OpcDaServer.IsConnected)
                            {
                                foreach (var group in OpcDaServer.Groups)
                                {
                                    if (group.IsActive)
                                    {
                                        if (group.Items.Count > 0)
                                        {
                                            OpcDaItemValue[] itemValues = group.Read(group.Items, OpcDaDataSource.Cache);
                                            foreach (OpcDaItemValue itemValue in itemValues)
                                            {
                                                if (cache[itemValue.Item.ItemId] is ITagCore tagCore)
                                                {
                                                    if (itemValue.Error.Succeeded)
                                                    {
                                                        if (!firstScan)
                                                        {
                                                            if (itemValue.Item != null)
                                                            {
                                                                tagCore.DataTypeName = itemValue.Item?.CanonicalDataType.Name;
                                                                tagCore.AccessPermission = GetAccessPermission(itemValue.Item.AccessRights);
                                                            }
                                                        }

                                                        tagCore.Quality = GetOpcItemQuality((int)itemValue.Quality.Status);
                                                        tagCore.TimeStamp = itemValue.Timestamp.DateTime;
                                                        Type valueType = itemValue.Value?.GetType();
                                                        if (valueType != null)
                                                        {
                                                            if (valueType == typeof(bool))
                                                            {
                                                                string boolStr = itemValue.Value?.ToString();
                                                                tagCore.Value = boolStr == "True" ? "1" : "0";
                                                            }
                                                            else
                                                            {
                                                                tagCore.Value = itemValue.Value?.ToString();
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        tagCore.Quality = Quality.Bad;
                                                        tagCore.TimeStamp = DateTime.Now;
                                                    }
                                                }
                                            }
                                            firstScan = true;
                                        }
                                    }
                                }
                            }
                            else { delay = 100; }
                        }
                    }

                    if (OpcDaServer == null)
                        Station.ConnectionStatus = ConnectionStatus.Disconnected;
                    else if (OpcDaServer != null && !OpcDaServer.IsConnected)
                        Station.ConnectionStatus = ConnectionStatus.Disconnected;
                    if (OpcDaServer != null && OpcDaServer.IsConnected)
                        Station.ConnectionStatus = ConnectionStatus.Connected;
                }
                catch (Exception)
                {
                    Station.ConnectionStatus = ConnectionStatus.Disconnected;
                    foreach (var item in Station.GetAllTags())
                    {
                        if (item is ITagCore tag)
                            tag.Quality = Quality.Bad;
                    }
                }
                finally
                {
                    semaphore.Release();
                    Thread.Sleep(delay);
                }
            }

            if (OpcDaServer != null)
            {
                OpcDaServer.Disconnect();
                OpcDaServer?.Dispose();
            }
        }

        private void RefreshTagItemProperties(IOpcDaBrowser browser, List<string> itemIds, List<ITagCore> tags)
        {
            Task.Factory.StartNew(() =>
            {
                if (itemIds.Count > 0 && itemIds.Count == tags.Count)
                {
                    OpcDaPropertiesQuery query = new OpcDaPropertiesQuery(
                        true,                                           // Alllow get value
                        (int)OpcDaItemPropertyIds.OPC_PROP_SCANRATE,    // Get scan rate
                        (int)OpcDaItemPropertyIds.OPC_PROP_ADRESS);     // Get address property
                    OpcDaItemProperties[] properties = browser.GetProperties(itemIds, query);
                    Debug.WriteLine("Get OPC DA items properties success.");
                    if (properties.Length == tags.Count)
                    {
                        for (int i = 0; i < tags.Count; i++)
                        {
                            ITagCore tag = tags[i];
                            OpcDaItemProperties tagProperties = properties[i];
                            foreach (var prop in tagProperties.Properties)
                            {
                                // DataType
                                if (prop.PropertyId == (int)OpcDaItemPropertyIds.OPC_PROP_CDT)
                                {
                                    if (prop.ErrorId.Succeeded && prop.Value != null)
                                        tag.DataTypeName = GetOpcItemDataType(prop.Value.ToString());
                                }
                                // AccessRights
                                else if (prop.PropertyId == (int)OpcDaItemPropertyIds.OPC_PROP_RIGHTS)
                                {
                                    if (prop.ErrorId.Succeeded && prop.Value != null)
                                    {
                                        AccessPermission permission = AccessPermission.ReadOnly;
                                        if (int.TryParse(prop.Value.ToString(), out int value))
                                        {
                                            if (value == 3)
                                                permission = AccessPermission.ReadAndWrite;
                                        }
                                        tag.AccessPermission = permission;
                                    }
                                }
                                // ScanRate
                                else if (prop.PropertyId == (int)OpcDaItemPropertyIds.OPC_PROP_SCANRATE)
                                {
                                    if (prop.ErrorId.Succeeded && prop.Value != null)
                                        tag.RefreshRate = int.Parse(prop.Value.ToString());
                                }
                                // Address
                                else if (prop.PropertyId == (int)OpcDaItemPropertyIds.OPC_PROP_ADRESS)
                                {
                                    if (prop.ErrorId.Succeeded && prop.Value != null)
                                        tag.Address = prop.Value.ToString();
                                }
                            }
                        }
                    }
                }
            });
        }

        public void Dispose()
        {
            IsDisposed = true;
        }

        public async Task<WriteResponse> WriteTagValue(WriteCommand writeCommand, WriteResponse response)
        {
            response.WriteCommand = writeCommand;
            if (OpcDaServer != null && OpcDaServer.IsConnected)
            {
                try
                {
                    await semaphore.WaitAsync();

                    if (OpcDaServer.Groups.Count > 0)
                    {
                        string tagId = $"{writeCommand.Prefix?.Replace('/', '.')}.{writeCommand.TagName}";
                        OpcDaGroup group = OpcDaServer.Groups[0];
                        if (opcDaItemCache[tagId] is OpcDaItem opcDaItem)
                        {
                            response.ExecuteTime = DateTime.Now;
                            HRESULT[] results = group.Write(new OpcDaItem[] { opcDaItem }, new object[] { writeCommand.Value });
                            if (results != null && results.Length > 0)
                            {
                                response.IsSuccess = results[0].Succeeded;
                                response.WriteCommand = writeCommand;
                            }
                            else
                            {
                                response.IsSuccess = false;
                                response.WriteCommand = writeCommand;
                                response.Error = "The OPC DA server doesn't response the write result.";
                            }
                        }
                    }
                    else
                    {
                        response.Error = $"The tag '{writeCommand.Prefix}/{writeCommand.TagName}' was not contains in group.";
                    }

                    return response;
                }
                catch (Exception)
                {
                    response.Error = "Some error occur when send write command to OPC DA server.";
                    return response;
                }
                finally { semaphore.Release(); }
            }
            else
            {
                response.Error = "The connection state of OPC DA server was disconnected";
            }
            return response;
        }

        #endregion

        #region Utils

        private AccessPermission GetAccessPermission(OpcDaAccessRights accessRight)
        {
            switch (accessRight)
            {
                case OpcDaAccessRights.Read:
                    return AccessPermission.ReadOnly;
                case OpcDaAccessRights.Ignore:
                case OpcDaAccessRights.ReadWrite:
                case OpcDaAccessRights.Write:
                    return AccessPermission.ReadAndWrite;
                default:
                    break;
            }
            return AccessPermission.ReadOnly;
        }

        private Quality GetOpcItemQuality(int value)
        {
            // For more information https://www.opcsupport.com/s/article/What-are-the-OPC-Quality-Codes
            Quality quality = Quality.Uncertain;
            if (value >= 0 && value <= 28)
                quality = Quality.Bad;
            else if (value >= 192 && value <= 219)
                quality = Quality.Good;
            else if (value >= 65536 && value <= 65564)
                quality = Quality.Bad;
            return quality;
        }

        private string GetOpcItemDataType(string value)
        {
            string dataType = "Unknown";
            switch (value)
            {
                case "11":
                    return "Bool";
                case "8230":
                    return "Bool Array";
                case "17":
                    return "Byte";
                case "8209":
                    return "Byte Array";
                case "2":
                    return "Short";
                case "8194":
                    return "Short Array";
                case "18+":
                    return "BCD";
                case "18":
                    return "Word";
                case "8210":
                    return "Word Array";
                case "3":
                    return "Long";
                case "8195":
                    return "Long Array";
                case "19":
                    return "DWord";
                case "8211":
                    return "DWord";
                case "4":
                    return "Float";
                case "8196":
                    return "Float Array";
                case "5":
                    return "Double";
                case "8197":
                    return "Double Array";
                case "20":
                    return "LLong";
                case "8212":
                    return "LLong Array";
                case "21":
                    return "QWord";
                case "8213":
                    return "QWord Array";
                case "16":
                    return "Char";
                case "8":
                    return "String";
                default:
                    break;
            }
            return dataType;
        }

        #endregion
    }
}
