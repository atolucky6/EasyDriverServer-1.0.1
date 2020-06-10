using EasyScada.ServerApplication.Workspace;
using System;

namespace EasyScada.ServerApplication
{
    public interface IWorkspaceManagerService: IWorkspaceManagerService<IWorkspacePanel>
    {

    }

    public class WorkspaceManagerService : WorkspaceManagerService<IWorkspacePanel>, IWorkspaceManagerService
    {
        public WorkspaceManagerService(Func<object, IWorkspacePanel> createPanelByTokenFunc) : base(createPanelByTokenFunc)
        {

        }
    }

    public static class WorkspaceRegion
    {
        public const string ProjectTree = "ProjectTree";
        public const string DocumentHost = "DocumentHost";
        public const string Information = "Information";
        public const string Properties = "Properties";
    }
}
