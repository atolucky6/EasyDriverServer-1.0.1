using System;

namespace EasyScada.WorkspaceManager
{
    /// <summary>
    /// Tham số của sự kiện <see cref="WorkspacePanelState"/> của <see cref="IWorkspacePanel"/> thay đổi
    /// </summary>
    public class WorkspacePanelStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Trạng thái trước đó
        /// </summary>
        public WorkspacePanelState OldState { get; private set; }

        /// <summary>
        /// Trạng thái hiện tại
        /// </summary>
        public WorkspacePanelState NewState { get; private set; }

        public WorkspacePanelStateChangedEventArgs(WorkspacePanelState oldState, WorkspacePanelState newState)
        {
            OldState = oldState;
            NewState = newState;
        }
    }
}
