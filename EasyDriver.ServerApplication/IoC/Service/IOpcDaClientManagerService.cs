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
using EasyDriver.Opc.Client.Da;
using EasyDriver.Opc.Client.Common;

namespace EasyScada.ServerApplication
{
    public interface IOpcDaClientManagerService
    {
        Dictionary<IStationCore, OpcDaRemoteStationConnection> ConnectionDictonary { get; }
        void AddConnection(IStationCore stationCore, OpcDaServer opcDaServer = null);
        void RemoveConnection(IStationCore stationCore);
        void ReloadConnection(IStationCore stationCore);
    }

    public class OpcDaClientManagerService : IOpcDaClientManagerService
    {
        public OpcDaClientManagerService()
        {
            ConnectionDictonary = new Dictionary<IStationCore, OpcDaRemoteStationConnection>();
        }

        public Dictionary<IStationCore, OpcDaRemoteStationConnection> ConnectionDictonary { get; private set; }

        public async void AddConnection(IStationCore stationCore, OpcDaServer opcDaServer = null)
        {
            await Task.Run(() =>
            {
                if (!ConnectionDictonary.ContainsKey(stationCore) && stationCore != null)
                {
                    Thread.Sleep(100);
                    ConnectionDictonary[stationCore] = new OpcDaRemoteStationConnection(stationCore, opcDaServer);
                }
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

    public class OpcDaRemoteStationConnection : IDisposable
    {
        #region Members

        public IStationCore StationCore { get; set; }
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

        #endregion

        #region Constructors

        public OpcDaRemoteStationConnection(IStationCore stationCore, OpcDaServer opcDaServer = null)
        {
            StationCore = stationCore;
            RemoteAddress = StationCore.RemoteAddress;
            Port = StationCore.Port;
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
                        if (StationCore != null && StationCore.Parent.Contains(StationCore) && !IsDisposed)
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

                            Uri url = new Uri(StationCore.OpcDaServerName);
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
                foreach (var item in GetAllChildItems(StationCore))
                {
                    if (item is ITagCore tag)
                        tag.Quality = Quality.Bad;
                }

                StationCore.ConnectionStatus = ConnectionStatus.Disconnected;
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
                    StationCore.ConnectionStatus = ConnectionStatus.Connected;
                    // Delay a little bit before subscribe to server
                    Thread.Sleep(1000);
                    IsSubscribed = false;
                    // Initialize the request task if it null
                    if (requestTask == null)
                        requestTask = Task.Factory.StartNew(RefreshData, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                }
                else
                {
                    if (StationCore != null)
                    {
                        StationCore.ConnectionStatus = ConnectionStatus.Disconnected;
                        foreach (var item in GetAllChildItems(StationCore))
                        {
                            if (item is ITagCore tag)
                                tag.Quality = Quality.Bad;
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
                        foreach (var item in GetAllChildItems(StationCore))
                        {
                            if (item is ITagCore tagCore)
                            {
                                cache.Add(item.Path.Remove(0, StationCore.Name.Length + 1).Replace('/', '.'), item);
                                OpcDaItemDefinition itemDefinition = new OpcDaItemDefinition();
                                itemDefinition.ItemId = tagCore.ParameterContainer.Parameters["ItemId"].ToString();
                                itemDefinition.IsActive = true;
                                definitions.Add(itemDefinition);
                            }
                        }
                        OpcDaItemResult[] results = group.AddItems(definitions.ToArray());
                        foreach (OpcDaItem item in group.Items)
                        {
                            opcDaItemCache.Add(item.ItemId, item);
                        }
                        IsSubscribed = true;
                        delay = 100;
                    }

                    if (IsSubscribed)
                    {
                        if (StationCore != null && !IsDisposed && 
                            StationCore.Parent.Contains(StationCore) && 
                            OpcDaServer != null && OpcDaServer.IsConnected)
                        {
                            if (StationCore.CommunicationMode == CommunicationMode.RequestToServer &&
                                StationCore.Childs != null && StationCore.Childs.Count > 0 &&
                                OpcDaServer.IsConnected)
                            {
                                foreach (var group in OpcDaServer.Groups)
                                {
                                    if (group.IsActive)
                                    {
                                        OpcDaItemValue[] itemValues = group.Read(group.Items, OpcDaDataSource.Device);
                                        foreach (OpcDaItemValue itemValue in itemValues)
                                        {
                                            if (cache[itemValue.Item.ItemId] is ITagCore tagCore)
                                            {
                                                if (itemValue.Error.Succeeded)
                                                {
                                                    tagCore.Quality = GetOpcItemQuality((int)itemValue.Quality.Status);
                                                    tagCore.TimeStamp = itemValue.Timestamp.DateTime;
                                                    Type valueType = itemValue.Value.GetType();
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
                                                else
                                                {
                                                    tagCore.Quality = Quality.Bad;
                                                    tagCore.TimeStamp = DateTime.Now;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else { delay = 100; }
                        }
                    }

                    if (OpcDaServer == null)
                        StationCore.ConnectionStatus = ConnectionStatus.Disconnected;
                    else if (OpcDaServer != null && !OpcDaServer.IsConnected)
                        StationCore.ConnectionStatus = ConnectionStatus.Disconnected;
                    if (OpcDaServer != null && OpcDaServer.IsConnected)
                        StationCore.ConnectionStatus = ConnectionStatus.Connected;
                }
                catch
                {
                    StationCore.ConnectionStatus = ConnectionStatus.Disconnected;
                    foreach (var item in GetAllChildItems(StationCore))
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

        public void Dispose()
        {
            IsDisposed = true;
        }

        public async Task<WriteResponse> WriteTagValue(WriteCommand writeCommand)
        {
            WriteResponse response = new WriteResponse();
            response.WriteCommand = writeCommand;
            if (OpcDaServer != null && OpcDaServer.IsConnected)
            {
                try
                {
                    await semaphore.WaitAsync();

                    if (OpcDaServer.Groups.Count > 0)
                    {
                        string tagId = writeCommand.PathToTag.Replace('/', '.');
                        OpcDaGroup group = OpcDaServer.Groups[0];
                        if (opcDaItemCache[tagId] is OpcDaItem opcDaItem)
                        {
                            HRESULT[] results = group.Write(new OpcDaItem[] { opcDaItem }, new object[] { writeCommand.Value });
                            if (results != null && results.Length > 0)
                            {
                                response.ExecuteTime = DateTime.Now;
                                response.IsSuccess = results[0].Succeeded;
                                response.WriteCommand = writeCommand;
                            }
                            else
                            {
                                response.ExecuteTime = DateTime.Now;
                                response.IsSuccess = false;
                                response.WriteCommand = writeCommand;
                                response.Error = "The OPC DA server doesn't response the write result.";
                            }
                        }
                    }
                    else
                    {
                        response.Error = $"The tag '{writeCommand.PathToTag}' was not contains in group.";
                    }
                    
                    return response;
                }
                catch (Exception ex)
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
