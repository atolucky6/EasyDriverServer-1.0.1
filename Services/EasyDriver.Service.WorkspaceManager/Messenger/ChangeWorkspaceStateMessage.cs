using System;

namespace EasyScada.WorkspaceManager
{
    /// <summary>
    /// Mẫu tin yêu cầu thay đổi trạng thái của <see cref="IWorkspacePanel"/>
    /// </summary>
    public class ChangeWorkspaceStateMessage
    {
        /// <summary>
        /// Điều kiện để chọn <see cref="IWorkspacePanel"/>
        /// </summary>
        public Func<IWorkspacePanel, bool> Condition { get; private set; }

        /// <summary>
        /// Trạng thái cần chuyển 
        /// </summary>
        public WorkspacePanelState State { get; private set; }

        /// <summary>
        /// Mẫu tin yêu cầu thay đổi trạng thái của <see cref="IWorkspacePanel"/>
        /// </summary>
        /// <param name="state">Trạng thái cần chuyển</param>
        /// <param name="condition">Điều kiện để chọn <see cref="IWorkspacePanel"/></param>
        public ChangeWorkspaceStateMessage(WorkspacePanelState state, Func<IWorkspacePanel, bool> condition)
        {
            State = state;
            Condition = condition;
        }
    }
}
