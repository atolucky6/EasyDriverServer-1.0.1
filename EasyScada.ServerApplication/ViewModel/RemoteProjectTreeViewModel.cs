using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriverPlugin;
using EasyScada.Api.Interfaces;
using EasyScada.Core;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasyScada.ServerApplication
{
    public class RemoteProjectTreeViewModel : ISupportParameter, ISupportParentViewModel
    {
        #region Constructors

        public RemoteProjectTreeViewModel(
            IHubConnectionManagerService hubConnectionManagerService)
        {
            Title = "Easy Driver Server";
            SizeToContent = SizeToContent.Manual;
            Height = 600;
            Width = 400;
            HubConnectionManagerService = hubConnectionManagerService;
        }

        #endregion

        #region Injected services

        protected IHubConnectionManagerService HubConnectionManagerService { get; set; }

        #endregion

        #region UI services

        protected ICurrentWindowService CurrentWindowService { get => this.GetService<ICurrentWindowService>(); }
        protected ITreeListViewUtilities TreeListViewUtilities { get => this.GetService<ITreeListViewUtilities>(); }
        protected IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }
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
        public HubConnection HubConnection { get; private set; }
        public IHubProxy HubProxy { get; private set; }
        public HubModel HubModel { get; private set; }
        public List<HubModel> Source { get; set; }

        #endregion

        #region Commands

        public void Refresh()
        {
            IsBusy = true;
            try
            {
                DispatcherService.BeginInvoke(async () =>
                {
                    if (HubConnection.State == ConnectionState.Disconnected)
                        await HubConnection.Start(new LongPollingTransport());
                    await Task.Delay(500);
                    if (HubConnection.State == ConnectionState.Connected)
                    {
                        string resJson = await HubProxy.Invoke<string>("getAllStations");
                        List<Station> stations = JsonConvert.DeserializeObject<List<Station>>(resJson);
                        HubModel.Stations = new List<Station>(stations);
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
            catch (Exception ex)
            {
                IsBusy = false;
            }
        }

        public bool CanRefresh()
        {
            return !IsBusy && HubConnection != null && HubProxy != null && HubModel != null;
        }

        public void Comfirm()
        {
            IsBusy = true;
            try
            {
                List<Station> checkedStations = GetCheckedStations(HubModel);
                IsBusy = false;
                Messenger.Default.Send(new CreateRemoteStationSuccess(checkedStations, HubConnection, HubProxy));
                CurrentWindowService.Close();
            }
            catch (Exception ex)
            {
                IsBusy = false;
            }
        }

        public bool CanComfirm()
        {
            return !IsBusy && HubConnection != null && HubProxy != null && HubModel != null;
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
                    if (Parameter != null)
                    {
                        if (Parameter is ArrayList array)
                        {
                            foreach (var item in array)
                            {
                                if (item is HubConnection hubConnection)
                                    HubConnection = hubConnection;
                                if (item is IHubProxy hubProxy)
                                    HubProxy = hubProxy;
                                if (item is HubModel hubModel)
                                    HubModel = hubModel;
                            }

                            if (HubConnection != null && HubProxy != null && HubModel != null)
                                Refresh();
                        }
                    }
                });
            }
            catch (Exception ex)
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

        public List<Station> GetCheckedStations(HubModel hubModel)
        {
            List<Station> checkedStations = new List<Station>();
            if (!hubModel.Checked)
            {
                foreach (var station in hubModel.Stations)
                {
                    Station checkedStation = station.DeepCopy();
                    checkedStation.Channels = GetCheckedChannels(station);
                    if (checkedStation.Channels.Count > 0)
                        checkedStations.Add(checkedStation);
                }
            }
            else
            {
                checkedStations.AddRange(HubModel.Stations);
            }
            return checkedStations;
        }

        public List<Channel> GetCheckedChannels(Station station)
        {
            List<Channel> checkedChannels = new List<Channel>();
            if (!station.Checked)
            {
                foreach (var channel in station.Channels)
                {
                    Channel checkedChannel = channel.DeepCopy();
                    checkedChannel.Devices = GetCheckedDevices(channel);
                    if (checkedChannel.Devices.Count > 0)
                        checkedChannels.Add(checkedChannel);
                }
            }
            else
            {
                checkedChannels.AddRange(station.Channels);
            }
            return checkedChannels;
        }

        public List<Device> GetCheckedDevices(Channel channel)
        {
            List<Device> checkedDevices = new List<Device>();
            if (!channel.Checked)
            {
                foreach (var device in channel.Devices)
                {
                    Device checkedDevice = device.DeepCopy();
                    checkedDevice.Tags = GetCheckedTags(device);
                    if (checkedDevice.Tags.Count > 0)
                        checkedDevices.Add(checkedDevice);
                }
            }
            else
            {
                checkedDevices.AddRange(channel.Devices);
            }
            return checkedDevices;
        }

        public List<Tag> GetCheckedTags(Device device)
        {
            List<Tag> checkedTags = new List<Tag>();
            if (!device.Checked)
            {
                foreach (var tag in device.Tags.Where(x => x.Checked))
                {
                    Tag checkedTag = tag.DeepCopy();
                    checkedTags.Add(checkedTag);
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
