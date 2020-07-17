﻿using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriverPlugin;
using EasyScada.ServerApplication.Workspace;
using System;
using System.Collections.ObjectModel;
using EasyScada.ServerApplication.Reversible;
using EasyDriver.Core;

namespace EasyScada.ServerApplication
{
    public class ProjectTreeWorkspaceViewModel : WorkspacePanelViewModelBase, ISupportEdit
    {
        #region Public members

        public override bool IsBusy
        {
            get => MainViewModel.IsBusy;
            set => MainViewModel.IsBusy = value;
        }

        public MainViewModel MainViewModel { get => ParentViewModel as MainViewModel; }

        #endregion

        #region Injected members

        protected IReverseService ReverseService { get; set; }
        protected IProjectManagerService ProjectManagerService { get; set; }
        protected IDriverManagerService DriverManagerService { get; set; }
        protected IHubFactory HubFactory { get; set; }
        protected IHubConnectionManagerService HubConnectionManagerService { get; set; }

        #endregion

        #region UI services

        protected ITreeListViewUtilities TreeListViewUtilities { get => this.GetService<ITreeListViewUtilities>(); }
        protected IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }
        public IWindowService WindowService { get => this.GetService<IWindowService>(); }
        protected IContextWindowService ContextWindowService { get => this.GetService<IContextWindowService>(); }

        #endregion

        #region Constructors

        public ProjectTreeWorkspaceViewModel(
            IWorkspaceManagerService workspaceManagerService, 
            IReverseService reverseService,
            IProjectManagerService projectManagerService,
            IDriverManagerService driverManagerService,
            IHubFactory hubFactory,
            IHubConnectionManagerService hubConnectionManagerService) : base(null, workspaceManagerService)
        {
            WorkspaceName = WorkspaceRegion.ProjectTree;
            Caption = "Project Explorer";
            WorkspaceManagerService = workspaceManagerService;
            ReverseService = reverseService;
            ProjectManagerService = projectManagerService;
            DriverManagerService = driverManagerService;
            HubFactory = hubFactory;
            HubConnectionManagerService = hubConnectionManagerService;

            ProjectManagerService.ProjectChanged += OnProjectChanged;

        }

        #endregion

        #region Public members

        public override string WorkspaceName { get; protected set; }

        public IEasyScadaProject CurrentProject => ProjectManagerService.CurrentProject;

        public virtual object SelectedItem { get; set; }

        public virtual object CurrentItem { get; set; }

        public virtual ObservableCollection<object> SelectedItems { get; set; }

        #endregion

        #region Property changed handlers

