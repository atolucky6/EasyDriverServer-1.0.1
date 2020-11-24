using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.Docking;
using EasyDriver.RemoteConnectionPlugin;
using EasyDriver.Service.ApplicationProperties;
using EasyDriver.Service.BarManager;
using EasyDriver.Service.Clipboard;
using EasyDriver.Service.ContextWindow;
using EasyDriver.Service.DriverManager;
using EasyDriver.Service.InternalStorage;
using EasyDriver.Service.ProjectManager;
using EasyDriver.Service.RemoteConnectionManager;
using EasyDriver.Service.Reversible;
using EasyDriver.ServicePlugin;
using EasyDriver.WorkspacePlugin;
using EasyDriverPlugin;
using EasyScada.WorkspaceManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EasyDriver.Workspace.Main
{
    [Service(int.MaxValue, true)]
    public class ProjectTreeViewModel : WorkspacePanel, IMVVMDockingProperties, ISupportCopyPaste
    {
        #region Public properties
        public virtual ObservableCollection<IBarComponent> BarItems { get; protected set; }
        public virtual ObservableCollection<IBarComponent> TreeViewContextMenuSource { get; protected set; }
        public virtual object SelectedItem { get; set; }
        public virtual object CurrentItem { get; set; }
        public virtual ObservableCollection<object> SelectedItems { get; set; } = new ObservableCollection<object>();
        public virtual bool AllowDragDrop { get; set; }

        /// <summary>
        /// Project đang được xử lý bởi chương trình
        /// </summary>
        public IProjectItem CurrentProject => ProjectManagerService.CurrentProject;
        #endregion

        #region Bar items
        public IBarComponent MainMenu { get => BarManagerService.MainMenu; }
        public IBarComponent Toolbar { get => BarManagerService.Toolbar; }
        public IBarComponent StatusBar { get => BarManagerService.StatusBar; }
        public IBarComponent InternalToolbar { get; set; }
        
        public IBarComponent FileMenu { get; set; }
        public IBarComponent EditMenu { get; set; }
        public IBarComponent ViewMenu { get; set; }
        public IBarComponent ToolsMenu { get; set; }

        // Bar items for File menu
        public IBarComponent NewBarItem { get; set; }
        public IBarComponent OpenBarItem { get; set; }
        public IBarComponent SaveBarItem { get; set; }
        public IBarComponent SaveAsBarItem { get; set; }
        public IBarComponent ExitBarItem { get; set; }

        // Bar items for Edit menu
        public IBarComponent UndoBarItem { get; set; }
        public IBarComponent RedoBarItem { get; set; }
        public IBarComponent AddRemoteStationBarItem { get; set; }
        public IBarComponent AddChannelBarItem { get; set; }
        public IBarComponent AddDeviceBarItem { get; set; }
        public IBarComponent AddGroupBarItem { get; set; }
        public IBarComponent AddTagBarItem { get; set; }
        public IBarComponent AddInternalTagBarItem { get; set; }
        public IBarComponent CopyBarItem { get; set; }
        public IBarComponent CutBarItem { get; set; }
        public IBarComponent PasteBarItem { get; set; }
        public IBarComponent DeleteBarItem { get; set; }

        // Internal bar items
        public IBarComponent ChangeEnabledStateBarItem { get; set; }
        public IBarComponent EditBarItem { get; set; }
        public IBarComponent OpenDocumentBarItem { get; set; }
        public IBarComponent ExpandAllBarItem { get; set; }
        public IBarComponent CollapseAllBarItem { get; set; }
        public IBarComponent ShowSearchPanelBarItem { get; set; }
        public IBarComponent ToggleEnabledDragAndDropBarItem { get; set; }
        #endregion

        #region Injected services
        public IBarManagerService BarManagerService { get; set; }
        public IReversibleService ReversibleService { get; set; }
        public IProjectManagerService ProjectManagerService { get; set; }
        public IRemoteConnectionManagerService RemoteConnectionManagerService { get; set; }
        public IDriverManagerService DriverManagerService { get; set; }
        public IInternalStorageService InternalStorageService { get; set; }
        public IContextWindowService ContextWindowService { get; set; }
        public IApplicationPropertiesService ApplicationPropertiesService { get; set; }
        public IClipboardService ClipboardService { get; set; }
        #endregion

        #region UI services
        protected ISaveFileDialogService SaveFileDialogService { get => this.GetService<ISaveFileDialogService>(); }
        protected ITreeListViewUtilities TreeListViewUtilities { get => this.GetService<ITreeListViewUtilities>(); }
        protected IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }
        protected IDispatcherService DispatcherService { get => this.GetService<IDispatcherService>(); }
        #endregion

        #region Constructors
        public ProjectTreeViewModel(
            IWorkspaceManagerService workspaceManagerService,
            IBarManagerService barManagerService,
            IReversibleService reversibleService,
            IProjectManagerService projectManagerService,
            IRemoteConnectionManagerService remoteConnectionManagerService,
            IDriverManagerService driverManagerService,
            IInternalStorageService internalStorageService,
            IContextWindowService contextWindowService,
            IApplicationPropertiesService applicationPropertiesService,
            IClipboardService clipboardService) : base(null, workspaceManagerService)
        {
            Caption = "Project Tree";
            WorkspaceContext = this;
            TargetName = "ProjectTree";
            BarItems = new ObservableCollection<IBarComponent>();
            TreeViewContextMenuSource = new ObservableCollection<IBarComponent>();

            BarManagerService = barManagerService;
            ReversibleService = reversibleService;
            ProjectManagerService = projectManagerService;
            RemoteConnectionManagerService = remoteConnectionManagerService;
            DriverManagerService = driverManagerService;
            InternalStorageService = internalStorageService;
            ContextWindowService = contextWindowService;
            ApplicationPropertiesService = applicationPropertiesService;
            ClipboardService = clipboardService;

            ServiceLocator.ReversibleService = reversibleService;
            ServiceLocator.ProjectManagerService = projectManagerService;
            ServiceLocator.RemoteConnectionManagerService = remoteConnectionManagerService;
            ServiceLocator.DriverManagerService = driverManagerService;
            ServiceLocator.ClipboardService = clipboardService;
            ServiceLocator.WorkspaceManagerService = workspaceManagerService;
            ServiceLocator.InternalStorageService = internalStorageService;
            ServiceLocator.ApplicationPropertiesService = applicationPropertiesService;

            UIDispatcher.Default.Start();

            WorkspaceManagerService.AddCreatePanelFunc((context) =>
            {
                if (context is IHaveTag haveTagObj)
                {
                    if (haveTagObj.HaveTags)
                    {
                        var documentViewModel = ServiceContainer.GetService<TagCollectionViewModel>();
                        documentViewModel.ParentViewModel = this;
                        documentViewModel.WorkspaceContext = context;
                        return documentViewModel;
                    }
                }
                return null;
            });
            WorkspaceManagerService.Workspaces.CollectionChanged += OnWorkspacesCollectionChanged;
        }
        #endregion

        #region Methods
        protected void InitializeBarItems()
        {
            #region File menu
            NewBarItem = BarFactory.Default.CreateButton(
                displayName: "New",
                keyGesture: new KeyGesture(Key.N, ModifierKeys.Control),
                imageSource: ImageHelper.GetImageSource("add_file_48px.png"),
                command: this.GetCommand(x => x.New()));
            OpenBarItem = BarFactory.Default.CreateButton(
                displayName: "Open",
                keyGesture: new KeyGesture(Key.O, ModifierKeys.Control),
                imageSource: ImageHelper.GetImageSource("opened_folder_48px.png"),
                command: this.GetCommand(x => x.Open()));
            SaveBarItem = BarFactory.Default.CreateButton(
                displayName: "Save",
                keyGesture: new KeyGesture(Key.S, ModifierKeys.Control),
                imageSource: ImageHelper.GetImageSource("save_48px.png"),
                command: this.GetAsyncCommand(x => x.Save()));
            SaveAsBarItem = BarFactory.Default.CreateButton(
                displayName: "Savs as...",
                keyGesture: null,
                imageSource: null,
                command: this.GetAsyncCommand(x => x.SaveAs()));
            ExitBarItem = BarFactory.Default.CreateButton(
                displayName: "Exit",
                keyGesture: new KeyGesture(Key.F4, ModifierKeys.Alt),
                imageSource: ImageHelper.GetImageSource("close_window_48px.png"),
                command: this.GetAsyncCommand(x => x.Exit()));

            FileMenu = BarFactory.Default.CreateSubItem(false, "File");
            FileMenu.Add(NewBarItem).Add(BarFactory.Default.CreateSeparator())
                .Add(OpenBarItem).Add(BarFactory.Default.CreateSeparator())
                .Add(SaveBarItem).Add(SaveAsBarItem).Add(BarFactory.Default.CreateSeparator())
                .Add(ExitBarItem);
            MainMenu.Add(FileMenu);
            Toolbar.Add(NewBarItem).Add(OpenBarItem).Add(BarFactory.Default.CreateSeparator())
                .Add(SaveBarItem).Add(BarFactory.Default.CreateSeparator());
            #endregion

            #region Edit menu
            UndoBarItem = BarFactory.Default.CreateButtonSplitItem(
                displayName: "Undo",
                keyGesture: new KeyGesture(Key.Z, ModifierKeys.Control),
                imageSource: ImageHelper.GetImageSource("undo_48px.png"),
                command: this.GetCommand(x => x.Undo()));

            RedoBarItem = BarFactory.Default.CreateButtonSplitItem(
                displayName: "Redo",
                keyGesture: new KeyGesture(Key.Y, ModifierKeys.Control),
                imageSource: ImageHelper.GetImageSource("redo_48px.png"),
                command: this.GetCommand(x => x.Redo()));

            ReversibleService.HistoryChanged += (s, e) =>
            {
                UndoBarItem.BarItems.Clear();
                int index = -1;
                foreach (var item in ReversibleService.Session.GetUndoTextList())
                {
                    int count = index++;
                    UndoBarItem.BarItems.Add(BarFactory.Default.CreateButton(
                        displayName: item,
                        command: new DelegateCommand(() =>
                        {
                            ReversibleService.Undo(count);
                        }))
                    );
                }

                RedoBarItem.BarItems.Clear();
                index = -1;
                foreach (var item in ReversibleService.Session.GetRedoTextList())
                {
                    int count = index++;
                    RedoBarItem.BarItems.Add(BarFactory.Default.CreateButton(
                        displayName: item,
                        command: new DelegateCommand(() =>
                        {
                            ReversibleService.Redo(count);
                        }))
                    );
                }
            };

            AddRemoteStationBarItem = BarFactory.Default.CreateButton(
                displayName: "Add Remote Station",
                keyGesture: new KeyGesture(Key.F1),
                imageSource: ImageHelper.GetImageSource("clouds_48px.png"),
                command: this.GetCommand(x => x.AddRemoteStation()));
            AddChannelBarItem = BarFactory.Default.CreateButton(
                displayName: "Add Channel",
                keyGesture: new KeyGesture(Key.F2),
                imageSource: ImageHelper.GetImageSource("mind_map_48px.png"),
                command: this.GetCommand(x => x.AddChannel()));
            AddDeviceBarItem = BarFactory.Default.CreateButton(
                displayName: "Add Device",
                keyGesture: new KeyGesture(Key.F3),
                imageSource: ImageHelper.GetImageSource("module_48px.png"),
                command: this.GetCommand(x => x.AddDevice()));
            AddGroupBarItem = BarFactory.Default.CreateButton(
                displayName: "Add Group",
                keyGesture: new KeyGesture(Key.F4),
                imageSource: ImageHelper.GetImageSource("folder_48px.png"),
                command: this.GetCommand(x => x.AddGroup()));
            AddTagBarItem = BarFactory.Default.CreateButton(
                displayName: "Add Tag",
                keyGesture: new KeyGesture(Key.F5),
                imageSource: ImageHelper.GetImageSource("tag_48px.png"),
                command: this.GetCommand(x => x.AddTag()));
            AddInternalTagBarItem = BarFactory.Default.CreateButton(
                displayName: "Add Internal Tag",
                keyGesture: new KeyGesture(Key.F6),
                imageSource: ImageHelper.GetImageSource("tag_window_48px.png"),
                command: this.GetCommand(x => x.AddInternalTag()));

            CopyBarItem = BarFactory.Default.CreateButton(
                displayName: "Copy",
                keyGesture: new KeyGesture(Key.C, ModifierKeys.Control),
                imageSource: ImageHelper.GetImageSource("copy_48px.png"),
                command: this.GetCommand(x => x.CopyMain()));
            CutBarItem = BarFactory.Default.CreateButton(
                displayName: "Cut",
                keyGesture: new KeyGesture(Key.X, ModifierKeys.Control),
                imageSource: ImageHelper.GetImageSource("cut_48px.png"),
                command: this.GetCommand(x => x.CutMain()));
            PasteBarItem = BarFactory.Default.CreateButton(
                displayName: "Paste",
                keyGesture: new KeyGesture(Key.V, ModifierKeys.Control),
                imageSource: ImageHelper.GetImageSource("paste_48px.png"),
                command: this.GetCommand(x => x.PasteMain()));
            DeleteBarItem = BarFactory.Default.CreateButton(
                displayName: "Delete",
                keyGesture: new KeyGesture(Key.Delete),
                imageSource: ImageHelper.GetImageSource("delete_48px.png"),
                command: this.GetCommand(x => x.DeleteMain()));

            ChangeEnabledStateBarItem = BarFactory.Default.CreateCheckItem(
                hideWhenDisable: true,
                displayName: "Enabled",
                imageSource: null,
                command: this.GetCommand(x => x.ChangeEnabledState()));

            EditMenu = BarFactory.Default.CreateSubItem(false, "Edit");
            Toolbar.Add(UndoBarItem).Add(RedoBarItem).Add(BarFactory.Default.CreateSeparator());
            Toolbar.Add(BarFactory.Default.CreateSeparator());
            Toolbar.Add(CopyBarItem).Add(CutBarItem).Add(PasteBarItem).Add(DeleteBarItem).Add(BarFactory.Default.CreateSeparator());
            Toolbar.Add(AddRemoteStationBarItem).Add(AddChannelBarItem).Add(AddDeviceBarItem).Add(AddGroupBarItem).Add(AddTagBarItem).Add(AddInternalTagBarItem);

            EditMenu.Add(BarFactory.Default.CreateSeparator());
            EditMenu.Add(UndoBarItem).Add(RedoBarItem);
            EditMenu.Add(BarFactory.Default.CreateSeparator());
            EditMenu.Add(AddRemoteStationBarItem).Add(AddChannelBarItem).Add(AddDeviceBarItem).Add(AddGroupBarItem).Add(AddTagBarItem).Add(AddInternalTagBarItem);
            EditMenu.Add(BarFactory.Default.CreateSeparator());
            EditMenu.Add(CopyBarItem).Add(CutBarItem).Add(PasteBarItem).Add(DeleteBarItem);
            EditMenu.Add(BarFactory.Default.CreateSeparator());

            MainMenu.Add(EditMenu);
            #endregion

            #region Tools menu

            #endregion

            #region View menu
            ViewMenu = BarFactory.Default.CreateSubItem(false, "View");
            MainMenu.Add(ViewMenu);
            #endregion

            #region Treeview menu
            OpenDocumentBarItem = BarFactory.Default.CreateButton(
               hideWhenDisable: true,
               displayName: "Open",
               keyGesture: null,
               imageSource: null,
               command: this.GetCommand(x => x.OpenDocument()));
            EditBarItem = BarFactory.Default.CreateButton(
                hideWhenDisable: true,
                displayName: "Edit",
                keyGesture: new KeyGesture(Key.E, ModifierKeys.Control),
                imageSource: null,
                command: this.GetCommand(x => x.Edit()));
            ExpandAllBarItem = BarFactory.Default.CreateButton(
                displayName: "Expand all",
                keyGesture: null,
                imageSource: ImageHelper.GetImageSource("expand_arrow_48px.png"),
                command: this.GetCommand(x => x.ExpandAll()));
            CollapseAllBarItem = BarFactory.Default.CreateButton(
                displayName: "Collapse all",
                keyGesture: null,
                imageSource: ImageHelper.GetImageSource("collapse_arrow_48px.png"),
                command: this.GetCommand(x => x.CollapseAll()));
            ShowSearchPanelBarItem = BarFactory.Default.CreateButton(
                displayName: "Show search panel",
                imageSource: ImageHelper.GetImageSource("search_48px.png"),
                command: this.GetCommand(x => x.ShowSearchPanel()));
            ToggleEnabledDragAndDropBarItem = BarFactory.Default.CreateCheckItem(
                displayName: "Enabled/Disabled drag and drop",
                imageSource: ImageHelper.GetImageSource("drag_reorder_48px.png"),
                command: this.GetCommand(x => x.ToggleEnabledDragAndDrop()));


            TreeViewContextMenuSource.Add(EditBarItem);
            TreeViewContextMenuSource.Add(OpenDocumentBarItem);
            TreeViewContextMenuSource.Add(BarFactory.Default.CreateSeparator());
            TreeViewContextMenuSource.Add(AddRemoteStationBarItem);
            TreeViewContextMenuSource.Add(AddChannelBarItem);
            TreeViewContextMenuSource.Add(AddDeviceBarItem);
            TreeViewContextMenuSource.Add(AddGroupBarItem);
            TreeViewContextMenuSource.Add(BarFactory.Default.CreateSeparator());
            TreeViewContextMenuSource.Add(CopyBarItem);
            TreeViewContextMenuSource.Add(CutBarItem);
            TreeViewContextMenuSource.Add(PasteBarItem);
            TreeViewContextMenuSource.Add(DeleteBarItem);
            TreeViewContextMenuSource.Add(BarFactory.Default.CreateSeparator());
            TreeViewContextMenuSource.Add(ChangeEnabledStateBarItem);
            TreeViewContextMenuSource.Add(BarFactory.Default.CreateSeparator());
            TreeViewContextMenuSource.Add(ShowSearchPanelBarItem);
            TreeViewContextMenuSource.Add(ToggleEnabledDragAndDropBarItem);
            TreeViewContextMenuSource.Add(BarFactory.Default.CreateSeparator());
            TreeViewContextMenuSource.Add(ExpandAllBarItem);
            TreeViewContextMenuSource.Add(CollapseAllBarItem);

            #region Internal toolbar
            InternalToolbar = BarFactory.Default.CreateToolBar();
            BarItems.Add(InternalToolbar);
            InternalToolbar.Add(ExpandAllBarItem).Add(CollapseAllBarItem);
            InternalToolbar.Add(BarFactory.Default.CreateSeparator());
            InternalToolbar.Add(ShowSearchPanelBarItem).Add(ToggleEnabledDragAndDropBarItem);
            #endregion
            #endregion
        }

        public override void EndInit()
        {
            Application.Current.Resources.MergedDictionaries.Add(
                new ResourceDictionary()
                {
                    Source = new Uri("pack://application:,,,/EasyDriver.Workspace.Main;component/Resources/Templates/Commons.xaml", UriKind.Absolute)
                }
            );

            InitializeBarItems();
            ProjectManagerService.ProjectChanged += OnProjectChanged;
        }

        private void OnWorkspacesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    var workspace = item as IWorkspacePanel;
                    if (!workspace.IsDocument)
                    {
                        foreach (var viewBarItem in ViewMenu.BarItems)
                        {
                            if (viewBarItem.Parameter == item)
                            {
                                return;
                            }
                        }

                        var barItem = BarFactory.Default.CreateCheckItem(
                            hideWhenDisable: false,
                            displayName: workspace.Caption,
                            imageSource: workspace.Glyph as ImageSource,
                            command: new DelegateCommand(() =>
                            {
                                workspace.IsClosed = !workspace.IsClosed;
                            }));

                        workspace.StateChanged += (s, args) =>
                        {
                            if (args.NewState == WorkspacePanelState.Closed)
                            {
                                barItem.IsChecked = false;
                            }
                            else
                            {
                                barItem.IsChecked = true;
                            }
                        };
                        barItem.Parameter = workspace;
                        barItem.IsChecked = true;
                        ViewMenu.Add(barItem);
                    }
                }
            }
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
                    if (item is IStationCore station)
                    {
                        // Nếu đối tượng con là 1 LocalStation
                        if (item is LocalStation)
                        {
                            // Xóa tất cả các DriverPlugin liên quan tới Channel có trong LocalStation
                            foreach (var x in station.GetAllChannels())
                            {
                                if (x is IChannelCore channel)
                                    DriverManagerService.RemoveDriver(channel);
                            }
                        }
                        // Nếu đối tượng con là 1 RemoteStation
                        else 
                        {
                            // Xóa connection liên quan tới RemoteStation
                            RemoteConnectionManagerService.RemoveRemoteConnection(station);
                        }
                    }
                }
            }

            // Nếu project mới khác null thì khởi tạo các DriverPlugin và RemoteConnection
            if (e.NewProject != null)
            {
                // Loop qua các đối tương con của project mới để xử lý
                foreach (var item in e.NewProject.Childs)
                {
                    if (item is IStationCore station)
                    {
                        // Nếu đối tương là một LocalStation thì khới tạo các DriverPlugin tương ứng với các Channel có trong LocalStation
                        if (item is LocalStation)
                        {
                            foreach (var child in station.GetAllChannels())
                            {
                                if (child is IChannelCore channel)
                                {
                                    string driverName = Path.GetFileNameWithoutExtension(channel.DriverPath);

                                    // Thêm driver tương ứng với channel vào DriverManagerService
                                    IEasyDriverPlugin driver = DriverManagerService.AddDriver(channel, driverName);
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

                                                if (tag.Retain)
                                                {
                                                    if (string.IsNullOrEmpty(tag.GUID))
                                                    {
                                                        tag.GUID = Guid.NewGuid().ToString();
                                                        if (tag.Retain)
                                                            InternalStorageService.AddOrUpdateStoreValue(tag.GUID, tag.Value);
                                                    }
                                                    // Nếu tag được set retain thì lấy giá trị từ trong store và set lại cho tag
                                                    var internalValue = InternalStorageService.GetStoreValue(tag.GUID);
                                                    if (internalValue != null && internalValue.GUID == tag.GUID)
                                                        tag.Value = internalValue.Value;
                                                }
                                                else
                                                {
                                                    tag.Value = tag.DefaultValue;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        /// Nếu đối tượng là một RemoteStation thì khởi tạo các HubConnection tương ứng với các RemoteStation
                        else
                        {
                            string connectionName = Path.GetFileNameWithoutExtension(station.StationType);

                            IEasyRemoteConnectionPlugin connectionPlugin = RemoteConnectionManagerService.AddRemoteConnection(station, connectionName);
                            if (connectionPlugin != null)
                            {
                                connectionPlugin.Start(station);
                                // Thêm connection liên quan tới RemoteStation
                                RemoteConnectionManagerService.AddRemoteConnection(station, connectionPlugin);
                            }
                        }
                    }
                }
            }
        }

        public virtual void OnMouseDown(object args)
        {
            SelectedItem = args;
            CurrentItem = args;
        }

        public virtual void OnDoubleClick(object args)
        {
            if (CanOpenDocument())
                OpenDocument();
        }

        public virtual void OnSelectedItemChanged()
        {
            if (SelectedItem is ICoreItem coreItem)
                ChangeEnabledStateBarItem.IsChecked = coreItem.Enabled;
            else
                ChangeEnabledStateBarItem.IsChecked = false;
        }

        public virtual void OnLoaded()
        {
            ViewModelLocator.ProjectTreeViewModel = this;
        }
        #endregion

        #region Commands
        #region File menu commands
        /// <summary>
        /// Lệnh tạo project
        /// </summary>
        public void New()
        {
            try
            {
                IsBusy = true;
                NewProjectViewModel vm = ViewModelSource.Create(() => new NewProjectViewModel());
                vm.ParentViewModel = this;
                NewProjectView view = new NewProjectView() { DataContext = vm };
                ContextWindowService.ShowDialog(view, vm.Title, vm.SizeToContent, vm.Width, vm.Height);
            }
            catch (Exception) { }
            finally { IsBusy = false; }
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="New"/>
        /// </summary>
        /// <returns></returns>
        public bool CanNew()
        {
            return !IsBusy;
        }

        /// <summary>
        /// Lệnh mở project
        /// </summary>
        public void Open()
        {
            try
            {
                IsBusy = true;
                OpenProjectViewModel vm = ViewModelSource.Create(() => new OpenProjectViewModel());
                vm.ParentViewModel = this;
                OpenProjectView view = new OpenProjectView() { DataContext = vm };
                ContextWindowService.ShowDialog(view, vm.Title, vm.SizeToContent, vm.Width, vm.Height);

            }
            catch (Exception) { }
            finally { IsBusy = false; }
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="Open"/>
        /// </summary>
        /// <returns></returns>
        public bool CanOpen()
        {
            return !IsBusy;
        }

        /// <summary>
        /// Lệnh lưu project
        /// </summary>
        /// <returns></returns>
        public async Task Save()
        {
            try
            {
                IsBusy = true;
                await ProjectManagerService.SaveAsync(CurrentProject);
                // Xóa lịch sử undo/redo khi lưu project
                ReversibleService.ClearHistory();
            }
            catch (Exception) { }
            finally { IsBusy = false; }
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="Save"/>
        /// </summary>
        /// <returns></returns>
        public bool CanSave()
        {
            return !IsBusy && CurrentProject != null;
        }

        /// <summary>
        /// Lệnh lưu project ở một nơi khác
        /// </summary>
        /// <returns></returns>
        public async Task SaveAs()
        {
            try
            {
                // Khóa luồng làm việc của chương trình
                IsBusy = true;
                // Kiểm tra xem project đang xử lý có sự thay đổi hay không
                if (CurrentProject.HasChanges())
                {
                    // Nếu có thì hỏi người dùng có muốn lưu lại không
                    var mbr = MessageBoxService.ShowMessage(
                        "The current working project has changes. Do you want to save now ?", "Message", MessageButton.YesNoCancel, MessageIcon.Question);
                    // Nếu người dùng chọn 'Cancel' thì thoát khỏi hàm
                    if (mbr == MessageResult.Cancel)
                        return;
                    // Nếu người dùng chọn 'Yes' thì lưu lại
                    if (mbr == MessageResult.Yes)
                    {
                        // Lưu lại project
                        await ProjectManagerService.SaveAsync(CurrentProject);
                        // Xóa lịch sử hoạt động của dịch vụ Reverse
                        ReversibleService.ClearHistory();
                    }
                    // Mở luồng làm việc của chương trình
                    IsBusy = false;
                }

                // Khởi tạo thông tin của SaveFileDialog
                SaveFileDialogService.Title = "Save as...";
                SaveFileDialogService.Filter = "Project file (*.json)|*.json";
                // Mở SaveDialog để lấy thông tin đường dẫn và tên của project mới
                if (SaveFileDialogService.ShowDialog())
                {
                    // Khóa luồng làm việc của chương trình
                    IsBusy = true;
                    // Lấy đường dẫn của project mới
                    string projectPath = SaveFileDialogService.File.GetFullName();
                    // Thay đổi tên của chương trình hiện tại bằng tên của file đã nhập trên SaveDialog
                    CurrentProject.Name = Path.GetFileNameWithoutExtension(projectPath);
                    // Thay đổi đường dẫn của chương trình hiện tại bằng đường dẫn ta đã chọn trên SaveDialog
                    CurrentProject.ProjectPath = projectPath;
                    // Xóa lịch sử hoạt động của dịch vụ Reverse
                    ReversibleService.ClearHistory();
                    // Lưu lại project
                    await ProjectManagerService.SaveAsync(CurrentProject);
                }
            }
            catch (Exception) { }
            finally { IsBusy = false; }
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="SaveAs"/>
        /// </summary>
        /// <returns></returns>
        public bool CanSaveAs()
        {
            // Điều kiện là luồng làm việc không bận và chương trình đang mở một project
            return !IsBusy && ProjectManagerService.CurrentProject != null;
        }

        /// <summary>
        /// Lệnh thoát chương trình
        /// </summary>
        /// <returns></returns>
        public async Task Exit()
        {
            bool needClose = true;
            // Kiểm tra xem có project nào đang được mở hay không
            if (CurrentProject != null)
            {
                // Nếu có thì kiểm tra project đó có sự thay đổi nào khôngs
                if (CurrentProject.HasChanges())
                {
                    // Nếu có thì hỏi người dùng có muốn lưu lại những thay đổi đó không
                    var mbr = MessageBoxService.ShowMessage(
                        "The current working project has changes. Do you want to save now ?", "Message", MessageButton.YesNoCancel, MessageIcon.Question);

                    // Nếu người dùng chọn 'Yes' thì sẽ lưu lại
                    if (mbr == MessageResult.Yes)
                    {
                        // Khóa luồng làm việc của chương trình 
                        IsBusy = true;
                        // Lưu lại project
                        await ProjectManagerService.SaveAsync(CurrentProject);
                        // Mở luồng làm việc
                        IsBusy = false;
                        // Xác nhận là thoát chương trình
                        ApplicationPropertiesService.IsMainWindowExit = true;
                        // Tắt chương trình 
                        _ = DispatcherService.BeginInvoke(() => Application.Current.MainWindow.Close());
                        return;
                    }
                    // Nếu người dùng chọn 'No' thì đóng chương trình
                    else if (mbr == MessageResult.No)
                    {
                        IsBusy = true;
                        // Xác nhận là thoát chương trình
                        //ApplicationViewModel.IsMainWindowExit = true;
                        // Tắt chương trình 
                        _ = DispatcherService.BeginInvoke(() => Application.Current.MainWindow.Close());
                        IsBusy = false;
                        return;
                    }
                    else
                    {
                        needClose = false;
                    }
                }
            }

            if (needClose)
            {
                var mbr = MessageBoxService.ShowMessage("Do you want to exit application?", "Message", MessageButton.YesNo, MessageIcon.Question);
                if (mbr == MessageResult.Yes)
                {
                    // Xác nhận là thoát chương trình
                    ApplicationPropertiesService.IsMainWindowExit = true;
                    // Tắt chương trình 
                    DispatcherService.Invoke(() => Application.Current.MainWindow.Close());
                }
            }
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="Exit"/>
        /// </summary>
        /// <returns></returns>
        public bool CanExit()
        {
            return !IsBusy;
        }
        #endregion

        #region Edit menu commands
        public void Undo()
        {
            ReversibleService.Undo();
        }

        public bool CanUndo()
        {
            return !IsBusy && ReversibleService.CanUndo();
        }

        public void Redo()
        {
            ReversibleService.Redo();
        }

        public bool CanRedo()
        {
            return !IsBusy && ReversibleService.CanRedo();
        }

        public void AddRemoteStation()
        {
            try
            {
                IsBusy = true;
                AddStationViewModel vm = ViewModelSource.Create(() => new AddStationViewModel(TreeListViewUtilities, ContextWindowService));
                vm.ParentViewModel = this;
                vm.Parameter = CurrentProject;
                AddStationView view = new AddStationView() { DataContext = vm };
                ContextWindowService.ShowDialog(view, vm.Title, vm.SizeToContent, vm.Width, vm.Height);
            }
            catch (Exception) { }
            finally { IsBusy = false; }
        }

        public bool CanAddRemoteStation()
        {
            return !IsBusy && CurrentProject != null;
        }

        /// <summary>
        /// Lệnh tạo một channel
        /// </summary>
        public void AddChannel()
        {
            try
            {
                IsBusy = true;
                AddChannelViewModel vm = ViewModelSource.Create(() => new AddChannelViewModel(
                    ProjectManagerService, DriverManagerService, TreeListViewUtilities, ContextWindowService));
                vm.ParentViewModel = this;
                vm.Parameter = SelectedItem;
                AddChannelView view = new AddChannelView() { DataContext = vm };
                ContextWindowService.ShowDialog(view, vm.Title, vm.SizeToContent, vm.Width, vm.Height);
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
                return !IsBusy && localStation.Parent is IProjectItem && SelectedItems != null && SelectedItems.Count == 1;
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
                    if (ContextWindowService.ShowDialog(addDeviceView, "Add Device") is IDeviceCore deviceCore)
                    {
                        // Bắt đầu undo redo transaction
                        using (Transaction transaction = ReversibleService.Begin("Add Device"))
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
                                    WorkspaceManagerService.RemovePanel(x => x.WorkspaceContext == deviceCore);
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
            catch (Exception) { }
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
                return !IsBusy && !channelCore.IsReadOnly && SelectedItems != null && SelectedItems.Count == 1;
            if (SelectedItem is GroupCore groupItem)
            {
                if (groupItem.FindParent<IDeviceCore>(x => x is IDeviceCore) is IDeviceCore)
                    return false;
                if (groupItem.FindParent<IChannelCore>(x => x is IChannelCore) is IChannelCore)
                    return !IsBusy && !groupItem.IsReadOnly && SelectedItems != null && SelectedItems.Count == 1;
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
                using (Transaction transaction = ReversibleService.Begin("Add group"))
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
                            WorkspaceManagerService.RemovePanel(x => x.WorkspaceContext == groupItem);
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
                return !IsBusy && !groupItem.IsReadOnly && SelectedItems != null && SelectedItems.Count == 1;
            return false;
        }

        /// <summary>
        /// Lệnh thêm <see cref="ITag"/> vào <see cref="IDevice"/>
        /// </summary>
        public void AddTag()
        {
            // Đảm bảo ActivePanel là một TagCollectionDocument
            if (WorkspaceManagerService.CurrentActivePanel is TagCollectionViewModel tagCollection)
                tagCollection.AddTag(); // Gọi hàm thêm tag của Panel đó
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="AddTag"/>
        /// </summary>
        /// <returns></returns>
        public bool CanAddTag()
        {
            // Điều kiện để có thể thêm Tag là có một ActivePanel là TagCollectionDocument
            // và Panel đó phải cho phép thêm Tag
            if (WorkspaceManagerService.CurrentActivePanel is TagCollectionViewModel tagCollection)
                return tagCollection.CanAddTag();
            return false;
        }

        /// <summary>
        /// Lệnh thêm <see cref="ITag"/> vào <see cref="IDevice"/>
        /// </summary>
        public void AddInternalTag()
        {
            // Đảm bảo ActivePanel là một TagCollectionDocument
            if (WorkspaceManagerService.CurrentActivePanel is TagCollectionViewModel tagCollection)
                tagCollection.AddInternalTag(); // Gọi hàm thêm tag của Panel đó
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="AddTag"/>
        /// </summary>
        /// <returns></returns>
        public bool CanAddInternalTag()
        {
            // Điều kiện để có thể thêm Tag là có một ActivePanel là TagCollectionDocument
            // và Panel đó phải cho phép thêm Tag
            if (WorkspaceManagerService.CurrentActivePanel is TagCollectionViewModel tagCollection)
                return tagCollection.CanAddInternalTag();
            return false;
        }

        #region ISupportCopyPaste

        /// <summary>
        /// Lệnh copy đối tượng
        /// </summary>
        public void Copy()
        {
            ClipboardService.CopyToClipboard(SelectedItem, this);
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
                return !coreItem.IsReadOnly && !IsBusy && SelectedItems != null && SelectedItems.Count == 1;
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
                if (SelectedItem is LocalStation localStation && localStation.Parent is IProjectItem && ClipboardService.ObjectToCopy is IChannelCore channelCore)
                {
                    IsBusy = true;
                    IChannelCore newChannelCore = new ChannelCore(localStation)
                    {
                        Name = localStation.GetUniqueNameInGroup(channelCore.Name),
                        DriverPath = channelCore.DriverPath
                    };

                    IEasyDriverPlugin driver = DriverManagerService.CreateDriver(newChannelCore.DriverPath);
                    if (driver != null)
                    {
                        if (ContextWindowService.ShowDialog(driver.GetCreateChannelControl(localStation, channelCore), "Add Channel") == newChannelCore)
                        {
                            using (Transaction transaction = ReversibleService.Begin("Paste Channel"))
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
                                            WorkspaceManagerService.RemovePanel(x => x.WorkspaceContext == item);
                                        if (DriverManagerService.DriverPoll.ContainsKey(newChannelCore))
                                        {
                                            DriverManagerService.DriverPoll[newChannelCore].Dispose();
                                            DriverManagerService.RemoveDriver(newChannelCore);
                                        }
                                    }
                                    else
                                    {
                                        IEasyDriverPlugin driverPlugin = DriverManagerService.CreateDriver(newChannelCore.DriverPath);
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
                else if (SelectedItem is IChannelCore selectedChannel && !selectedChannel.IsReadOnly && ClipboardService.ObjectToCopy is IDeviceCore deviceCore)
                {
                    IsBusy = true;
                    IEasyDriverPlugin driver = DriverManagerService.GetDriver(SelectedItem as IChannelCore);
                    if (driver != null)
                    {
                        if (ContextWindowService.ShowDialog(driver.GetCreateDeviceControl(SelectedItem as IChannelCore, deviceCore), "Add Device") is IDeviceCore newDeviceCore)
                        {
                            using (Transaction transaction = ReversibleService.Begin("Add Device"))
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
            if (IsBusy || !ClipboardService.ContainData() || SelectedItem == null || ClipboardService.Context != this)
                return false;
            if (SelectedItem is LocalStation localStation && localStation.Parent is IProjectItem && ClipboardService.ObjectToCopy is IChannelCore)
                return !IsBusy && SelectedItems != null && SelectedItems.Count == 1;
            else if (SelectedItem is IChannelCore selectedChannel && !selectedChannel.IsReadOnly && ClipboardService.ObjectToCopy is IDeviceCore deviceCore)
                return !IsBusy && (deviceCore.Parent as IChannelCore).DriverPath == selectedChannel.DriverPath && SelectedItems != null && SelectedItems.Count == 1;
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
                    if (!(SelectedItem is LocalStation))
                    {
                        IStationCore station = SelectedItem as IStationCore;
                        if (RemoteConnectionManagerService.RemoteConnectionPool.ContainsKey(station))
                        {
                            RemoteConnectionManagerService.RemoteConnectionPool[station].Dispose();
                            RemoteConnectionManagerService.RemoveRemoteConnection(station);
                        }
                    }

                    // Nếu đối tượng là một Channel thì ta cần phải xỏa
                    // DriverPlugin liên quan đến nó ở IDriverManagerService
                    if (SelectedItem is IChannelCore channel)
                    {
                        if (DriverManagerService.DriverPoll.ContainsKey(channel))
                        {
                            DriverManagerService.DriverPoll[channel].Dispose();
                            DriverManagerService.RemoveDriver(channel);
                        }
                    }

                    // Xóa tất cả các TagDocument liên quan
                    foreach (var item in (SelectedItem as IGroupItem).Find(x => x is IGroupItem, true))
                        WorkspaceManagerService.RemovePanel(x => x.WorkspaceContext == item);
                    WorkspaceManagerService.RemovePanel(x => x.WorkspaceContext == SelectedItem);

                    object itemToRemote = SelectedItem;

                    using (Transaction transaction = ReversibleService.Begin($"Delete {SelectedItem.GetClassName(true)}"))
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
                                else if ((itemToRemote is LocalStation) && itemToRemote is IStationCore station)
                                {
                                    RemoteConnectionManagerService.RemoveRemoteConnection(station);
                                }
                            }
                            else if (e.Direction == ReverseDirection.Undo)
                            {
                                if (itemToRemote is IChannelCore channel)
                                {
                                    IEasyDriverPlugin driver = DriverManagerService.CreateDriver(channel.DriverPath);
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
                                else if ((itemToRemote is LocalStation) && itemToRemote is IStationCore station)
                                {
                                    IEasyRemoteConnectionPlugin remoteConnection = RemoteConnectionManagerService.CreateRemoteConnection(station.StationType);
                                    if (remoteConnection != null)
                                    {
                                        station.Name = station.Parent.GetUniqueNameInGroup(station.Name);
                                        RemoteConnectionManagerService.AddRemoteConnection(station as IStationCore, remoteConnection);
                                    }
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
            if (IsBusy)
                return false;
            if (SelectedItem is ICoreItem coreItem)
            {
                if (coreItem.IsReadOnly)
                    return false;
            }
            return SelectedItems != null && SelectedItems.Count == 1;
        }

        #endregion
        #endregion

        #region Tools menu commands
        #endregion

        #region Context menu commands
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
                    ContextWindowService.ShowDialog(driver.GetEditChannelControl(channel), $"Edit Channel - {channel.Name}");
                }
                else if (SelectedItem is IDeviceCore device)
                {
                    IEasyDriverPlugin driver = DriverManagerService.GetDriver(device);
                    ContextWindowService.ShowDialog(driver.GetEditDeviceControl(device), $"Edit Device - {device.Name}");
                }
                // Nếu là một group thì mở GroupView
                else if (SelectedItem is GroupCore group)
                {
                    if (!group.IsReadOnly)
                    {
                        GroupViewModel vm = ViewModelSource.Create(() => new GroupViewModel());
                        vm.ParentViewModel = this;
                        vm.Parameter = group;
                        GroupView view = new GroupView() { DataContext = vm };
                        ContextWindowService.ShowDialog(view, vm.Title, vm.SizeToContent, vm.Width, vm.Height);
                    }
                }
                // Nếu là station thì mở các edit view tương ứng với StationType
                else if (SelectedItem is IStationCore remoteConnection && !(SelectedItem is LocalStation))
                {
                    IEasyRemoteConnectionPlugin connectionPlugin = RemoteConnectionManagerService.GetRemoteConnection(remoteConnection);
                    if (connectionPlugin != null)
                    {
                        object editView = connectionPlugin.GetEditRemoteConnectionView(remoteConnection);
                        if (editView != null)
                            ContextWindowService.ShowDialog(editView, $"Edit Remote Station - {remoteConnection.Name}");
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
                return !coreItem.IsReadOnly && SelectedItems != null && SelectedItems.Count == 1;
            }
            return false;
        }

        /// <summary>
        /// Lệnh mở document
        /// </summary>
        public void OpenDocument()
        {
            IsBusy = true;
            WorkspaceManagerService.OpenPanel(SelectedItem, true, true, ParentViewModel);
            IsBusy = false;
        }

        /// <summary>
        /// Điều kiện để thực hiện lệnh <see cref="OpenDocument"/>
        /// </summary>
        /// <returns></returns>
        public bool CanOpenDocument()
        {
            return !IsBusy && SelectedItem is IHaveTag && SelectedItems != null && SelectedItems.Count == 1;
        }

        /// <summary>
        /// Lệnh thay đổi trạng thái Enabled của đối tượng
        /// </summary>
        public void ChangeEnabledState()
        {
            if (SelectedItem is ICoreItem item)
            {
                item.Enabled = !item.Enabled;
            }
        }

        /// <summary>
        /// Điều kiện để thực hiện lệnh <see cref="ChangeEnabledState"/>
        /// </summary>
        /// <returns></returns>
        public bool CanChangeEnabledState()
        {
            return !IsBusy && SelectedItem is ICoreItem && SelectedItems != null && SelectedItems.Count == 1;
        }

        /// <summary>
        /// Mở tất cả node của tree
        /// </summary>
        public void ExpandAll()
        {
            TreeListViewUtilities?.ExpandAll();
        }

        /// <summary>
        /// Đóng tất cả node của tree
        /// </summary>
        public void CollapseAll()
        {
            TreeListViewUtilities?.CollapseAll();
        }

        public void ShowSearchPanel()
        {
            TreeListViewUtilities.ShowSearchPanel();
        }

        public void ToggleEnabledDragAndDrop()
        {
            AllowDragDrop = ToggleEnabledDragAndDropBarItem.IsChecked = !AllowDragDrop;
        }
        #endregion

        #region ISupportCopyPaste Main
        /// <summary>
        /// Lệnh copy một hoặc nhiệu đối tượng
        /// </summary>
        public void CopyMain()
        {
            // Kiểm tra xem có WorkspacePanel đang được chọn hay không
            if (WorkspaceManagerService.CurrentActivePanel != null)
            {
                // Nếu có thì ép sang kiểu ISupportCopyPaste và thực thi lệnh tương ứng
                if (WorkspaceManagerService.CurrentActivePanel is ISupportCopyPaste edit)
                    edit.Copy();
            }
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="Copy"/>
        /// </summary>
        /// <returns></returns>
        public bool CanCopyMain()
        {
            // Điều kiện để thực thi là có một WorkspacePanel được active
            // và WorkspacePanel đó phải implement ISupportCopyPaste
            // nếu như ISupportCopyPaste đó cho phép thực thi lệnh thì đối tượng này cũng vậy
            if (WorkspaceManagerService.CurrentActivePanel != null)
                if (WorkspaceManagerService.CurrentActivePanel is ISupportCopyPaste edit)
                    return edit.CanCopy();
            return false;
        }

        /// <summary>
        /// Lệnh cut một hoặc nhiều đối tượng
        /// </summary>
        public void CutMain()
        {
            // Kiểm tra xem có WorkspacePanel đang được chọn hay không
            if (WorkspaceManagerService.CurrentActivePanel != null)
            {
                // Nếu có thì ép sang kiểu ISupportCopyPaste và thực thi lệnh tương ứng
                if (WorkspaceManagerService.CurrentActivePanel is ISupportCopyPaste edit)
                    edit.Cut();
            }
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="Cut"/>
        /// </summary>
        /// <returns></returns>
        public bool CanCutMain()
        {
            // Điều kiện để thực thi là có một WorkspacePanel được active
            // và WorkspacePanel đó phải implement ISupportCopyPaste
            // nếu như ISupportCopyPaste đó cho phép thực thi lệnh thì đối tượng này cũng vậy
            if (WorkspaceManagerService.CurrentActivePanel != null)
                if (WorkspaceManagerService.CurrentActivePanel is ISupportCopyPaste edit)
                    return edit.CanCut();
            return false;
        }

        /// <summary>
        /// Lệnh paste một hoặc nhiều đối tượng
        /// </summary>
        public void PasteMain()
        {
            // Kiểm tra xem có WorkspacePanel đang được chọn hay không
            if (WorkspaceManagerService.CurrentActivePanel != null)
            {
                // Nếu có thì ép sang kiểu ISupportCopyPaste và thực thi lệnh tương ứng
                if (WorkspaceManagerService.CurrentActivePanel is ISupportCopyPaste edit)
                    edit.Paste();
            }
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="Paste"/>
        /// </summary>
        /// <returns></returns>
        public bool CanPasteMain()
        {
            // Điều kiện để thực thi là có một WorkspacePanel được active
            // và WorkspacePanel đó phải implement ISupportCopyPaste
            // nếu như ISupportCopyPaste đó cho phép thực thi lệnh thì đối tượng này cũng vậy
            if (WorkspaceManagerService.CurrentActivePanel != null)
                if (WorkspaceManagerService.CurrentActivePanel is ISupportCopyPaste edit)
                    return edit.CanPaste();
            return false;
        }

        /// <summary>
        /// Lệnh xóa một hoặc nhiều đối tượng
        /// </summary>
        public void DeleteMain()
        {
            // Kiểm tra xem có WorkspacePanel đang được chọn hay không
            if (WorkspaceManagerService.CurrentActivePanel != null)
            {
                // Nếu có thì ép sang kiểu ISupportCopyPaste và thực thi lệnh tương ứng
                if (WorkspaceManagerService.CurrentActivePanel is ISupportCopyPaste edit)
                    edit.Delete();
            }
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="Delete"/>
        /// </summary>
        /// <returns></returns>
        public bool CanDeleteMain()
        {
            // Điều kiện để thực thi là có một WorkspacePanel được active
            // và WorkspacePanel đó phải implement ISupportCopyPaste
            // nếu như ISupportCopyPaste đó cho phép thực thi lệnh thì đối tượng này cũng vậy
            if (WorkspaceManagerService.CurrentActivePanel != null)
                if (WorkspaceManagerService.CurrentActivePanel is ISupportCopyPaste edit)
                    return edit.CanDelete();
            return false;
        }
        #endregion
        #endregion
    }
}
