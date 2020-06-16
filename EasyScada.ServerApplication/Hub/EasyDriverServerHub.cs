using EasyDriverPlugin;
using EasyScada.Api.Interfaces;
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
        private readonly IoC ioc;

        public EasyDriverServerHub() : this(IoC.Instance) { }

        public EasyDriverServerHub(IoC ioc) { this.ioc = ioc; }

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

        public string WriteTagValue(string pathToTag, string value, byte piorityLevel = 255, WriteMode writeMode = WriteMode.WriteAllValue)
        {
            throw new NotImplementedException();
        }

        public Task<string> WriteTagValueAsync(string pathToTag, string value, byte piorityLevel = 255, WriteMode writeMode = WriteMode.WriteAllValue)
        {
            throw new NotImplementedException();
        }

        public override Task OnConnected()
        {
            string key = Context.QueryString["key"];
            //if (key == ioc.Key)
            //    Clients.Caller.GetAllStations();
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }
    }
}
