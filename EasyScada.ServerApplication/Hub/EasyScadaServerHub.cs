using EasyDriverPlugin;
using EasyScada.Api.Interfaces;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyScada.ServerApplication
{
    public class EasyScadaServerHub : Hub<IEasyScadaClientApi>, IEasyScadaClientApi
    {
        private readonly IoC ioc;

        public EasyScadaServerHub() : this(IoC.Instance) { }

        public EasyScadaServerHub(IoC ioc) { this.ioc = ioc; }

        public IEnumerable<IChannel> GetAllChannels(string stationName = "Local Station")
        {
            if (ioc.ProjectManagerService.CurrentProject != null)
            {
                if (ioc.ProjectManagerService.CurrentProject.Childs.FirstOrDefault(x => x.Name == stationName) is IStation station)
                    return station.Channels;
            }
            return null;
        }

        public async Task<IEnumerable<IChannel>> GetAllChannelsAsync(string stationName = "Local Station")
        {
            return await Task.Run(() => GetAllChannels(stationName));
        }

        public IEnumerable<IDevice> GetAllDevices(string channelName, string stationName = "Local Station")
        {
            IChannel channel = GetChannel(channelName);
            if (channel != null)
                return channel.Devices;
            return null;
        }

        public async Task<IEnumerable<IDevice>> GetAllDevicesAsync(string channelName, string stationName = "Local Station")
        {
            return await Task.Run(() => GetAllDevices(channelName, stationName));
        }

        public IEnumerable<IStation> GetAllStations()
        {
            if (ioc.ProjectManagerService.CurrentProject != null)
            {
                return ioc.ProjectManagerService.CurrentProject.Childs.Select(x => x as IStation);
            }
            return null;
        }

        public async Task<IEnumerable<IStation>> GetAllStationsAsync()
        {
            return await Task.Run(() => GetAllStations());
        }

        public IChannel GetChannel(string channelName, string stationName = "Local Station")
        {
            IStation station = GetStation(stationName);
            if (station != null)
            {
                return station.Channels.FirstOrDefault(x => x.Name == channelName);
            }
            return null;
        }

        public async Task<IChannel> GetChannelAsync(string channelName, string stationName = "Local Station")
        {
            return await Task.Run(() => GetChannel(channelName, stationName));
        }

        public IDevice GetDevice(string deviceName, string channelName, string stationName = "Local Station")
        {
            IChannel channel = GetChannel(channelName, stationName);
            if (channel != null)
            {
                return channel.Devices.FirstOrDefault(x => x.Name == deviceName);
            }
            return null;
        }

        public async Task<IDevice> GetDeviceAsync(string deviceName, string channelName, string stationName = "Local Station")
        {
            return await Task.Run(() => GetDevice(deviceName, channelName, stationName));
        }


        public IStation GetStation(string stationName = "Local Station")
        {
            if (ioc.ProjectManagerService.CurrentProject != null)
            {
                return ioc.ProjectManagerService.CurrentProject.Childs.FirstOrDefault(x => x.Name == stationName) as IStation;
            }
            return null;
        }

        public async Task<IStation> GetStationAsync(string stationName = "Local Station")
        {
            return await Task.Run(() => GetStation(stationName));
        }

        public string SetChannelParameters(string jsonValue, string channelName, string stationName = "Local Station")
        {
            throw new NotImplementedException();
        }

        public string SetChannelParametersAsync(string jsonValue, string channelName, string stationName = "Local Station")
        {
            throw new NotImplementedException();
        }

        public string SetDeviceParameters(string jsonValue, string deviceName, string channelName, string stationName = "Local Station")
        {
            throw new NotImplementedException();
        }

        public Task<string> SetDeviceParametersAsync(string jsonValue, string deviceName, string channelName, string stationName = "Local Station")
        {
            throw new NotImplementedException();
        }

        public string SetTagParameters(string jsonValue, string tagName, string deviceName, string channelName, string stationName = "Local Station")
        {
            throw new NotImplementedException();
        }

        public Task<string> SetTagParametersAsync(string jsonValue, string tagName, string deviceName, string channelName, string stationName = "Local Station")
        {
            throw new NotImplementedException();
        }

        public string WriteTagValue(ITag tag, byte piorityLevel, WriteMode writeMode)
        {
            throw new NotImplementedException();
        }

        public string WriteTagValue(string tagName, string value, string deviceName, string channelName, string stationName = "Local Station", byte piorityLevel = 255, WriteMode writeMode = WriteMode.WriteAllValue)
        {
            throw new NotImplementedException();
        }

        public Task<string> WriteTagValueAsync(ITag tag, byte piorityLevel = 255, WriteMode writeMode = WriteMode.WriteAllValue)
        {
            throw new NotImplementedException();
        }

        public Task<string> WriteTagValueAsync(string tagName, string value, string deviceName, string channelName, string stationName = "Local Station", byte piorityLevel = 255, WriteMode writeMode = WriteMode.WriteAllValue)
        {
            throw new NotImplementedException();
        }
    }
}
