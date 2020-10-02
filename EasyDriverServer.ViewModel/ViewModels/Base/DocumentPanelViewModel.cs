using EasyDriver.WorkspaceManager;

namespace EasyDriverServer.ViewModel
{
    public abstract class DocumentPanelViewModel : WorkspacePanelViewModelBase
    {
        public DocumentPanelViewModel(object workspaceContext) : 
            base(workspaceContext, null)
        {
            IsDocument = true;
            WorkspaceManagerService = IoC.Instance.Get<IWorkspaceManagerService>();
        }
    }
}
