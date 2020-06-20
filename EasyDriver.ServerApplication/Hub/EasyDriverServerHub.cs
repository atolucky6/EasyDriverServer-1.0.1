using EasyDriverPlugin;
using EasyDriver.Client.Models;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyScada.ServerApplication
{
    public class EasyDriverServerHub : Hub<IEasyDriverClientApi>, IEasyDriverClientApi
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
            List<Station> stations = JsonConvert.DeserializeObject<List<Station>>(stationsJson);
            if (stations != null && stations.Count > 0)
            {
                if (Enum.TryParse(communicationMode, out CommunicationMode mode))
                {
                    ioc.ServerBroadcastService.ClientToCommunicationMode[Context.ConnectionId] = mode;
                    ioc.ServerBroadcastService.ClientToSubscribedStations[Context.ConnectionId] = stations;
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
            ioc.ServerBroadcastService.ClientToCommunicationMode.Remove(Context.ConnectionId);
            ioc.ServerBroadcastService.ClientToSubscribedStations.Remove(Context.ConnectionId);
            return "Ok";
        }
        public async Task<string> UnsubscribeAsync()
        {
            return await Task.Run(() => Unsubscribe());
        }

        #endregion

        #region Station

        public string GetStation(string pathToStation = "Local Station")
        {
            if (ioc.ProjectManagerService.CurrentProject != null)
                return JsonConvert.SerializeObject(ioc.ProjectManagerService.CurrentProject.GetItem<IStation>(pathToStation));
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
                var result = ioc.ProjectManagerService.CurrentProject.Childs?.Select(x => x as IStation)?.ToList();
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
                List<Station> stations = new List<Station>();
                foreach (var path in pathToStations)
                {
                    Station station = ioc.ProjectManagerService.CurrentProject.GetItem<Station>(path);
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
                return JsonConvert.SerializeObject(ioc.ProjectManagerService.CurrentProject.GetItem<IChannel>(pathToChannel));
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
                IStation station = ioc.ProjectManagerService.CurrentProject.GetItem<IStation>(pathToStation);
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
                return JsonConvert.SerializeObject(ioc.ProjectManagerService.CurrentProject.GetItem<IDevice>(pathToDevice));
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
                IChannel channel = ioc.ProjectManagerService.CurrentProject.GetItem<IChannel>(pathToChannel);
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
                return JsonConvert.SerializeObject(ioc.ProjectManagerService.CurrentProject.GetItem<ITag>(pathToTag));
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
                IDevice device = ioc.ProjectManagerService.CurrentProject.GetItem<IDevice>(pathToDevice);
                if (device != null)
                    return JsonConvert.SerializeObject(device.Tags?.Select(x => x as ITag));
            }
            return null;
        }
        public async Task<string> GetAllTagsAsync(string pathToDevice)
        {
            return await Task.Run(() => GetAllTags(pathToDevice));
        }

        #endregion

        #region Write

        public WriteResponse WriteTagValue(string pathToTag, string value, WritePiority writePiority = WritePiority.Default, WriteMode writeMode = WriteMode.WriteAllValue)
        {
            throw new NotImplementedException();
        }
        public WriteResponse WriteTagValue(WriteCommand writeCommand)
        {
            if (writeCommand != null)
                return WriteTagValue(writeCommand.PathToTag, writeCommand.Value, writeCommand.WritePiority, writeCommand.WriteMode);
            return null;
        }

        public Task<WriteResponse> WriteTagValueAsync(WriteCommand writeCommand)
        {
            return Task.Run(() => WriteTagValue(writeCommand));
        }
        public Task<WriteResponse> WriteTagValueAsync(string pathToTag, string value, WritePiority writePiority = WritePiority.Default, WriteMode writeMode = WriteMode.WriteAllValue)
        {
            return Task.Run(() => WriteTagValue(pathToTag, value, writePiority, writeMode));
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
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            ioc.ServerBroadcastService.ClientToCommunicationMode.Remove(Context.ConnectionId);
            ioc.ServerBroadcastService.ClientToSubscribedStations.Remove(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }

        #endregion
    }
}
