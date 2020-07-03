using DevExpress.Xpf.Docking;

namespace EasyScada.ServerApplication.Workspace
{
    public abstract class WorkspacePanelViewModelBase : IWorkspacePanel
    {
        #region Constructors

        public WorkspacePanelViewModelBase(object token, IWorkspaceManagerService workspaceManagerService)
        {
            Token = token;
            IsClosed = true;
            WorkspaceManagerService = workspaceManagerService;
        }

        #endregion

        #region Public Members

        protected IWorkspaceManagerService WorkspaceManagerService { get; set; }

        public virtual string Caption { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual bool IsClosed { get; set; }
        public virtual bool IsOpened { get; protected set; }
        public virtual bool AutoHidden { get; set; }
        public abstract string WorkspaceName { get; protected set; }
        public virtual object Token { get; set; }
        public virtual bool IsBusy { get; set; }
        public virtual bool IsDocument { get; protected set; }
        public virtual object ParentViewModel { get; set; }

        #endregion

        #region Methods

        public void Hide()
        {
            IsClosed = true;
        }

        public void Show()
        {
            IsClosed = false;
        }

        #endregion

        #region Properties Changed Callback

        public virtual void OnIsClosedChanged()
        {
            IsOpened = !IsClosed;
            if (IsClosed && WorkspaceManagerService != null)
                WorkspaceManagerService.CurrentActivePanel = null;
        }

        public virtual void OnIsActiveChanged()
        {
            if (IsActive && WorkspaceManagerService != null)
                WorkspaceManagerService.CurrentActivePanel = this;
        }

        public virtual void OnTokenChanged()
        {

        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            OnDispose();
        }

        public virtual void OnDispose()
        {
        }

        #endregion

        #region IMVVMDockingProperties

        string IMVVMDockingProperties.TargetName
        {
            get => WorkspaceName;
            set => WorkspaceName = value;
        }

        #endregion
    }
}
