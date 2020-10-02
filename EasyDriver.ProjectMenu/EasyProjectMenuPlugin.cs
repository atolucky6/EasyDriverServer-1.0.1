using DevExpress.Mvvm;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Core;
using EasyDriver.Core;
using EasyDriver.MenuPlugin;
using EasyDriver.ProjectManager;
using EasyDriver.Reversible;
using EasyDriver.SyncContext;
using EasyDriver.WorkspaceManager;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using IServiceContainer = EasyDriver.ServiceContainer.IServiceContainer;

namespace EasyDriver.ProjectMenu
{
    public class EasyProjectMenuPlugin : EasyMenuPlugin
    {
        internal readonly IBarComponent fileToolbar;
        internal readonly IBarComponent fileSubMenu;
        internal readonly IBarComponent newBarItem;
        internal readonly IBarComponent openBarItem;
        internal readonly IBarComponent saveBarItem;
        internal readonly IBarComponent saveAsBarItem;
        internal readonly IBarComponent recentProjectsBarItem;
        internal readonly IBarComponent exitBarItem;

        internal readonly IWorkspaceManagerService workspaceManagerService;
        internal readonly IProjectManagerService projectManagerService;
        internal readonly IApplicationSyncService applicationSyncService;
        internal readonly IReverseService reverseService;

        internal readonly ISaveFileDialogService saveFileDialogService;
        internal readonly IOpenFileDialogService openFileDialogService;
        internal readonly IDispatcherService dispatcherService;

        public ObservableCollection<RecentOpenProjectModel> RecentOpenProjects { get; set; }

        public EasyProjectMenuPlugin(IServiceContainer serviceContainer) : base(serviceContainer)
        {
            this.workspaceManagerService = serviceContainer.Get<IWorkspaceManagerService>();
            this.projectManagerService = serviceContainer.Get<IProjectManagerService>();
            this.applicationSyncService = serviceContainer.Get<IApplicationSyncService>();
            this.reverseService = serviceContainer.Get<IReverseService>();

            saveFileDialogService = new SaveFileDialogService();
            saveFileDialogService.RestoreDirectory = true;
            saveFileDialogService.Filter = "Project file (*.json)|*.json";

            openFileDialogService = new OpenFileDialogService();
            openFileDialogService.RestoreDirectory = true;
            openFileDialogService.Multiselect = false;
            openFileDialogService.Filter = "Project file (*.json)|*.json";

            dispatcherService = new DispatcherService();

            string path = "pack://application:,,,/EasyDriver.ProjectMenu;component/Images/"; ;
            ImageSource newImageSource = new BitmapImage(new Uri(path + "file_48px.png", UriKind.Absolute));
            ImageSource openImageSource = new BitmapImage(new Uri(path + "opened_folder_48px.png", UriKind.Absolute));
            ImageSource saveImageSource = new BitmapImage(new Uri(path + "save_48px.png", UriKind.Absolute));
            ImageSource saveAsImageSource = new BitmapImage(new Uri(path + "save_as_48px.png", UriKind.Absolute));
            ImageSource exitImageSource = new BitmapImage(new Uri(path + "close-window-48.png", UriKind.Absolute));

            fileSubMenu = BarFactory.Default.CreateSubItem("File");
            fileToolbar = BarFactory.Default.CreateToolBar("File");
            newBarItem = BarFactory.Default.CreateButton(
                displayName: "New",
                command: new DelegateCommand(New, CanNew),
                keyGesture: new KeyGesture(Key.N, ModifierKeys.Control),
                imageSource: newImageSource);
            openBarItem = BarFactory.Default.CreateButton(
                displayName: "Open",
                command: new DelegateCommand(Open, CanOpen),
                keyGesture: new KeyGesture(Key.O, ModifierKeys.Control),
                imageSource: openImageSource);
            saveBarItem = BarFactory.Default.CreateButton(
                displayName: "Save",
                command: new DelegateCommand(Save, CanSave),
                keyGesture: new KeyGesture(Key.S, ModifierKeys.Control),
                imageSource: saveImageSource);
            saveAsBarItem = BarFactory.Default.CreateButton(
                displayName: "Save as...",
                command: new DelegateCommand(SaveAs, CanSaveAs),
                keyGesture: null,
                imageSource: saveAsImageSource);
            recentProjectsBarItem = BarFactory.Default.CreateButtonSplitItem(
                displayName: "Recent projects",
                command: new DelegateCommand(OpenRecentProjects, CanOpenRecentProjects));
            exitBarItem = BarFactory.Default.CreateButton(
                displayName: "Exit",
                command: new DelegateCommand(Exit, CanExit),
                keyGesture: new KeyGesture(Key.F4, ModifierKeys.Alt),
                imageSource: exitImageSource);

            fileSubMenu
                .Add(newBarItem)
                .Add(openBarItem)
                .Add(BarFactory.Default.CreateSeparator())
                .Add(saveBarItem)
                .Add(saveAsBarItem)
                .Add(BarFactory.Default.CreateSeparator())
                .Add(recentProjectsBarItem)
                .Add(BarFactory.Default.CreateSeparator())
                .Add(exitBarItem);

            fileToolbar
                .Add(newBarItem)
                .Add(openBarItem)
                .Add(BarFactory.Default.CreateSeparator())
                .Add(saveBarItem);

            RecentOpenProjects = new ObservableCollection<RecentOpenProjectModel>(GetRecentProjects());
            ReloadRecentProjectBarItem();
            RecentOpenProjects.CollectionChanged += RecentOpenProjects_CollectionChanged;
            projectManagerService.ProjectChanged += ProjectManagerService_ProjectChanged;
            projectManagerService.ProjectSaved += ProjectManagerService_ProjectSaved;
        }

