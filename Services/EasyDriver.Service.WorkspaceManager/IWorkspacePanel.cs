using System;
using System.ComponentModel;

namespace EasyScada.WorkspaceManager
{
    /// <summary>
    /// Đối tượng đại diện cho 
    /// </summary>
    public interface IWorkspacePanel : ISupportInitialize
    {
        /// <summary>
        /// Đường dẫn đến template cho workspace này
        /// </summary>
        Uri UriTemplate { get; }

        /// <summary>
        /// Tiêu đề
        /// </summary>
        string Caption { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string TargetName { get; set; }

        /// <summary>
        /// Xác định xem panel này là document hay workspace
        /// </summary>
        bool IsDocument { get; }

        /// <summary>
        /// Tự động ẩn khi không kích hoạt
        /// </summary>
        bool AutoHidden { get; set; }

        /// <summary>
        /// Đang kích hoạt
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        /// Đang đóng
        /// </summary>
        bool IsClosed { get; set; }

        /// <summary>
        /// Đang mở
        /// </summary>
        bool IsOpened { get; }

        /// <summary>
        /// Đại diện cho icon của panel
        /// </summary>
        object Glyph { get; }

        /// <summary>
        /// Trạng thái của panel này
        /// </summary>
        WorkspacePanelState State { get; set; }

        /// <summary>
        /// Context của workspace 
        /// </summary>
        object WorkspaceContext { get; set; }

        /// <summary>
        /// ViewModel cha của panel
        /// </summary>
        object ParentViewModel { get; set; }

        /// <summary>
        /// Thực hiện ẩn panel
        /// </summary>
        void Hide();

        /// <summary>
        /// Thực hiện hiển thị panel
        /// </summary>
        void Show();

        /// <summary>
        /// Sự kiện khi trạng thái thay đổi
        /// </summary>
        event EventHandler<WorkspacePanelStateChangedEventArgs> StateChanged;
    }
}
