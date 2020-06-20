using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriverPlugin;
using EasyScada.ServerApplication.Workspace;
using System.Collections.ObjectModel;
using System.Linq;

namespace EasyScada.ServerApplication
{
    public class TagCollectionViewModel : WorkspacePanelViewModelBase, ISupportEdit
    {
        #region Constructors

        public TagCollectionViewModel(
            IWorkspaceManagerService workspaceManagerService, 
            IReverseService reverseService,
            IDriverManagerService driverManagerService) : base(null)
        {
            WorkspaceName = WorkspaceRegion.DocumentHost;
            WorkspaceManagerService = workspaceManagerService;
            ReverseService = reverseService;
            DriverManagerService = driverManagerService;
        }

        #endregion

        #region Injected members

        protected IWorkspaceManagerService WorkspaceManagerService { get; set; }
        protected IReverseService ReverseService { get; set; }
        protected IDriverManagerService DriverManagerService { get; set; }

        #endregion

        #region UI services

        protected ITreeListViewUtilities TreeListViewUtilities { get => this.GetService<ITreeListViewUtilities>(); }
        protected IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }
        protected ISaveFileDialogService SaveFileDialogService { get => this.GetService<ISaveFileDialogService>(); }
        protected IOpenFileDialogService OpenFileDialogService { get => this.GetService<IOpenFileDialogService>(); }
        protected IWindowService WindowService { get => this.GetService<IWindowService>(); }
        protected IContextWindowService ContextWindowService { get => this.GetService<IContextWindowService>(); }

        #endregion

        #region Public members

        public override string WorkspaceName { get; protected set; }

        public virtual object SelectedItem { get; set; }

        public virtual object CurrentItem { get; set; }

        public virtual ObservableCollection<object> SelectedItems { get; set; }

        public virtual IDeviceCore Parent => Token as IDeviceCore;

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
            return !IsBusy;
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
                    int selectedIndex = Parent.Childs.IndexOf(SelectedItem as ICoreItem);
                    Parent.Childs.Move(Parent.Childs.Count - 1, selectedIndex);
                }
            }
            IsBusy = false;
        }

        public bool CanInsertAbove()
        {
            return !IsBusy && SelectedItem != null;
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
                    int selectedIndex = Parent.Childs.IndexOf(SelectedItem as ICoreItem);
                    Parent.Childs.Move(Parent.Childs.Count - 1, selectedIndex + 1);
                }
            }
            IsBusy = false;
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
            return !IsBusy && SelectedItem != null;
        }

        public bool CanInsertBelow()
        {
            return !IsBusy && SelectedItem != null;
        }

        public void Export()
        {

        }

        public void Import()
        {

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

        public void Delete()
        {
        }

        public bool CanDelete()
        {
            if (SelectedItems == null)
                return false;
            if (SelectedItems.Count == 0)
                return false;
            if (SelectedItems.FirstOrDefault(x => !(x as ICoreItem).IsReadOnly) != null)
                return true;
            return false;
        }

        #endregion
    }
}
