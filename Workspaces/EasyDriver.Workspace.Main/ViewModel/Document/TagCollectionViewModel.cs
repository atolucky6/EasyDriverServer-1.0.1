using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
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
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DocumentPanel = EasyDriver.WorkspacePlugin.DocumentPanel;

namespace EasyDriver.Workspace.Main
{
    [Service(int.MaxValue, false)]
    public class TagCollectionViewModel : DocumentPanel, IMVVMDockingProperties, ISupportCopyPaste, ISupportParentViewModel
    {
        #region Public properties
        public virtual ObservableCollection<IBarComponent> BarItems { get; protected set; }
        public virtual ObservableCollection<IBarComponent> TreeViewContextMenuSource { get; protected set; }
        public virtual IGroupItem Parent => WorkspaceContext as IGroupItem;
        public virtual IHaveTag ObjectHaveTag => WorkspaceContext as IHaveTag;
        public virtual IProjectItem CurrentProject => ProjectManagerService.CurrentProject;
        public virtual object CurrentItem { get; set; }
        public virtual object SelectedItem { get; set; }
        public virtual ObservableCollection<object> SelectedItems { get; set; } = new ObservableCollection<object>();
        public virtual bool AllowDragDrop { get; set; }
        public ProjectTreeViewModel ProjectTreeViewModel => ViewModelLocator.ProjectTreeViewModel;

        private object workspaceContext;
        public override object WorkspaceContext
        {
            get => workspaceContext;
            set
            {
                if (workspaceContext != value)
                {
                    workspaceContext = value;
                    int tagCount = WorkspaceContext == null ? 0 : (WorkspaceContext as IHaveTag).Tags.Count;
                    StaticCountTagsBarItem.DisplayName = $"Total: {tagCount}";
                    (WorkspaceContext as IHaveTag).Tags.CollectionChanged += (s, e) =>
                    {
                        StaticCountTagsBarItem.DisplayName = $"Total: {ObjectHaveTag.Tags.Count}"; 
                    };
                }
            }
        }
        #endregion

        #region BarItems
        public IBarComponent Toolbar { get; set; }

        public IBarComponent EditTagBarItem { get; set; }
        public IBarComponent WriteTagBarItem { get; set; }
        public IBarComponent ExpandAllBarItem { get; set; }
        public IBarComponent CollapseAllBarItem { get; set; }
        public IBarComponent ChangeEnabledStateBarItem { get; set; }
        public IBarComponent ExportBarItem { get; set; }
        public IBarComponent ImportBarItem { get; set; }
        public IBarComponent AddTagBarItem { get; set; }
        public IBarComponent AddInternalTagBarItem { get; set; }
        public IBarComponent InsertAboveTagBarItem { get; set; }
        public IBarComponent InsertBelowTagBarItem { get; set; }
        public IBarComponent InsertAboveInternalTagBarItem { get; set; }
        public IBarComponent InsertBelowInternalTagBarItem { get; set; }
        public IBarComponent ShowSearchPanelBarItem { get; set; }
        public IBarComponent ToggleEnabledDragAndDropBarItem { get; set; }
        public IBarComponent StaticCountTagsBarItem { get; set; }
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
        protected IOpenFileDialogService OpenFileDialogService { get => this.GetService<IOpenFileDialogService>(); }
        protected ISaveFileDialogService SaveFileDialogService { get => this.GetService<ISaveFileDialogService>(); }
        protected ITreeListViewUtilities TreeListViewUtilities { get => this.GetService<ITreeListViewUtilities>(); }
        protected IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }
        #endregion

        #region Constructors
        public TagCollectionViewModel(
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
            TargetName = "DocumentHost";
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

            InitializeBarItems();
        }
        #endregion

        #region Event handlers
        public virtual void OnLoaded()
        {
        }

        public virtual void OnMouseDown(object args)
        {
            if (args == null)
            {
                SelectedItem = args;
                CurrentItem = args;
            }
        }

        public virtual void OnDoubleClick(object args)
        {

        }
        #endregion

