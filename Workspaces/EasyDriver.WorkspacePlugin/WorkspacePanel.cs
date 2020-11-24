using EasyScada.WorkspaceManager;

namespace EasyDriver.WorkspacePlugin
{
    public abstract class WorkspacePanel : WorkspacePanelBase
    {
        public WorkspacePanel(
            object context,
            IWorkspaceManagerService workspaceManagerService) : base(context, workspaceManagerService)
        {
            IsDocument = false;
            IsClosed = false;
        }
    }
}
