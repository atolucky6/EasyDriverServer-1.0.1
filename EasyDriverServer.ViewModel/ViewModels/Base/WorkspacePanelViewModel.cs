using EasyDriver.WorkspaceManager;

namespace EasyDriverServer.ViewModel
{
    public abstract class WorkspacePanelViewModel : WorkspacePanelViewModelBase
    {
        public WorkspacePanelViewModel() : 
            base(null, null)
        {
            IsDocument = false;
            WorkspaceContext = this;
            WorkspaceManagerService = IoC.Instance.Get<IWorkspaceManagerService>();
        }
    }
}
