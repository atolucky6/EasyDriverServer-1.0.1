using EasyDriver.ServicePlugin;
using EasyScada.WorkspaceManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EasyDriver.WorkspacePlugin
{
    public abstract class WorkspacePanelBase : EasyServicePluginBase, IWorkspacePanel, ISupportInitialize
    {
        #region Public properties
        public virtual Uri UriTemplate { get; protected set; }
        public virtual string Caption { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual bool IsClosed { get; set; }
        public virtual bool IsOpened { get; protected set; }
        public virtual bool AutoHidden { get; set; }
        public virtual object WorkspaceContext { get; set; }
        public virtual bool IsBusy { get; set; }
        public virtual bool IsDocument { get; protected set; }
        public virtual object ParentViewModel { get; set; }
        public virtual object Glyph { get; set; }
        public virtual WorkspacePanelState State { get; set; }
        public virtual string TargetName { get; set; }
        #endregion

        #region Events
        public event EventHandler<WorkspacePanelStateChangedEventArgs> StateChanged;
        #endregion

        #region Injected services
        public virtual IWorkspaceManagerService WorkspaceManagerService { get; set; }
        #endregion

        #region Constructors
        public WorkspacePanelBase(
            object context,
            IWorkspaceManagerService workspaceManagerService) : base()
        {
            WorkspaceContext = context;
            WorkspaceManagerService = workspaceManagerService;
        }
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

        #region Event handlers
        public virtual void OnIsClosedChanged()
        {
            WorkspacePanelState oldState = State;
            IsOpened = !IsClosed;
            State = IsOpened ? WorkspacePanelState.Opened : WorkspacePanelState.Closed;
            if (IsClosed && WorkspaceManagerService != null)
                WorkspaceManagerService.CurrentActivePanel = null;
            StateChanged?.Invoke(this, new WorkspacePanelStateChangedEventArgs(oldState, State));
        }

        public virtual void OnIsActiveChanged()
        {
            if (IsActive && WorkspaceManagerService != null)
                WorkspaceManagerService.CurrentActivePanel = this;

            WorkspacePanelState oldState = State;
            State = IsActive ? WorkspacePanelState.Active : WorkspacePanelState.UnActive;
            StateChanged?.Invoke(this, new WorkspacePanelStateChangedEventArgs(oldState, State));
        }

        public virtual void OnWorkspaceContextChanged()
        {

        }
        #endregion
    }
}
