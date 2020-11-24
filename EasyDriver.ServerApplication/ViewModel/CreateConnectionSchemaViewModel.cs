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

namespace EasyScada.ServerApplication
{
    public class CreateConnectionSchemaViewModel : IDataErrorInfo, ISupportParameter, ISupportParentViewModel
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
        protected ApplicationViewModel ApplicationViewModel { get; set; }

        #endregion

        #region Constructors

        public CreateConnectionSchemaViewModel(
            IProjectManagerService projectManagerService,
            IDriverManagerService driverManagerService,
            IHubFactory hubFactory,
            IRemoteConnectionManagerService hubConnectionManagerService,
            ApplicationViewModel applicationViewModel)
        {
            SizeToContent = SizeToContent.WidthAndHeight;
            Width = 400;
            Height = 260;
            ProjectManagerService = projectManagerService;
            DriverManagerService = driverManagerService;
            ApplicationViewModel = applicationViewModel;
            HubFactory = hubFactory;
            HubConnectionManagerService = hubConnectionManagerService;
            CommunicationModeSource = Enum.GetValues(typeof(CommunicationMode)).Cast<CommunicationMode>().ToList();
            Messenger.Default.Register<CreateConnectionSchemaSuccessMessage>(this, OnCreateConnectionSchemaSuccessMessage);
        }

        #endregion

        #region Public members

        public string Title { get; set; } = "Create Connection Schema";
        public SizeToContent SizeToContent { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public virtual string ServerAddress { get; set; } = "127.0.0.1";
        public virtual ushort Port { get; set; } 
        public virtual CommunicationMode CommunicationMode { get; set; }
        public virtual int RefreshRate { get; set; }

        public virtual List<CommunicationMode> CommunicationModeSource { get; set; }
        public object Parameter { get; set; }
        public object ParentViewModel { get; set; }
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

                if (hubConnection == null)
                {
                    hubConnection = HubFactory.CreateHubConnection("127.0.0.1", ApplicationViewModel.ServerConfiguration.Port);
                    hubProxy = HubFactory.CreateHubProxy(hubConnection);
                }
                else
                {
                    if (hubConnection.State == ConnectionState.Disconnected)
                    {
                        hubConnection?.Dispose();
                        hubConnection = HubFactory.CreateHubConnection("127.0.0.1", ApplicationViewModel.ServerConfiguration.Port);
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
                    MessageBoxService.ShowMessage($"Can't connect to server 127.0.0.1:{ApplicationViewModel.ServerConfiguration.Port}", "Message", MessageButton.OK, MessageIcon.Warning);
                    return;
                }

                await Task.Delay(50);
                if (hubConnection.State == ConnectionState.Connected)
                {
                    ConnectionSchema connectionSchema = new ConnectionSchema()
                    {
                        CommunicationMode = CommunicationMode,
                        CreatedDate = DateTime.Now,
                        ServerAddress = ServerAddress,
                        Port = Port,
                        RefreshRate = RefreshRate,
                    };
                    IsBusy = false;
                    WindowService.Show("RemoteProjectTreeView", new object[] { connectionSchema, hubConnection, hubProxy }, this);
                }
                else
                {
                    MessageBoxService.ShowMessage($"Can't connect to server 127.0.0.1:{ApplicationViewModel.ServerConfiguration.Port}", "Message", MessageButton.OK, MessageIcon.Warning);
                }
            }
            catch (Exception ex)
            {

            }
            finally { IsBusy = false; }
        }

        public bool CanSave() => string.IsNullOrEmpty(Error) && !IsBusy;

        public void Close() => CurrentWindowService.Close();

        public bool CanClose() => !IsBusy;

        #endregion

        #region Event handlers

        private void OnCreateConnectionSchemaSuccessMessage(CreateConnectionSchemaSuccessMessage message)
        {
            IsBusy = false;
            if (message != null)
            {
                CurrentWindowService.Close();
            }
        }

        public virtual async void OnUnloaded()
        {
            await Task.Run(() =>
            {
                hubConnection?.Dispose();
                Messenger.Default.Unregister<CreateConnectionSchemaSuccessMessage>(this, OnCreateConnectionSchemaSuccessMessage);
            });
        }

        public virtual void OnLoaded()
        {
            Port = ApplicationViewModel.ServerConfiguration.Port;
            RefreshRate = 1000;
            CommunicationMode = CommunicationMode.ReceiveFromServer;
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
                    case nameof(ServerAddress):
                        if (!ServerAddress.IsIpAddress())
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
