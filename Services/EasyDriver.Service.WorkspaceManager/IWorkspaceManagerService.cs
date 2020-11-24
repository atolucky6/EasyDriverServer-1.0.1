using EasyDriver.ServicePlugin;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EasyScada.WorkspaceManager
{
    /// <summary>
    /// Đối tượng quản lý các <see cref="IWorkspacePanel"/> và cung cấp các hàm quản lý, tìm kiếm...
    /// </summary>
    public interface IWorkspaceManagerService : IEasyServicePlugin, INotifyPropertyChanged
    {
        /// <summary>
        /// Danh sách chứa các <see cref="IWorkspacePanel"/>
        /// </summary>
        ObservableCollection<IWorkspacePanel> Workspaces { get; }

        /// <summary>
        /// <see cref="IWorkspacePanel"/> đang được chọn 
        /// </summary>
        IWorkspacePanel CurrentActivePanel { get; set; }

        /// <summary>
        /// Thêm một <see cref="IWorkspacePanel"/>
        /// </summary>
        /// <param name="panelViewModel">Đối tượng thêm vào</param>
        /// <param name="addAndOpen">Xác định rằng có mở <see cref="IWorkspacePanel"/> sau khi thêm vào hay không</param>
        void AddPanel(IWorkspacePanel panelViewModel, bool addAndOpen = true);

        /// <summary>
        /// Xóa <see cref="IWorkspacePanel"/> thôngt qua điều kiện truyền vào
        /// </summary>
        /// <param name="predicate">Điều kiện cần để xóa</param>
        void RemovePanel(Func<IWorkspacePanel, bool> predicate);

        /// <summary>
        /// Xóa tất cả các <see cref="IWorkspacePanel"/> được đánh dấu là document
        /// </summary>
        void RemoveAllDocumentPanel();

        /// <summary>
        /// Xóa tất cả các <see cref="IWorkspacePanel"/>
        /// </summary>
        void Clear();

        /// <summary>
        /// Xóa <see cref="IWorkspacePanel"/>
        /// </summary>
        /// <param name="panelViewModel">Đối tượng cần xóa</param>
        /// <returns></returns>
        bool RemovePanel(IWorkspacePanel panelViewModel);

        /// <summary>
        /// Kiểm tra đối tượng có tồn tại trong danh sách quản lý hay không
        /// </summary>
        /// <param name="panelViewModel">Đối tượng cần kiểm tra</param>
        /// <returns>Trả về true nếu nó tồn tại và ngược lại</returns>
        bool ContainPanel(IWorkspacePanel panelViewModel);

        /// <summary>
        /// Hàm tìm <see cref="IWorkspacePanel"/> thông qua Context của nó
        /// </summary>
        /// <param name="context">Context của <see cref="IWorkspacePanel"/></param>
        /// <returns>Trả về <see cref="IWorkspacePanel"/> nếu tồn tại hoặc null nếu không tồn tại</returns>
        IWorkspacePanel FindPanelByContext(object context);

        /// <summary>
        /// Trả về <see cref="IWorkspacePanel"/> đang được chọn
        /// </summary>
        /// <returns>Trả về <see cref="IWorkspacePanel"/> nếu tồn tại hoặc null nếu không tồn tại</returns>
        IWorkspacePanel GetCurrentActivePanel();

        /// <summary>
        /// Lấy tất cả các <see cref="IWorkspacePanel"/> được đánh dấu là một workspace nghĩa là IsDocument = false
        /// </summary>
        /// <returns>Danh sách <see cref="IWorkspacePanel"/></returns>
        IEnumerable<IWorkspacePanel> GetAllWorkspacePanel();

        /// <summary>
        /// Lấy tất cả các <see cref="IWorkspacePanel"/> được đánh dấu là một document nghĩa là IsDocument = true
        /// </summary>
        /// <returns>Danh sách <see cref="IWorkspacePanel"/></returns>
        IEnumerable<IWorkspacePanel> GetAllDocumentPanel();

        /// <summary>
        /// Làm cho tất cả <see cref="IWorkspacePanel"/> vào chế độ tự động ẩn
        /// </summary>
        void AutoHideAll();

        /// <summary>
        /// Đóng tất cả các <see cref="IWorkspacePanel"/>
        /// </summary>
        void CloseAllDocumentPanel();

        /// <summary>
        /// Đóng <see cref="IWorkspacePanel"/> nếu context truyền vào bằng với context của workspace panel
        /// </summary>
        /// <param name="context">Context truyền vào</param>
        void ClosePanel(object context);

        /// <summary>
        /// Đóng <see cref="IWorkspacePanel"/> truyền vào 
        /// </summary>
        /// <param name="panelViewModel">Đối tượng cần đóng</param>
        void ClosePanel(IWorkspacePanel panelViewModel);

        /// <summary>
        /// Mở <see cref="IWorkspacePanel"/> nếu context truyền vào bằng với context của workspace panel
        /// </summary>
        /// <param name="context">Context truyền vào</param>
        /// <param name="activeOnOpen">Cho phép chọn vào sau khi mở</param>
        /// <param name="createIfNotExists">Cho phép tạo mới nếu panel không tồn tại</param>
        /// <param name="parentViewModel">ViewModel cha của <see cref="IWorkspacePanel"/></param>
        void OpenPanel(object context, bool activeOnOpen = true, bool createIfNotExists = false, object parentViewModel = null);

        /// <summary>
        /// Mở <see cref="IWorkspacePanel"/> truyền vào
        /// </summary>
        /// <param name="panelViewModel"><see cref="IWorkspacePanel"/> cần mở</param>
        /// <param name="activeOnOpen">Cho phép chọn vào sau khi mở</param>
        void OpenPanel(IWorkspacePanel panelViewModel, bool activeOnOpen = true);

        /// <summary>
        /// Đảo trạng thái đóng mở của <see cref="IWorkspacePanel"/>  nếu context truyền vào bằng với context của workspace panel
        /// </summary>
        /// <param name="context">Context của <see cref="IWorkspacePanel"/></param>
        /// <param name="createIfNotExists">Cho phép tạo mới nếu panel không tồn tại</param>
        void TogglePanel(object context, bool createIfNotExists = false);

        /// <summary>
        /// Đảo trạng thái đóng mở của <see cref="IWorkspacePanel"/> xác định
        /// </summary>
        /// <param name="panelViewModel">Đối tượng cần xử lý</param>
        void TogglePanel(IWorkspacePanel panelViewModel);

        /// <summary>
        /// Thêm vào một định nghĩa hàm tạo <see cref="IWorkspacePanel"/> thông qua context
        /// </summary>
        /// <param name="createPanelFunc"></param>
        void AddCreatePanelFunc(Func<object, IWorkspacePanel> createPanelFunc);

        /// <summary>
        /// Tạo một <see cref="IWorkspacePanel"/> thông qua context
        /// </summary>
        /// <param name="context">Context của <see cref="IWorkspacePanel"/></param>
        /// <returns></returns>
        IWorkspacePanel CreatePanelByWorkspaceContext(object context);
    }
}
