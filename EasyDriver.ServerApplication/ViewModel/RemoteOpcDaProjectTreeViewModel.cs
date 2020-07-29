using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using EasyDriver.Core;
using EasyDriver.Opc.Client.Da;
using EasyDriver.Opc.Client.Da.Browsing;
using System.Threading.Tasks;
using System;
using EasyDriverPlugin;

namespace EasyScada.ServerApplication
{
    public class RemoteOpcDaProjectTreeViewModel : ISupportParameter, ISupportParentViewModel
    {
        #region Constructors

        public RemoteOpcDaProjectTreeViewModel(
            IOpcDaClientManagerService opcDaClientManagerService)
        {
            Title = "Easy Driver Server";
            SizeToContent = SizeToContent.Manual;
            Height = 600;
            Width = 400;
            OpcDaClientManagerService = opcDaClientManagerService;
            Source = new List<StationClient>();
        }

        #endregion

        #region Injected services

        protected IOpcDaClientManagerService OpcDaClientManagerService { get; set; }

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
        public bool IsCreateStationMode { get; private set; }
        public List<StationClient> Source { get; set; }
        public OpcDaServer OpcDaServer { get; private set; }

        #endregion

        #region Commands

        public void Refresh()
        {
            IsBusy = true;
            try
            {
                DispatcherService.BeginInvoke(async () =>
                {
                    if (OpcDaServer.IsConnected)
                    {
                        try
                        {
                            StationClient station = new StationClient();
                            station.StationType = StationType.OPC_DA;
                            station.Name = OpcDaServer.Uri.ToString();
                            OpcDaBrowserAuto browser = new OpcDaBrowserAuto(OpcDaServer);
                            station.Channels = await GetChannelClientsAsync(browser, null);
                            Source = new List<StationClient>() { station };
                            this.RaisePropertyChanged(x => x.Source);
                        }   
                        catch
                        {
                            MessageBoxService.ShowMessage($"Can't connect to server OPC server '{OpcDaServer.Uri.ToString()}'", "Easy Driver Server", MessageButton.OK, MessageIcon.Warning);
                        }
                        finally
                        {
                            IsBusy = false;
                            this.RaisePropertyChanged(x => x.IsBusy);
                        }
                    }
                });
            }
            catch
            {
                IsBusy = false;
                MessageBoxService.ShowMessage($"Can't connect to server OPC server '{OpcDaServer.Uri.ToString()}'", "Easy Driver Server", MessageButton.OK, MessageIcon.Warning);
            }
        }

        Task<List<ChannelClient>> GetChannelClientsAsync(IOpcDaBrowser browser, string itemId)
        {
            return Task.Run(() => GetChannelClients(browser, itemId));
        }

        List<ChannelClient> GetChannelClients(IOpcDaBrowser browser, string itemId)
        {
            List<ChannelClient> result = new List<ChannelClient>();
            OpcDaBrowseElement[] elements = browser.GetElements(itemId);
            foreach (OpcDaBrowseElement element in elements)
            {
                if (!element.Name.StartsWith("_"))
                {
                    ChannelClient client = new ChannelClient();
                    client.Name = element.Name;
                    client.DriverName = "";

                    if (!element.HasChildren)
                        client.Devices = new List<DeviceClient>();
                    else
                        client.Devices = GetDeviceClients(browser, element.ItemId);
                    result.Add(client);
                }
            }
            return result;
        }