        private void ProjectManagerService_ProjectSaved(object sender, ProjectSavedEventArgs e)
        {
            if (e.NewProject != null && !string.IsNullOrWhiteSpace(e.NewProject.ProjectPath))
            {
                //Create project info
                var projectInfo = new RecentOpenProjectModel
                {
                    CreatedDate = File.GetCreationTime(e.NewProject.ProjectPath),
                    ModifiedDate = Directory.GetLastWriteTime(e.NewProject.ProjectPath),
                    Path = e.NewProject.ProjectPath
                };

                if (RecentOpenProjects.FirstOrDefault(x => x.Path == projectInfo.Path) == null)
                    RecentOpenProjects.Add(projectInfo);
            }
        }

        private void ProjectManagerService_ProjectChanged(object sender, ProjectChangedEventArgs e)
        {
            if (e.NewProject != null && !string.IsNullOrWhiteSpace(e.NewProject.ProjectPath))
            {
                //Create project info
                var projectInfo = new RecentOpenProjectModel
                {
                    CreatedDate = File.GetCreationTime(e.NewProject.ProjectPath),
                    ModifiedDate = Directory.GetLastWriteTime(e.NewProject.ProjectPath),
                    Path = e.NewProject.ProjectPath
                };

                if (RecentOpenProjects.FirstOrDefault(x => x.Path == projectInfo.Path) == null)
                    RecentOpenProjects.Add(projectInfo);
            }
        }

        private void RecentOpenProjects_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ReloadRecentProjectBarItem();
        }

        internal void ReloadRecentProjectBarItem()
        {
            recentProjectsBarItem.Clear();
            for (int i = 0; i < RecentOpenProjects.Count; i++)
            {
                RecentOpenProjectModel recentOpenProject = RecentOpenProjects[i];
                IBarComponent barItem = BarFactory.Default.CreateButton(
                    displayName: $"{i + 1} {recentOpenProject.Name} ({recentOpenProject.Path})");

                barItem.SetCommand(new DelegateCommand(() =>
                {
                    Open(recentOpenProject);
                }, CanOpen));
                recentProjectsBarItem.Add(barItem);
            }
        }

