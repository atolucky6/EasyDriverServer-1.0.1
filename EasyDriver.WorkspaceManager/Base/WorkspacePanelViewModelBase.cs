using DevExpress.Xpf.Docking;
using System;

namespace EasyDriver.WorkspaceManager
{
    public abstract class WorkspacePanelViewModelBase : IWorkspacePanel
    {
        #region Constructors

        public WorkspacePanelViewModelBase(
            object workspaceContext, 
            IWorkspaceManagerService workspaceManagerService)
        {
            WorkspaceContext = workspaceContext;
            IsClosed = true;
            WorkspaceManagerService = workspaceManagerService;
        }

        #endregion

        #region Public Members

        public IWorkspaceManagerService WorkspaceManagerService { get; set; }

        public virtual Uri UriTemplate { get; protected set; }
        public virtual string Caption { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual bool IsClosed { get; set; }
        public virtual bool IsOpened { get; protected set; }
        public virtual bool AutoHidden { get; set; }
        public abstract string WorkspaceRegion { get; protected set; }
        public virtual object WorkspaceContext { get; set; }
        public virtual bool IsBusy { get; set; }
        public virtual bool IsDocument { get; protected set; }
        public virtual object ParentViewModel { get; set; }

        public event EventHandler<WorkspacePanelStateChangedEventArgs> StateChanged;

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

            if (IsOpened)
                StateChanged?.Invoke(this, new WorkspacePanelStateChangedEventArgs(WorkspacePanelState.Opened));
            else
                StateChanged?.Invoke(this, new WorkspacePanelStateChangedEventArgs(WorkspacePanelState.Closed));
        }

        public virtual void OnIsActiveChanged()
        {
            if (IsActive && WorkspaceManagerService != null)
                WorkspaceManagerService.CurrentActivePanel = this;
            if (IsActive)
                StateChanged?.Invoke(this, new WorkspacePanelStateChangedEventArgs(WorkspacePanelState.Active));
            else
                StateChanged?.Invoke(this, new WorkspacePanelStateChangedEventArgs(WorkspacePanelState.UnActive));
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
            get => WorkspaceRegion;
            set => WorkspaceRegion = value;
        }

        #endregion
    }
}