        #region Methods
        private void InitializeBarItems()
        {
            Toolbar = BarFactory.Default.CreateToolBar();
            BarItems.Add(Toolbar);

            EditTagBarItem = BarFactory.Default.CreateButton(
                displayName: "Edit",
                keyGesture: null,
                imageSource: null,
                command: this.GetCommand(x => x.EditTag()));
            WriteTagBarItem = BarFactory.Default.CreateButton(
                displayName: "Write tag",
                keyGesture: new KeyGesture(Key.K, ModifierKeys.Control),
                imageSource: null,
                command: this.GetCommand(x => x.WriteTag()));
            ImportBarItem = BarFactory.Default.CreateButton(
                displayName: "Import",
                keyGesture: null,
                imageSource: ImageHelper.GetImageSource("excel_import_32px.png"),
                command: this.GetCommand(x => x.Import()));
            ExportBarItem = BarFactory.Default.CreateButton(
                displayName: "Export",
                keyGesture: null,
                imageSource: ImageHelper.GetImageSource("execel_export_32px.png"),
                command: this.GetCommand(x => x.Export()));
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

            AddTagBarItem = BarFactory.Default.CreateButtonSplitItem(
                displayName: "Add tag",
                keyGesture: null,
                imageSource: ImageHelper.GetImageSource("tag_48px.png"),
                command: this.GetCommand(x => x.AddTag()));
            InsertAboveTagBarItem = BarFactory.Default.CreateButton(
                displayName: "Insert above",
                keyGesture: null,
                imageSource: null,
                command: this.GetCommand(x => x.InsertAboveTag()));
            InsertBelowTagBarItem = BarFactory.Default.CreateButton(
                displayName: "Insert below",
                keyGesture: null,
                imageSource: null,
                command: this.GetCommand(x => x.InsertBelowTag()));
            AddTagBarItem.BarItems.Add(InsertAboveTagBarItem);
            AddTagBarItem.BarItems.Add(InsertBelowTagBarItem);

            AddInternalTagBarItem = BarFactory.Default.CreateButtonSplitItem(
                displayName: "Add internal tag",
                keyGesture: null,
                imageSource: ImageHelper.GetImageSource("tag_window_48px.png"),
                command: this.GetCommand(x => x.AddInternalTag()));
            InsertAboveInternalTagBarItem = BarFactory.Default.CreateButton(
                displayName: "Insert above",
                keyGesture: null,
                imageSource: null,
                command: this.GetCommand(x => x.InsertAboveInternalTag()));
            InsertBelowInternalTagBarItem = BarFactory.Default.CreateButton(
                displayName: "Insert below",
                keyGesture: null,
                imageSource: null,
                command: this.GetCommand(x => x.InsertBelowInternalTag()));
            AddInternalTagBarItem.BarItems.Add(InsertAboveInternalTagBarItem);
            AddInternalTagBarItem.BarItems.Add(InsertBelowInternalTagBarItem);

            ShowSearchPanelBarItem = BarFactory.Default.CreateButton(
                displayName: "Show search panel",
                imageSource: ImageHelper.GetImageSource("search_48px.png"),
                command: this.GetCommand(x => x.ShowSearchPanel()));

            ToggleEnabledDragAndDropBarItem = BarFactory.Default.CreateCheckItem(
                displayName: "Enabled/Disabled drag and drop",
                imageSource: ImageHelper.GetImageSource("drag_reorder_48px.png"),
                command: this.GetCommand(x => x.ToggleEnabledDragAndDrop()));

            StaticCountTagsBarItem = BarFactory.Default.CreateStaticItem(
                displayName: $"Total: 0");
            StaticCountTagsBarItem.Alignment = BarItemAlignment.Far;

            Toolbar.Add(ImportBarItem).Add(ExportBarItem);
            Toolbar.Add(BarFactory.Default.CreateSeparator());
            Toolbar.Add(ExpandAllBarItem).Add(CollapseAllBarItem);
            Toolbar.Add(BarFactory.Default.CreateSeparator());
            Toolbar.Add(AddTagBarItem).Add(AddInternalTagBarItem);
            Toolbar.Add(BarFactory.Default.CreateSeparator());
            Toolbar.Add(ShowSearchPanelBarItem).Add(ToggleEnabledDragAndDropBarItem);
            Toolbar.Add(BarFactory.Default.CreateSeparator());
            Toolbar.Add(StaticCountTagsBarItem);
            this.RaisePropertyChanged(x => BarItems);

            TreeViewContextMenuSource.Add(WriteTagBarItem);
            TreeViewContextMenuSource.Add(EditTagBarItem);
            TreeViewContextMenuSource.Add(BarFactory.Default.CreateSeparator());
            TreeViewContextMenuSource.Add(AddTagBarItem);
            TreeViewContextMenuSource.Add(AddInternalTagBarItem);
            TreeViewContextMenuSource.Add(BarFactory.Default.CreateSeparator());
            TreeViewContextMenuSource.Add(ProjectTreeViewModel.CopyBarItem);
            TreeViewContextMenuSource.Add(ProjectTreeViewModel.CutBarItem);
            TreeViewContextMenuSource.Add(ProjectTreeViewModel.PasteBarItem);
            TreeViewContextMenuSource.Add(ProjectTreeViewModel.DeleteBarItem);
            TreeViewContextMenuSource.Add(BarFactory.Default.CreateSeparator());
            TreeViewContextMenuSource.Add(ImportBarItem);
            TreeViewContextMenuSource.Add(ExportBarItem);
            TreeViewContextMenuSource.Add(BarFactory.Default.CreateSeparator());
            TreeViewContextMenuSource.Add(ShowSearchPanelBarItem);
            TreeViewContextMenuSource.Add(ToggleEnabledDragAndDropBarItem);
            TreeViewContextMenuSource.Add(BarFactory.Default.CreateSeparator());
            TreeViewContextMenuSource.Add(ExpandAllBarItem);
            TreeViewContextMenuSource.Add(CollapseAllBarItem);

        }
        #endregion

