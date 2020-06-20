using EasyDriverPlugin;
using EasyDriver.Client.Models;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EasyScada.ServerApplication
{
    public interface IHubConnectionManagerService
    {
        Dictionary<IStationCore, HubConnection> HubConnectionDictionary { get; }
        Dictionary<HubConnection, IHubProxy> HubProxyDictionary { get; }
        Dictionary<HubConnection, Timer> TimerDictionary { get; }
        HubConnection AddHubConnection(IStationCore station, HubConnection hubConnection, IHubProxy hubProxy);
        bool RemoveHubConnection(IStationCore station);
        HubConnection GetHubConnection(IStationCore station);
        IHubProxy GetHubProxy(HubConnection connection);
        IHubProxy GetHubProxy(IStationCore station);
        Timer GetHubTimer(HubConnection connection);
    }

    public class HubConnectionManagerService : IHubConnectionManagerService
    {
        public Dictionary<IStationCore, HubConnection> HubConnectionDictionary { get; protected set; }

        public Dictionary<HubConnection, IHubProxy> HubProxyDictionary { get; protected set; }

        public Dictionary<HubConnection, Timer> TimerDictionary { get; protected set; }

        public HubConnectionManagerService()
        {
            HubConnectionDictionary = new Dictionary<IStationCore, HubConnection>();
            HubProxyDictionary = new Dictionary<HubConnection, IHubProxy>();
            TimerDictionary = new Dictionary<HubConnection, Timer>();
        }

        public HubConnection AddHubConnection(IStationCore station, HubConnection hubConnection, IHubProxy hubProxy)
        {
            if (HubConnectionDictionary.ContainsKey(station))
            {
                HubConnection oldHub = HubConnectionDictionary[station];
                IHubProxy oldProxy = HubProxyDictionary[oldHub];
                Timer oldTimer = TimerDictionary[oldHub];
                HubConnectionDictionary.Remove(station);
                oldHub.Stop();
                oldHub.Dispose();
                oldTimer.Change(Timeout.Infinite, Timeout.Infinite);
                oldTimer.Dispose();
            }
            else
            {
                hubProxy.On<string>("broadcastStations", (x) => { OnReceivedStations(x, station); });
                HubConnectionDictionary[station] = hubConnection;
                HubProxyDictionary[hubConnection] = hubProxy;
                hubProxy["communicationMode"] = station.CommunicationMode.ToString();
                TimerDictionary[hubConnection] = new Timer(new TimerCallback(HubTimerCallback), station, 0, station.RefreshRate);
            }

            return hubConnection;
        }

        public bool RemoveHubConnection(IStationCore station)
        {
            throw new System.NotImplementedException();
        }

        public HubConnection GetHubConnection(IStationCore station)
        {
            if (HubConnectionDictionary.ContainsKey(station))
                return HubConnectionDictionary[station];
            return null;
        }

        public IHubProxy GetHubProxy(HubConnection connection)
        {
            if (HubProxyDictionary.ContainsKey(connection))
                return HubProxyDictionary[connection];
            return null;
        }

        public IHubProxy GetHubProxy(IStationCore station)
        {
            HubConnection connection = GetHubConnection(station);
            if (connection != null)
                return GetHubProxy(connection);
            return null;
        }

        public Timer GetHubTimer(HubConnection connection)
        {
            if (TimerDictionary.ContainsKey(connection))
                return TimerDictionary[connection];
            return null;
        }

        private async void HubTimerCallback(object state)
        {
            if (state is IStationCore stationCore)
            {
                HubConnection hubConnection = GetHubConnection(stationCore);
                IHubProxy hubProxy = GetHubProxy(hubConnection);
                Timer timer = GetHubTimer(hubConnection);
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                try
                {
                    if (hubConnection != null && hubProxy != null && timer != null)
                    {
                        if (stationCore.CommunicationMode == CommunicationMode.RequestToServer)
                        {
                            if (hubConnection.State == ConnectionState.Disconnected)
                                await hubConnection.Start();

                            List<IStationCore> notUpdatedStations = stationCore.Childs.Select(x => x as IStationCore).ToList();
                            foreach (var item in stationCore.Childs)
                            {
                                string resJson = await hubProxy.Invoke<string>("getStation", item.Name);
                                Station station = JsonConvert.DeserializeObject<Station>(resJson);
                                if (station != null)
                                {
                                    if (stationCore.Childs.FirstOrDefault(x => x.Name == station.Name) is IStationCore innerStation)
                                    {
                                        UpdateStationCore(station, innerStation, $"{stationCore}/");
                                        notUpdatedStations.Remove(innerStation);
                                    }
                                }
                            }
                            stationCore.LastRefreshTime = DateTime.Now;
                        }
                    }
                }
                catch (Exception)
                {

                }
                finally  
                {
                    if (hubConnection == null || hubProxy == null || timer == null)
                    {
                        
                    }
                    else
                    {
                        timer.Change(stationCore.RefreshRate, 0);
                    }
                }
            }
        }

        private async void OnReceivedStations(string stationsJson, IStationCore stationCore)
        {
            await Task.Run(() =>
            {
                List<IStationCore> notUpdatedStations = stationCore.Childs.Select(x => x as IStationCore).ToList();
                List<Station> stations = JsonConvert.DeserializeObject<List<Station>>(stationsJson);
                if (stations != null && stations.Count > 0)
                {
                    foreach (var station in stations)
                    {
                        if (stationCore.Childs.FirstOrDefault(x => x.Name == station.Name) is IStationCore innerStation)
                        {
                            UpdateStationCore(station, innerStation, $"{stationCore}/");
                            notUpdatedStations.Remove(innerStation);
                        }
                    }
                }
                stationCore.LastRefreshTime = DateTime.Now;
            });
        }

        private void UpdateStationCore(Station station, IStationCore stationCore, string startPath)
        {
            if (station != null && stationCore != null && station.Path == stationCore.Path.Replace(startPath, "") && stationCore.IsReadOnly)
            {
                stationCore.RefreshRate = station.RefreshRate;
                stationCore.LastRefreshTime = station.LastRefreshTime;
                stationCore.RemoteAddress = station.RemoteAddress;
                stationCore.Port = station.Port;
                stationCore.CommunicationMode = station.CommunicationMode;
                stationCore.CommunicationError = station.Error;
                stationCore.RefreshRate = station.RefreshRate;
                stationCore.ParameterContainer.Parameters = station.Parameters;

                List<IChannelCore> channelCores = new List<IChannelCore>();
                List<IStationCore> stationCores = new List<IStationCore>();

                foreach (var item in stationCore.Childs)
                {
                    if (item is IChannelCore)
                        channelCores.Add(item as IChannelCore);
                    else if (item is IStationCore)
                        stationCores.Add(item as IStationCore);
                }

                if (channelCores != null && channelCores.Count > 0 && station.Channels != null)
                {
                    foreach (var channel in station.Channels)
                    {
                        if (stationCore.FirstOrDefault(x => x.Path.Replace(startPath, "") == channel.Path) is IChannelCore channelCore)
                        {
                            UpdateChannelCore(channel, channelCore, startPath);
                            channelCores.Remove(channelCore);
                        }
                    }
                }
                channelCores.ForEach(x => x.CommunicationError = "This channel could not be found on server.");

                if (stationCores != null && stationCores.Count > 0 && station.RemoteStations != null)
                {
                    foreach (var remoteStation in station.RemoteStations)
                    {
                        if (stationCores.FirstOrDefault(x => x.Path.Replace(startPath, "") == remoteStation.Path) is IStationCore updateStation)
                        {
                            UpdateStationCore(remoteStation, updateStation, startPath);
                            stationCores.Remove(updateStation);
                        }
                    }
                }
                stationCores.ForEach(x => x.CommunicationError = "This station could not be found on server.");
            }
        }

        private void UpdateChannelCore(Channel channel, IChannelCore channelCore, string startPath)
        {
            if (channel != null && channelCore != null && channelCore.IsReadOnly)
            {
                channelCore.CommunicationError = channel.Error;
                channelCore.LastRefreshTime = channel.LastRefreshTime;
                channelCore.ParameterContainer.Parameters = channel.Parameters;

                List<IDeviceCore> deviceCores = channelCore.Childs.Select(x => x as IDeviceCore)?.ToList();
                if (deviceCores != null && deviceCores.Count > 0 && channel.Devices != null)
                {
                    foreach (var device in channel.Devices)
                    {
                        if (channelCore.FirstOrDefault(x => x.Path.Replace(startPath, "") == device.Path) is IDeviceCore deviceCore)
                        {
                            UpdateDeviceCore(device, deviceCore, startPath);
                            deviceCores.Remove(deviceCore);
                        }
                    }
                }

                deviceCores.ForEach(x => x.CommunicationError = "This device could not be found on server.");
            }
        }

        private void UpdateDeviceCore(Device device, IDeviceCore deviceCore, string startPath)
        {
            if (device != null && deviceCore != null && deviceCore.IsReadOnly)
            {
                deviceCore.LastRefreshTime = device.LastRefreshTime;
                deviceCore.CommunicationError = device.Error;
                deviceCore.LastRefreshTime = device.LastRefreshTime;
                deviceCore.ParameterContainer.Parameters = device.Parameters;

                List<ITagCore> tagCores = deviceCore.Childs.Select(x => x as ITagCore)?.ToList();
                if (tagCores != null && tagCores.Count > 0 && device.Tags != null)
                {
                    foreach (var tag in device.Tags)
                    {
                        if (deviceCore.FirstOrDefault(x => x.Path.Replace(startPath, "") == tag.Path) is ITagCore tagCore)
                        {
                            UpdateTagCore(tag, tagCore, startPath);
                            tagCores.Remove(tagCore);
                        }
                    }
                }

                tagCores.ForEach(x =>
                {
                    x.Quality = Quality.Bad;
                    x.CommunicationError = "This tag could not be found on server.";
                });
            }
        }

        private void UpdateTagCore(Tag tag, ITagCore tagCore, string startPath)
        {
            if (tag != null && tagCore != null && tagCore.IsReadOnly)
            {
                tagCore.Value = tag.Value;
                tagCore.TimeStamp = tag.TimeStamp;
                tagCore.Quality = tag.Quality;
                tagCore.RefreshInterval = tag.RefreshInterval;
                tagCore.RefreshRate = tag.RefreshRate;
                tagCore.Address = tag.Address;
                tagCore.DataTypeName = tag.DataType;
                tagCore.CommunicationError = tag.Error;
                tagCore.ParameterContainer.Parameters = tag.Parameters;
                tagCore.AccessPermission = tag.AccessPermission;
            }
        }
    }
}
