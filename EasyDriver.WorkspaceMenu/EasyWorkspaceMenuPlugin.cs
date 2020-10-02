using DevExpress.Mvvm;
using EasyDriver.MenuPlugin;
using EasyDriver.WorkspaceManager;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using IServiceContainer = EasyDriver.ServiceContainer.IServiceContainer;

namespace EasyDriver.LayoutMenu
{
    public class EasyWorkspaceMenuPlugin : EasyMenuPlugin
    {
        readonly IWorkspaceManagerService workspaceManagerService;
        readonly IBarComponent windowSubMenuItem;
        readonly IBarComponent closeAllBarItemm;
        readonly IBarComponent autoHideAllBarItem;

        private IBarComponent viewSubMenuItem;

        public EasyWorkspaceMenuPlugin(IServiceContainer serviceContainer) : base(serviceContainer)
        {
            this.workspaceManagerService = serviceContainer.Get<IWorkspaceManagerService>();
            windowSubMenuItem = BarFactory.Default.CreateSubItem("Window");
            windowSubMenuItem.Add(BarFactory.Default.CreateSeparator());

            closeAllBarItemm = BarFactory.Default.CreateButton(
                displayName: "Close All Documents",
                command: new DelegateCommand(() =>
                {
                    this.workspaceManagerService.CloseAllDocumentPanel();
                }));

            autoHideAllBarItem = BarFactory.Default.CreateButton(
                displayName: "Auto Hide All",
                command: new DelegateCommand(() =>
                {
                    foreach (var workspace in this.workspaceManagerService.GetAllWorkspacePanel())
                    {
                        workspace.AutoHidden = true;
                    };
                }));

            windowSubMenuItem.Add(closeAllBarItemm).Add(autoHideAllBarItem);
        }

        private void Workspaces_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is IWorkspacePanel workspacePanel)
                    {
                        if (!workspacePanel.IsDocument)
                        {
                            IBarComponent barItem = BarFactory.Default.CreateCheckItem(
                                displayName: workspacePanel.Caption);
                            barItem.SetCommand(new DelegateCommand(() =>
                            {
                                workspacePanel.IsClosed = !workspacePanel.IsClosed;
                            }));
                            barItem.IsChecked = workspacePanel.IsOpened;
                            workspacePanel.StateChanged += (s, args) =>
                            {
                                if (args.State == WorkspacePanelState.Opened)
                                    barItem.IsChecked = true;
                                else if (args.State == WorkspacePanelState.Closed)
                                    barItem.IsChecked = false;
                            };
                            viewSubMenuItem.Add(barItem);
                        }
                    }
                }
            }
        }

        public override IEnumerable<IBarComponent> GetExtendApplicationMenuItems(IBarComponent container, object context)
        {
            if (container != null)
            {
                viewSubMenuItem = container.BarItems.FirstOrDefault(x => x.DisplayName == "View");
                if (viewSubMenuItem != null)
                {
                    foreach (var item in workspaceManagerService.Workspaces)
                    {
                        if (item is IWorkspacePanel workspacePanel)
                        {
                            if (!workspacePanel.IsDocument)
                            {
                                IBarComponent barItem = BarFactory.Default.CreateCheckItem(
                                    displayName: workspacePanel.Caption);
                                barItem.SetCommand(new DelegateCommand(() =>
                                {
                                    workspacePanel.IsClosed = !workspacePanel.IsClosed;
                                    barItem.IsChecked = workspacePanel.IsOpened;
                                }));
                                barItem.IsChecked = workspacePanel.IsOpened;
                                viewSubMenuItem.Add(barItem);
                            }
                        }
                    }
                }
                if (workspaceManagerService != null)
                    workspaceManagerService.Workspaces.CollectionChanged += Workspaces_CollectionChanged;
            }

            return new List<IBarComponent>() { windowSubMenuItem };
        }
    }
}
