using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriver.MenuPlugin;
using EasyDriver.Reversible;
using EasyDriver.WorkspaceManager;
using System.Collections.ObjectModel;

namespace EasyDriverServer.ViewModel
{
    public class MainViewModel
    {
        #region Constructors

        public MainViewModel(
            IWorkspaceManagerService workspaceManagerService,
            IReverseService reverseService,
            MenuModule menuModule,
            WorkspaceModule workspaceModule)
        {
            WorkspaceManagerService = workspaceManagerService;
            ReverseService = reverseService;
            MenuModule = menuModule;
            WorkspaceModule = workspaceModule;
        }

        #endregion

        #region Injected services

        protected IWorkspaceManagerService WorkspaceManagerService { get; set; }
        protected IReverseService ReverseService { get; set; }
        public MenuModule MenuModule { get; protected set; }
        public WorkspaceModule WorkspaceModule { get; protected set; }

        #endregion

        #region UI services

        public IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }
        public IWindowService WindowService { get => this.GetService<IWindowService>(); }
        public IWindowService LicenseWindowService { get => this.GetService<IWindowService>("LicenseWindowService"); }
        public ISaveFileDialogService SaveFileDialogService { get => this.GetService<ISaveFileDialogService>(); }
        public IOpenFileDialogService OpenFileDialogService { get => this.GetService<IOpenFileDialogService>(); }
        public IDispatcherService DispatcherService { get => this.GetService<IDispatcherService>(); }
        public ICurrentWindowService CurrentWindowService { get => this.GetService<ICurrentWindowService>(); }

        #endregion

        #region Public properties

        /// <summary>
        /// Danh sách các workspace có trong chương trình
        /// </summary>
        public ObservableCollection<IWorkspacePanel> Workspaces => WorkspaceManagerService.Workspaces;

        /// <summary>
        /// Biến xác nhận chương trình đang rảnh hay không
        /// </summary>
        /// <remarks>True nghĩa là đang bận và ngược lại</remarks>
        public virtual bool IsBusy { get; set; }

        #endregion

        #region Fields

        #endregion

        #region Event handlers

        /// <summary>
        /// Sự kiện khi MainView được Load
        /// </summary>
        public virtual void OnLoaded()
        {
        }

        #endregion

        #region Private methods

        #endregion
    }
}
