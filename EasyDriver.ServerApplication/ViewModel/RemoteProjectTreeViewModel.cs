using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using EasyDriver.Core;

namespace EasyScada.ServerApplication
{
    public class RemoteProjectTreeViewModel : ISupportParameter, ISupportParentViewModel
    {
        #region Constructors

        public RemoteProjectTreeViewModel(
            IHubConnectionManagerService hubConnectionManagerService,
            IHubFactory hubFactory)
        {
            Title = "Easy Driver Server";
            SizeToContent = SizeToContent.Manual;
            Height = 600;
            Width = 400;
            HubConnectionManagerService = hubConnectionManagerService;
            HubFactory = hubFactory;
        }

        #endregion

        #region Injected services

        protected IHubFactory HubFactory { get; set; }
        protected IHubConnectionManagerService HubConnectionManagerService { get; set; }

        #endregion

        #region UI services

        protected ICurrentWindowService CurrentWindowService { get => this.GetService<ICurrentWindowService>(); }
        protected ITreeListViewUtilities TreeListViewUtilities { get => this.GetService<ITreeListViewUtilities>(); }
        protected IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }
        protected ISaveFileDialogService SaveFileDialogService { get => this.GetService<ISaveFileDialogService>(); }
        protected IDispatcherService DispatcherService { get => this.GetService<IDispatcherService>(); }

        #endregion

        #region Public members

