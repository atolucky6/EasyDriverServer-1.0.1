using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EasyScada.ServerApplication.Workspace
{
    public class WorkspaceManagerService<TPanel> : IWorkspaceManagerService<TPanel>
        where TPanel : IWorkspacePanel
    {
        #region Members

        public ObservableCollection<TPanel> Workspaces { get; protected set; }
        protected Func<object, TPanel> CreatePanelByTokenFunc { get; set; }
        public virtual bool IsBusy { get; protected set; }
        public TPanel CurrentActivePanel { get; set; }

        #endregion

        #region Constructors

        public WorkspaceManagerService(Func<object, TPanel> createPanelByTokenFunc = null)
        {
            CreatePanelByTokenFunc = createPanelByTokenFunc;
            Workspaces = new ObservableCollection<TPanel>();
        }

        #endregion

        #region Methods

        #region Hàm quản lý panel

        /// <summary>
        /// Thêm Panel vào danh sách Workspaces và mở Panel nếu cần thiết
        /// </summary>
        /// <param name="panelViewModel"></param>
        /// <param name="addAndOpen"></param>
        public void AddPanel(TPanel panelViewModel, bool addAndOpen = true)
        {
            if (panelViewModel != null)
            {
                if (!Workspaces.Contains(panelViewModel))
                    Workspaces.Add(panelViewModel);
                if (addAndOpen)
                {
                    panelViewModel.Show();
                    panelViewModel.IsActive = addAndOpen;
                }
            }
        }

        /// <summary>
        /// Loại bỏ Panel khỏi danh sách Workspaces nếu phù hợp điều kiện truyền vào
        /// </summary>
        /// <param name="predicate"></param>
        public void RemovePanel(Func<TPanel, bool> predicate)
        {
            Workspaces.Where(predicate).ToList().ForEach(panel => Workspaces.Remove(panel));
        }

        /// <summary>
        /// Loại bỏ Panel khỏi danh sách Workspaces
        /// </summary>
        /// <param name="panelViewModel"></param>
        /// <returns></returns>
        public bool RemovePanel(TPanel panelViewModel)
        {
            return Workspaces.Remove(panelViewModel);
        }

        /// <summary>
        /// Loại bỏ toàn bộ các DocumentPanel trong Workspaces
        /// </summary>
        public void RemoveAllDocumentPanel()
        {
            RemovePanel(panel => panel.IsDocument);
        }

        /// <summary>
        /// Xóa toàn bộ các Panel trong Workspaces
        /// </summary>
        public void Clear()
        {
            Workspaces.Clear();
        }

        #endregion

        #region Hàm tìm kiếm panel

        /// <summary>
        /// Hàm kiểm tra Panel có trong Workspaces hay không
        /// </summary>
        /// <param name="panelViewModel"></param>
        /// <returns></returns>
        public bool ContainPanel(TPanel panelViewModel)
        {
            return Workspaces.Contains(panelViewModel);
        }


        /// <summary>
        /// Hàm tìm Panel bằng Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public TPanel FindPanelByToken(object token)
        {
            return Workspaces.FirstOrDefault(panel => panel.Token == token);
        }

        /// <summary>
        /// Hàm lấy tất cả các DocumentPanel
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TPanel> GetAllDocumentPanel()
        {
            return Workspaces.Where(panel => panel.IsDocument);
        }


        /// <summary>
        /// Hàm lấy tất cả các WorkspacePanel
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TPanel> GetAllWorkspacePanel()
        {
            return Workspaces.Where(panel => !panel.IsDocument);
        }

        #endregion

        #region Hàm điều khiển panel

        /// <summary>
        /// Set tự động ẩn cho tất cả WorkspacePanel
        /// </summary>
        public void AutoHideAll()
        {
            GetAllWorkspacePanel().ToList().ForEach(panel => panel.AutoHidden = true);
        }

        /// <summary>
        /// Đóng tất cả các DocumentPanel
        /// </summary>
        public void CloseAllDocumentPanel()
        {
            GetAllDocumentPanel().ToList().ForEach(panel => panel.Hide());
        }

        /// <summary>
        /// Tìm Panel bằng Token truyền vào và đóng nó
        /// </summary>
        /// <param name="token"></param>
        public void ClosePanel(object token)
        {
            FindPanelByToken(token)?.Hide();
        }

        /// <summary>
        /// Đóng Panel được truyền vào
        /// </summary>
        /// <param name="panelViewModel"></param>
        public void ClosePanel(TPanel panelViewModel)
        {
            panelViewModel?.Hide();
        }

        /// <summary>
        /// Tìm Panel nào đang được Active
        /// </summary>
        /// <returns></returns>
        public TPanel GetCurrentActivePanel()
        {
            return Workspaces.FirstOrDefault(panel => panel.IsActive);
        }

        /// <summary>
        /// Tìm Panel bằng Token truyền vào và mở nó
        /// </summary>
        /// <param name="token"></param>
        /// <param name="activeOnOpen"></param>
        public void OpenPanel(object token, bool activeOnOpen = true, bool createIfNotExists = false, object parentViewModel = null)
        {
            TPanel panel = FindPanelByToken(token);
            if (panel != null)
            {
                panel.Show();
                panel.IsActive = activeOnOpen;
            }
            else if (panel == null && createIfNotExists)
            {
                AddPanel(CreatePanelByToken(token), activeOnOpen);
            }

            panel = FindPanelByToken(token);
            if (panel != null && parentViewModel != null)
                panel.ParentViewModel = parentViewModel;
        }

        /// <summary>
        /// Mở Panel được truyền vào
        /// </summary>
        /// <param name="panelViewModel"></param>
        /// <param name="activeOnOpen"></param>
        public void OpenPanel(TPanel panelViewModel, bool activeOnOpen = true)
        {
            if (panelViewModel != null)
            {
                panelViewModel.Show();
                panelViewModel.IsActive = activeOnOpen;
            }
        }

        /// <summary>
        /// Tìm Panel bằng Token truyền vào và đóng nó nếu đang mở vào ngược lại
        /// </summary>
        /// <param name="token"></param>
        public void TogglePanel(object token, bool createIfNotExists = false)
        {
            TPanel panel = FindPanelByToken(token);
            if (panel == null)
                AddPanel(CreatePanelByToken(token));
            else
                panel.IsClosed ^= true;
        }

        /// <summary>
        /// Đóng Panel nếu nó đang mở và ngược lại
        /// </summary>
        /// <param name="panelViewModel"></param>
        public void TogglePanel(TPanel panelViewModel)
        {
            if (panelViewModel != null)
                panelViewModel.IsClosed ^= true;
        }

        /// <summary>
        /// Hàm tạo Panel bằng Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public TPanel CreatePanelByToken(object token)
        {
            if (CreatePanelByTokenFunc == null)
                return default(TPanel);
            var panel = CreatePanelByTokenFunc(token);
            if (panel != null)
                panel.Token = token;
            return panel;
        }

        #endregion

        #endregion
    }
}
