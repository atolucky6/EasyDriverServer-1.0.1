using DevExpress.Xpf.Docking;
using EasyDriver.LayoutManager;
using EasyDriver.WorkspaceManager;
using EasyDriverServer.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EasyDriverServer.View
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public DockLayoutManager DockLayoutManager { get => dockContainer; }

        public MainView()
        {
            InitializeComponent();
            Loaded += MainView_Loaded;
        }

        public override void EndInit()
        {
            base.EndInit();
            LayoutGroup root = dockContainer.GetItem("root") as LayoutGroup;
            var layouts = dockContainer.GetItems().Where(x => x is LayoutGroup && x != root && !string.IsNullOrEmpty(x.Name)).ToList();
            if (root != null)
            {
                List<IWorkspacePanel> newWorkspacePanels = new List<IWorkspacePanel>();
                foreach (var workspacePanel in IoC.Instance.Get<IWorkspaceManagerService>().Workspaces)
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
                newWorkspacePanels.ForEach(x => IoC.Instance.Get<IWorkspaceManagerService>().Workspaces.Remove(x));
                newWorkspacePanels.ForEach(x => IoC.Instance.Get<IWorkspaceManagerService>().Workspaces.Add(x));
            }
        }

        private void MainView_Loaded(object sender, RoutedEventArgs args)
        {
            ILayoutManagerService layoutManagerService = IoC.Instance.Get<ILayoutManagerService>();
            layoutManagerService.DockLayoutManager = DockLayoutManager;
            Application.Current.MainWindow.Closing += (s, e) =>
            {
                layoutManagerService.UpdateLastLayout();
            };
            Application.Current.MainWindow.Loaded += (s, e) =>
            {
                layoutManagerService.UpdateDefaultLayout();
                layoutManagerService.RestoreLastLayout();
            };
        }
    }
}
