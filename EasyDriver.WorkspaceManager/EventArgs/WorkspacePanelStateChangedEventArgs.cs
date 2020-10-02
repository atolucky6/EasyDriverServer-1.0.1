using System;

namespace EasyDriver.WorkspaceManager
{
    public class WorkspacePanelStateChangedEventArgs : EventArgs
    {
        public WorkspacePanelState State { get; private set; }

        public WorkspacePanelStateChangedEventArgs(WorkspacePanelState state)
        {
            State = state;
        }
    }

    public enum WorkspacePanelState
    {
        Active,
        UnActive,
        Opened,
        Closed
    }
}
