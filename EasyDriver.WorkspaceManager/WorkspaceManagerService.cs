using EasyDriver.ServiceContainer;
using EasyDriver.ServicePlugin;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace EasyDriver.WorkspaceManager
{
    public class WorkspaceManagerService : EasyServicePlugin, IWorkspaceManagerService
    {
        #region Members

        public ObservableCollection<IWorkspacePanel> Workspaces { get; protected set; }
        public List<Func<object, IWorkspacePanel>> CreatePanelByContextFuncs { get; set; }
        private bool isBusy;
        public virtual bool IsBusy
        {
            get { return isBusy; }
            protected set
            {
                if (value != isBusy)
                {
                    isBusy = value;
                    RaisePropertyChanged();
                }
            }
        }

        private IWorkspacePanel currentActivePanel;
        public IWorkspacePanel CurrentActivePanel
        {
            get { return currentActivePanel; }
            set
            {
                currentActivePanel = value;
                RaisePropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public WorkspaceManagerService(IServiceContainer serviceContainer) : base(serviceContainer)
        {
            CreatePanelByContextFuncs = new List<Func<object, IWorkspacePanel>>();
            Workspaces = new ObservableCollection<IWorkspacePanel>();
        }

        #endregion

        #region Methods

        #region Hàm quản lý panel

        /// <summary>
        /// Thêm Panel vào danh sách Workspaces và mở Panel nếu cần thiết
        /// </summary>
        /// <param name="panelViewModel"></param>
        /// <param name="addAndOpen"></param>
        public void AddPanel(IWorkspacePanel panelViewModel, bool addAndOpen = true)
        {
            if (panelViewModel != null)
            {
                if (addAndOpen)
                {
                    panelViewModel.Show();
                    panelViewModel.IsActive = addAndOpen;
                }
                if (!Workspaces.Contains(panelViewModel))
                    Workspaces.Add(panelViewModel);
            }
        }

        /// <summary>
        /// Loại bỏ Panel khỏi danh sách Workspaces nếu phù hợp điều kiện truyền vào
        /// </summary>
        /// <param name="predicate"></param>
        public void RemovePanel(Func<IWorkspacePanel, bool> predicate)
        {
            Workspaces.Where(predicate).ToList().ForEach(panel => Workspaces.Remove(panel));
        }

        /// <summary>
        /// Loại bỏ Panel khỏi danh sách Workspaces
        /// </summary>
        /// <param name="panelViewModel"></param>
        /// <returns></returns>
        public bool RemovePanel(IWorkspacePanel panelViewModel)
        {
            return Workspaces.Remove(panelViewModel);
        }

        /// <summary>
        /// Loại bỏ toàn bộ các DocumenIWorkspacePanel trong Workspaces
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
        public bool ContainPanel(IWorkspacePanel panelViewModel)
        {
            return Workspaces.Contains(panelViewModel);
        }


        /// <summary>
        /// Hàm tìm Panel bằng Context
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public IWorkspacePanel FindPanelByContext(object context)
        {
            return Workspaces.FirstOrDefault(panel => panel.WorkspaceContext == context);
        }

        /// <summary>
        /// Hàm lấy tất cả các DocumenIWorkspacePanel
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IWorkspacePanel> GetAllDocumentPanel()
        {
            return Workspaces.Where(panel => panel.IsDocument);
        }


        /// <summary>
        /// Hàm lấy tất cả các WorkspacePanel
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IWorkspacePanel> GetAllWorkspacePanel()
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
        /// Đóng tất cả các DocumenIWorkspacePanel
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
            FindPanelByContext(token)?.Hide();
        }

        /// <summary>
        /// Đóng Panel được truyền vào
        /// </summary>
        /// <param name="panelViewModel"></param>
        public void ClosePanel(IWorkspacePanel panelViewModel)
        {
            panelViewModel?.Hide();
        }

        /// <summary>
        /// Tìm Panel nào đang được Active
        /// </summary>
        /// <returns></returns>
        public IWorkspacePanel GetCurrentActivePanel()
        {
            return Workspaces.FirstOrDefault(panel => panel.IsActive);
        }

        /// <summary>
        /// Tìm Panel bằng Context truyền vào và mở nó
        /// </summary>
        /// <param name="context"></param>
        /// <param name="activeOnOpen"></param>
        public void OpenPanel(object context, bool activeOnOpen = true, bool createIfNotExists = false, object parentViewModel = null)
        {
            IWorkspacePanel panel = FindPanelByContext(context);
            if (panel != null)
            {
                panel.Show();
                panel.IsActive = activeOnOpen;
            }
            else if (panel == null && createIfNotExists)
            {
                AddPanel(CreatePanelByWorkspaceContext(context), activeOnOpen);
            }

            panel = FindPanelByContext(context);
            if (panel != null && parentViewModel != null)
                panel.ParentViewModel = parentViewModel;
        }

        /// <summary>
        /// Mở Panel được truyền vào
        /// </summary>
        /// <param name="panelViewModel"></param>
        /// <param name="activeOnOpen"></param>
        public void OpenPanel(IWorkspacePanel panelViewModel, bool activeOnOpen = true)
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
        public void TogglePanel(object context, bool createIfNotExists = false)
        {
            IWorkspacePanel panel = FindPanelByContext(context);
            if (panel == null)
                AddPanel(CreatePanelByWorkspaceContext(context));
            else
                panel.IsClosed ^= true;
        }

        /// <summary>
        /// Đóng Panel nếu nó đang mở và ngược lại
        /// </summary>
        /// <param name="panelViewModel"></param>
        public void TogglePanel(IWorkspacePanel panelViewModel)
        {
            if (panelViewModel != null)
                panelViewModel.IsClosed ^= true;
        }

        /// <summary>
        /// Hàm tạo Panel bằng Context
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public IWorkspacePanel CreatePanelByWorkspaceContext(object context)
        {
            if (CreatePanelByContextFuncs == null)
                return default(IWorkspacePanel);
            if (CreatePanelByContextFuncs.Count == 0)
                return default(IWorkspacePanel);
            foreach (var func in CreatePanelByContextFuncs)
            {
                var panel = func(context);
                if (panel != null)
                    panel.WorkspaceContext = context;
                return panel;
            }
            return default(IWorkspacePanel);
        }

        #endregion

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
        }

        public void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
