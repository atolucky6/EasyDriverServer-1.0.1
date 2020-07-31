using EasyDriverPlugin;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using DevExpress.Mvvm.POCO;
using EasyDriver.Core;

namespace EasyScada.ServerApplication
{
    public class EasyDriverServerHub : Hub
    {
        #region Members

        private readonly IoC ioc;

        #endregion

        #region Constructors

        public EasyDriverServerHub() : this(IoC.Instance) { }

        public EasyDriverServerHub(IoC ioc) { this.ioc = ioc; }

        #endregion

        #region Authentication 

        public bool Login(string password)
        {
            return false;
        }

        public bool Logout()
        {
            return true;
        }

        #endregion

        #region Subscribe/Unsubscribe

        public string Subscribe(string stationsJson, string communicationMode, int refreshRate)
        {
            List<StationClient> stations = JsonConvert.DeserializeObject<List<StationClient>>(stationsJson);
            if (stations != null && stations.Count > 0)
            {
                if (Enum.TryParse(communicationMode, out CommunicationMode comMode))
                {
                    ioc.ServerBroadcastService.AddEndpoint(Context.ConnectionId, stations, comMode, refreshRate);
                    return "Ok";
                }
            }
            return "Fail";
        }
        public async Task<string> SubscribeAsync(string stationsJson, string communicationMode, int refreshRate)
        {
            return await Task.Run(() => Subscribe(stationsJson, communicationMode, refreshRate));
        }

        public string Unsubscribe()
        {
            ioc.ServerBroadcastService.RemoveEndpoint(Context.ConnectionId);
            return "Ok";
        }
        public async Task<string> UnsubscribeAsync()
        {
            return await Task.Run(() => Unsubscribe());
        }

        public string GetSubscribedData()
        {
            if (ioc.ServerBroadcastService.BroadcastEndpoints.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId) is BroadcastEndpoint endpoint)
            {
                return endpoint.GetBroadcastResponse();
            }
            return string.Empty;
        }

        public async Task<string> GetSubscribedDataAsync()
        {
            return await Task.Run(() => GetSubscribedData());
        }

        #endregion

        #region Station

        public string GetStation(string pathToStation = "Local Station")
        {
            if (ioc.ProjectManagerService.CurrentProject != null)
                return JsonConvert.SerializeObject(ioc.ProjectManagerService.CurrentProject.GetItem<IStationClient>(pathToStation));
            return null;
        }
        public async Task<string> GetStationAsync(string pathToStation = "Local Station")
        {
            return await Task.Run(() => GetStation());
        }

        public string GetAllStations()
        {
            if (ioc.ProjectManagerService.CurrentProject != null)
            {
                var result = ioc.ProjectManagerService.CurrentProject.Childs?.Select(x => x as IStationClient)?.ToList();
                return JsonConvert.SerializeObject(result, Formatting.Indented);
            }
            return null;
        }
        public async Task<string> GetAllStationsAsync()
        {
            return await Task.Run(() => GetAllStations());
        }

        public string GetStations(IEnumerable<string> pathToStations)
        {
            if (pathToStations != null && ioc.ProjectManagerService.CurrentProject != null)
            {
                List<IStationClient> stations = new List<IStationClient>();
                foreach (var path in pathToStations)
                {
                    IStationClient station = ioc.ProjectManagerService.CurrentProject.GetItem<IStationClient>(path);
                    if (station != null)
                        stations.Add(station);
                }
                return JsonConvert.SerializeObject(stations);
            }
            return null;
        }
        public async Task<string> GetStationsAsync(IEnumerable<string> pathToStations)
        {
            return await Task.Run(() => GetStations(pathToStations));
        }

        #endregion

        #region Channel

        public string GetChannel(string pathToChannel)
        {
            if (ioc.ProjectManagerService.CurrentProject != null)
                return JsonConvert.SerializeObject(ioc.ProjectManagerService.CurrentProject.GetItem<IChannelClient>(pathToChannel));
            return null;
        }
        public async Task<string> GetChannelAsync(string pathToChannel)
        {
            return await Task.Run(() => GetChannel(pathToChannel));
        }

        public string GetAllChannels(string pathToStation = "Local Station")
        {
            if (ioc.ProjectManagerService.CurrentProject != null)
            {
                IStationClient station = ioc.ProjectManagerService.CurrentProject.GetItem<IStationClient>(pathToStation);
                if (station != null)
                    return JsonConvert.SerializeObject(station.Channels);
            }
            return null;
        }
        public async Task<string> GetAllChannelsAsync(string pathToStation = "Local Station")
        {
            return await Task.Run(() => GetAllChannels(pathToStation));
        }

