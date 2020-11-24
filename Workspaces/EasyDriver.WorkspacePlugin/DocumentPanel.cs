using EasyScada.WorkspaceManager;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyDriver.WorkspacePlugin
{
    public abstract class DocumentPanel : WorkspacePanelBase
    {
        public DocumentPanel(
            object context, 
            IWorkspaceManagerService workspaceManagerService) : base(context, workspaceManagerService)
        {
            IsDocument = true;
            IsClosed = false;
        }
    }
}
