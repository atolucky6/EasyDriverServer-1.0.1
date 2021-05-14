using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriverPlugin;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using EasyDriver.Core;

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
        protected IRemoteConnectionManagerService HubConnectionManagerService { get; set; }
        protected IHubFactory HubFactory { get; set; }

        #endregion

        #region Constructors

        public RemoteStationViewModel(
            IProjectManagerService projectManagerService,
            IDriverManagerService driverManagerService,
            IHubFactory hubFactory,
            IRemoteConnectionManagerService hubConnectionManagerService)
        {
            SizeToContent = SizeToContent.WidthAndHeight;
            Width = 800;
            Height = 260;
            ProjectManagerService = projectManagerService;
            DriverManagerService = driverManagerService;
            HubFactory = hubFactory;
            HubConnectionManagerService = hubConnectionManagerService;
            CommunicationModeSource = Enum.GetValues(typeof(CommunicationMode)).Cast<CommunicationMode>().ToList();
            Messenger.Default.Register<CreateRemoteStationSuccessMessage>(this, OnCreateRemoteStationSuccessMessage);
        }

        #endregion

        #region Public members

        public string Title { get; set; } = "";
        public SizeToContent SizeToContent { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public virtual string Name { get; set; }
        public virtual string RemoteAddress { get; set; } = "";
        public virtual ushort Port { get; set; }
        public virtual CommunicationMode CommunicationMode { get; set; }
        public virtual int RefreshRate { get;set; }
        public virtual List<CommunicationMode> CommunicationModeSource { get; set; }
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
                    if (Parent.Childs.FirstOrDefault(x => x != RemoteStation && (x as ICoreItem).Name == Name?.Trim()) != null)
                    {
                        MessageBoxService.ShowMessage($"The station name '{Name?.Trim()}' is already in use.", "Message", MessageButton.OK, MessageIcon.Warning);
                    }
                    else
                    {
                        if (HubConnectionManagerService.ConnectionDictonary.ContainsKey(RemoteStation))
                        {
                            RemoteStation.RemoteAddress = RemoteAddress;
                            RemoteStation.Port = Port;
                            RemoteStation.RefreshRate = RefreshRate;
                            RemoteStation.CommunicationMode = CommunicationMode;
                            HubConnectionManagerService.ReloadConnection(RemoteStation);
                            RemoteStation.Parent.Childs.NotifyItemInCollectionChanged(RemoteStation);
                            RemoteStation.RaisePropertyChanged("RemoteAddress");
                            RemoteStation.RaisePropertyChanged("Port");
                            IsBusy = false;
                            CurrentWindowService.Close();
                        }
                    }
                }
                else
                {
                    if (Parent.Childs.FirstOrDefault(x => x != RemoteStation && (x as ICoreItem).Name == Name?.Trim()) != null)
                    {
                        MessageBoxService.ShowMessage($"The station name '{Name?.Trim()}' is already in use.", "Message", MessageButton.OK, MessageIcon.Warning);
                    }
                    else
                    {

                        if (hubConnection == null)
                        {
                            hubConnection = HubFactory.CreateHubConnection(RemoteAddress, Port);
                            hubProxy = HubFactory.CreateHubProxy(hubConnection);
                        }
                        else
                        {
                            if (hubConnection.State == ConnectionState.Disconnected)
                            {
                                hubConnection?.Dispose();
                                hubConnection = HubFactory.CreateHubConnection(RemoteAddress, Port);
                                hubProxy = HubFactory.CreateHubProxy(hubConnection);
                            }
                        }

                        try
                        {
                            await hubConnection.Start(new LongPollingTransport());
                        }
                        catch
                        {
                            IsBusy = false;
                            MessageBoxService.ShowMessage($"Can't connect to server {RemoteAddress}:{Port}", "Message", MessageButton.OK, MessageIcon.Warning);
                            return;
                        }

                        await Task.Delay(50);
                        if (hubConnection.State == ConnectionState.Connected)
                        {
                            HubModel hubModel = new HubModel()
                            {
                                StationName = Name,
                                Port = Port.ToString(),
                                RemoteAddress = RemoteAddress,
                                CommunicationMode = CommunicationMode.ReceiveFromServer.ToString()
                            };
                            IsBusy = false;
                            WindowService.Show("RemoteProjectTreeView", new object[] { hubModel, hubConnection, hubProxy}, this);
                        }
                        else
                        {
                            MessageBoxService.ShowMessage($"Can't connect to server {RemoteAddress}:{Port}", "Message", MessageButton.OK, MessageIcon.Warning);
                        }

                    }
                }
                IsBusy = false;
            }
            catch (Exception)
            {
                IsBusy = false;
            }
        }

        public bool CanSave() => string.IsNullOrEmpty(Error) && !IsBusy;

        public void Close() => CurrentWindowService.Close();

        public bool CanClose() => !IsBusy;

        #endregion

        #region Event handlers

        public void OnCreateRemoteStationSuccessMessage(CreateRemoteStationSuccessMessage message)
        {
            IsBusy = true;
            if (message != null && ProjectManagerService.CurrentProject != null)
            {
                RemoteStation remoteStation = new RemoteStation(ProjectManagerService.CurrentProject)
                {
                    Name = Name,
                    Port = Port,
                    RemoteAddress = RemoteAddress,
                    CommunicationMode = CommunicationMode,
                    RefreshRate = RefreshRate
                };

                if (message.HubModel != null && message.HubModel.Childs != null)
                {
                    foreach (var item in message.HubModel.Childs)
                    {
                        IGroupItem groupItem = item.ToCoreItem(remoteStation, true);
                        if (groupItem != null)
                            remoteStation.Childs.Add(groupItem);
                    }
                }

                ProjectManagerService.CurrentProject.Childs.Add(remoteStation);
                Thread.Sleep(100);
                HubConnectionManagerService.AddConnection(remoteStation, message.HubConnection, message.HubProxy);
                IsBusy = false;
                ProjectTreeWorkspaceViewModel.IsBusy = false;
                CreateSuccess = true;
                CurrentWindowService.Close();
            }
        }

        public virtual async void OnUnloaded()
        {
            await Task.Run(() =>
            {
                if (!CreateSuccess)
                    hubConnection?.Dispose();
                Messenger.Default.Unregister<CreateRemoteStationSuccessMessage>(this, OnCreateRemoteStationSuccessMessage);
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
                CommunicationMode = remoteStation.CommunicationMode;
                RemoteStation = remoteStation;
                Parent = remoteStation.Parent as IEasyScadaProject;
            }
            else if (Parameter is IEasyScadaProject project)
            {
                EditMode = false;
                Title = "Add Remote Station";
                Name = project.GetUniqueNameInGroup("RemoteStation1");
                Parent = project;
                Port = 8800;
                RemoteAddress = "127.0.0.1";
                RefreshRate = 1000;
                CommunicationMode = CommunicationMode.ReceiveFromServer; 
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
                        //if (!RemoteAddress.IsIpAddress())
                        //    Error = "The Remote Address was not in IPv4 format.";
                        //else
                        //    Error = string.Empty;
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