        #region Commands
        public void WriteTag()
        {
            //WindowService.Show("WriteTagView", SelectedItem as ITagCore, this);
        }

        public bool CanWriteTag()
        {
            return SelectedItem is ITagCore && SelectedItems != null && SelectedItems.Count == 1;
        }

        public void ExpandAll()
        {
            TreeListViewUtilities.ExpandAll();
        }

        public void CollapseAll()
        {
            TreeListViewUtilities.CollapseAll();
        }

        public void AddTag()
        {
            IsBusy = true;
            IEasyDriverPlugin driver = DriverManagerService.GetDriver(Parent);
            if (driver != null)
            {
                if (ContextWindowService.ShowDialog(driver.GetCreateTagControl(Parent), "Add Tag") is IEnumerable<ITagCore> newTags)
                {
                    if (newTags.Count() > 0)
                    {
                        using (Transaction transaction = ReversibleService.Begin($"Add {newTags.Count()} Tag"))
                        {
                            ObjectHaveTag.Tags.AsReversibleCollection().AddRange(newTags.ToArray());
                            this.SetPropertyReversible(x => x.SelectedItems, new ObservableCollection<object>(newTags));

                            transaction.Reversing += (s, e) =>
                            {
                                WorkspaceManagerService.OpenPanel(this);
                            };
                            transaction.Commit();
                        }
                    }
                }
            }
            IsBusy = false;
        }

        public bool CanAddTag()
        {
            return !IsBusy && !Parent.IsReadOnly && ObjectHaveTag.HaveTags;
        }

        public void InsertAboveTag()
        {
            IsBusy = true;
            IEasyDriverPlugin driver = DriverManagerService.GetDriver(Parent);
            if (driver != null)
            {
                if (ContextWindowService.ShowDialog(driver.GetCreateTagControl(Parent), "Add Tag") is List<ITagCore> newTags)
                {
                    if (newTags.Count > 0)
                    {
                        int selectedIndex = ObjectHaveTag.Tags.IndexOf(SelectedItem);
                        if (selectedIndex > -1)
                        {
                            using (Transaction transaction = ReversibleService.Begin("Insert Tag"))
                            {
                                ObjectHaveTag.Tags.SetPropertyReversible(x => x.DisableNotifyChanged, true);
                                ReversibleCollection<object> reversibleCollection = ObjectHaveTag.Tags.AsReversibleCollection();
                                for (int i = newTags.Count - 1; i <= 0; i--)
                                {
                                    if (i >= 0)
                                        reversibleCollection.Insert(selectedIndex, newTags[i]);
                                }
                                ObjectHaveTag.Tags.SetPropertyReversible(x => x.DisableNotifyChanged, false);
                                ObjectHaveTag.Tags.NotifyResetCollection();
                                transaction.Reversing += (s, e) =>
                                {
                                    WorkspaceManagerService.OpenPanel(this);
                                };
                                transaction.Reversed += (s, e) =>
                                {
                                    ObjectHaveTag.Tags.NotifyResetCollection();
                                };
                                transaction.Commit();
                            }
                        }
                    }
                }
            }
            IsBusy = false;
        }

