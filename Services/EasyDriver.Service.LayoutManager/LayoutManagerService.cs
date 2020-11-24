using DevExpress.Xpf.Docking;
using EasyDriver.ServicePlugin;
using EasyScada.WorkspaceManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace EasyDriver.Service.LayoutManager
{
    /// <summary>
    /// Đối tượng quản lý layout cho application
    /// </summary>
    [Service(100, true)]
    public class LayoutManagerService : EasyServicePluginBase, ILayoutManagerService
    {
        #region Public properties
        public string SaveLayoutDirectory { get; set; }
        public DockLayoutManager DockLayoutManager { get; set; }
        public Window MainWindow { get; set; }
        #endregion

        #region Constructors
        public LayoutManagerService() : base()
        {
            SaveLayoutDirectory = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\Layouts\\";
            if (!Directory.Exists(SaveLayoutDirectory))
                Directory.CreateDirectory(SaveLayoutDirectory);
        }
        #endregion

        #region Public methods
        public void SaveLayout(string layoutName)
        {
            if (!layoutName.EndsWith(".xml"))
                layoutName += ".xml";
            string layoutPath = SaveLayoutDirectory + layoutName;
            DockLayoutManager.SaveLayoutToXml(layoutPath);
        }

        public void RemoveLayout(string layoutName)
        {
            string layoutPath = SaveLayoutDirectory + layoutName;
            if (File.Exists(layoutPath))
                File.Delete(layoutPath);
        }

        public void ResetToDefault()
        {
            ApplyLayout("DefaultLayout.xml");
        }

        public void ApplyLayout(string layoutName)
        {
            if (!layoutName.EndsWith(".xml"))
                layoutName += ".xml";
            string layoutPath = SaveLayoutDirectory + layoutName;
            if (File.Exists(layoutPath))
                DockLayoutManager?.RestoreLayoutFromXml(layoutPath);
        }

        public void RestoreLastLayout()
        {
            ApplyLayout("LastLayout.xml");
        }

        public void UpdateLastLayout()
        {
            string layoutPath = SaveLayoutDirectory + "LastLayout.xml";
            DockLayoutManager.SaveLayoutToXml(layoutPath);
        }

        public void UpdateDefaultLayout()
        {
            string layoutPath = SaveLayoutDirectory + "LastLayout.xml";
            DockLayoutManager.SaveLayoutToXml(layoutPath);
        }

        public IEnumerable<string> GetLayouts()
        {
            foreach (var filePath in Directory.GetFiles(SaveLayoutDirectory, "*.xml"))
            {
                yield return Path.GetFileNameWithoutExtension(filePath);
            }
        }

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            Application.Current.Activated += OnApplicationActivated;
        }
        #endregion

        #region Private methods
        private void OnApplicationActivated(object sender, EventArgs e)
        {
            if (Application.Current.MainWindow != null)
            {
                DockLayoutManager = GetFirstElement<DockLayoutManager>(Application.Current.MainWindow);
                IWorkspaceManagerService workspaceManagerService = ServiceContainer.GetService<IWorkspaceManagerService>();
                if (DockLayoutManager != null)
                {
                    LayoutGroup root = DockLayoutManager.GetItem("root") as LayoutGroup;
                    var layouts = DockLayoutManager.GetItems().Where(x => x is LayoutGroup && x != root && !string.IsNullOrEmpty(x.Name)).ToList();
                    if (root != null)
                    {
                        List<IWorkspacePanel> newWorkspacePanels = new List<IWorkspacePanel>();
                        foreach (var workspacePanel in workspaceManagerService.Workspaces)
                        {
                            if (layouts.FirstOrDefault(x => x.Name == workspacePanel.TargetName) == null)
                            {
                                LayoutGroup group = new LayoutGroup();
                                group.Name = workspacePanel.TargetName;
                                group.DestroyOnClosingChildren = false;
                                GridLengthConverter lengthConverter = new GridLengthConverter();
                                group.ItemWidth = (GridLength)lengthConverter.ConvertFrom(200);
                                root.Add(group);
                                newWorkspacePanels.Add(workspacePanel);
                            }
                        }
                        newWorkspacePanels.ForEach(x => workspaceManagerService.Workspaces.Remove(x));
                        newWorkspacePanels.ForEach(x => workspaceManagerService.Workspaces.Add(x));

                        UpdateDefaultLayout();
                        RestoreLastLayout();
                    }

                    Application.Current.MainWindow.Closing += (o, a) =>
                    {
                        UpdateLastLayout();
                    };

                    Application.Current.Activated -= OnApplicationActivated;
                }
            }
        }

        /// <summary>
        /// Tìm kiếm element đầu tiên trùng với <typeparamref name="T"/> trong con của đối tượng truyền vào
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private T GetFirstElement<T>(DependencyObject obj) where T : DependencyObject
        {
            if (obj is T)
                return (T)obj;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                T result = GetFirstElement<T>(child);
                if (result == null)
                    continue;
                return result;
            }
            return null;
        }
        #endregion
    }
}