        List<DeviceClient> GetDeviceClients(IOpcDaBrowser browser, string itemId)
        {
            List<DeviceClient> result = new List<DeviceClient>();
            OpcDaBrowseElement[] elements = browser.GetElements(itemId);
            foreach (OpcDaBrowseElement element in elements)
            {
                if (!element.Name.StartsWith("_"))
                {
                    DeviceClient client = new DeviceClient();
                    client.Name = element.Name;

                    if (!element.HasChildren)
                        client.Tags = new List<TagClient>();
                    else
                    {

                        client.Tags = GetTagClients(browser, element.ItemId);
                        List<string> tagIds = new List<string>();
                        foreach (var tag in client.Tags)
                            tagIds.Add(tag.Parameters["ItemId"].ToString());
                        OpcDaItemProperties[] properties = browser.GetProperties(tagIds);
                        if (properties.Length == client.Tags.Count)
                        {
                            for (int i = 0; i < client.Tags.Count; i++)
                            {
                                TagClient tag = client.Tags[i];
                                OpcDaItemProperties tagProperties = properties[i];
                                foreach (var prop in tagProperties.Properties)
                                {
                                    // DataType
                                    if (prop.PropertyId == (int)OpcDaItemPropertyIds.OPC_PROP_CDT)
                                    {
                                        if (prop.ErrorId.Succeeded && prop.Value != null)
                                            tag.DataType = GetOpcItemDataType(prop.Value.ToString());
                                    }
                                    // Value
                                    else if (prop.PropertyId == (int)OpcDaItemPropertyIds.OPC_PROP_VALUE)
                                    {
                                        if (prop.ErrorId.Succeeded && prop.Value != null)
                                            tag.Value = prop.Value.ToString();
                                    }
                                    // Quality
                                    else if (prop.PropertyId == (int)OpcDaItemPropertyIds.OPC_PROP_QUALITY)
                                    {
                                        if (prop.ErrorId.Succeeded && prop.Value != null)
                                        {
                                            if (int.TryParse(prop.Value.ToString(), out int value))
                                            {
                                                tag.Quality = GetOpcItemQuality(value);
                                            }
                                        }
                                    }
                                    // TimeStamp
                                    else if (prop.PropertyId == (int)OpcDaItemPropertyIds.OPC_PROP_TIME)
                                    {
                                        if (prop.ErrorId.Succeeded && prop.Value != null)
                                            tag.TimeStamp = Convert.ToDateTime(prop.Value.ToString());
                                    }
                                    // AccessRights
                                    else if (prop.PropertyId == (int)OpcDaItemPropertyIds.OPC_PROP_RIGHTS)
                                    {
                                        if (prop.ErrorId.Succeeded && prop.Value != null)
                                        {
                                            AccessPermission permission = AccessPermission.ReadOnly;
                                            if (int.TryParse(prop.Value.ToString(), out int value))
                                            {
                                                if (value == 3)
                                                    permission = AccessPermission.ReadAndWrite;
                                            }
                                            tag.AccessPermission = permission;
                                        }
                                    }
                                    else if (prop.PropertyId == (int)OpcDaItemPropertyIds.OPC_PROP_SCANRATE)
                                    {
                                        if (prop.ErrorId.Succeeded && prop.Value != null)
                                            tag.RefreshRate = int.Parse(prop.Value.ToString());
                                    }
                                    // Address
                                    else if (prop.PropertyId == 5005)
                                    {
                                        if (prop.ErrorId.Succeeded && prop.Value != null)
                                            tag.Address = prop.Value.ToString();
                                    }
                                }
                            }
                        }
                    }
                    result.Add(client);
                }
            }
            return result;
        }

        List<TagClient> GetTagClients(IOpcDaBrowser browser, string itemId)
        {
            List<TagClient> result = new List<TagClient>();
            OpcDaBrowseElement[] elements = browser.GetElements(itemId);
            foreach (OpcDaBrowseElement element in elements)
            {
                if (!element.Name.StartsWith("_"))
                {
                    TagClient client = new TagClient();
                    client.Name = element.Name;
                    client.Parameters = new Dictionary<string, object>();
                    client.Parameters["ItemId"] = element.ItemId;
                    result.Add(client);
                }
            }
            return result;
        }

        private Quality GetOpcItemQuality(int value)
        {
            // For more information https://www.opcsupport.com/s/article/What-are-the-OPC-Quality-Codes
            Quality quality = Quality.Uncertain;
            if (value >= 0 && value <= 28)
                quality = Quality.Bad;
            else if (value >= 192 && value <= 219)
                quality = Quality.Good;
            else if (value >= 65536 && value <= 65564)
                quality = Quality.Bad;
            return quality;
        }

        private string GetOpcItemDataType(string value)
        {
            string dataType = "Unknown";
            switch (value)
            {
                case "11":
                    return "Bool";
                case "8230":
                    return "Bool Array";
                case "17":
                    return "Byte";
                case "8209":
                    return "Byte Array";
                case "2":
                    return "Short";
                case "8194":
                    return "Short Array";
                case "18+":
                    return "BCD";
                case "18":
                    return "Word";
                case "8210":
                    return "Word Array";
                case "3":
                    return "Long";
                case "8195":
                    return "Long Array";
                case "19":
                    return "DWord";
                case "8211":
                    return "DWord";
                case "4":
                    return "Float";
                case "8196":
                    return "Float Array";
                case "5":
                    return "Double";
                case "8197":
                    return "Double Array";
                case "20":
                    return "LLong";
                case "8212":
                    return "LLong Array";
                case "21":
                    return "QWord";
                case "8213":
                    return "QWord Array";
                case "16":
                    return "Char";
                case "8":
                    return "String";
                default:
                    break;
            }
            return dataType;
        }

        public bool CanRefresh()
        {
            return !IsBusy && OpcDaServer != null;
        }

        public void Comfirm()
        {
            try
            {
                if (IsCreateStationMode)
                {
                    IsBusy = true;
                    StationClient station = Source.FirstOrDefault();
                    if (station != null)
                    {
                        List<ChannelClient> checkedChannels = GetCheckedChannels(station);
                        station.Channels = checkedChannels;
                    }
                    else
                    {
                        station = new StationClient();
                    }
                    Messenger.Default.Send(new CreateRemoteOpcDaStationSuccessMessage(OpcDaServer, station));
                    CurrentWindowService.Close();
                    IsBusy = false;
                }
            }
            catch
            {
            }
            finally { IsBusy = false; }
        }

        public bool CanComfirm()
        {
            return !IsBusy && OpcDaServer != null;
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
                        if (arrays[0] is OpcDaServer)
                        {
                            OpcDaServer = arrays[0] as OpcDaServer;
                            IsCreateStationMode = true;
                        }
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