        public bool CanInsertAboveTag()
        {
            return !IsBusy && SelectedItem != null && !Parent.IsReadOnly && SelectedItems != null && SelectedItems.Count == 1;
        }

        public void InsertBelowTag()
        {
            IsBusy = true;
            IEasyDriverPlugin driver = DriverManagerService.GetDriver(Parent);
            if (driver != null)
            {
                if (ContextWindowService.ShowDialog(driver.GetCreateTagControl(Parent), "Add Tag") is List<ITagCore> newTags)
                {
                    int selectedIndex = ObjectHaveTag.Tags.IndexOf(SelectedItem);
                    if (selectedIndex > -1)
                    {
                        if (selectedIndex > -1)
                        {
                            using (Transaction transaction = ReversibleService.Begin("Insert Tag"))
                            {
                                ObjectHaveTag.Tags.SetPropertyReversible(x => x.DisableNotifyChanged, true);
                                ReversibleCollection<object> reversibleCollection = ObjectHaveTag.Tags.AsReversibleCollection();
                                for (int i = newTags.Count - 1; i >= 0; i--)
                                {
                                    if (i >= 0)
                                        reversibleCollection.Insert(selectedIndex + 1, newTags[i]);
                                }
                                ObjectHaveTag.Tags.SetPropertyReversible(x => x.DisableNotifyChanged, false);
                                ObjectHaveTag.Tags.NotifyResetCollection();
                                transaction.Reversing += (s, e) =>
                                {
                                    WorkspaceManagerService.OpenPanel(this);
                                };
                                transaction.Reversed += (s, e) =>
                                {
                                    ObjectHaveTag.Tags.NotifyResetCollection();
                                };
                                transaction.Commit();
                            }
                        }
                    }
                }
            }
            IsBusy = false;
        }

        public bool CanInsertBelowTag()
        {
            return !IsBusy && SelectedItem != null && !Parent.IsReadOnly && SelectedItems != null && SelectedItems.Count == 1;
        }

        public void AddInternalTag()
        {
            try
            {
                IsBusy = true;
                InternalTagViewModel vm = ViewModelSource.Create(() => new InternalTagViewModel());
                vm.ParentViewModel = this;
                vm.Parameter = Parent;
                InternalTagView view = new InternalTagView() { DataContext = vm };
                ContextWindowService.ShowDialog(view, vm.Title, vm.SizeToContent, vm.Width, vm.Height);
            }
            catch (Exception) { }
            finally { IsBusy = false; }
        }

        public bool CanAddInternalTag()
        {
            return !IsBusy && !Parent.IsReadOnly && ObjectHaveTag.HaveTags;
        }

        public void InsertAboveInternalTag()
        {
            try
            {
                IsBusy = true;
                InternalTagViewModel vm = ViewModelSource.Create(() => new InternalTagViewModel());
                vm.InsertAbove = true;
                vm.InsertIndex = (Parent as IHaveTag).Tags.IndexOf(SelectedItem);
                vm.ParentViewModel = this;
                vm.Parameter = Parent;
                InternalTagView view = new InternalTagView() { DataContext = vm };
                ContextWindowService.ShowDialog(view, vm.Title, vm.SizeToContent, vm.Width, vm.Height);
            }
            catch (Exception) { }
            finally { IsBusy = false; }
        }

        public bool CanInsertAboveInternalTag()
        {
            return !IsBusy && SelectedItem != null && !Parent.IsReadOnly && SelectedItems != null && SelectedItems.Count == 1;
        }

        public void InsertBelowInternalTag()
        {
            try
            {
                IsBusy = true;
                InternalTagViewModel vm = ViewModelSource.Create(() => new InternalTagViewModel());
                vm.InsertBelow = true;
                vm.InsertIndex = (Parent as IHaveTag).Tags.IndexOf(SelectedItem);
                vm.ParentViewModel = this;
                vm.Parameter = Parent;
                InternalTagView view = new InternalTagView() { DataContext = vm };
                ContextWindowService.ShowDialog(view, vm.Title, vm.SizeToContent, vm.Width, vm.Height);
            }
            catch (Exception) { }
            finally { IsBusy = false; }
        }

        public bool CanInsertBelowInternalTag()
        {
            return !IsBusy && SelectedItem != null && !Parent.IsReadOnly && SelectedItems != null && SelectedItems.Count == 1;
        }

