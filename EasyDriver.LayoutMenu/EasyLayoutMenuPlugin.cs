using DevExpress.Mvvm;
using EasyDriver.LayoutManager;
using EasyDriver.MenuPlugin;
using EasyDriver.ServiceContainer;
using System.Collections.Generic;
using IServiceContainer = EasyDriver.ServiceContainer.IServiceContainer;

namespace EasyDriver.LayoutMenu
{
    public class EasyLayoutMenuPlugin : EasyMenuPlugin
    {
        readonly ILayoutManagerService layoutManagerService;
        readonly IBarComponent layoutManagerMenuItem;
        readonly IBarComponent applySubItem;
        readonly IBarComponent saveLayoutItem;
        readonly IBarComponent manageLauoutItem;
        readonly IBarComponent resetLayoutItem;

        public EasyLayoutMenuPlugin(IServiceContainer serviceContainer) : base(serviceContainer)
        {
            layoutManagerService = serviceContainer.Get<ILayoutManagerService>();
            layoutManagerMenuItem = BarFactory.Default.CreateSubItem("Window");

            applySubItem = BarFactory.Default.CreateSubItem("Apply Window Layout");

            foreach (var layout in layoutManagerService.GetLayouts())
            {
                IBarComponent layoutBarItem = BarFactory.Default.CreateButton(layout);
                layoutBarItem.SetCommand(new DelegateCommand(() => layoutManagerService.ApplyLayout(layoutBarItem.DisplayName)));
                applySubItem.Add(layoutBarItem);
            }

            saveLayoutItem = BarFactory.Default.CreateButton(
                displayName: "Save Window Layout",
                command: new DelegateCommand(() =>
                {
                    LayoutWindow layoutWindow = new LayoutWindow(layoutManagerService, applySubItem);
                    layoutWindow.ShowDialog();
                }));

            manageLauoutItem = BarFactory.Default.CreateButton(
                displayName: "Manage Window Layout",
                command: new DelegateCommand(() =>
                {
                    ManageLayoutWindow manageLayoutWindow = new ManageLayoutWindow(applySubItem, layoutManagerService);
                    manageLayoutWindow.ShowDialog();
                }));

            resetLayoutItem = BarFactory.Default.CreateButton(
                displayName: "Reset Window Layout",
                command: new DelegateCommand(() =>
                {
                    layoutManagerService.ResetToDefault();
                }));

            layoutManagerMenuItem.Add(saveLayoutItem).Add(applySubItem).Add(manageLauoutItem).Add(resetLayoutItem);
        }

        public override IEnumerable<IBarComponent> GetExtendApplicationMenuItems(IBarComponent container, object context)
        {
            return new List<IBarComponent>() { layoutManagerMenuItem };
        }
    }
}
