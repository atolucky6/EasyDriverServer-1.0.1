using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriver.ContextWindow;
using EasyDriver.CopyPaste;
using EasyDriver.Core;
using EasyDriver.CoreItemFactory;
using EasyDriver.DriverManager;
using EasyDriver.ProjectManager;
using EasyDriver.RemoteConnectionManager;
using EasyDriver.Reversible;
using EasyDriver.SyncContext;
using EasyDriverPlugin;
using EasyDriverServer.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace EasyDriverServer.Workspace.ProjectTree
{
    public class ProjectTreeWorkspaceViewModel : WorkspacePanelViewModel, ISupportCopyPaste
    {
        #region Constructors

        public ProjectTreeWorkspaceViewModel(
            IReverseService reverseService,
            IProjectManagerService projectManagerService,
            IDriverManagerService driverManagerService,
            IApplicationSyncService applicationSyncService,
            ICoreFactory coreFactory,
            ICopyPasteService copyPasteService,
            IRemoteConnectionManagerService remoteConnectionManagerService,
            IContextWindowService contextWindowService) : base()
        {
            ReverseService = reverseService;
            ProjectManagerService = projectManagerService;
            DriverManagerService = driverManagerService;
            ApplicationSyncService = applicationSyncService;
            CoreFactory = coreFactory;
            CopyPasteService = copyPasteService;
            RemoteConnectionManagerService = remoteConnectionManagerService;
            ContextWindowService = contextWindowService;

            Caption = "Project Tree";
            UriTemplate = new Uri("pack://application:,,,/EasyDriverServer.Workspace.ProjectTree;component/Template.xaml", UriKind.Absolute);
            WorkspaceRegion = "ProjectTree";
        }

        #endregion

        #region Injected services

        protected IReverseService ReverseService { get; set; }
        protected IProjectManagerService ProjectManagerService { get; set; }
        protected IDriverManagerService DriverManagerService { get; set; }
        protected IApplicationSyncService ApplicationSyncService { get; set; }
        protected ICoreFactory CoreFactory { get; set; }
        protected ICopyPasteService CopyPasteService { get; set; }
        protected IRemoteConnectionManagerService RemoteConnectionManagerService { get; set; }
        protected IContextWindowService ContextWindowService { get; set; }

        #endregion

        #region UI services

        protected ISaveFileDialogService SaveFileDialogService { get => this.GetService<ISaveFileDialogService>(); }
        protected ITreeListViewUtilities TreeListViewUtilities { get => this.GetService<ITreeListViewUtilities>(); }
        protected IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }
        public IWindowService WindowService { get => this.GetService<IWindowService>(); }

        #endregion

        #region Public properties

        public override string WorkspaceRegion { get; protected set; }
        public IEasyScadaProject CurrentProject => ProjectManagerService.CurrentProject;
        public virtual object SelectedItem { get; set; }
        public virtual object CurrentItem { get; set; }
        public virtual ObservableCollection<object> SelectedItems { get; set; }

        #endregion

        #region Event handlers

        /// <summary>
        /// Hàm sử lý sự kiện khi click vào item ở project tree
        /// </summary>
        /// <param name="item"></param>
        public virtual void ShowPropertyOnClick(object item)
        {
        }

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
                        RemoteConnectionManagerService.RemoveConnection(remoteStation);
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
                                }
                            }
                        }
                    }
                    /// Nếu đối tượng là một RemoteStation thì khởi tạo các HubConnection tương ứng với các RemoteStation
                    else if (item is RemoteStation remoteStation)
                    {
                        if (remoteStation.StationType == "OPC_DA")
                        {
                            // Xóa connection liên quan tới RemoteStation
                            //OpcDaClientManagerService.AddConnection(remoteStation);
                        }
                        else if (remoteStation.StationType == "Remote")
                        {
                            // Xóa connection liên quan tới RemoteStation
                            //HubConnectionManagerService.AddConnection(remoteStation);
                        }
                    }
                }
            }
        }

        #endregion

        #region Commands

        public void AddOpcDaStation()
        {
            try
            {
                IsBusy = true;
                WindowService.Show("RemoteOpcDaStationView", ProjectManagerService.CurrentProject, this);
                IsBusy = false;
            }
            catch (Exception)
            {

            }
            finally { IsBusy = false; }
        }

        public bool CanAddOpcDaStation()
        {
            if (ProjectManagerService.CurrentProject != null)
                return !IsBusy;
            return false;
        }

        public void AddStation()
        {
            try
            {
                IsBusy = true;
                IStationCore localStation = ProjectManagerService.CurrentProject.LocalStation;
                IChannelCore channelCore = CoreFactory.ShowCreateChannelView(localStation, out IEasyDriverPlugin driver);
                if (channelCore != null && driver != null)
                {
                    DriverManagerService.AddDriver(channelCore, driver);
                    localStation.Childs.Add(channelCore);
                }
                IsBusy = false;
            }
            catch (Exception)
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
            catch (Exception)
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
                    if (CoreFactory.ShowCreateDeviceView(SelectedItem as IGroupItem, driver) is IDeviceCore deviceCore)
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
                                    WorkspaceManagerService.RemovePanel(x => x.WorkspaceContext == deviceCore);
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
            catch (Exception)
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
                    CoreFactory.ShowEditChannelView(channel, driver);
                }
                else if (SelectedItem is IDeviceCore device)
                {
                    IEasyDriverPlugin driver = DriverManagerService.GetDriver(device);
                    CoreFactory.ShowEditDeviceView(device, driver);
                }
                else if (SelectedItem is RemoteStation remoteStation)
                {
                    if (remoteStation.StationType == "OPC_DA")
                    {
                        WindowService.Show("RemoteOpcDaStationView", SelectedItem, this);
                    }
                    else if (remoteStation.StationType == "Remote")
                    {
                        WindowService.Show("RemoteStationView", SelectedItem, this);
                    }
                }
            }
            catch (Exception)
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

        public void Export()
        {
            try
            {
                if (SelectedItem is IDeviceCore device)
                {
                    SaveFileDialogService.Title = "Export Csv";
                    SaveFileDialogService.Filter = "CSV Files (*.csv)|*.csv";
                    SaveFileDialogService.DefaultExt = ".csv";
                    SaveFileDialogService.DefaultFileName = device.Name;
                    if (SaveFileDialogService.ShowDialog())
                    {
                        IsBusy = true;

                        //string filePath = SaveFileDialogService.File.GetFullName();
                        //CsvBuilder csv = new CsvBuilder();
                        //csv.AddColumn("Name").AddColumn("Address").
                        //    AddColumn("DataType").AddColumn("RefreshRate").AddColumn("AccessPermission").
                        //    AddColumn("Gain").AddColumn("Offset").AddColumn("Description");
                        //foreach (var item in device.Childs)
                        //{
                        //    if (item is ITagCore tag)
                        //    {
                        //        csv.AddRow(
                        //            tag.Name, tag.Address,
                        //            tag.DataType.Name, tag.RefreshRate.ToString(), tag.AccessPermission.ToString(),
                        //            tag.Gain.ToString(), tag.Offset.ToString(), tag.Description);
                        //    }
                        //}

                        try
                        {
                            //File.WriteAllText(filePath, csv.ToString());
                        }
                        catch (PathTooLongException)
                        {
                            MessageBoxService.ShowMessage($"The specified path or file name exceed the maximun " +
                                $"length. The path must be less than 248 characters, and file names must be less " +
                                $"than 260 character", "Easy Driver Server", MessageButton.OK, MessageIcon.Error);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            MessageBoxService.ShowMessage("The specified path file is read-only or you does " +
                                "not have required permissions", "Easy DriverServer", MessageButton.OK, MessageIcon.Error);
                        }
                        catch (Exception)
                        {
                            MessageBoxService.ShowMessage("An error occurred while opening the file.",
                                "Easy Driver Server", MessageButton.OK, MessageIcon.Error);
                        }
                    }
                }
            }
            catch { }
            finally { IsBusy = false; }
        }

        public bool CanExport()
        {
            return !IsBusy && SelectedItem is IDeviceCore;
        }

        public void Import()
        {

        }

        public bool CanImport()
        {
            return !IsBusy && SelectedItem is IDeviceCore;
        }

        #endregion

        #region ISupportEdit

        public void Copy()
        {
            CopyPasteService.CopyToClipboard(SelectedItem, this);
        }

        public bool CanCopy()
        {
            if (IsBusy || CurrentProject == null)
                return false;
            if (SelectedItem is IChannelCore)
                return false;
            if (SelectedItem is ICoreItem coreItem)
                return !coreItem.IsReadOnly && !IsBusy;
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
                if (SelectedItem is IChannelCore selectedChannel && 
                    !selectedChannel.IsReadOnly && 
                    CopyPasteService.ObjectToCopy is IDeviceCore deviceCore)
                {
                    IsBusy = true;
                    IEasyDriverPlugin driver = DriverManagerService.GetDriver(SelectedItem as IChannelCore);
                    if (driver != null)
                    {
                        IDeviceCore newDeviceCore = CoreFactory.ShowCreateDeviceView(selectedChannel, driver, deviceCore);
                        if (newDeviceCore != null)
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
                                        WorkspaceManagerService.RemovePanel(x => x.WorkspaceContext == newDeviceCore);
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
            if (IsBusy || 
                !CopyPasteService.ContainData() || 
                SelectedItem == null || 
                CopyPasteService.Context != this)
            {
                return false;
            }
            if (SelectedItem is IChannelCore selectedChannel && 
                !selectedChannel.IsReadOnly && 
                CopyPasteService.ObjectToCopy is IDeviceCore deviceCore)
            {
                return !IsBusy && (deviceCore.Parent as IChannelCore).DriverPath == selectedChannel.DriverPath;
            }
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
                        RemoteConnectionManagerService.RemoveConnection(remoteStation);

                    // Nếu đối tượng là một Channel thì ta cần phải xỏa
                    // DriverPlugin liên quan đến nó ở IDriverManagerService
                    if (SelectedItem is ChannelCore channelCore)
                        DriverManagerService.RemoveDriver(channelCore);

                    // Xóa tất cả các TagDocument liên quan
                    foreach (var item in (SelectedItem as IGroupItem).Find(x => x is IDeviceCore, true))
                        WorkspaceManagerService.RemovePanel(x => x.WorkspaceContext == item);
                    if (SelectedItem is IDeviceCore deviceCore)
                        WorkspaceManagerService.RemovePanel(x => x.WorkspaceContext == deviceCore);

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
                                    RemoteConnectionManagerService.RemoveConnection(station);
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
                                        MessageBoxService.ShowMessage($"Could not load driver '{Path.GetFileNameWithoutExtension(channel.DriverPath)}'", "Easy Driver Server", MessageButton.OK, MessageIcon.Error);
                                    }
                                }
                                else if (itemToRemote is RemoteStation station)
                                {
                                    station.Name = station.Parent.GetUniqueNameInGroup(station.Name);
                                    RemoteConnectionManagerService.AddConnection(station);
                                }
                            }
                        };

                        transaction.Reversed += (s, e) =>
                        {
                            if (e.Direction == ReverseDirection.Redo)
                            {
                                // Xóa tất cả các TagDocument liên quan
                                foreach (var item in (itemToRemote as IGroupItem).Find(x => x is IDeviceCore, true))
                                    WorkspaceManagerService.RemovePanel(x => x.WorkspaceContext == item);
                                if (itemToRemote is IDeviceCore device)
                                    WorkspaceManagerService.RemovePanel(x => x.WorkspaceContext == device);
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
