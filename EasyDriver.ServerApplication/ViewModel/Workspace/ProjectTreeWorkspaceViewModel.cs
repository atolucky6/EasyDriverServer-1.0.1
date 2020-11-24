using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriverPlugin;
using EasyScada.ServerApplication.Workspace;
using System;
using System.Collections.ObjectModel;
using EasyScada.ServerApplication.Reversible;
using EasyDriver.Core;
using System.IO;
using System.Linq;
using AssemblyHelper = EasyDriver.Core.AssemblyHelper;

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

        protected IInternalStorageService InternalStorageService { get; set; }
        protected IReverseService ReverseService { get; set; }
        protected IProjectManagerService ProjectManagerService { get; set; }
        protected IDriverManagerService DriverManagerService { get; set; }
        protected IHubFactory HubFactory { get; set; }
        protected IRemoteConnectionManagerService RemoteConnectionManagerService { get; set; }

        #endregion

        #region UI services

        protected ISaveFileDialogService SaveFileDialogService { get => this.GetService<ISaveFileDialogService>(); }
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
            IRemoteConnectionManagerService remoteConnectionManagerService,
            IInternalStorageService internalStorageService) : base(null, workspaceManagerService)
        {
            WorkspaceName = WorkspaceRegion.ProjectTree;
            Caption = "Project Explorer";
            WorkspaceManagerService = workspaceManagerService;
            ReverseService = reverseService;
            ProjectManagerService = projectManagerService;
            DriverManagerService = driverManagerService;
            HubFactory = hubFactory;
            RemoteConnectionManagerService = remoteConnectionManagerService;
            InternalStorageService = internalStorageService;

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
            
        /// <summary>
        /// Hảm xử lý sự kiện khi click vào item trên project tree
        /// </summary>
        /// <param name="item"></param>
        public virtual void ShowPropertyOnClick(object item)
        {
            // Gửi message tới Properties page để hiện thị item đã chọn 
            if (item != null)
                Messenger.Default.Send(new ShowPropertiesMessage(this, item));
        }

        #endregion

        #region Event handlers

        public virtual void OnLoaded()
        {
            IoC.Instance.Kernel.Bind<ITreeListViewUtilities>().ToConstant(TreeListViewUtilities);
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
                        // Xóa connection liên quan tới RemoteStation
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
                        foreach (var i in localStation.Childs)
                        {
                            if (i is IChannelCore channel)
                            {
                                string driverPath = channel.DriverPath;
                                string driverName = Path.GetFileName(driverPath);
                                driverPath = $"{Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)}\\Driver\\{driverName}.dll";

                                // Thêm driver tương ứng với channel vào DriverManagerService
                                IEasyDriverPlugin driver = DriverManagerService.AddDriver(channel, driverPath); 
                                if (driver != null)
                                {
                                    // Khởi động driver bằng hàm connect
                                    driver?.Start(channel);
                                    var dataTypes = driver.SupportDataTypes;
                                    foreach (var tag in channel.GetAllTags())
                                    {
                                        // Gán data type cho tag. Danh sách DataType lấy từ driver
                                        tag.DataType = dataTypes.FirstOrDefault(x => x.Name == tag.DataTypeName);

                                        // Nếu tag là internal tag thì phục hồi lại giá trị nếu tag được set retain
                                        if (tag.IsInternalTag)
                                        {
                                            // Nếu tag ko có GUID thì tạo mới GUID và lưu vào store nếu cần thiết
                                            if (string.IsNullOrEmpty(tag.GUID))
                                            {
                                                tag.GUID = Guid.NewGuid().ToString();
                                                if (tag.Retain)
                                                    InternalStorageService.AddOrUpdateInternalTag(tag);
                                            }
                                            else
                                            {
                                                if (tag.Retain)
                                                {
                                                    // Nếu tag được set retain thì lấy giá trị từ trong store và set lại cho tag
                                                    var internalValue = InternalStorageService.GetInternalTagValue(tag.GUID);
                                                    if (internalValue != null && internalValue.GUID == tag.GUID)
                                                        tag.Value = internalValue.Value;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    /// Nếu đối tượng là một RemoteStation thì khởi tạo các HubConnection tương ứng với các RemoteStation
                    else if (item is RemoteStation remoteStation)
                    {
                        // Xóa connection liên quan tới RemoteStation
                        RemoteConnectionManagerService.AddConnection(remoteStation);
                    }
                }
            }
        }

        /// <summary>
        /// Hàm sử lý sự kiện double click trên project tree
        /// </summary>
        /// <param name="item"></param>
        public virtual void OpenOnDoubleClick(object item)
        {
            if (IsBusy)
                return;
                
            IsBusy = true;
            if (item != null)
            {
                // Đóng hoặc mở tree node
                TreeListViewUtilities.ToggleCurrentNode();
                // Mở document nếu có
                WorkspaceManagerService.OpenPanel(item, true, true, ParentViewModel);
            }
            IsBusy = false;
        }

        #endregion

        #region Commands

        /// <summary>
        /// Lệnh tạo một OPC DA Station
        /// </summary>
        public void AddOpcDaStation()
        {
            try
            {
                IsBusy = true;
                // Mở cửa số tạo remote opc da
                WindowService.Show("RemoteOpcDaStationView", ProjectManagerService.CurrentProject, this);
                IsBusy = false;
            }
            catch (Exception) { }
            finally { IsBusy = false; }
        }

        /// <summary>
        /// Điều kiện để thực hiện lệnh <see cref="AddOpcDaStation"/>
        /// </summary>
        /// <returns></returns>
        public bool CanAddOpcDaStation()
        {
            if (ProjectManagerService.CurrentProject != null)
                return !IsBusy;
            return false;
        }

        /// <summary>
        /// Lệnh tạo một Easy Remote Station
        /// </summary>
        public void AddStation()
        {
            try
            {
                IsBusy = true;
                // Mở cửa sổ tạo remote station
                WindowService.Show("RemoteStationView", ProjectManagerService.CurrentProject, this);
                IsBusy = false;
            }
            catch (Exception) { }
            finally { IsBusy = false; }
        }

        /// <summary>
        /// Điều kiện để thực hiện lệnh <see cref="AddStation"/>
        /// </summary>
        /// <returns></returns>
        public bool CanAddStation()
        {
            if (ProjectManagerService.CurrentProject != null)
                return !IsBusy;
            return false;
        }

        /// <summary>
        /// Lệnh tạo một channel
        /// </summary>
        public void AddChannel()
        {
            try
            {
                IsBusy = true;
                // Mở cửa số tạo channel
                WindowService.Show("AddChannelView", SelectedItem, this);
            }
            catch (Exception) { }
            finally { IsBusy = false; }
        }

        /// <summary>
        /// Điều kiện để thực hiện lệnh <see cref="AddChannel"/>
        /// </summary>
        /// <returns></returns>
        public bool CanAddChannel()
        {
            // Điều kiện để tạo channel là item đang chọn trên project tree phải là LocalStation
            if (SelectedItem is LocalStation localStation)
                return !IsBusy && localStation.Parent is IEasyScadaProject;
            return false;
        }

        /// <summary>
        /// Lệnh tạo một device
        /// </summary>
        public void AddDevice()
        {
            try
            {
                // Đánh dấu chương trình đang bận
                IsBusy = true;

                // Lấy driver của đối tượng được chọn
                IEasyDriverPlugin driver = DriverManagerService.GetDriver(SelectedItem as IGroupItem);
                if (driver != null)
                {
                    // Lấy Create Device View từ driver
                    object addDeviceView = driver.GetCreateDeviceControl(SelectedItem as IGroupItem);
                    // Hiện thị create device view
                    if (ContextWindowService.Show(addDeviceView, "Add Device") is IDeviceCore deviceCore)
                    { 
                        // Bắt đầu undo redo transaction
                        using (Transaction transaction = ReverseService.Begin("Add Device"))
                        {
                            IGroupItem parent = (SelectedItem as IGroupItem);

                            parent.Childs.AsReversibleCollection().Add(deviceCore);
                            this.SetPropertyReversible(x => x.SelectedItem, deviceCore);
                            this.SetPropertyReversible(x => x.CurrentItem, deviceCore);

                            // Đăng ký sự kiện khi transaction undo hoặc redo
                            transaction.Reversing += (s, e) =>
                            {
                                // Mở lại trang này nếu nó tắt
                                WorkspaceManagerService.OpenPanel(this);
                                if (e.Direction == ReverseDirection.Undo)
                                {
                                    // Khi thực hiện undo việc tạo device thì có nghĩa là 
                                    // device đã tạo sẽ bị xóa khỏi parent của nó
                                    // cho nên ta sẽ xóa các trang device 
                                    WorkspaceManagerService.RemovePanel(x => x.Token == deviceCore);
                                }
                                else
                                {
                                    // Nếu thực hiện redo thì device sẽ thêm vào parent của nó
                                    // cho nên cần kiểm tra tên hoặc tạo mới tên device nếu cần thiết
                                    // làm như vậy để đảm bảo tên của các đối tượng trong cùng một level phải là duy nhất
                                    deviceCore.Name = parent.GetUniqueNameInGroup(deviceCore.Name);
                                }
                                TreeListViewUtilities.ExpandNodeByContent(deviceCore);
                            };
                            // Xác nhận transaction hoàn tất
                            transaction.Commit();
                        }
                        TreeListViewUtilities.ExpandCurrentNode();
                    }
                }
            }
            catch (Exception)
            {

            }
            // Đánh dấu chương trình hết bận
            finally { IsBusy = false; }
        }

        /// <summary>
        /// Điều kiện để thực hiện lệnh <see cref="AddDevice"/>
        /// </summary>
        /// <returns></returns>
        public bool CanAddDevice()
        {
            // Điều kiện để tạo device là item được chọn trên project tree là một channel 
            // hoặc là một group với parent là một channel
            // Và item được chọn đó được đánh dấu không phải là ReadOnly
           
            if (SelectedItem is ICoreItem coreItem)
                if (coreItem.IsReadOnly)
                    return false;
            
            if (SelectedItem is IChannelCore channelCore)
                return !IsBusy && !channelCore.IsReadOnly;
            if (SelectedItem is GroupCore groupItem)
            {
                if (groupItem.FindParent<IDeviceCore>(x => x is IDeviceCore) is IDeviceCore)
                    return false;
                if (groupItem.FindParent<IChannelCore>(x => x is IChannelCore) is IChannelCore)
                    return !IsBusy && !groupItem.IsReadOnly;
            }
            return false;
        }
        
        /// <summary>
        /// Lệnh tạo một group
        /// </summary>
        public void AddGroup()
        {
            try
            {
                // Đánh dấu chương trình đang bận
                IsBusy = true;

                // Đối tượng đang chọn trên project tree là cha của group sẽ tạo
                IGroupItem parent = SelectedItem as IGroupItem;
                // Khởi tạo group mới
                GroupCore groupItem = null;

                // Xác định xem group nằm trong device hay channel
                if (parent.FindParent<IDeviceCore>(x => x is IDeviceCore) is IDeviceCore)
                    // Nếu là device thì group này có chứa tag
                    groupItem = new GroupCore(parent, true, false);
                else
                    // Nếu là channel thì group này không chứa tag mà sẽ chứa device
                    groupItem = new GroupCore(parent, false, false);

                // Gán tên cho group. Phải đảm bảo tên của group phải là duy nhất trong cung một level
                groupItem.Name = parent.GetUniqueNameInGroup("Group1");

                // Bắt đầu undo redo transaction
                using (Transaction transaction = ReverseService.Begin("Add group"))
                {
                    parent.Childs.AsReversibleCollection().Add(groupItem);
                    this.SetPropertyReversible(x => x.SelectedItem, groupItem);
                    this.SetPropertyReversible(x => x.CurrentItem, groupItem);

                    // Đăng ký sự kiện khi transaction undo hoặc redo
                    transaction.Reversing += (s, e) =>
                    {
                        // Mở lại trang này nếu nó tắt
                        WorkspaceManagerService.OpenPanel(this);

                        if (e.Direction == ReverseDirection.Undo)
                        {
                            // Khi thực hiện undo việc tạo group thì có nghĩa là 
                            // group đã tạo sẽ bị xóa khỏi parent của nó
                            // cho nên ta sẽ xóa các trang group 
                            WorkspaceManagerService.RemovePanel(x => x.Token == groupItem);
                        }
                        else
                        {
                            // Nếu thực hiện redo thì group sẽ thêm vào parent của nó
                            // cho nên cần kiểm tra tên hoặc tạo mới tên group nếu cần thiết
                            // làm như vậy để đảm bảo tên của các đối tượng trong cùng một level phải là duy nhất
                            groupItem.Name = parent.GetUniqueNameInGroup(groupItem.Name);
                        }
                        TreeListViewUtilities.ExpandNodeByContent(groupItem);
                    };
                    // Xác nhận transaction hoàn tất
                    transaction.Commit();
                }
                TreeListViewUtilities.ExpandCurrentNode();
            }
            catch (Exception) { }
            // Đánh dấu chương trình hết bận
            finally { IsBusy = false; }
        }

        /// <summary>
        /// Điều kiện để thực hiện lệnh <see cref="AddGroup"/>
        /// </summary>
        /// <returns></returns>
        public bool CanAddGroup()
        {
            // Để tạo group thì đối tượng đang chọn trên project tree phải là một group
            // và nó không phải là Station
            if (SelectedItem is IStationCore)
                return false;
            if (SelectedItem is IGroupItem groupItem)
                return !IsBusy && !groupItem.IsReadOnly;
            return false;
        }

        /// <summary>
        /// Lệnh mở document
        /// </summary>
        public void Open()
        {
            IsBusy = true;
            WorkspaceManagerService.OpenPanel(SelectedItem, true, true, ParentViewModel);
            IsBusy = false;
        }

        /// <summary>
        /// Điều kiện để thực hiện lệnh <see cref="Open"/>
        /// </summary>
        /// <returns></returns>
        public bool CanOpen()
        {
            if (SelectedItem is IHaveTag groupItem)
                return !IsBusy && groupItem.HaveTags;
            return false;
        }

        /// <summary>
        /// Lệnh chỉnh sửa một đối tượng
        /// </summary>
        public void Edit()
        {
            try
            {
                // Đánh dấu chương trình đang bận
                IsBusy = true;
                // Nếu đối tượng đang chọn là channel hoặc device thì lấy edit view từ driver và hiện thị nó
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
                // Nếu là một group thì mở GroupView
                else if (SelectedItem is GroupCore group)
                {
                    if (!group.IsReadOnly)
                        WindowService.Show("GroupView", group, this);
                }
                // Nếu là station thì mở các edit view tương ứng với StationType
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
            // Đánh dấu chương trình hết bận
            finally { IsBusy = false; }
        }

        /// <summary>
        /// Điều kiện để thực hiện lệnh <see cref="Edit"/>
        /// </summary>
        /// <returns></returns>
        public bool CanEdit()
        {
            // Điều kiện để thực hiện lệnh edit là đối tượng được
            // đánh dấu không phải là ReadOnly
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

        /// <summary>
        /// Lệnh mở tất cả các node trên tree
        /// </summary>
        public void ExpandAll()
        {
            TreeListViewUtilities.ExpandAll();
        }

        /// <summary>
        /// Điều kiện để thực hiện <see cref="CanExpandAll"/>
        /// </summary>
        /// <returns></returns>
        public bool CanExpandAll()
        {
            return !IsBusy;
        }

        /// <summary>
        /// Lệnh đóng tất cả các node trên tree
        /// </summary>
        public void CollapseAll()
        {
            TreeListViewUtilities.CollapseAll();
        }

        /// <summary>
        /// Điều kiện để thực hiện <see cref="CollapseAll"/>
        /// </summary>
        /// <returns></returns>
        public bool CanCollapseAll()
        {
            return !IsBusy;
        }

        /// <summary>
        /// Lệnh export đối tượng thành file csv
        /// </summary>
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
                        string filePath = SaveFileDialogService.File.GetFullName();
                        CsvBuilder csv = new CsvBuilder();
                        csv.AddColumn("Name").AddColumn("Address").
                            AddColumn("DataType").AddColumn("RefreshRate").AddColumn("AccessPermission").
                            AddColumn("Gain").AddColumn("Offset").AddColumn("Description");

                        foreach (var item in device.Childs)
                        {
                            if (item is ITagCore tag)
                            {
                                csv.AddRow(
                                    tag.Name, tag.Address,
                                    tag.DataType.Name, tag.RefreshRate.ToString(), tag.AccessPermission.ToString(),
                                    tag.Gain.ToString(), tag.Offset.ToString(), tag.Description);
                            }
                        }

                        try
                        {
                            File.WriteAllText(filePath, csv.ToString());
                        }
                        catch (PathTooLongException)
                        {
                            MessageBoxService.ShowMessage($"The specified path or file name exceed the maximun " +
                                $"length. The path must be less than 248 characters, and file names must be less " +
                                $"than 260 character", "Message", MessageButton.OK, MessageIcon.Error);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            MessageBoxService.ShowMessage("The specified path file is read-only or you does " +
                                "not have required permissions", "Message", MessageButton.OK, MessageIcon.Error);
                        }
                        catch (Exception)
                        {
                            MessageBoxService.ShowMessage("An error occurred while opening the file.",
                                "Message", MessageButton.OK, MessageIcon.Error);
                        }
                    }
                }
            }
            catch { }
            finally { IsBusy = false; }
        }

        /// <summary>
        /// Điều kiện để thực hện lệnh <see cref="Export"/>
        /// </summary>
        /// <returns></returns>
        public bool CanExport()
        {
            return !IsBusy && SelectedItem is IDeviceCore;
        }

        /// <summary>
        /// Lệnh import tag file vào device hoặc group
        /// </summary>
        public void Import()
        {

        }

        /// <summary>
        /// Điều kiện để thực hiện kệnh <see cref="Import"/>
        /// </summary>
        /// <returns></returns>
        public bool CanImport()
        {
            return !IsBusy && SelectedItem is IDeviceCore;
        }

        public void ChangeEnabledState()
        {
            //if (SelectedItem is ICoreItem coreItem)
            //{
            //    coreItem.Enabled = !coreItem.Enabled;
            //}
        }

        public bool CanChangeEnabledState()
        {
            return !IsBusy && SelectedItem != null;
        }

        #endregion

        #region ISupportEdit

        /// <summary>
        /// Lệnh copy đối tượng
        /// </summary>
        public void Copy()
        {
            ClipboardManager.CopyToClipboard(SelectedItem, this);
        }

        /// <summary>
        /// Điều kiện để thực hiện lệnh <see cref="Copy"/>
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Lệnh cut đối tượng 
        /// </summary>
        public void Cut()
        {
        }

        /// <summary>
        /// Điều kiện để thực hiện lệnh <see cref="Cut"/>
        /// </summary>
        /// <returns></returns>
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
                                        if (driverPlugin != null)
                                        {
                                            newChannelCore.Name = newChannelCore.Parent.GetUniqueNameInGroup(newChannelCore.Name);
                                            DriverManagerService.AddDriver(newChannelCore, driver);
                                            driverPlugin.Start(newChannelCore);
                                        }
                                        else
                                        {
                                            e.Handled = true;
                                            MessageBoxService.ShowMessage($"Could not load driver '{Path.GetFileNameWithoutExtension(newChannelCore.DriverPath)}'", "Message", MessageButton.OK, MessageIcon.Error);
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
                        MessageBoxService.ShowMessage($"Could not load the driver {Path.GetFileNameWithoutExtension(channelCore.DriverPath)}.", "Message", MessageButton.OK, MessageIcon.Error);
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
                    "Message",
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
                    foreach (var item in (SelectedItem as IGroupItem).Find(x => x is IGroupItem, true))
                        WorkspaceManagerService.RemovePanel(x => x.Token == item);
                    WorkspaceManagerService.RemovePanel(x => x.Token == SelectedItem);

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
                                    if (RemoteConnectionManagerService.ConnectionDictonary.ContainsKey(station))
                                        RemoteConnectionManagerService.RemoveConnection(station);
                                }
                            }
                            else if (e.Direction == ReverseDirection.Undo)
                            {
                                if (itemToRemote is IChannelCore channel)
                                {
                                    IEasyDriverPlugin driver = AssemblyHelper.LoadAndCreateInstance<IEasyDriverPlugin>(channel.DriverPath);
                                    if (driver != null)
                                    {
                                        channel.Name = channel.Parent.GetUniqueNameInGroup(channel.Name);
                                        DriverManagerService.AddDriver(channel, driver);
                                        driver.Start(channel);
                                    }
                                    else
                                    {
                                        e.Handled = true;
                                        MessageBoxService.ShowMessage($"Could not load driver '{Path.GetFileNameWithoutExtension(channel.DriverPath)}'", "Message", MessageButton.OK, MessageIcon.Error);
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
