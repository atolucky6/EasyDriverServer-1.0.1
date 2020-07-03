using DevExpress.Data;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriverPlugin;
using EasyScada.ServerApplication.Workspace;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace EasyScada.ServerApplication
{
    public class TagCollectionViewModel : WorkspacePanelViewModelBase, ISupportEdit
    {
        #region Constructors

        public TagCollectionViewModel(
            IWorkspaceManagerService workspaceManagerService, 
            IReverseService reverseService,
            IDriverManagerService driverManagerService,
            IProjectManagerService projectManagerService) : base(null, workspaceManagerService)
        {
            IsDocument = true;
            WorkspaceName = WorkspaceRegion.DocumentHost;
            WorkspaceManagerService = workspaceManagerService;
            ReverseService = reverseService;
            DriverManagerService = driverManagerService;
            ProjectManagerService = projectManagerService;
        }

        #endregion

        #region Injected members

        protected IReverseService ReverseService { get; set; }
        protected IDriverManagerService DriverManagerService { get; set; }
        protected IProjectManagerService ProjectManagerService { get; set; }

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

        public virtual ObservableCollection<object> SelectedItems { get; set; }

        public virtual IDeviceCore Parent => Token as IDeviceCore;

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
            if (IsBusy)
                return;

            IsBusy = true;
            if (item is ITagCore tag)
            {
                IEasyDriverPlugin driver = DriverManagerService.GetDriver(tag);
                ContextWindowService.Show(driver.GetEditTagControl(tag), $"Edit Tag - {tag.Name}");
            }
            IsBusy = false;
        }

        public virtual void OnLoaded()
        {
        }

        #endregion

        #region Commands

        public void Add()
        {
            IsBusy = true;
            IChannelCore channel = Parent.FindParent<IChannelCore>(x => x is IChannelCore);
            IEasyDriverPlugin driver = DriverManagerService.GetDriver(channel);
            if (driver != null)
            {
                if (ContextWindowService.Show(driver.GetCreateTagControl(Parent), "Add Tag") is ITagCore newTag)
                {
                    CurrentItem = SelectedItem = newTag;
                }
            }
            IsBusy = false;
        }

        public bool CanAdd()
        {
            return !IsBusy && !Parent.IsReadOnly;
        }

        public void InsertAbove()
        {
            IsBusy = true;
            IChannelCore channel = Parent.FindParent<IChannelCore>(x => x is IChannelCore);
            IEasyDriverPlugin driver = DriverManagerService.GetDriver(channel);
            if (driver != null)
            {
                if (ContextWindowService.Show(driver.GetCreateTagControl(Parent), "Add Tag") is ITagCore newTag)
                {
                    if (newTag != null)
                    {
                        int selectedIndex = Parent.Childs.IndexOf(SelectedItem);
                        if (selectedIndex >= 0)
                            Parent.Childs.Move(Parent.Childs.Count - 1, selectedIndex);
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
                if (ContextWindowService.Show(driver.GetCreateTagControl(Parent), "Add Tag") is ITagCore newTag)
                {
                    if (newTag != null)
                    {
                        int selectedIndex = Parent.Childs.IndexOf(SelectedItem);
                        if (selectedIndex >= 0)
                            Parent.Childs.Move(Parent.Childs.Count - 1, selectedIndex + 1);
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
                IEasyDriverPlugin driver = DriverManagerService.GetDriver(tag);
                ContextWindowService.Show(driver.GetEditTagControl(tag), $"Edit Tag - {tag.Name}");
            }

            IsBusy = false;
        }

        public bool CanEdit()
        {
            return !IsBusy && SelectedItem != null && !Parent.IsReadOnly;
        }

        public void Export()
        {

        }

        public bool CanExport()
        {
            return !IsBusy && !Parent.IsReadOnly;
        }

        public void Import()
        {

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
        }

        public bool CanCopy()
        {
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

        }

        public bool CanPaste()
        {
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
                        Parent.Childs.RemoveRange(SelectedItems);
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