        public void Export()
        {
            try
            {
                SaveFileDialogService.Title = "Export Csv";
                SaveFileDialogService.Filter = "CSV Files (*.csv)|*.csv";
                SaveFileDialogService.DefaultExt = ".csv";
                SaveFileDialogService.DefaultFileName = Parent.Name;
                if (SaveFileDialogService.ShowDialog())
                {
                    IsBusy = true;
                    string filePath = SaveFileDialogService.File.GetFullName();
                    CsvBuilder csv = new CsvBuilder();
                    csv.AddColumn("Name").AddColumn("Address").
                        AddColumn("DataType").AddColumn("RefreshRate").AddColumn("AccessPermission").
                        AddColumn("Gain").AddColumn("Offset").AddColumn("Unit").AddColumn("IsInternal").AddColumn("DefaultValue").
                        AddColumn("WriteMaxLimit").AddColumn("WriteMinLimit").AddColumn("EnabledWriteLimit").AddColumn("Description");
                    foreach (var item in ObjectHaveTag.Tags)
                    {
                        if (item is ITagCore tag)
                        {
                            csv.AddRow(
                                tag.Name, tag.Address,
                                tag.DataType.Name, tag.RefreshRate.ToString(), tag.AccessPermission.ToString(),
                                tag.Gain.ToString(), tag.Offset.ToString(), tag.Unit, tag.IsInternalTag.ToString(), tag.DefaultValue, tag.WriteMaxLimit.ToString(),
                                tag.WriteMinLimit.ToString(), tag.EnabledWriteLimit.ToString(), tag.Description);
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
            catch { }
            finally { IsBusy = false; }
        }

        public bool CanExport()
        {
            return !IsBusy;
        }

        public void Import()
        {
            try
            {
                OpenFileDialogService.Title = "Import CSV...";
                OpenFileDialogService.Filter = "CSV Files (*.csv)|*.csv";
                if (OpenFileDialogService.ShowDialog())
                {
                    string csvPath = OpenFileDialogService.File.GetFullName();
                    IsBusy = true;
                    ImportTagViewModel vm = ViewModelSource.Create(() => new ImportTagViewModel());
                    vm.ParentViewModel = this;
                    vm.Parameter = new object[] { Parent, csvPath };
                    ImportTagView view = new ImportTagView() { DataContext = vm };
                    ContextWindowService.ShowDialog(view, vm.Title, vm.SizeToContent, vm.Width, vm.Height);
                }
            }
            catch { }
            finally { IsBusy = false; }
        }

        public bool CanImport()
        {
            return !IsBusy && !Parent.IsReadOnly;
        }

        public void ShowSearchPanel()
        {
            TreeListViewUtilities.ShowSearchPanel();
        }

        public void ToggleEnabledDragAndDrop()
        {
            AllowDragDrop = ToggleEnabledDragAndDropBarItem.IsChecked = !AllowDragDrop;
        }

        public void EditTag()
        {
            IsBusy = true;

            if (SelectedItem is ITagCore tag)
            {
                if (tag.IsInternalTag)
                {
                    InternalTagViewModel vm = ViewModelSource.Create(() => new InternalTagViewModel());
                    vm.ParentViewModel = this;
                    vm.Parameter = tag;
                    InternalTagView view = new InternalTagView() { DataContext = vm };
                    ContextWindowService.ShowDialog(view, vm.Title, vm.SizeToContent, vm.Width, vm.Height);
                }
                else
                {
                    IEasyDriverPlugin driver = DriverManagerService.GetDriver(tag);
                    string oldName = tag.Name;
                    string oldAddress = tag.Address;
                    double oldGain = tag.Gain;
                    double oldOffset = tag.Offset;
                    string oldDefaultValue = tag.DefaultValue;
                    string oldUnit = tag.Unit;
                    IDataType oldDataType = tag.DataType;

                    int oldRefreshRate = tag.RefreshRate;
                    ByteOrder oldByteOrder = tag.ByteOrder;
                    AccessPermission oldAccessPermission = tag.AccessPermission;

                    if (ContextWindowService.ShowDialog(driver.GetEditTagControl(tag), $"Edit Tag - {tag.Name}") is ITagCore tagCore)
                    {
                        if (tagCore.HasChanges())
                        {
                            using (Transaction transaction = ReversibleService.Begin("Edit Tag"))
                            {
                                if (oldName != tagCore.Name)
                                    tagCore.AddPropertyChangedReversible(x => x.Name, oldName, tagCore.Name);
                                if (oldAddress != tagCore.Address)
                                    tagCore.AddPropertyChangedReversible(x => x.Address, oldAddress, tagCore.Address);
                                if (oldGain != tagCore.Gain)
                                    tagCore.AddPropertyChangedReversible(x => x.Gain, oldGain, tagCore.Gain);
                                if (oldOffset != tagCore.Offset)
                                    tagCore.AddPropertyChangedReversible(x => x.Offset, oldOffset, tagCore.Offset);
                                if (oldDataType.Name != tagCore.DataType.Name)
                                    tagCore.AddPropertyChangedReversible(x => x.DataType, oldDataType, tagCore.DataType);
                                if (oldRefreshRate != tagCore.RefreshRate)
                                    tagCore.AddPropertyChangedReversible(x => x.RefreshRate, oldRefreshRate, tagCore.RefreshRate);
                                if (oldByteOrder != tagCore.ByteOrder)
                                    tagCore.AddPropertyChangedReversible(x => x.ByteOrder, oldByteOrder, tagCore.ByteOrder);
                                if (oldAccessPermission != tagCore.AccessPermission)
                                    tagCore.AddPropertyChangedReversible(x => x.AccessPermission, oldAccessPermission, tagCore.AccessPermission);
                                if (oldDefaultValue != tagCore.DefaultValue)
                                    tagCore.AddPropertyChangedReversible(x => x.DefaultValue, oldDefaultValue, tagCore.DefaultValue);
                                if (oldUnit != tagCore.Unit)
                                    tagCore.AddPropertyChangedReversible(x => x.Unit, oldUnit, tagCore.Unit);

                                transaction.Reversing += (s, e) =>
                                {
                                    WorkspaceManagerService.OpenPanel(this);
                                };

                                transaction.Commit();
                            }
                        }
                    }
                }
            }

            IsBusy = false;
        }

        public bool CanEdit()
        {
            return !IsBusy && SelectedItem != null && !Parent.IsReadOnly && SelectedItems != null && SelectedItems.Count == 1;
        }

        #region ISupportCopyPaste
        public void Copy()
        {
            var tagsToCopy = SelectedItems.Select(x => x as ICoreItem).OrderBy(x => x.CreatedDate).ToList();
            ClipboardService.CopyToClipboard(tagsToCopy, this);
        }

        public bool CanCopy()
        {
            if (IsBusy)
                return false;
            if (Parent != null && SelectedItems != null)
                return !Parent.IsReadOnly && SelectedItems.Count > 0;
            return false;
        }

        public void Cut()
        {
            ClipboardService.CopyToClipboard(SelectedItems.Select(x => x as ICoreItem).OrderBy(x => x.CreatedDate).ToList(), this);
            ObjectHaveTag.Tags.RemoveRange(SelectedItems.ToArray());
        }

        public bool CanCut()
        {
            if (IsBusy)
                return false;
            if (Parent != null && SelectedItems != null)
                return !Parent.IsReadOnly && SelectedItems.Count > 0;
            return false;
        }

        public void Paste()
        {
            try
            {
                IsBusy = true;
                if (ClipboardService.ContainData())
                {
                    if (ClipboardService.ObjectToCopy is List<object> tagsToCopy)
                    {
                        IEasyDriverPlugin driver = DriverManagerService.GetDriver(Parent.Parent);
                        IEnumerable<IDataType> dataTypesSource = driver.SupportDataTypes;
                        using (Transaction transaction = ReversibleService.Begin($"Paste {tagsToCopy.Count} Tags"))
                        {
                            ObjectHaveTag.Tags.SetPropertyReversible(x => x.DisableNotifyChanged, true);
                            ReversibleCollection<object> reversibleCollection = ObjectHaveTag.Tags.AsReversibleCollection();
                            foreach (var item in tagsToCopy)
                            {
                                if (item is ITagCore tagCore)
                                {
                                    ITagCore newTag = driver.CreateTag(Parent);
                                    newTag.Name = (Parent as IHaveTag).GetUniqueNameInGroupTags(tagCore.Name, true);
                                    newTag.Address = tagCore.Address;
                                    newTag.Offset = tagCore.Offset;
                                    newTag.Gain = tagCore.Gain;
                                    newTag.ByteOrder = tagCore.ByteOrder;
                                    newTag.RefreshRate = tagCore.RefreshRate;
                                    newTag.AccessPermission = tagCore.AccessPermission;
                                    newTag.Description = tagCore.Description;
                                    foreach (var kvp in tagCore.ParameterContainer.Parameters)
                                    {
                                        newTag.ParameterContainer.SetValue(kvp.Key, kvp.Value);
                                    }
                                    newTag.ParameterContainer.DisplayParameters = tagCore.ParameterContainer.DisplayParameters;
                                    newTag.ParameterContainer.DisplayName = tagCore.ParameterContainer.DisplayName;
                                    newTag.IsInternalTag = tagCore.IsInternalTag;
                                    if (newTag.IsInternalTag)
                                    {
                                        newTag.DataTypeName = tagCore.DataTypeName;
                                    }
                                    else
                                    {
                                        newTag.DataType = dataTypesSource.FirstOrDefault(x => x.Name == tagCore.DataType.Name);
                                    }
                                    reversibleCollection.Add(newTag);
                                }
                            }
                            ObjectHaveTag.Tags.SetPropertyReversible(x => x.DisableNotifyChanged, false);
                            ObjectHaveTag.Tags.NotifyResetCollection();
                            transaction.Reversing += (s, e) =>
                            {
                                WorkspaceManagerService.OpenPanel(this);

                            };
                            transaction.Reversed += (s, e) =>
                            {
                                ObjectHaveTag.Tags.NotifyResetCollection();
                            };

                            transaction.Commit();
                        }
                    }
                }
            }
            catch { }
            finally { IsBusy = false; }
        }

        public bool CanPaste()
        {
            if (IsBusy || !ClipboardService.ContainData() || Parent.IsReadOnly)
                return false;
            if (ClipboardService.Context is TagCollectionViewModel context)
            {
                if (context.Parent.Parent is IChannelCore contextChannel && Parent.Parent is IChannelCore currentChannel)
                    return contextChannel.DriverPath == currentChannel.DriverPath;
            }
            return false;
        }

        /// <summary>
        /// Lệnh xóa các <see cref="ITagCore"/> được chọn trên Table
        /// </summary>
        public void Delete()
        {
            try
            {
                // Nếu như số lượng các TagCore được chọn lớn hơn 0 thì mới cho phép xóa
                if (SelectedItems.Count > 0)
                {
                    // Hỏi người dùng có muốn xóa đối tượng đang chọn hay không
                    var mbr = MessageBoxService.ShowMessage($"Do you want to delete all selected items and all object associated with it?",
                        "Message",
                        MessageButton.YesNo, MessageIcon.Question);

                    // Nếu người dùng chọn 'Yes' thì thực hiện việc xóa đối tượng
                    if (mbr == MessageResult.Yes)
                    {
                        // Khóa luồng làm việc của chương trình
                        IsBusy = true;
                        using (Transaction transaction = ReversibleService.Begin($"Delete {SelectedItems.Count} Tags"))
                        {
                            List<object> itemsToRemove = SelectedItems.ToList();
                            ObjectHaveTag.Tags.AsReversibleCollection().RemoveRange(itemsToRemove);

                            foreach (var item in itemsToRemove)
                            {
                                if (item is ITagCore tagCore)
                                {
                                    if (!string.IsNullOrEmpty(tagCore.GUID))
                                    {
                                        InternalStorageService.RemoveStoreValue(tagCore.GUID);
                                    }
                                }
                            }

                            transaction.Reversing += (s, e) =>
                            {
                                WorkspaceManagerService.OpenPanel(this);
                            };

                            transaction.Reversed += (s, e) =>
                            {
                                foreach (var item in itemsToRemove)
                                {
                                    if (item is ITagCore tagCore)
                                    {
                                        if (tagCore.IsInternalTag && !string.IsNullOrEmpty(tagCore.GUID) && tagCore.Retain)
                                        {
                                            StoreValue storeValue = InternalStorageService.GetStoreValue(tagCore.GUID);
                                            if (storeValue != null)
                                                tagCore.Value = storeValue.Value;
                                        }
                                    }
                                }
                            };
                            transaction.Commit();
                        }
                    }
                }
            }
            catch { }
            // Mở luồng làm việc
            finally { IsBusy = false; }
        }

        /// <summary>
        /// Điều kiện để thực hiện lệnh <see cref="Delete"/>
        /// </summary>
        /// <returns></returns>
        public bool CanDelete()
        {
            return !IsBusy && SelectedItems != null && !Parent.IsReadOnly && SelectedItems.Count > 0;
        }
        #endregion
        #endregion
    }
}
