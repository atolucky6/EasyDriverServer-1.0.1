using EasyDriver.Core;
using EasyDriverPlugin;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EasyScada.ServerApplication
{
    public interface IServerBroadcastService
    {
        BroadcastMode BroadcastMode { get; set; }
        int BroadcastRate { get; set; }
        IHubConnectionContext<dynamic> Clients { get; }
        List<BroadcastEndpoint> BroadcastEndpoints { get; }
        void AddEndpoint(string connectionId, List<StationClient> subscribeData, CommunicationMode communicationMode, int refreshRate);
        void RemoveEndpoint(string connectionId);
        long BroadcastExecuteInterval { get; }
    }

    public class ServerBroadcastService : IServerBroadcastService, IDisposable
    {
        #region Members

        readonly Stopwatch stopwatch;
        readonly IProjectManagerService projectManagerService;
        readonly ApplicationViewModel applicationViewModel;
        readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        readonly Task broadcastTask;

        public IHubConnectionContext<dynamic> Clients { get; private set; }

        public List<BroadcastEndpoint> BroadcastEndpoints { get; private set; }

        public bool IsDisposed { get; protected set; }

        public long BroadcastExecuteInterval { get; private set; }

        public BroadcastMode BroadcastMode { get; set; }
        public int BroadcastRate { get; set; }

        #endregion

        #region Constructors

        public ServerBroadcastService(
            IProjectManagerService projectManagerService, 
            ApplicationViewModel applicationViewModel,
            BroadcastMode broadcastMode,
            int broadcastRate)
        {
            BroadcastMode = broadcastMode;
            BroadcastRate = broadcastRate;
            this.projectManagerService = projectManagerService;
            this.applicationViewModel = applicationViewModel;
            Clients = GlobalHost.ConnectionManager.GetHubContext<EasyDriverServerHub>().Clients;
            BroadcastEndpoints = new List<BroadcastEndpoint>();
            stopwatch = new Stopwatch();
            broadcastTask = Task.Factory.StartNew(BroadcastToClients, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        #endregion

        #region Methods

        private void BroadcastToClients()
        {
            while (!IsDisposed)
            {
                stopwatch.Restart();
                try
                {
                    semaphore.Wait();

                    if (projectManagerService.CurrentProject != null && BroadcastEndpoints.Count > 0)
                    {
                        switch (BroadcastMode)
                        {
                            case BroadcastMode.SendAskedData:
                                foreach (var endpoint in BroadcastEndpoints)
                                    endpoint.Send();
                                break;
                            case BroadcastMode.SendAllData:
                                using (StringWriter sw = new StringWriter())
                                {
                                    JsonTextWriter writer = new JsonTextWriter(sw);
                                    writer.WriteStartArray();
                                    foreach (var item in projectManagerService.CurrentProject.Childs)
                                    {
                                        if (item is IStationCore station)
                                            writer.WriteStationCoreJson(station);
                                    }
                                    writer.WriteEndArray();
                                    List<string> receivedClients = new List<string>(BroadcastEndpoints.Select(x => x.ConnectionId));
                                    Clients.Clients(receivedClients).broadcastSubscribeData(sw.ToString());
                                    writer.Close();
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch { }
                finally
                {
                    stopwatch.Stop();
                    BroadcastExecuteInterval = stopwatch.ElapsedMilliseconds;
                    long delay = BroadcastRate - stopwatch.ElapsedMilliseconds;
                    if (projectManagerService.CurrentProject == null)
                        delay = 100;
                    semaphore.Release();
                    Thread.Sleep((int)(delay < 0 ? 100 : delay));
                }
            }
        }

        private void BroadcastCallback(object state)
        {
            IsDisposed = true;
        }

        public void AddEndpoint(string connectionId, List<StationClient> subscribeData, CommunicationMode communicationMode, int refreshRate)
        {
            semaphore.Wait();
            try
            {
                if (BroadcastEndpoints.FirstOrDefault(x => x.ConnectionId == connectionId) is BroadcastEndpoint endpoint)
                {
                    endpoint.Update(communicationMode, refreshRate, subscribeData);
                }
                else
                {
                    endpoint = new BroadcastEndpoint(Clients, connectionId, projectManagerService)
                    {
                        CommunicationMode = communicationMode,
                        RereshRate = refreshRate,
                        SubscribeData = subscribeData,
                    };
                    BroadcastEndpoints.Add(endpoint);
                }
            }
            catch { }
            finally { semaphore.Release(); }         
        }

        public void RemoveEndpoint(string connectionId)
        {
            semaphore.Wait();
            try
            {
                BroadcastEndpoint endpoint = BroadcastEndpoints.FirstOrDefault(x => x.ConnectionId == connectionId);
                if (endpoint != null)
                    BroadcastEndpoints.Remove(endpoint);
            }
            catch { }
            finally { semaphore.Release(); }
        }

        private BroadcastEndpoint GetEndpoint(string connectionId)
        {
            return BroadcastEndpoints.FirstOrDefault(x => x.ConnectionId == connectionId);
        }

        private string GetAllStationsJson()
        {
            if (IoC.Instance.ProjectManagerService.CurrentProject != null)
            {
                var result = IoC.Instance.ProjectManagerService.CurrentProject.Childs?.Select(x => x as IStationClient)?.ToList();
                return JsonConvert.SerializeObject(result);
            }
            return null;
        }

        private async Task<string> GetAllStationsJsonAsync()
        {
            return await Task.Run(() => GetAllStationsJson());
        }

        private async Task<List<StationClient>> GetListStationsAsync()
        {
            return await Task.Run(() => JsonConvert.DeserializeObject<List<StationClient>>(GetAllStationsJson()));
        }

        public void Dispose()
        {
            IsDisposed = true;
            semaphore.Wait();
            broadcastTask?.Dispose();
            semaphore.Release();
            semaphore.Dispose();
        }

        #endregion
    }

    public class BroadcastEndpoint
    {
        public List<StationClient> SubscribeData { get; set; }
        public CommunicationMode CommunicationMode { get; set; }
        public DateTime LastBroadcastTime { get; set; }
        public int RereshRate { get; set; }
        public IHubConnectionContext<dynamic> Clients { get; private set; }
        public string ConnectionId { get; private set; }
        public IProjectManagerService ProjectManagerService { get; private set; }
        public long BroadcastExecuteInterval { get; private set; }
        readonly Stopwatch sw;
        readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
       
        public BroadcastEndpoint(
            IHubConnectionContext<dynamic> clients, 
            string connectionId, 
            IProjectManagerService projectManagerService)
        {
            Clients = clients;
            ConnectionId = connectionId;
            ProjectManagerService = projectManagerService;
            sw = new Stopwatch();
        }

        public void Send()
        {
            sw.Restart();
            try
            {
                semaphore.Wait();
                if (CommunicationMode == CommunicationMode.ReceiveFromServer)
                {
                    if (Math.Abs((DateTime.Now - LastBroadcastTime).TotalMilliseconds) > RereshRate)
                    {
                        Clients.Client(ConnectionId).broadcastSubscribeData(GetBroadcastResponse());
                        LastBroadcastTime = DateTime.Now;
                    }
                }
            }
            catch { }
            finally
            {
                sw.Stop();
                BroadcastExecuteInterval = sw.ElapsedMilliseconds;
                semaphore.Release();
            }
        }

        public string GetBroadcastResponse()
        {
            using (StringWriter sw = new StringWriter())
            {
                JsonTextWriter writer = new JsonTextWriter(sw);
                writer.WriteStartArray();

                if (ProjectManagerService.CurrentProject != null)
                {
                    if (SubscribeData != null && SubscribeData.Count > 0)
                    {
                        foreach (var station in SubscribeData)
                        {
                            if (station != null)
                            {
                                IStationCore stationCore = ProjectManagerService.CurrentProject.FirstOrDefault(x => x.Path == station.Path) as IStationCore;
                                UpdateStation(station, stationCore);
                            }
                        }
                        foreach (var item in SubscribeData)
                            JsonUtils.WriteStationClientJson(writer, item);
                    }
                }
                writer.WriteEndArray();
                return sw.ToString();
            }
        }

        public void Update(CommunicationMode communicationMode, int refreshRate, List<StationClient> subscribeData)
        {
            semaphore.Wait();
            CommunicationMode = communicationMode;
            RereshRate = refreshRate;
            SubscribeData = subscribeData;
            semaphore.Release();
        }

        private void UpdateStation(StationClient station, IStationCore stationCore)
        {
            if (stationCore != null)
            {
                station.CommunicationMode = stationCore.CommunicationMode;
                station.Error = stationCore.CommunicationError;
                station.RefreshRate = stationCore.RefreshRate;
                station.RemoteAddress = stationCore.RemoteAddress;
                station.Port = stationCore.Port;
                station.Parameters = stationCore.ParameterContainer.Parameters;
                station.LastRefreshTime = stationCore.LastRefreshTime;
                station.ConnectionStatus = stationCore.ConnectionStatus;

                if (station.RemoteStations != null && station.RemoteStations.Count > 0)
                {
                    foreach (StationClient remoteStation in station.RemoteStations)
                    {
                        if (remoteStation != null)
                        {
                            IStationCore remoteStationCore = stationCore.FirstOrDefault(x => x.Path == remoteStation.Path) as IStationCore;
                            UpdateStation(remoteStation, remoteStationCore);
                        }
                    }
                }

                if (station.Channels != null && station.Channels.Count > 0)
                {
                    foreach (ChannelClient channel in station.Channels)
                    {
                        if (channel != null)
                        {
                            IChannelCore channelCore = stationCore.FirstOrDefault(x => x.Path == channel.Path) as IChannelCore;
                            UpdateChannel(channel, channelCore);
                        }
                    }
                }
            }
            else
            {
                station.Error = "This station could not be found on server.";
            }
        }

        private void UpdateChannel(ChannelClient channel, IChannelCore channelCore)
        {
            if (channelCore != null)
            {
                channel.LastRefreshTime = channelCore.LastRefreshTime;
                channel.Parameters = channelCore.ParameterContainer.Parameters;
                channel.Error = channelCore.CommunicationError;

                if (channel.Devices != null && channel.Devices.Count > 0)
                {
                    foreach (var device in channel.Devices)
                    {
                        if (device != null)
                        {
                            IDeviceCore deviceCore = channelCore.FirstOrDefault(x => x.Path == device.Path) as IDeviceCore;
                            UpdateDevice(device, deviceCore);
                        }
                    }
                }
            }
            else
            {
                channel.Error = "This channel could not be found on server.";
            }
        }

        private void UpdateDevice(DeviceClient device, IDeviceCore deviceCore)
        {
            if (deviceCore != null)
            {
                device.LastRefreshTime = deviceCore.LastRefreshTime;
                device.Parameters = deviceCore.ParameterContainer.Parameters;
                device.Error = deviceCore.CommunicationError;

                if (device.Tags != null && device.Tags.Count > 0)
                {
                    List<ICoreItem> tagCores = deviceCore.Childs.Select(x => x as ICoreItem).ToList();
                    foreach (var tag in device.Tags)
                    {
                        if (tag != null)
                        {
                            ITagCore tagCore = tagCores.FirstOrDefault(x => x.Path == tag.Path) as ITagCore;
                            if (tagCore != null)
                                tagCore.Remove(tagCore);
                            UpdateTag(tag, tagCore);
                        }
                    }
                }
            }
            else
            {
                device.Error = "This device could not be found on server.";
            }

        }

        private void UpdateTag(TagClient tag, ITagCore tagCore)
        {
            if (tagCore != null)
            {
                tag.Address = tagCore.Address;
                tag.DataType = tagCore.DataTypeName;
                tag.AccessPermission = tagCore.AccessPermission;
                tag.TimeStamp = tagCore.TimeStamp;
                tag.Value = tagCore.Value;
                tag.Quality = tagCore.Quality;
                tag.Parameters = tagCore.ParameterContainer.Parameters;
                tag.RefreshInterval = tagCore.RefreshInterval;
                tag.RefreshRate = tagCore.RefreshRate;
                tag.Error = tagCore.CommunicationError;
            }
            else
            {
                tag.Error = "This tag could not be found on server.";
            }
        }
    }
}