        public string GetChannels(IEnumerable<string> pathToChannels)
        {
            throw new NotImplementedException();
        }
        public Task<string> GetChannelsAsync(IEnumerable<string> pathToChannels)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Device

        public string GetDevice(string pathToDevice)
        {
            if (ioc.ProjectManagerService.CurrentProject != null)
                return JsonConvert.SerializeObject(ioc.ProjectManagerService.CurrentProject.GetItem<IDeviceClient>(pathToDevice));
            return null;
        }
        public async Task<string> GetDeviceAsync(string pathToDevice)
        {
            return await Task.Run(() => GetDevice(pathToDevice));
        }

        public string GetAllDevices(string pathToChannel)
        {
            if (ioc.ProjectManagerService.CurrentProject != null)
            {
                IChannelClient channel = ioc.ProjectManagerService.CurrentProject.GetItem<IChannelClient>(pathToChannel);
                if (channel != null)
                    return JsonConvert.SerializeObject(channel.Devices);
            }
            return null;
        }
        public async Task<string> GetAllDevicesAsync(string pathToChannel)
        {
            return await Task.Run(() => GetAllDevices(pathToChannel));
        }

        public string GetDevices(IEnumerable<string> pathToDevices)
        {
            throw new NotImplementedException();
        }
        public Task<string> GetDevicesAsync(IEnumerable<string> pathToDevices)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Tag

        public string GetTag(string pathToTag)
        {
            if (ioc.ProjectManagerService.CurrentProject != null)
                return JsonConvert.SerializeObject(ioc.ProjectManagerService.CurrentProject.GetItem<ITagClient>(pathToTag));
            return null;
        }
        public async Task<string> GetTagAsync(string pathToTag)
        {
            return await Task.Run(() => GetTag(pathToTag));
        }

        public string GetAllTags(string pathToDevice)
        {
            if (ioc.ProjectManagerService.CurrentProject != null)
            {
                IDeviceClient device = ioc.ProjectManagerService.CurrentProject.GetItem<IDeviceClient>(pathToDevice);
                if (device != null)
                    return JsonConvert.SerializeObject(device.Tags?.Select(x => x as ITagClient));
            }
            return null;
        }
        public async Task<string> GetAllTagsAsync(string pathToDevice)
        {
            return await Task.Run(() => GetAllTags(pathToDevice));
        }

        #endregion

        #region Write

        public async Task<WriteResponse> WriteTagValueAsync(WriteCommand writeCommand)
        {
            return await ioc.Get<ITagWriterService>().WriteTag(writeCommand); 
        }

        #endregion

        #region Utils

        public string SetChannelParameters(string jsonValue, string pathToChannel)
        {
            throw new NotImplementedException();
        }
        public string SetChannelParametersAsync(string jsonValue, string pathToChannel)
        {
            throw new NotImplementedException();
        }

        public string SetDeviceParameters(string jsonValue, string pathToDevice)
        {
            throw new NotImplementedException();
        }
        public Task<string> SetDeviceParametersAsync(string jsonValue, string pathToChannel)
        {
            throw new NotImplementedException();
        }

        public string SetTagParameters(string jsonValue, string pathToTag)
        {
            throw new NotImplementedException();
        }
        public Task<string> SetTagParametersAsync(string jsonValue, string pathToTag)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Lifecycle handler

        public override Task OnConnected()
        {
            if (!ioc.ApplicationViewModel.ConnectedClients.Contains(Context.ConnectionId))
            {
                ioc.ApplicationViewModel.ConnectedClients.Add(Context.ConnectionId);
                ioc.ApplicationViewModel.RaisePropertyChanged(x => x.TotalConnectedClients);
            }
            Debug.WriteLine($"Client {Context.ConnectionId} connected");
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            if (ioc.ApplicationViewModel.ConnectedClients.Contains(Context.ConnectionId))
            {
                ioc.ApplicationViewModel.ConnectedClients.Remove(Context.ConnectionId);
                ioc.ApplicationViewModel.RaisePropertyChanged(x => x.TotalConnectedClients);
            }
            Debug.WriteLine($"Client {Context.ConnectionId} disconnected");
            ioc.ServerBroadcastService.RemoveEndpoint(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            if (ioc.ApplicationViewModel.ConnectedClients.Contains(Context.ConnectionId))
            {
                ioc.ApplicationViewModel.ConnectedClients.Remove(Context.ConnectionId);
                ioc.ApplicationViewModel.RaisePropertyChanged(x => x.TotalConnectedClients);
            }
            Debug.WriteLine($"Client {Context.ConnectionId} reconnected");
            return base.OnReconnected();
        }

        #endregion
    }
}
