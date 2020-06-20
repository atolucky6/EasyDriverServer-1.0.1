using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriverPlugin;
using EasyDriver.Client.Models;
using EasyDriver.Server.Models;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EasyScada.ServerApplication
{
    public class RemoteStationViewModel : IDataErrorInfo, ISupportParameter, ISupportParentViewModel
    {
        #region UI services

        protected ICurrentWindowService CurrentWindowService { get => this.GetService<ICurrentWindowService>(); }
        protected IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }
        protected IWindowService WindowService { get => this.GetService<IWindowService>(); }

        #endregion

        #region Inject services

        protected IProjectManagerService ProjectManagerService { get; set; }
        protected IDriverManagerService DriverManagerService { get; set; }
        protected IHubConnectionManagerService HubConnectionManagerService { get; set; }
        protected IHubFactory HubFactory { get; set; }

        #endregion

        #region Constructors

        public RemoteStationViewModel(
            IProjectManagerService projectManagerService,
            IDriverManagerService driverManagerService,
            IHubFactory hubFactory,
            IHubConnectionManagerService hubConnectionManagerService)
        {
            SizeToContent = SizeToContent.WidthAndHeight;
            Width = 400;
            Height = 260;
            ProjectManagerService = projectManagerService;
            DriverManagerService = driverManagerService;
            HubFactory = hubFactory;
            HubConnectionManagerService = hubConnectionManagerService;

            Messenger.Default.Register<CreateRemoteStationSuccess>(this, OnCreateRemoteStationSuccess);
        }

        #endregion

        #region Public members

        public string Title { get; set; }
        public SizeToContent SizeToContent { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public virtual string Name { get; set; }
        public virtual string RemoteAddress { get; set; }
        public virtual ushort Port { get; set; }
        public virtual object WorkMode { get; set; }
        public virtual int RefreshRate { get;set; }

        public bool EditMode { get; set; }
        public RemoteStation RemoteStation { get; set; }
        public IEasyScadaProject Parent { get; set; }

        public object Parameter { get; set; }
        public object ParentViewModel { get; set; }
        public ProjectTreeWorkspaceViewModel ProjectTreeWorkspaceViewModel { get => ParentViewModel as ProjectTreeWorkspaceViewModel; }
        public virtual bool IsBusy { get; set; }
        public virtual bool CreateSuccess { get; set; }

        HubConnection hubConnection;
        IHubProxy hubProxy;

        #endregion

        #region Commands

        public async void Save()
        {
            try
            {
                IsBusy = true;
                if (EditMode)
                {
                    if (Parent.Childs.FirstOrDefault(x => x.Name == Name?.Trim()) != null)
                    {
                        MessageBoxService.ShowMessage($"The station name '{Name?.Trim()}' is already in use.", "Easy Driver Server", MessageButton.OK, MessageIcon.Warning);
                    }
                    else
                    {

                    }
                }
                else
                {
                    if (Parent.Childs.FirstOrDefault(x => x != RemoteStation && x.Name == Name?.Trim()) != null)
                    {
                        MessageBoxService.ShowMessage($"The station name '{Name?.Trim()}' is already in use.", "Easy Driver Server", MessageButton.OK, MessageIcon.Warning);
                    }
                    else
                    {
                        hubConnection = HubFactory.CreateHubConnection(RemoteAddress, Port);
                        hubProxy = HubFactory.CreateHubProxy(hubConnection);
                        await hubConnection.Start(new LongPollingTransport());
                        await Task.Delay(300);
                        if (hubConnection.State == ConnectionState.Connected)
                        {
                            HubModel hubModel = new HubModel()
                            {
                                StationName = Name,
                                Port = Port.ToString(),
                                RemoteAddress = RemoteAddress,
                                Stations = new List<Station>(),
                                CommunicationMode = CommunicationMode.ReceiveFromServer.ToString()
                            };
                            IsBusy = false;
                            
                            WindowService.Show("RemoteProjectTreeView", new ArrayList() { hubConnection, hubProxy, hubModel }, this);
                        }
                    }
                }
                IsBusy = false;
            }
            catch (Exception ex)
            {
                IsBusy = false;
            }
        }

        public bool CanSave() => string.IsNullOrEmpty(Error) && !IsBusy;

        public void Configure()
        {

        }
        
        public bool CanConfigure()
        {
            return !IsBusy && EditMode && string.IsNullOrEmpty(Error);
        }

        public void Close() => CurrentWindowService.Close();

        public bool CanClose() => !IsBusy;

        #endregion

        #region Event handlers

        public void OnCreateRemoteStationSuccess(CreateRemoteStationSuccess message)
        {
            IsBusy = true;
            if (message != null && ProjectManagerService.CurrentProject != null)
            {
                RemoteStation remoteStation = new RemoteStation(ProjectManagerService.CurrentProject)
                {
                    Name = Name,
                    Port = Port,
                    RemoteAddress = RemoteAddress,
                    CommunicationMode = CommunicationMode.ReceiveFromServer,
                    RefreshRate = RefreshRate
                };

                foreach (var station in message.SelectedStations)
                {
                    IStationCore stationCore = CreateRemoteStationCore(station, remoteStation);
                    if (stationCore != null)
                        remoteStation.Add(stationCore);
                }

                ProjectManagerService.CurrentProject.Childs.Add(remoteStation);
                HubConnectionManagerService.AddHubConnection(remoteStation, message.HubConnection, message.HubProxy);
                IsBusy = false;
                ProjectTreeWorkspaceViewModel.IsBusy = false;
                CreateSuccess = true;
                CurrentWindowService.Close();
            }
        }

        public IStationCore CreateRemoteStationCore(Station station, IGroupItem parent)
        {
            IStationCore stationCore = null;
            if (station.IsLocalStation)
                stationCore = new LocalStation(parent);
            else
                stationCore = new RemoteStation(parent);
            stationCore.Name = station.Name;
            stationCore.RemoteAddress = station.RemoteAddress;
            stationCore.Port = station.Port;
            stationCore.RefreshRate = station.RefreshRate;
            stationCore.CommunicationMode = station.CommunicationMode;

            if (station.Channels != null && station.Channels.Count > 0)
            {
                foreach (var channel in station.Channels)
                    if (channel != null)
                        stationCore.Childs.Add(CreateRemoteChannelCore(channel, stationCore));
            }

            if (station.RemoteStations != null && station.RemoteStations.Count > 0)
            {
                foreach (var innerStation in station.RemoteStations)
                    if (innerStation != null)
                        stationCore.Childs.Add(CreateRemoteStationCore(innerStation, stationCore));
            }

            return stationCore;
        }

        public IChannelCore CreateRemoteChannelCore(Channel channel, IStationCore parent)
        {
            IChannelCore channelCore = new ChannelCore(parent, true);
            channelCore.Name = channel.Name;
            channelCore.DriverPath = channel.DriverName;
            channelCore.ConnectionType = channel.ConnectionType;
            channelCore.ParameterContainer.Parameters = channel.Parameters;
            foreach (var device in channel.Devices)
                if (device != null)
                    channelCore.Childs.Add(CreateRemoteDeviceCore(device, channelCore));
            return channelCore;
        }

        public IDeviceCore CreateRemoteDeviceCore(Device device, IChannelCore parent)
        {
            IDeviceCore deviceCore = new DeviceCore(parent, true);
            deviceCore.Name = device.Name;
            deviceCore.LastRefreshTime = device.LastRefreshTime;
            deviceCore.ParameterContainer.Parameters = device.Parameters;
            foreach (var tag in device.Tags)
                if (tag != null)
                    deviceCore.Add(CreateRemoteTagCore(tag, deviceCore));
            return deviceCore;
        }

        public ITagCore CreateRemoteTagCore(Tag tag, IDeviceCore parent)
        {
            ITagCore tagCore = new TagCore(parent, true);
            tagCore.Name = tag.Name;
            tagCore.Address = tag.Address;
            tagCore.DataTypeName = tag.DataType;
            tagCore.Value = tag.Value;
            tagCore.Quality = tag.Quality;
            tagCore.RefreshRate = tag.RefreshRate;
            tagCore.RefreshInterval = tag.RefreshInterval;
            tagCore.AccessPermission = tag.AccessPermission;
            tagCore.TimeStamp = tag.TimeStamp;
            tagCore.ParameterContainer.Parameters = tag.Parameters;
            parent.Childs.Add(tagCore);
            return tagCore;
        }

        public virtual async void OnUnloaded()
        {
            await Task.Run(() =>
            {
                if (!EditMode && !CreateSuccess)
                {
                    hubConnection?.Stop();
                    hubConnection?.Dispose();
                }
                Messenger.Default.Unregister<CreateRemoteStationSuccess>(this, OnCreateRemoteStationSuccess);
            });
        }

        public virtual void OnLoaded()
        {
            if (Parameter is RemoteStation remoteStation)
            {
                EditMode = true;
                Title = $"Edit Remote Station - {remoteStation.Name}";

                Name = remoteStation.Name;
                RemoteAddress = remoteStation.RemoteAddress;
                Port = remoteStation.Port;
                RefreshRate = remoteStation.RefreshRate;
                WorkMode = remoteStation.CommunicationMode;

                RemoteStation = remoteStation;

            }
            else if (Parameter is IEasyScadaProject project)
            {
                EditMode = false;
                Title = "Add Remote Station";
                Name = project.GetUniqueNameInGroup("RemoteStation1");
                Parent = project;

                Port = 8800;
                RemoteAddress = "192.168.0.1";
                RefreshRate = 1000;
                WorkMode = CommunicationMode.ReceiveFromServer;
            }
            this.RaisePropertiesChanged();
           
        }

        #endregion

        #region IDataErrorInfo

        public string Error { get; private set; }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(Name):
                        Error = Name.ValidateFileName();
                        break;
                    case nameof(RemoteAddress):
                        if (!RemoteAddress.IsIpAddress())
                            Error = "The Remote Address was not in IPv4 format.";
                        else
                            Error = string.Empty;
                        break;
                    default:
                        Error = string.Empty;
                        break;
                }
                return Error;
            }
        }

        #endregion
    }
}
