using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriver.Core;
using EasyScada.ServerApplication.Workspace;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace EasyScada.ServerApplication
{
    public class MainViewModel
    {
        #region Public members

        /// <summary>
        /// Danh sách các workspace có trong chương trình
        /// </summary>
        public ObservableCollection<IWorkspacePanel> Workspaces => WorkspaceManagerService.Workspaces;

        /// <summary>
        /// Biến xác nhận chương trình đang rảnh hay không
        /// </summary>
        /// <remarks>True nghĩa là đang bận và ngược lại</remarks>
        public virtual bool IsBusy { get; set; }

        /// <summary>
        /// Project đang được xử lý bởi chương trình
        /// </summary>
        public IEasyScadaProject CurrentProject => ProjectManagerService.CurrentProject;

        #endregion

        #region Private members

        protected ProjectTreeWorkspaceViewModel ProjectTreeWorkspace { get; set; }
        protected ItemPropertiesWorkspaceViewModel ItemPropertiesWorkspace { get; set; }

        #endregion

        #region Injected services

        protected IWorkspaceManagerService WorkspaceManagerService { get; set; }
        protected IProjectManagerService ProjectManagerService { get; set; }
        protected IReverseService ReverseService { get; set; }
        protected ApplicationViewModel ApplicationViewModel { get; set; }

        #endregion

        #region UI services

        public IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }
        public IWindowService WindowService { get => this.GetService<IWindowService>(); }
        public ISaveFileDialogService SaveFileDialogService { get => this.GetService<ISaveFileDialogService>(); }
        public IOpenFileDialogService OpenFileDialogService { get => this.GetService<IOpenFileDialogService>(); }
        public IDispatcherService DispatcherService { get => this.GetService<IDispatcherService>(); }
        public ICurrentWindowService CurrentWindowService { get => this.GetService<ICurrentWindowService>(); }

        #endregion

        #region Constructors

        public MainViewModel(
            IWorkspaceManagerService workspaceManagerService,
            IReverseService reverseService,
            IProjectManagerService projectManagerService,
            ApplicationViewModel applicationViewModel)
        {
            WorkspaceManagerService = workspaceManagerService;
            ReverseService = reverseService;
            ProjectManagerService = projectManagerService;
            ApplicationViewModel = applicationViewModel;
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Sự kiện khi MainView được Load
        /// </summary>
        public virtual void OnLoaded()
        {
            // Khởi tạo các workspaces
            InitializeWorkspaces();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Hàm dùng để khởi tạo nhựng Workspace có trong chương trình. 
        /// </summary>
        /// <remarks>Hàm chỉ chạy một lần duy nhất khi chương trình hoạt động</remarks>
        private void InitializeWorkspaces()
        {
            // Khởi tạo ProjectTree workspace
            ProjectTreeWorkspace = IoC.Instance.GetPOCOViewModel<ProjectTreeWorkspaceViewModel>(this);
            // Khởi tạo Properties workspace
            ItemPropertiesWorkspace = IoC.Instance.GetPOCOViewModel<ItemPropertiesWorkspaceViewModel>(this);
            // Thêm ProjectTree workspace vào WorkspaceManager
            WorkspaceManagerService.AddPanel(ProjectTreeWorkspace, true);
            // Thêm Properties workspace vào WorkspaceManager
            WorkspaceManagerService.AddPanel(ItemPropertiesWorkspace, false);
        }

        #endregion

        #region Commands

        #region File category

        /// <summary>
        /// Lệnh tạo mới project
        /// </summary>
        public void New()
        {
            try
            {
                // Khóa luồng làm việc của chương trình
                IsBusy = true;
                // Mở NewProjectView với ParentViewModel là MainViewModel
                WindowService.Show("NewProjectView", null, this);
            }
            catch (Exception ex)
            {

            }
            // Mở khóa luồng làm việc của chương trình
            finally { IsBusy = false; }
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="New"/>
        /// </summary>
        /// <returns></returns>
        public bool CanNew()
        {
            return !IsBusy; // Đảm bảo là chương trình không bận
        }

        /// <summary>
        /// Lệnh mở project
        /// </summary>
        public void Open()
        {
            try
            {
                // Khóa luồng làm việc của chương trình
                IsBusy = true;
                // Mở OpenProjectView với ParentViewModel là MainViewModel
                WindowService.Show("OpenProjectView", null, this);
            }
            catch (Exception ex)
            {

            }
            // Mở khóa luồng làm việc của chương trình
            finally { IsBusy = false; }
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="Open"/>
        /// </summary>
        /// <returns></returns>
        public bool CanOpen()
        {
            return !IsBusy; // Đảm bảo là chương trình không bận
        }

        /// <summary>
        /// Lệnh lưu project
        /// </summary>
        /// <returns></returns>
        public async Task Save()
        {
            try
            {
                // Khóa luồng làm việc của chương trình
                IsBusy = true;
                // Lưu lại project
                await ProjectManagerService.SaveAsync(CurrentProject);
                // Xóa lịch sử hoạt động của dịch vụ Reverse
                ReverseService.ClearHistory();
            }
            catch (Exception ex)
            {

            }
            // Sau tất cả ta mở khóa luồng làm việc của chương trình
            finally { IsBusy = false; }
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="Save"/>
        /// </summary>
        /// <returns></returns>
        public bool CanSave()
        {
            // Điều kiện là luồng làm việc không bận và chương trình đang mở một project
            return !IsBusy && ProjectManagerService.CurrentProject != null;
        }

        /// <summary>
        /// Lệnh lưu project ở một nơi khác
        /// </summary>
        /// <returns></returns>
        public async Task SaveAs()
        {
            try
            {
                // Khóa luồng làm việc của chương trình
                IsBusy = true;
                // Kiểm tra xem project đang xử lý có sự thay đổi hay không
                if (CurrentProject.HasChanges())
                {
                    // Nếu có thì hỏi người dùng có muốn lưu lại không
                    var mbr = MessageBoxService.ShowMessage(
                        "The current working project has changes. Do you want to save now ?", "Easy Scada", MessageButton.YesNoCancel, MessageIcon.Question);
                    // Nếu người dùng chọn 'Cancel' thì thoát khỏi hàm
                    if (mbr == MessageResult.Cancel)
                        return;
                    // Nếu người dùng chọn 'Yes' thì lưu lại
                    if (mbr == MessageResult.Yes)
                    {
                        // Lưu lại project
                        await ProjectManagerService.SaveAsync(CurrentProject);
                        // Xóa lịch sử hoạt động của dịch vụ Reverse
                        ReverseService.ClearHistory();
                    }
                    // Mở luồng làm việc của chương trình
                    IsBusy = false;
                }

                // Khởi tạo thông tin của SaveFileDialog
                SaveFileDialogService.Title = "Save as...";
                SaveFileDialogService.Filter = "Easy Scada Project (*.esprj)|*.esprj";
                // Mở SaveDialog để lấy thông tin đường dẫn và tên của project mới
                if (SaveFileDialogService.ShowDialog())
                {
                    // Khóa luồng làm việc của chương trình
                    IsBusy = true;
                    // Lấy đường dẫn của project mới
                    string projectPath = SaveFileDialogService.File.GetFullName();
                    // Thay đổi tên của chương trình hiện tại bằng tên của file đã nhập trên SaveDialog
                    CurrentProject.Name = Path.GetFileNameWithoutExtension(projectPath);
                    // Thay đổi đường dẫn của chương trình hiện tại bằng đường dẫn ta đã chọn trên SaveDialog
                    CurrentProject.ProjectPath = projectPath;
                    // Xóa lịch sử hoạt động của dịch vụ Reverse
                    ReverseService.ClearHistory();
                    // Lưu lại project
                    await ProjectManagerService.SaveAsync(CurrentProject);
                }
            }
            catch (Exception ex)
            {

            }
            // Sau tất cả ta mở khóa luồng làm việc của chương trình
            finally { IsBusy = false; }
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="SaveAs"/>
        /// </summary>
        /// <returns></returns>
        public bool CanSaveAs()
        {
            // Điều kiện là luồng làm việc không bận và chương trình đang mở một project
            return !IsBusy && ProjectManagerService.CurrentProject != null;
        }

        /// <summary>
        /// Lệnh thoát chương trình
        /// </summary>
        /// <returns></returns>
        public async Task Close()
        {
            bool needClose = true;
            // Kiểm tra xem có project nào đang được mở hay không
            if (CurrentProject != null)
            {
                // Nếu có thì kiểm tra project đó có sự thay đổi nào khôngs
                if (CurrentProject.HasChanges())
                {
                    // Nếu có thì hỏi người dùng có muốn lưu lại những thay đổi đó không
                    var mbr = MessageBoxService.ShowMessage(
                        "The current working project has changes. Do you want to save now ?", "Easy Scada", MessageButton.YesNoCancel, MessageIcon.Question);

                    // Nếu người dùng chọn 'Yes' thì sẽ lưu lại
                    if (mbr == MessageResult.Yes)
                    {
                        // Khóa luồng làm việc của chương trình 
                        IsBusy = true;
                        // Lưu lại project
                        await ProjectManagerService.SaveAsync(CurrentProject);
                        // Mở luồng làm việc
                        IsBusy = false;
                        // Xác nhận là thoát chương trình
                        ApplicationViewModel.IsMainWindowExit = true;
                        // Tắt chương trình 
                        DispatcherService.BeginInvoke(() => Application.Current.MainWindow.Close());
                        return;
                    }
                    // Nếu người dùng chọn 'No' thì đóng chương trình
                    else if (mbr == MessageResult.No)
                    {
                        IsBusy = true;
                        // Xác nhận là thoát chương trình
                        ApplicationViewModel.IsMainWindowExit = true;
                        // Tắt chương trình 
                        DispatcherService.BeginInvoke(() => Application.Current.MainWindow.Close());
                        IsBusy = false;
                        return;
                    }
                    else
                    {
                        needClose = false;
                    }
                }
            }

            if (needClose)
            {
                var mbr = MessageBoxService.ShowMessage("Do you want to exit Easy Driver Server?", "Easy Driver Server", MessageButton.YesNo, MessageIcon.Question);
                if (mbr == MessageResult.Yes)
                {
                    // Xác nhận là thoát chương trình
                    ApplicationViewModel.IsMainWindowExit = true;
                    // Tắt chương trình 
                    DispatcherService.BeginInvoke(() => Application.Current.MainWindow.Close());
                }
            }
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="Close"/>
        /// </summary>
        /// <returns></returns>
        public bool CanClose()
        {
            return !IsBusy; // Đảm bảo luồng làm việc chương trình không bận
        }

        #endregion

        #region Edit category

        /// <summary>
        /// Lệnh thêm <see cref="RemoteStation"/> vào <see cref="IEasyScadaProject"/>
        /// </summary>
        public void AddStation()
        {
            // Gọi lệnh AddStation ở ProjectTreeWorkspace
            ProjectTreeWorkspace.AddStation();
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="AddStation"/>
        /// </summary>
        /// <returns></returns>
        public bool CanAddStation()
        {
            // Đảm bảo rằng ProjectTreeWorkspace không null và có thể thêm Station
            return ProjectTreeWorkspace != null && ProjectTreeWorkspace.CanAddStation();
        }

        /// <summary>
        /// Lệnh thêm <see cref="IChannel"/> vào <see cref="IStation"/>
        /// </summary>
        public void AddChannel()
        {
            // Gọi lệnh AddChannel ở ProjectTreeWorkspace
            ProjectTreeWorkspace.AddChannel();
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="AddChannel"/>
        /// </summary>
        /// <returns></returns>
        public bool CanAddChannel()
        {
            // Đảm bảo rằng ProjectTreeWorkspace không null và có thể thêm Channel
            return ProjectTreeWorkspace != null && ProjectTreeWorkspace.CanAddChannel();
        }

        /// <summary>
        /// Lệnh thêm <see cref="IDevice"/> vào <see cref="IChannel"/>
        /// </summary>
        public void AddDevice()
        {
            // Gọi lệnh AddDevice ở ProjectTreeWorkspace
            ProjectTreeWorkspace.AddDevice();
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="AddDevice"/>
        /// </summary>
        /// <returns></returns>
        public bool CanAddDevice()
        {
            // Đảm bảo rằng ProjectTreeWorkspace không null và có thể thêm Device
            return ProjectTreeWorkspace != null && ProjectTreeWorkspace.CanAddDevice();
        }

        /// <summary>
        /// Lệnh thêm <see cref="ITag"/> vào <see cref="IDevice"/>
        /// </summary>
        public void AddTag()
        {
            // Đảm bảo ActivePanel là một TagCollectionDocument
            if (WorkspaceManagerService.CurrentActivePanel is TagCollectionViewModel tagCollection)
                tagCollection.Add(); // Gọi hàm thêm tag của Panel đó
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="AddTag"/>
        /// </summary>
        /// <returns></returns>
        public bool CanAddTag()
        {
            // Điều kiện để có thể thêm Tag là có một ActivePanel là TagCollectionDocument
            // và Panel đó phải cho phép thêm Tag
            if (WorkspaceManagerService.CurrentActivePanel is TagCollectionViewModel tagCollection)
                return tagCollection.CanAdd();
            return false;
        }

        /// <summary>
        /// Lệnh hoàn tác việc đã làm
        /// </summary>
        public void Undo()
        {
            try
            {
                IsBusy = true;
                ReverseService.Undo();
            }
            catch { }
            finally { IsBusy = false; }
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="Undo"/>
        /// </summary>
        /// <returns></returns>
        public bool CanUndo()
        {
            return !IsBusy && ReverseService.CanUndo();
        }

        /// <summary>
        /// Lệnh làm lại việc đã hoàn tác
        /// </summary>
        public void Redo()
        {
            try
            {
                IsBusy = true;
                ReverseService.Redo();
            }
            catch { }
            finally { IsBusy = false; }
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="Redo"/>
        /// </summary>
        /// <returns></returns>
        public bool CanRedo()
        {
            return !IsBusy && ReverseService.CanRedo();
        }

        /// <summary>
        /// Lệnh copy một hoặc nhiệu đối tượng
        /// </summary>
        public void Copy()
        {
            // Kiểm tra xem có WorkspacePanel đang được chọn hay không
            if (WorkspaceManagerService.CurrentActivePanel != null)
            {
                // Nếu có thì ép sang kiểu ISupportEdit và thực thi lệnh tương ứng
                if (WorkspaceManagerService.CurrentActivePanel is ISupportEdit edit)
                    edit.Copy();
            }
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="Copy"/>
        /// </summary>
        /// <returns></returns>
        public bool CanCopy()
        {
            // Điều kiện để thực thi là có một WorkspacePanel được active
            // và WorkspacePanel đó phải implement ISupportEdit
            // nếu như ISupportEdit đó cho phép thực thi lệnh thì đối tượng này cũng vậy
            if (WorkspaceManagerService.CurrentActivePanel != null)
                if (WorkspaceManagerService.CurrentActivePanel is ISupportEdit edit)
                    return edit.CanCopy();
            return false;
        }

        /// <summary>
        /// Lệnh cut một hoặc nhiều đối tượng
        /// </summary>
        public void Cut()
        {
            // Kiểm tra xem có WorkspacePanel đang được chọn hay không
            if (WorkspaceManagerService.CurrentActivePanel != null)
            {
                // Nếu có thì ép sang kiểu ISupportEdit và thực thi lệnh tương ứng
                if (WorkspaceManagerService.CurrentActivePanel is ISupportEdit edit)
                    edit.Cut();
            }
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="Cut"/>
        /// </summary>
        /// <returns></returns>
        public bool CanCut()
        {
            // Điều kiện để thực thi là có một WorkspacePanel được active
            // và WorkspacePanel đó phải implement ISupportEdit
            // nếu như ISupportEdit đó cho phép thực thi lệnh thì đối tượng này cũng vậy
            if (WorkspaceManagerService.CurrentActivePanel != null)
                if (WorkspaceManagerService.CurrentActivePanel is ISupportEdit edit)
                    return edit.CanCut();
            return false;
        }

        /// <summary>
        /// Lệnh paste một hoặc nhiều đối tượng
        /// </summary>
        public void Paste()
        {
            // Kiểm tra xem có WorkspacePanel đang được chọn hay không
            if (WorkspaceManagerService.CurrentActivePanel != null)
            {
                // Nếu có thì ép sang kiểu ISupportEdit và thực thi lệnh tương ứng
                if (WorkspaceManagerService.CurrentActivePanel is ISupportEdit edit)
                    edit.Paste();
            }
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="Paste"/>
        /// </summary>
        /// <returns></returns>
        public bool CanPaste()
        {
            // Điều kiện để thực thi là có một WorkspacePanel được active
            // và WorkspacePanel đó phải implement ISupportEdit
            // nếu như ISupportEdit đó cho phép thực thi lệnh thì đối tượng này cũng vậy
            if (WorkspaceManagerService.CurrentActivePanel != null)
                if (WorkspaceManagerService.CurrentActivePanel is ISupportEdit edit)
                    return edit.CanPaste();
            return false;
        }

        /// <summary>
        /// Lệnh xóa một hoặc nhiều đối tượng
        /// </summary>
        public void Delete()
        {
            // Kiểm tra xem có WorkspacePanel đang được chọn hay không
            if (WorkspaceManagerService.CurrentActivePanel != null)
            {
                // Nếu có thì ép sang kiểu ISupportEdit và thực thi lệnh tương ứng
                if (WorkspaceManagerService.CurrentActivePanel is ISupportEdit edit)
                    edit.Delete();
            }
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="Delete"/>
        /// </summary>
        /// <returns></returns>
        public bool CanDelete()
        {
            // Điều kiện để thực thi là có một WorkspacePanel được active
            // và WorkspacePanel đó phải implement ISupportEdit
            // nếu như ISupportEdit đó cho phép thực thi lệnh thì đối tượng này cũng vậy
            if (WorkspaceManagerService.CurrentActivePanel != null)
                if (WorkspaceManagerService.CurrentActivePanel is ISupportEdit edit)
                    return edit.CanDelete();
            return false;
        }

        public void OpenProjectPathFolder()
        {
            if (File.Exists(ProjectManagerService.CurrentProject.ProjectPath))
            {
                ProcessStartInfo info = new ProcessStartInfo()
                {
                    Arguments = Path.GetDirectoryName(ProjectManagerService.CurrentProject.ProjectPath),
                    FileName = "explorer.exe"
                };
                Process.Start(info);
            }
            else
            {
                MessageBoxService.ShowMessage($"Directory {ProjectManagerService.CurrentProject.ProjectPath} doesn't exist!", "Easy Driver Server", MessageButton.OK, MessageIcon.Warning);
            }
        }

        public bool CanOpenProjectPathFolder()
        {
            return ProjectManagerService != null && ProjectManagerService.CurrentProject != null;
        }

        #endregion

        #region Tools category

        public void CreateConnectionSchemaFile()
        {
            try
            {
                WindowService.Show("CreateConnectionSchemaView", ProjectManagerService.CurrentProject, this);
            }
            catch (Exception ex)
            {

            }
        }

        public bool CanCreateConnectionSchemaFile()
        {
            return !IsBusy && ProjectManagerService != null && ProjectManagerService.CurrentProject != null;
        }

        public void ShowOptionsView()
        {
            WindowService.Show("OptionsView", null, this);
        }

        public bool CanShowOptionsView()
        {
            return !IsBusy;
        }

        #endregion

        #endregion
    }
}