        public virtual void ShowPropertyOnClick(object item)
        {
            if (item != null)
                Messenger.Default.Send(new ShowPropertiesMessage(this, item));
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Hàm sử lý sự kiện khi project đang xử lý của chương trình thay đổi 
        /// </summary>
        /// <remarks>Hàm chạy khi người dùng thực hiện lệnh New hoặc Open một project khác</remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnProjectChanged(object sender, ProjectChangedEventArgs e)
        {
            // Thông báo thuộc tính CurrentProject thay đổi để cập nhật lên UI
            this.RaisePropertyChanged(x => x.CurrentProject);

            // Xóa tất cả các TagDocument liên quan đến project
            WorkspaceManagerService.RemoveAllDocumentPanel();

            // Nếu project cũ khác null thì xóa tất cả các thông tin liên quan đến project
            // như là các DriverPlugin và HubConnection
            if (e.OldProject != null)
            {
                // Loop qua tất cả các đối tượng con của project để xử lý
                foreach (var item in e.OldProject.Childs)
                {
                    // Nếu đối tượng con là 1 LocalStation
                    if (item is LocalStation localStation)
                    {
                        // Xóa tất cả các DriverPlugin liên quan tới Channel có trong LocalStation
                        foreach (var x in localStation.Childs)
                        {
                            if (x is IChannelCore channel)
                                DriverManagerService.RemoveDriver(channel);
                        }
                    }
                    // Nếu đối tượng con là 1 RemoteStation
                    else if (item is RemoteStation remoteStation)
                    {
                        // Xóa connection liên quan tới RemoteStation
                        HubConnectionManagerService.RemoveConnection(remoteStation);
                    }
                }
            }
            
            // Nếu project mới khác null thì khởi tạo các DriverPlugin và HubConnection
            if (e.NewProject != null)
            {
                // Loop qua các đối tương con của project mới để xử lý
                foreach (var item in e.NewProject.Childs)
                {
                    // Nếu đối tương là một LocalStation thì khới tạo các DriverPlugin tương ứng với các Channel có trong LocalStation
                    if (item is LocalStation localStation)
                    {
                        foreach (var x in localStation.Childs)
                        {
                            if (x is IChannelCore channel)
                            {
                                // Thêm driver tương ứng với channel vào DriverManagerService
                                IEasyDriverPlugin driver = DriverManagerService.AddDriver(channel, channel.DriverPath); 
                                if (driver != null)
                                {
                                    // Khởi động driver bằng hàm connect
                                    driver?.Connect();
                                    //DriverManagerService.AddDriver(channel, driver);
                                }
                            }
                        }
                    }
                    /// Nếu đối tượng là một RemoteStation thì khởi tạo các HubConnection tương ứng với các RemoteStation
                    else if (item is RemoteStation remoteStation)
                    {
                        // Thêm Hubconnection vào HubConnectionManagerService
                        HubConnectionManagerService.AddConnection(remoteStation);
                    }
                }
            }
        }

        public virtual void OpenOnDoubleClick(object item)
        {
            if (IsBusy)
                return;

            IsBusy = true;
            if (item != null)
            {
                TreeListViewUtilities.ToggleCurrentNode();
                WorkspaceManagerService.OpenPanel(item, true, true, ParentViewModel);
            }
            IsBusy = false;
        }

        #endregion

        #region Commands

        public void AddStation()
        {
            try
            {
                IsBusy = true;
                WindowService.Show("RemoteStationView", ProjectManagerService.CurrentProject, this);
                IsBusy = false;
            }
            catch (Exception ex)
            {

            }
            finally { IsBusy = false; }
        }

        public bool CanAddStation()
        {
            if (ProjectManagerService.CurrentProject != null)
                return !IsBusy;
            return false;
        }

        public void AddChannel()
        {
            try
            {
                IsBusy = true;
                WindowService.Show("AddChannelView", SelectedItem, this);
            }
            catch (Exception ex)
            {

            }
            finally { IsBusy = false; }
        }

        public bool CanAddChannel()
        {
            if (SelectedItem is LocalStation localStation)
                return !IsBusy && localStation.Parent is IEasyScadaProject;
            return false;
        }

        public void AddDevice()
        {
            try
            {
                IsBusy = true;
                IEasyDriverPlugin driver = DriverManagerService.GetDriver(SelectedItem as IChannelCore);
                if (driver != null)
                {
                    if (ContextWindowService.Show(driver.GetCreateDeviceControl(SelectedItem as IChannelCore), "Add Device") is IDeviceCore deviceCore)
                    {
                        using (Transaction transaction = ReverseService.Begin("Add Device"))
                        {
                            IChannelCore parent = SelectedItem as IChannelCore;
                            parent.Childs.AsReversibleCollection().Add(deviceCore);
                            this.SetPropertyReversible(x => x.SelectedItem, deviceCore);
                            this.SetPropertyReversible(x => x.CurrentItem, deviceCore);
                            transaction.Reversing += (s, e) =>
                            {
                                WorkspaceManagerService.OpenPanel(this);
                                if (e.Direction == ReverseDirection.Undo)
                                {
                                    WorkspaceManagerService.RemovePanel(x => x.Token == deviceCore);
                                }
                                else
                                {
                                    deviceCore.Name = parent.GetUniqueNameInGroup(deviceCore.Name);
                                }
                                TreeListViewUtilities.ExpandNodeByContent(deviceCore);
                            };
                            transaction.Commit();
                        }
                        TreeListViewUtilities.ExpandCurrentNode();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally { IsBusy = false; }
        }

        public bool CanAddDevice()
        {
            if (SelectedItem is IChannelCore channelCore)
                return !IsBusy && !channelCore.IsReadOnly;
            return false;
        }

        public void Open()
        {
            IsBusy = true;
            WorkspaceManagerService.OpenPanel(SelectedItem, true, true, ParentViewModel);
            IsBusy = false;
        }

        public bool CanOpen()
        {
            if (SelectedItem is IDeviceCore)
                return !IsBusy;
            return false;
        }

        public void Edit()
        {
            try
            {
                IsBusy = true;
                if (SelectedItem is IChannelCore channel)
                {
                    IEasyDriverPlugin driver = DriverManagerService.GetDriver(channel);
                    ContextWindowService.Show(driver.GetEditChannelControl(channel), $"Edit Channel - {channel.Name}");
                }
                else if (SelectedItem is IDeviceCore device)
                {
                    IEasyDriverPlugin driver = DriverManagerService.GetDriver(device);
                    ContextWindowService.Show(driver.GetEditDeviceControl(device), $"Edit Device - {device.Name}");
                }
                else if (SelectedItem is RemoteStation remoteStation)
                {
                    WindowService.Show("RemoteStationView", SelectedItem, this);
                }
            }
            catch (Exception ex)
            {

            }
            finally { IsBusy = false; }
        }

        public bool CanEdit()
        {
            if (SelectedItem is ICoreItem coreItem && !IsBusy)
            {
                if (!coreItem.IsReadOnly)
                    return !(SelectedItem is LocalStation);
                else
                {
                    if (SelectedItem is RemoteStation remoteStation)
                        return remoteStation.Parent is IEasyScadaProject;
                }
            }
            return false;
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

        #region ISupportEdit

        public void Copy()
        {
            ClipboardManager.CopyToClipboard(SelectedItem, this);
        }

        public bool CanCopy()
        {
            if (IsBusy || CurrentProject == null)
                return false;
            if (SelectedItem is ICoreItem coreItem)
            {
                return !coreItem.IsReadOnly && !IsBusy;
            }
            return false;
        }

        public void Cut()
        {
        }

        public bool CanCut()
        {
            return false;
        }

        public void Paste()
        {
            try
            {
                if (SelectedItem is LocalStation localStation && localStation.Parent is IEasyScadaProject && ClipboardManager.ObjectToCopy is IChannelCore channelCore)
                {
                    IsBusy = true;
                    IChannelCore newChannelCore = new ChannelCore(localStation)
                    {
                        Name = localStation.GetUniqueNameInGroup(channelCore.Name),
                        DriverPath = channelCore.DriverPath
                    };
                    IEasyDriverPlugin driver = AssemblyHelper.LoadAndCreateInstance<IEasyDriverPlugin>(newChannelCore.DriverPath);
                    if (driver != null)
                    {
                        driver.Channel = newChannelCore;
                        if (ContextWindowService.Show(driver.GetCreateChannelControl(localStation, channelCore), "Add Channel") == newChannelCore)
                        {
                            using (Transaction transaction = ReverseService.Begin("Paste Channel"))
                            {
                                DriverManagerService.AddDriver(newChannelCore, driver);
                                newChannelCore.Parent.Childs.AsReversibleCollection().Add(newChannelCore);
                                this.SetPropertyReversible(x => x.SelectedItem, newChannelCore);
                                this.SetPropertyReversible(x => x.CurrentItem, newChannelCore);
                                transaction.Reversing += (s, e) =>
                                {
                                    WorkspaceManagerService.OpenPanel(this);
                                    if (e.Direction == ReverseDirection.Undo)
                                    {
                                        // Xóa tất cả các TagDocument liên quan
                                        foreach (var item in (newChannelCore as IGroupItem).Find(x => x is IDeviceCore, true))
                                            WorkspaceManagerService.RemovePanel(x => x.Token == item);
                                        if (DriverManagerService.DriverPoll.ContainsKey(newChannelCore))
                                        {
                                            DriverManagerService.DriverPoll[newChannelCore].Dispose();
                                            DriverManagerService.RemoveDriver(newChannelCore);
                                        }
                                    }
                                    else
                                    {
                                        IEasyDriverPlugin driverPlugin = AssemblyHelper.LoadAndCreateInstance<IEasyDriverPlugin>(newChannelCore.DriverPath);
                                        driverPlugin.Channel = newChannelCore;
                                        if (driverPlugin != null)
                                        {
                                            newChannelCore.Name = newChannelCore.Parent.GetUniqueNameInGroup(newChannelCore.Name);
                                            DriverManagerService.AddDriver(newChannelCore, driver);
                                            driverPlugin.Connect();
                                        }
                                        else
                                        {
                                            e.Handled = true;
                                            MessageBoxService.ShowMessage($"Could not load driver '{(newChannelCore as IChannelClient).DriverName}'", "Easy Driver Server", MessageButton.OK, MessageIcon.Error);
                                        }
                                    }
                                    TreeListViewUtilities.ExpandNodeByContent(newChannelCore);
                                };
                                transaction.Commit();
                            }
                        }
                    }
                    else
                    {
                        MessageBoxService.ShowMessage($"Could not load the driver {(channelCore as IChannelClient).DriverName}.", "Easy Driver Server", MessageButton.OK, MessageIcon.Error);
                    }
                }
                else if (SelectedItem is IChannelCore selectedChannel && !selectedChannel.IsReadOnly && ClipboardManager.ObjectToCopy is IDeviceCore deviceCore)
                {
                    IsBusy = true;
                    IEasyDriverPlugin driver = DriverManagerService.GetDriver(SelectedItem as IChannelCore);
                    if (driver != null)
                    {
                        if (ContextWindowService.Show(driver.GetCreateDeviceControl(SelectedItem as IChannelCore, deviceCore), "Add Device") is IDeviceCore newDeviceCore)
                        { 
                            using (Transaction transaction = ReverseService.Begin("Add Device"))
                            {
                                IChannelCore parent = SelectedItem as IChannelCore;
                                parent.Childs.AsReversibleCollection().Add(newDeviceCore);
                                this.SetPropertyReversible(x => x.SelectedItem, newDeviceCore);
                                this.SetPropertyReversible(x => x.CurrentItem, newDeviceCore);
                                transaction.Reversing += (s, e) =>
                                {
                                    WorkspaceManagerService.OpenPanel(this);
                                    if (e.Direction == ReverseDirection.Undo)
                                    {
                                        WorkspaceManagerService.RemovePanel(x => x.Token == newDeviceCore);
                                    }
                                    else
                                    {
                                        newDeviceCore.Name = parent.GetUniqueNameInGroup(newDeviceCore.Name);
                                    }
                                    TreeListViewUtilities.ExpandNodeByContent(newDeviceCore);
                                };
                                transaction.Commit();
                            }
                        }
                    }
                }
            }
            catch { }
            finally { IsBusy = false; }
        }

        public bool CanPaste()
        {
            if (IsBusy || !ClipboardManager.ContainData() || SelectedItem == null || ClipboardManager.Context != this)
                return false;
            if (SelectedItem is LocalStation localStation && localStation.Parent is IEasyScadaProject && ClipboardManager.ObjectToCopy is IChannelCore)
                return !IsBusy;
            else if (SelectedItem is IChannelCore selectedChannel && !selectedChannel.IsReadOnly && ClipboardManager.ObjectToCopy is IDeviceCore deviceCore)
                return !IsBusy && (deviceCore.Parent as IChannelCore).DriverPath == selectedChannel.DriverPath;
            return false;
        }

        /// <summary>
        /// Lệnh thực hiện việc xóa đối tượng
        /// </summary>
        public void Delete()
        {
            try
            {
                // Hỏi người dùng có muốn xóa đối tượng đang chọn hay không
                var mbr = MessageBoxService.ShowMessage($"Do you want to delete '{(SelectedItem as ICoreItem).Name}' and all object associated with it?",
                    "Easy Driver Server",
                    MessageButton.YesNo, MessageIcon.Question);

                // Nếu người dùng chọn 'Yes' thì thực hiện việc xóa đối tượng
                if (mbr == MessageResult.Yes)
                {
                    // Khóa luồng làm việc của chương trình
                    IsBusy = true;
                    // Lấy đối tương cha của đối tượng cần xóa
                    IGroupItem parent = (SelectedItem as ICoreItem).Parent;

                    // Nếu đối tượng là một RemoteStation thì ta cần phải xóa
                    // HubConnection liên quan đến nó ở IHubConnectionManagerService
                    if (SelectedItem is RemoteStation remoteStation)
                        HubConnectionManagerService.RemoveConnection(remoteStation);

                    // Nếu đối tượng là một Channel thì ta cần phải xỏa
                    // DriverPlugin liên quan đến nó ở IDriverManagerService
                    if (SelectedItem is ChannelCore channelCore)
                        DriverManagerService.RemoveDriver(channelCore);

                    // Xóa tất cả các TagDocument liên quan
                    foreach (var item in (SelectedItem as IGroupItem).Find(x => x is IDeviceCore, true))
                        WorkspaceManagerService.RemovePanel(x => x.Token == item);
                    if (SelectedItem is IDeviceCore deviceCore)
                        WorkspaceManagerService.RemovePanel(x => x.Token == deviceCore);

                    object itemToRemote = SelectedItem;

                    using (Transaction transaction = ReverseService.Begin($"Delete {SelectedItem.GetClassName(true)}"))
                    {
                        this.SetPropertyReversible(x => x.CurrentItem, CurrentItem);
                        this.SetPropertyReversible(x => x.SelectedItem, SelectedItem);

                        (itemToRemote as ICoreItem).Parent.Childs.AsReversibleCollection().Remove(itemToRemote);

                        transaction.Reversing += (s, e) =>
                        {
                            WorkspaceManagerService.OpenPanel(this);
                            if (e.Direction == ReverseDirection.Redo)
                            {
                                if (itemToRemote is IChannelCore channel)
                                {
                                    if (DriverManagerService.DriverPoll.ContainsKey(channel))
                                    {
                                        DriverManagerService.DriverPoll[channel].Dispose();
                                        DriverManagerService.RemoveDriver(channel);
                                    }
                                }
                                else if (itemToRemote is RemoteStation station)
                                {
                                    if (HubConnectionManagerService.ConnectionDictonary.ContainsKey(station))
                                        HubConnectionManagerService.RemoveConnection(station);
                                }
                            }
                            else if (e.Direction == ReverseDirection.Undo)
                            {
                                if (itemToRemote is IChannelCore channel)
                                {
                                    IEasyDriverPlugin driver = AssemblyHelper.LoadAndCreateInstance<IEasyDriverPlugin>(channel.DriverPath);
                                    driver.Channel = channel;
                                    if (driver != null)
                                    {
                                        channel.Name = channel.Parent.GetUniqueNameInGroup(channel.Name);
                                        DriverManagerService.AddDriver(channel, driver);
                                        driver.Connect();
                                    }
                                    else
                                    {
                                        e.Handled = true;
                                        MessageBoxService.ShowMessage($"Could not load driver '{(channel as IChannelClient).DriverName}'", "Easy Driver Server", MessageButton.OK, MessageIcon.Error);
                                    }
                                }
                                else if (itemToRemote is RemoteStation station)
                                {
                                    station.Name = station.Parent.GetUniqueNameInGroup(station.Name);
                                    HubConnectionManagerService.AddConnection(station);
                                }
                            }
                        };

                        transaction.Reversed += (s, e) =>
                        {
                            if (e.Direction == ReverseDirection.Redo)
                            {
                                // Xóa tất cả các TagDocument liên quan
                                foreach (var item in (itemToRemote as IGroupItem).Find(x => x is IDeviceCore, true))
                                    WorkspaceManagerService.RemovePanel(x => x.Token == item);
                                if (itemToRemote is IDeviceCore device)
                                    WorkspaceManagerService.RemovePanel(x => x.Token == device);
                            }
                            else
                            {
                                TreeListViewUtilities.ExpandNodeByContent(itemToRemote);
                            }
                        };

                        transaction.Commit();
                    }
                }
            }
            catch { }
            // Mở khóa luồng làm việc của chương trình
            finally { IsBusy = false; }
        }

        /// <summary>
        /// Điều kiện để thực hiện lệnh xóa đối tượng
        /// </summary>
        /// <returns></returns>
        public bool CanDelete()
        {
            // TH1: Đối tượng đang chọn là một RemoteStation
            if (SelectedItem is RemoteStation remoteStation)
                return !IsBusy; // Cho phép xóa nếu chương trình không bận

            // TH2: Đối tượng đang chọn là một CoreItem
            if (SelectedItem is ICoreItem coreItem)
                return !coreItem.IsReadOnly && !IsBusy; // Cho phép xóa nếu chương trình không bận và CoreItem không phải ReadOnly

            // Các trường hợp khác thì không cho xóa
            return false;
        }

        #endregion
    }
}
