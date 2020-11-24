using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.WorkspaceManager
{
    /// <summary>
    /// Trạng thái của <see cref="IWorkspacePanel"/>
    /// </summary>
    public enum WorkspacePanelState
    {
        /// <summary>
        /// Đang được chọn trên view
        /// </summary>
        Active,
        /// <summary>
        /// Không được chọn trên view
        /// </summary>
        UnActive,
        /// <summary>
        /// Đã mở
        /// </summary>
        Opened,
        /// <summary>
        /// Đã đóng
        /// </summary>
        Closed
    }
}