        public string Title { get; set; }
        public SizeToContent SizeToContent { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public virtual bool IsBusy { get; set; }
        public object ParentViewModel { get; set; }
        public object Parameter { get; set; }
        public virtual object SelectedItem { get; set; }
        public virtual ObservableCollection<object> SelectedItems { get; set; }
        public HubModel HubModel { get; private set; }
        public ConnectionSchema ConnectionSchema { get; private set; }
        public bool IsCreateStationMode { get; private set; }
        public List<HubModel> Source { get; set; }

        HubConnection hubConnection;
        IHubProxy hubProxy;

        #endregion

        #region Commands

        public void Refresh()
        {
            IsBusy = true;
            try
            {
                DispatcherService.BeginInvoke(async () =>
                {
                    if (hubConnection.State == ConnectionState.Disconnected)
                    {
                        try
                        {
                            await hubConnection.Start();
                        }
                        catch { }
                    }
                    await Task.Delay(50);

                    if (hubConnection.State == ConnectionState.Connected)
                    {
                        string resJson = await hubProxy.Invoke<string>("getAllStationsAsync");
                        List<StationClient> stations = JsonConvert.DeserializeObject<List<StationClient>>(resJson);
                        HubModel.Stations = new List<StationClient>(stations);
                        Source = new List<HubModel>() { HubModel };
                        this.RaisePropertyChanged(x => x.Source);
                        IsBusy = false;
                    }
                    else
                    {
                        MessageBoxService.ShowMessage($"Can't connect to server {HubModel.RemoteAddress}:{HubModel.Port}", "Easy Driver Server", MessageButton.OK, MessageIcon.Warning);
                    }
                });
            }
            catch
            {
                IsBusy = false;
                MessageBoxService.ShowMessage($"Can't connect to server {HubModel.RemoteAddress}:{HubModel.Port}", "Easy Driver Server", MessageButton.OK, MessageIcon.Warning);
            }
        }

        public bool CanRefresh()
        {
            return !IsBusy && HubModel != null;
        }

        public void Comfirm()
        {
            try
            {
                if (IsCreateStationMode)
                {
                    IsBusy = true;
                    List<StationClient> checkedStations = GetCheckedStations(HubModel);
                    Messenger.Default.Send(new CreateRemoteStationSuccessMessage(checkedStations, HubModel, hubConnection, hubProxy));
                    CurrentWindowService.Close();
                    IsBusy = false;
                }
                else
                {
                    SaveFileDialogService.Title = "Save Connection Schema";
                    SaveFileDialogService.Filter = "Connection Schema File (*.json)|*.json";

                    if (SaveFileDialogService.ShowDialog())
                    {
                        IsBusy = true;
                        List<StationClient> checkedStations = GetCheckedStations(HubModel);
                        ConnectionSchema.Stations = checkedStations;
                        string savePath = SaveFileDialogService.File.GetFullName();
                        string connectionSchemaJson = JsonConvert.SerializeObject(ConnectionSchema, Formatting.Indented);
                        try
                        {
                            File.WriteAllText(savePath, connectionSchemaJson);
                            MessageBoxService.ShowMessage("Create connection schema successfully!", "Easy Driver Server", MessageButton.OK, MessageIcon.Information);
                            Messenger.Default.Send(new CreateConnectionSchemaSuccessMessage(ConnectionSchema, savePath));
                            CurrentWindowService.Close();
                            IsBusy = false;
                        }
                        catch
                        {
                            MessageBoxService.ShowMessage($"Can' create connection schema file at '{savePath}'", "Easy Driver Server", MessageButton.OK, MessageIcon.Error);
                        }
                    }
                }
            }
            catch
            {
            }
            finally { IsBusy = false; }
        }

        public bool CanComfirm()
        {
            return !IsBusy && HubModel != null;
        }

        public void ExpandAll()
        {
            TreeListViewUtilities.ExpandAll();
        }

        public bool CanExpandAll()
        {
            return !IsBusy;
        }

        public void CollapseAll()
        {
            TreeListViewUtilities.CollapseAll();
        }

        public bool CanCollapseAll()
        {
            return !IsBusy;
        }

        #endregion

        #region Event handlers

        public virtual void OnLoaded()
        {
            try
            {
                DispatcherService.BeginInvoke(() =>
                {
                    IsBusy = true;
                    this.RaisePropertyChanged(x => x.IsBusy);
                    if (Parameter is object[] arrays)
                    {
                        if (arrays[0] is HubModel)
                        {
                            HubModel = arrays[0] as HubModel;
                            IsCreateStationMode = true;
                        }
                        else if (arrays[0] is ConnectionSchema)
                        {
                            ConnectionSchema = arrays[0] as ConnectionSchema;
                            HubModel = new HubModel()
                            {
                                Port = ConnectionSchema.Port.ToString(),
                                RemoteAddress = ConnectionSchema.ServerAddress,
                                StationName = "Server Station"
                            };
                        }

                        hubConnection = arrays[1] as HubConnection;
                        hubProxy = arrays[2] as IHubProxy;
                        Refresh();
                    }
                });
            }
            catch
            {
                IsBusy = false;
            }
        }

        public virtual void OpenOnDoubleClick(object selectedItem)
        {
            if (selectedItem != null && selectedItem == SelectedItem)
                TreeListViewUtilities.ToggleCurrentNode();
        }

        #endregion

        #region Methods

        public List<StationClient> GetCheckedStations(HubModel hubModel)
        {
            List<StationClient> checkedStations = new List<StationClient>();
            if (!hubModel.Checked)
            {
                foreach (var station in hubModel.Stations)
                {
                    if (station != null)
                    {
                        StationClient checkedStation = station.DeepCopy();
                        checkedStation.Channels = GetCheckedChannels(station);
                        if (checkedStation.Channels.Count > 0)
                            checkedStations.Add(checkedStation);
                    }
                }
            }
            else
            {
                checkedStations.AddRange(HubModel.Stations);
            }
            return checkedStations;
        }

        public List<ChannelClient> GetCheckedChannels(StationClient station)
        {
            List<ChannelClient> checkedChannels = new List<ChannelClient>();
            if (!station.Checked)
            {
                foreach (var channel in station.Channels)
                {
                    if (channel != null)
                    {
                        ChannelClient checkedChannel = channel.DeepCopy();
                        checkedChannel.Devices = GetCheckedDevices(channel);
                        if (checkedChannel.Devices.Count > 0)
                            checkedChannels.Add(checkedChannel);
                    }
                }
            }
            else
            {
                checkedChannels.AddRange(station.Channels);
            }
            return checkedChannels;
        }

        public List<DeviceClient> GetCheckedDevices(ChannelClient channel)
        {
            List<DeviceClient> checkedDevices = new List<DeviceClient>();
            if (!channel.Checked)
            {
                foreach (var device in channel.Devices)
                {
                    if (device != null)
                    {
                        DeviceClient checkedDevice = device.DeepCopy();
                        checkedDevice.Tags = GetCheckedTags(device);
                        if (checkedDevice.Tags.Count > 0)
                            checkedDevices.Add(checkedDevice);
                    }
                }
            }
            else
            {
                checkedDevices.AddRange(channel.Devices);
            }
            return checkedDevices;
        }

        public List<TagClient> GetCheckedTags(DeviceClient device)
        {
            List<TagClient> checkedTags = new List<TagClient>();
            if (!device.Checked)
            {
                foreach (var tag in device.Tags.Where(x => x.Checked))
                {
                    if (tag != null)
                    {
                        TagClient checkedTag = tag.DeepCopy();
                        checkedTags.Add(checkedTag);
                    }
                }
            }
            else
            {
                checkedTags.AddRange(device.Tags);
            }
            return checkedTags;
        }

        #endregion
    }
}