        internal async void New()
        {
            //Check the current working project is not null and it has changes
            if (projectManagerService.CurrentProject != null && projectManagerService.CurrentProject.HasChanges() &&
                !string.IsNullOrWhiteSpace(projectManagerService.CurrentProject.ProjectPath))
            {
                //Ask the user want to save the current working project or not
                var mbr = DXMessageBox.Show("The current working project has changes. " +
                    "Do you want to save now ?", applicationSyncService.MessageTitle, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                //If choose cancel just return
                if (mbr == MessageBoxResult.Cancel)
                    return;
                //If choose yes then save the current project before open another project
                if (mbr == MessageBoxResult.Yes)
                {
                    //Block the view
                    applicationSyncService.IsBusy = true;
                    //Save the current working project
                    await projectManagerService.SaveAsync(projectManagerService.CurrentProject);
                }
                projectManagerService.CurrentProject = projectManagerService.CreateProject();
            }
            else
            {
                projectManagerService.CurrentProject = projectManagerService.CreateProject();
            }
        }

        internal bool CanNew()
        {
            return !applicationSyncService.IsBusy;
        }

        internal async void Open()
        {
            try
            {
                //Set title and filter for browse dialog
                openFileDialogService.Title = "Open project";
                //Show browse dialog 
                if (openFileDialogService.ShowDialog())
                {
                    //Get project path
                    string projectPath = openFileDialogService.File.GetFullName();
                    //Create project info
                    var projectInfo = new RecentOpenProjectModel
                    {
                        Pined = false,
                        CreatedDate = File.GetCreationTime(projectPath),
                        ModifiedDate = Directory.GetLastWriteTime(projectPath),
                        Path = projectPath
                    };

                    //Check the current working project is not null and it has changes
                    if (projectManagerService.CurrentProject != null && projectManagerService.CurrentProject.HasChanges())
                    {
                        //Ask the user want to save the current working project or not
                        var mbr = DXMessageBox.Show("The current working project has changes. " +
                            "Do you want to save now ?", applicationSyncService.MessageTitle, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                        //If choose cancel just return
                        if (mbr == MessageBoxResult.Yes)
                            return;
                        //If choose yes then save the current project before open another project
                        if (mbr == MessageBoxResult.Yes)
                        {
                            //Block the view
                            applicationSyncService.IsBusy = true;
                            //Save the current working project
                            await projectManagerService.SaveAsync(projectManagerService.CurrentProject);
                        }
                    }

                    //Block the view
                    applicationSyncService.IsBusy = true;
                    IEasyScadaProject openedProject = await projectManagerService.OpenProjectAsync(projectPath);
                    if (openedProject != null)
                    {
                        //Add the project info if it not exists
                        if (RecentOpenProjects.FirstOrDefault(x => x.Name == projectInfo.Name && x.Path == projectInfo.Path) == null)
                            RecentOpenProjects.Add(projectInfo);
                        projectManagerService.CurrentProject = openedProject;
                        workspaceManagerService.RemoveAllDocumentPanel();
                        reverseService.ClearHistory();
                    }
                    else
                    {
                        DXMessageBox.Show("Can't open project file.", applicationSyncService.MessageTitle, 
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                //Release the view
                applicationSyncService.IsBusy = false;
            }
        }

        internal async void Open(RecentOpenProjectModel recentOpenProject)
        {
            if (!File.Exists(recentOpenProject.Path))
            {
                DXMessageBox.Show($"The selected project '{recentOpenProject.Name} can't be found under the specified path. " +
                    $"After this message the project will remove from the recent open projects."
                    , applicationSyncService.MessageTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                RecentOpenProjects.Remove(recentOpenProject);
            }
            else
            {
                //Check the current working project is not null and it has changes
                if (projectManagerService.CurrentProject != null && projectManagerService.CurrentProject.HasChanges())
                {
                    //Ask the user want to save the current working project or not
                    var mbr = DXMessageBox.Show("The current working project has changes. " +
                        "Do you want to save now ?", applicationSyncService.MessageTitle, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    //If choose cancel just return
                    if (mbr == MessageBoxResult.Cancel)
                        return;
                    //If choose yes then save the current project before open another project
                    if (mbr == MessageBoxResult.Yes)
                    {
                        //Block the view
                        applicationSyncService.IsBusy = true;
                        //Save the current working project
                        await projectManagerService.SaveAsync(projectManagerService.CurrentProject);
                    }
                }
                applicationSyncService.IsBusy = true;
                IEasyScadaProject openedProject = await projectManagerService.OpenProjectAsync(recentOpenProject.Path);
                if (openedProject != null)
                {
                    projectManagerService.CurrentProject = openedProject;
                    workspaceManagerService.RemoveAllDocumentPanel();
                    reverseService.ClearHistory();
                }
                else
                {
                    DXMessageBox.Show("Can't open project file.", applicationSyncService.MessageTitle,
                           MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        internal bool CanOpen()
        {
            return !applicationSyncService.IsBusy;
        }

        internal async void Save()
        {
            try
            {
                bool needSave = false;

                if (string.IsNullOrWhiteSpace(projectManagerService.CurrentProject.ProjectPath) ||
                    !File.Exists(projectManagerService.CurrentProject.ProjectPath))
                {
                    saveFileDialogService.Title = "Save Project";
                    if (saveFileDialogService.ShowDialog())
                    {
                        string projectPath = saveFileDialogService.File.GetFullName();
                        projectManagerService.CurrentProject.ProjectPath = projectPath;
                        projectManagerService.CurrentProject.Name = Path.GetFileNameWithoutExtension(projectPath);
                    }
                }
                else
                {
                    needSave = true;
                }

                if (needSave)
                {
                    // Khóa luồng làm việc của chương trình
                    applicationSyncService.IsBusy = true;
                    // Lưu lại project
                    await projectManagerService.SaveAsync(projectManagerService.CurrentProject);
                    // Xóa lịch sử hoạt động của dịch vụ Reverse
                    reverseService.ClearHistory();
                }
            }
            catch (Exception)
            {

            }
            // Sau tất cả ta mở khóa luồng làm việc của chương trình
            finally { applicationSyncService.IsBusy = false; }
        }

        internal bool CanSave()
        {
            return !applicationSyncService.IsBusy && projectManagerService.CurrentProject != null;
        }

        internal async void SaveAs()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(projectManagerService.CurrentProject.ProjectPath) ||
                    !File.Exists(projectManagerService.CurrentProject.ProjectPath))
                {
                    Save();
                }
                else
                {
                    // Khóa luồng làm việc của chương trình
                    applicationSyncService.IsBusy = true;
                    // Kiểm tra xem project đang xử lý có sự thay đổi hay không
                    if (projectManagerService.CurrentProject.HasChanges())
                    {
                        // Nếu có thì hỏi người dùng có muốn lưu lại không
                        var mbr = DXMessageBox.Show(
                            "The current working project has changes. Do you want to save now ?", applicationSyncService.MessageTitle, 
                            MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                        // Nếu người dùng chọn 'Cancel' thì thoát khỏi hàm
                        if (mbr == MessageBoxResult.Cancel)
                            return;
                        // Nếu người dùng chọn 'Yes' thì lưu lại
                        if (mbr == MessageBoxResult.Yes)
                        {
                            // Lưu lại project
                            await projectManagerService.SaveAsync(projectManagerService.CurrentProject);
                            // Xóa lịch sử hoạt động của dịch vụ Reverse
                            reverseService.ClearHistory();
                        }
                        // Mở luồng làm việc của chương trình
                        applicationSyncService.IsBusy = false;
                    }

                    // Khởi tạo thông tin của SaveFileDialog
                    saveFileDialogService.Title = "Save as...";
                    // Mở SaveDialog để lấy thông tin đường dẫn và tên của project mới
                    if (saveFileDialogService.ShowDialog())
                    {
                        // Khóa luồng làm việc của chương trình
                        applicationSyncService.IsBusy = true;
                        // Lấy đường dẫn của project mới
                        string projectPath = saveFileDialogService.File.GetFullName();
                        // Thay đổi tên của chương trình hiện tại bằng tên của file đã nhập trên SaveDialog
                        projectManagerService.CurrentProject.Name = Path.GetFileNameWithoutExtension(projectPath);
                        // Thay đổi đường dẫn của chương trình hiện tại bằng đường dẫn ta đã chọn trên SaveDialog
                        projectManagerService.CurrentProject.ProjectPath = projectPath;
                        // Xóa lịch sử hoạt động của dịch vụ Reverse
                        reverseService.ClearHistory();
                        // Lưu lại project
                        await projectManagerService.SaveAsync(projectManagerService.CurrentProject);
                    }
                }
            }
            catch (Exception)
            {

            }
            // Sau tất cả ta mở khóa luồng làm việc của chương trình
            finally { applicationSyncService.IsBusy = false; }
        }

        internal bool CanSaveAs()
        {
            return !applicationSyncService.IsBusy && projectManagerService.CurrentProject != null;
        }

        internal void OpenRecentProjects()
        {
            RecentProjectsView recentProjectsView = new RecentProjectsView(this);
            recentProjectsView.ShowDialog();
        }

        internal bool CanOpenRecentProjects()
        {
            return !applicationSyncService.IsBusy;
        }

        internal async void Exit()
        {
            bool needClose = true;
            // Kiểm tra xem có project nào đang được mở hay không
            if (projectManagerService.CurrentProject != null)
            {
                // Nếu có thì kiểm tra project đó có sự thay đổi nào khôngs
                if (projectManagerService.CurrentProject.HasChanges())
                {
                    // Nếu có thì hỏi người dùng có muốn lưu lại những thay đổi đó không
                    var mbr = DXMessageBox.Show(
                        "The current working project has changes. Do you want to save now ?", applicationSyncService.MessageTitle, 
                        MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                    // Nếu người dùng chọn 'Yes' thì sẽ lưu lại
                    if (mbr == MessageBoxResult.Yes)
                    {
                        // Khóa luồng làm việc của chương trình 
                        applicationSyncService.IsBusy = true;
                        // Lưu lại project
                        await projectManagerService.SaveAsync(projectManagerService.CurrentProject);
                        // Mở luồng làm việc
                        applicationSyncService.IsBusy = false;
                        // Xác nhận là thoát chương trình
                        Application.Current.MainWindow.Tag = true;
                        // Tắt chương trình 
                        dispatcherService.Invoke(() => Application.Current.MainWindow.Close());
                        return;
                    }
                    // Nếu người dùng chọn 'No' thì đóng chương trình
                    else if (mbr == MessageBoxResult.No)
                    {
                        applicationSyncService.IsBusy = true;
                        // Xác nhận là thoát chương trình
                        Application.Current.MainWindow.Tag = true;
                        // Tắt chương trình 
                        dispatcherService.Invoke(() => Application.Current.MainWindow.Close());
                        applicationSyncService.IsBusy = false;
                        return;
                    }
                    else
                    {
                        Application.Current.MainWindow.Tag = null;
                        needClose = false;
                    }
                }
            }

            if (needClose)
            {
                var mbr = DXMessageBox.Show("Do you want to exit Easy Driver Server?", applicationSyncService.MessageTitle, 
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (mbr == MessageBoxResult.Yes)
                {
                    // Xác nhận là thoát chương trình
                    Application.Current.MainWindow.Tag = true;
                    // Tắt chương trình 
                    dispatcherService.Invoke(() => Application.Current.MainWindow.Close());
                }
            }
        }

        internal bool CanExit()
        {
            return !applicationSyncService.IsBusy;
        }

        internal List<RecentOpenProjectModel> GetRecentProjects()
        {
            try
            {
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\RecentProjects.json";
                if (File.Exists(path))
                {
                    string resJson = File.ReadAllText(path);
                    return JsonConvert.DeserializeObject<List<RecentOpenProjectModel>>(resJson);
                }
                return new List<RecentOpenProjectModel>();
            }
            catch { return new List<RecentOpenProjectModel>(); }
        }

        internal void SaveRecentProject()
        {
            try
            {
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\RecentProjects.json";
                File.WriteAllText(path, JsonConvert.SerializeObject(RecentOpenProjects.ToList()));
            }
            catch { }
        }

        public override IEnumerable<IBarComponent> GetExtendApplicationMenuItems(IBarComponent container, object context)
        {
            return new List<IBarComponent>() { fileSubMenu };
        }

        public override IEnumerable<IBarComponent> GetExtendApplicationStatusBarItems(IBarComponent container, object context)
        {
            return null;
        }

        public override IEnumerable<IBarComponent> GetExtendApplicationToolBarItems(IBarComponent container, object context)
        {
            return new List<IBarComponent>() { fileToolbar };
        }
    }
}
