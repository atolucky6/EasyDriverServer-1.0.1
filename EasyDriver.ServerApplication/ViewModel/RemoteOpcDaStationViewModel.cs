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
using EasyDriver.Opc.Client.Da;
using EasyDriver.Opc.Client.Common;

namespace EasyScada.ServerApplication
{
    public class RemoteOpcDaStationViewModel : IDataErrorInfo, ISupportParameter, ISupportParentViewModel
    {
        #region UI services

        protected ICurrentWindowService CurrentWindowService { get => this.GetService<ICurrentWindowService>(); }
        protected IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }
        protected IWindowService WindowService { get => this.GetService<IWindowService>(); }
        protected IDispatcherService DispatcherService { get => this.GetService<IDispatcherService>(); }

        #endregion

        #region Inject services

        protected IProjectManagerService ProjectManagerService { get; set; }
        protected IDriverManagerService DriverManagerService { get; set; }
        protected IRemoteConnectionManagerService RemoteConnectionManagerService { get; set; }
        protected IHubFactory HubFactory { get; set; }

        #endregion

        #region Constructors

        public RemoteOpcDaStationViewModel(
            IProjectManagerService projectManagerService,
            IDriverManagerService driverManagerService,
            IRemoteConnectionManagerService remoteConnectionManagerService)
        {
            SizeToContent = SizeToContent.WidthAndHeight;
            Width = 400;
            Height = 260;
            ProjectManagerService = projectManagerService;
            DriverManagerService = driverManagerService;
            RemoteConnectionManagerService = remoteConnectionManagerService;
            OpcServerSource = IoC.Instance.OpcDaServerHosts;
            Messenger.Default.Register<CreateRemoteOpcDaStationSuccessMessage>(this, OnCreateRemoteOpcDaStationSuccessMessage);
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
        public virtual int RefreshRate { get; set; }
        public string OpcServer { get; set; }
        public CommunicationMode CommunicationMode { get; set; } = CommunicationMode.RequestToServer;
        public List<string> OpcServerSource { get; set; }
        public bool EditMode { get; set; }
        public RemoteStation RemoteStation { get; set; }
        public IEasyScadaProject Parent { get; set; }

        public object Parameter { get; set; }
        public object ParentViewModel { get; set; }
        public ProjectTreeWorkspaceViewModel ProjectTreeWorkspaceViewModel { get => ParentViewModel as ProjectTreeWorkspaceViewModel; }
        public virtual bool IsBusy { get; set; }
        public virtual bool CreateSuccess { get; set; }

        public OpcDaServer OpcDaServer { get; set; }

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
                        if (RemoteConnectionManagerService.ConnectionDictonary.ContainsKey(RemoteStation))
                        {
                            RemoteStation.Name = Name;
                            RemoteStation.RemoteAddress = RemoteAddress;
                            RemoteStation.Port = Port;
                            RemoteStation.CommunicationMode = CommunicationMode;
                            RemoteStation.RefreshRate = RefreshRate;
                            RemoteStation.ConnectionString = UrlBuilder.Build(OpcServer).ToString();
                            RemoteStation.Parent.Childs.NotifyItemInCollectionChanged(RemoteStation);
                            RemoteStation.RaisePropertyChanged("OpcDaServerName");
                            RemoteConnectionManagerService.ReloadConnection(RemoteStation);
                            IsBusy = false;
                            CurrentWindowService.Close();
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(OpcServer))
                    {
                        MessageBoxService.ShowMessage($"The OPC DA server can't be empty!", "Message", MessageButton.OK, MessageIcon.Warning);
                    }
                    else
                    {
                        if (Parent.Childs.FirstOrDefault(x => x != RemoteStation && (x as ICoreItem).Name == Name?.Trim()) != null)
                        {
                            MessageBoxService.ShowMessage($"The station name '{Name?.Trim()}' is already in use.", "Message", MessageButton.OK, MessageIcon.Warning);
                        }
                        else
                        {
                            Uri url = UrlBuilder.Build(OpcServer);
                            if (OpcDaServer == null)
                            {
                                OpcDaServer = new OpcDaServer(url);
                            }
                            else
                            {
                                OpcDaServer?.Dispose();
                                OpcDaServer = new OpcDaServer(url);
                            }

                            await OpcDaServer.ConnectAsync();
                            
                            if (OpcDaServer.IsConnected)
                            {
                                WindowService.Show("RemoteOpcDaProjectTreeView", new object[] { OpcDaServer }, this);
                            }
                            else
                            {
                                MessageBoxService.ShowMessage($"Can't connect to OPC server '{url.ToString()}'", "Message", MessageButton.OK, MessageIcon.Warning);
                            }
                        }
                    }
                }
                IsBusy = false;
            }
            catch (Exception)
            {
                IsBusy = false;
                MessageBoxService.ShowMessage($"Can't connect to OPC server '{UrlBuilder.Build(OpcServer).ToString()}'", "Message", MessageButton.OK, MessageIcon.Warning);
            }
        }

        public bool CanSave() => string.IsNullOrEmpty(Error) && !IsBusy;

        public void Close() => CurrentWindowService.Close();

        public bool CanClose() => !IsBusy;

        #endregion

        #region Event handlers

        public void OnCreateRemoteOpcDaStationSuccessMessage(CreateRemoteOpcDaStationSuccessMessage message)
        {
            DispatcherService.Invoke(() =>
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
                        RefreshRate = RefreshRate,
                        StationType = "OPC_DA"
                    };
                    remoteStation.ConnectionString = UrlBuilder.Build(OpcServer).ToString();
                    foreach (var item in message.Parent.Childs)
                        remoteStation.Childs.Add(item);

                    ProjectManagerService.CurrentProject.Childs.Add(remoteStation);
                    Thread.Sleep(100);
                    RemoteConnectionManagerService.AddConnection(remoteStation, OpcDaServer);
                    IsBusy = false;
                    ProjectTreeWorkspaceViewModel.IsBusy = false;
                    CreateSuccess = true;
                    CurrentWindowService.Close();
                }

            });
        }

        public virtual async void OnUnloaded()
        {
            await Task.Run(() =>
            {
                if (!CreateSuccess)
                    OpcDaServer?.Dispose();
                Messenger.Default.Unregister<CreateRemoteOpcDaStationSuccessMessage>(this, OnCreateRemoteOpcDaStationSuccessMessage);
            });
        }

        public virtual void OnLoaded()
        {
            if (Parameter is RemoteStation remoteStation)
            {
                EditMode = true;
                Title = $"Edit Remote OPC DA Station - {remoteStation.Name}";

                Name = remoteStation.Name;
                RemoteAddress = remoteStation.RemoteAddress;
                Port = remoteStation.Port;
                RefreshRate = remoteStation.RefreshRate;
                RemoteStation = remoteStation;
                Parent = remoteStation.Parent as IEasyScadaProject;

                if (!string.IsNullOrEmpty(remoteStation.ConnectionString))
                {
                    string[] split = remoteStation.ConnectionString.Split('/');
                    OpcServer = split[split.Length - 1];
                }
            }
            else if (Parameter is IEasyScadaProject project)
            {
                EditMode = false;
                Title = "Add Remote OPC DA Station";
                Name = project.GetUniqueNameInGroup("RemoteStation1");
                Parent = project;
                Port = 135;
                RemoteAddress = "localhost";
                OpcServer = OpcServerSource.FirstOrDefault();
                RefreshRate = 1000;
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
