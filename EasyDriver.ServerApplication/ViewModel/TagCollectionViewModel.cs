using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriver.Core;
using EasyDriverPlugin;
using EasyScada.ServerApplication.Reversible;
using EasyScada.ServerApplication.Workspace;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace EasyScada.ServerApplication
{
    public class TagCollectionViewModel : WorkspacePanelViewModelBase, ISupportEdit
    {
        #region Constructors

        public TagCollectionViewModel(
            IWorkspaceManagerService workspaceManagerService, 
            IReverseService reverseService,
            IDriverManagerService driverManagerService,
            IProjectManagerService projectManagerService,
            IInternalStorageService internalStorageService) : base(null, workspaceManagerService)
        {
            IsDocument = true;
            WorkspaceName = WorkspaceRegion.DocumentHost;
            WorkspaceManagerService = workspaceManagerService;
            ReverseService = reverseService;
            DriverManagerService = driverManagerService;
            ProjectManagerService = projectManagerService;
            InternalStorageService = internalStorageService;
        }

        #endregion

        #region Injected members

        protected IReverseService ReverseService { get; set; }
        protected IDriverManagerService DriverManagerService { get; set; }
        protected IProjectManagerService ProjectManagerService { get; set; }
        protected IInternalStorageService InternalStorageService { get; set; }

        #endregion

        #region UI services

        protected ITreeListViewUtilities TreeListViewUtilities { get => this.GetService<ITreeListViewUtilities>(); }
        protected IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }
        protected ISaveFileDialogService SaveFileDialogService { get => this.GetService<ISaveFileDialogService>(); }
        protected IOpenFileDialogService OpenFileDialogService { get => this.GetService<IOpenFileDialogService>(); }
        protected IWindowService WindowService { get => this.GetService<IWindowService>(); }
        protected IContextWindowService ContextWindowService { get => this.GetService<IContextWindowService>(); }
        protected ITableViewUtilities TableViewUtilities { get => this.GetService<ITableViewUtilities>(); }

        #endregion

        #region Public members

        public override string WorkspaceName { get; protected set; }

        public virtual object SelectedItem { get; set; }

        public virtual object CurrentItem { get; set; }

        public virtual ObservableCollection<object> SelectedItems { get; set; } = new ObservableCollection<object>();

        public virtual IGroupItem Parent => Token as IGroupItem;

        public virtual IHaveTag ObjectHaveTag => Token as IHaveTag;

        public virtual ObservableCollection<object> ProjectChilds { get => ProjectManagerService?.CurrentProject?.Childs; }

        public MainViewModel MainViewModel => ParentViewModel as MainViewModel;

        public override bool IsBusy
        {
            get => MainViewModel == null ? true : MainViewModel.IsBusy;
            set => MainViewModel.IsBusy = value;
        }

        #endregion

        #region Event handlers

        public virtual void ShowProperty(object item)
        {
            if (item is ITagCore tag)
                Messenger.Default.Send(new ShowPropertiesMessage(this, item));
        }

        public virtual void OpenOnDoubleClick(object item)
        {
            if (IsBusy || Parent.IsReadOnly)
                return;

            IsBusy = true;
            if (item is ITagCore tag)
            {
                if (!tag.IsInternalTag)
                {
                    IEasyDriverPlugin driver = DriverManagerService.GetDriver(tag);
                    if (driver != null)
                        ContextWindowService.Show(driver.GetEditTagControl(tag), $"Edit Tag - {tag.Name}");
                }
                else
                {
                    WindowService.Show("InternalTagView", tag, this);
                }
            }
            IsBusy = false;
        }

        public virtual void OnLoaded()
        {
        }

        #endregion

        #region Commands

        public void WriteTag()
        {
            WindowService.Show("WriteTagView", SelectedItem as ITagCore, this);
        }

        public bool CanWriteTag()
        {
            return SelectedItem is ITagCore;
        }

        public void Add()
        {
            IsBusy = true;
            IChannelCore channel = Parent.FindParent<IChannelCore>(x => x is IChannelCore);
            IEasyDriverPlugin driver = DriverManagerService.GetDriver(channel);
            if (driver != null)
            {
                if (ContextWindowService.Show(driver.GetCreateTagControl(Parent), "Add Tag") is List<ITagCore> newTags)
                {
                    if (newTags.Count > 0)
                    {
                        using (Transaction transaction = ReverseService.Begin("Add Tag"))
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

        public bool CanAdd()
        {
            return !IsBusy && !Parent.IsReadOnly && ObjectHaveTag.HaveTags;
        }

        public void AddInternal()
        {
            IsBusy = true;
            WindowService.Show("InternalTagView", Parent, this);
            IsBusy = false;
        }

        public bool CanAddInternal()
        {
            return !IsBusy && !Parent.IsReadOnly && ObjectHaveTag.HaveTags;
        }

        public void InsertAbove()
        {
            IsBusy = true;
            IChannelCore channel = Parent.FindParent<IChannelCore>(x => x is IChannelCore);
            IEasyDriverPlugin driver = DriverManagerService.GetDriver(channel);
            if (driver != null)
            {
                if (ContextWindowService.Show(driver.GetCreateTagControl(Parent), "Add Tag") is List<ITagCore> newTags)
                {
                    if (newTags.Count > 0)
                    {
                        int selectedIndex = ObjectHaveTag.Tags.IndexOf(SelectedItem);
                        if (selectedIndex > -1)
                        {
                            using (Transaction transaction = ReverseService.Begin("Insert Tag"))
                            {
                                ObjectHaveTag.Tags.SetPropertyReversible(x => x.DisableNotifyChanged, true);
                                ReversibleCollection<object> reversibleCollection = ObjectHaveTag.Tags.AsReversibleCollection();
                                for (int i = newTags.Count - 1; i <= 0; i--)
                                {
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

        public bool CanInsertAbove()
        {
            return !IsBusy && SelectedItem != null && !Parent.IsReadOnly;
        }

        public void InsertBelow()
        {
            IsBusy = true;
            IChannelCore channel = Parent.FindParent<IChannelCore>(x => x is IChannelCore);
            IEasyDriverPlugin driver = DriverManagerService.GetDriver(channel);
            if (driver != null)
            {
                if (ContextWindowService.Show(driver.GetCreateTagControl(Parent), "Add Tag") is List<ITagCore> newTags)
                {
                    int selectedIndex = ObjectHaveTag.Tags.IndexOf(SelectedItem);
                    if (selectedIndex > -1)
                    {
                        if (selectedIndex > -1)
                        {
                            using (Transaction transaction = ReverseService.Begin("Insert Tag"))
                            {
                                ObjectHaveTag.Tags.SetPropertyReversible(x => x.DisableNotifyChanged, true);
                                ReversibleCollection<object> reversibleCollection = ObjectHaveTag.Tags.AsReversibleCollection();
                                for (int i = newTags.Count - 1; i <= 0; i--)
                                {
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

        public bool CanInsertBelow()
        {
            return !IsBusy && SelectedItem != null && !Parent.IsReadOnly;
        }

        public void Edit()
        {
            IsBusy = true;

            if (SelectedItem is ITagCore tag)
            {
                if (tag.IsInternalTag)
                {
                    WindowService.Show("InternalTagView", tag, this);
                }
                else
                {
                    IEasyDriverPlugin driver = DriverManagerService.GetDriver(tag);
                    string oldName = tag.Name;
                    string oldAddress = tag.Address;
                    double oldGain = tag.Gain;
                    double oldOffset = tag.Offset;
                    IDataType oldDataType = tag.DataType;

                    int oldRefreshRate = tag.RefreshRate;
                    ByteOrder oldByteOrder = tag.ByteOrder;
                    AccessPermission oldAccessPermission = tag.AccessPermission;

                    if (ContextWindowService.Show(driver.GetEditTagControl(tag), $"Edit Tag - {tag.Name}") is ITagCore tagCore)
                    {
                        if (tagCore.HasChanges())
                        {
                            using (Transaction transaction = ReverseService.Begin("Edit Tag"))
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
            return !IsBusy && SelectedItem != null && !Parent.IsReadOnly;
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
                        AddColumn("Gain").AddColumn("Offset").AddColumn("Description");
                    foreach (var item in ObjectHaveTag.Tags)
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
                    WindowService.Show("ImportTagView", new object[] { Parent, csvPath }, this);
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
            TableViewUtilities.ShowSearchPanel();
        }

        #endregion

        #region ISupportEdit

        public void Copy()
        {
            var tagsToCopy = SelectedItems.ToList();
            tagsToCopy.Sort((x1, x2) =>
            {
                if (x1 is ITagCore tag1 && x2 is ITagCore tag2)
                {
                    int index1 = (tag1.Parent as IHaveTag).Tags.IndexOf(tag1);
                    int index2 = (tag2.Parent as IHaveTag).Tags.IndexOf(tag2);
                    if (index1 > index2)
                        return -1;
                    else if (index1 < index2)
                        return 1;
                }
                return 0;
            });
            ClipboardManager.CopyToClipboard(SelectedItems.ToList(), this);
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
            ClipboardManager.CopyToClipboard(SelectedItems.ToList(), this);
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
                if (ClipboardManager.ContainData())
                {
                    if (ClipboardManager.ObjectToCopy is List<object> tagsToCopy)
                    {
                        IEasyDriverPlugin driver = DriverManagerService.GetDriver(Parent.Parent);
                        IEnumerable<IDataType> dataTypesSource = driver.GetSupportDataTypes();
                        using (Transaction transaction = ReverseService.Begin("Paste Tags"))
                        {

                            ObjectHaveTag.Tags.SetPropertyReversible(x => x.DisableNotifyChanged, true);
                            ReversibleCollection<object> reversibleCollection = ObjectHaveTag.Tags.AsReversibleCollection();
                            foreach (var item in tagsToCopy)
                            {
                                if (item is ITagCore tagCore)
                                {
                                    ITagCore newTag = new TagCore(Parent);
                                    newTag.Name = Parent.GetUniqueNameInGroup(tagCore.Name, true);
                                    newTag.Address = tagCore.Address;
                                    newTag.Offset = tagCore.Offset;
                                    newTag.Gain = tagCore.Gain;
                                    newTag.ByteOrder = tagCore.ByteOrder;
                                    newTag.RefreshRate = tagCore.RefreshRate;
                                    newTag.AccessPermission = tagCore.AccessPermission;
                                    newTag.Description = tagCore.Description;
                                    newTag.ParameterContainer = tagCore.ParameterContainer.DeepCopy();
                                    newTag.DataType = dataTypesSource.FirstOrDefault(x => x.Name == tagCore.DataType.Name);
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
            if (IsBusy || !ClipboardManager.ContainData() || Parent.IsReadOnly)
                return false;
            if (ClipboardManager.Context is TagCollectionViewModel context)
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
                        "Easy Driver Server",
                        MessageButton.YesNo, MessageIcon.Question);

                    // Nếu người dùng chọn 'Yes' thì thực hiện việc xóa đối tượng
                    if (mbr == MessageResult.Yes)
                    {
                        // Khóa luồng làm việc của chương trình
                        IsBusy = true;
                        using (Transaction transaction = ReverseService.Begin("Delete Tags"))
                        {
                            List<object> itemsToRemove = SelectedItems.ToList();
                            ObjectHaveTag.Tags.AsReversibleCollection().RemoveRange(itemsToRemove);

                            foreach (var item in itemsToRemove)
                            {
                                if (item is ITagCore tagCore)
                                {
                                    if (tagCore.ParameterContainer.Parameters.ContainsKey("GUID"))
                                    {
                                        string guid = tagCore.ParameterContainer.Parameters["GUID"];
                                        InternalStorageService.RemoveInternalTag(guid);
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
                                        if (tagCore.ParameterContainer.Parameters.ContainsKey("GUID"))
                                        {
                                            if (tagCore.ParameterContainer.Parameters.ContainsKey("Retain"))
                                            {
                                                if (tagCore.ParameterContainer.Parameters["Retain"] == bool.TrueString)
                                                {
                                                    string guid = tagCore.ParameterContainer.Parameters["GUID"];
                                                    InternalStorageService.AddOrUpdateInternalTag(tagCore);
                                                }
                                            }
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

        #region Methods

        #endregion
    }
}
