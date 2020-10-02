using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using EasyDriver.MenuPlugin;
using System.Collections.Generic;
using IServiceContainer = EasyDriver.ServiceContainer.IServiceContainer;

namespace EasyDriver.ThemeMenu
{
    public class EasyThemeMenuPlugin : EasyMenuPlugin
    {
        readonly IBarComponent toolsSubMenu;
        readonly IBarComponent themeSubMenu;
        readonly IBarComponent lightThemeBarItem;
        readonly IBarComponent darkThemeBarItem;
        readonly IBarComponent blueThemeBarItem;

        public EasyThemeMenuPlugin(IServiceContainer serviceContainer) : base(serviceContainer)
        {
            toolsSubMenu = BarFactory.Default.CreateSubItem("Tools");
            themeSubMenu = BarFactory.Default.CreateSubItem("Themes");
            lightThemeBarItem = BarFactory.Default.CreateCheckItem(
                displayName: "Light",
                command: new DelegateCommand(() =>
                {
                    ApplicationThemeHelper.ApplicationThemeName = Theme.VS2019LightName;
                    ApplicationThemeHelper.SaveApplicationThemeName();
                    lightThemeBarItem.IsChecked = true;
                    darkThemeBarItem.IsChecked = false;
                    blueThemeBarItem.IsChecked = false;
                }));
            darkThemeBarItem = BarFactory.Default.CreateCheckItem(
                displayName: "Dark",
                command: new DelegateCommand(() =>
                {
                    ApplicationThemeHelper.ApplicationThemeName = Theme.VS2019DarkName;
                    ApplicationThemeHelper.SaveApplicationThemeName();
                    lightThemeBarItem.IsChecked = false;
                    darkThemeBarItem.IsChecked = true;
                    blueThemeBarItem.IsChecked = false;
                }));
            blueThemeBarItem = BarFactory.Default.CreateCheckItem(
                displayName: "Blue",
                command: new DelegateCommand(() =>
                {
                    ApplicationThemeHelper.ApplicationThemeName = Theme.VS2019BlueName;
                    ApplicationThemeHelper.SaveApplicationThemeName();
                    lightThemeBarItem.IsChecked = false;
                    darkThemeBarItem.IsChecked = false;
                    blueThemeBarItem.IsChecked = true;
                }));

            if (ApplicationThemeHelper.ApplicationThemeName == Theme.VS2019LightName)
            {
                lightThemeBarItem.IsChecked = true;
                darkThemeBarItem.IsChecked = false;
                blueThemeBarItem.IsChecked = false;
            }
            else if (ApplicationThemeHelper.ApplicationThemeName == Theme.VS2019DarkName)
            {
                lightThemeBarItem.IsChecked = false;
                darkThemeBarItem.IsChecked = true;
                blueThemeBarItem.IsChecked = false;
            }
            else if (ApplicationThemeHelper.ApplicationThemeName == Theme.VS2019BlueName)
            {
                lightThemeBarItem.IsChecked = false;
                darkThemeBarItem.IsChecked = false;
                blueThemeBarItem.IsChecked = true;
            }

            themeSubMenu.Add(lightThemeBarItem).Add(darkThemeBarItem).Add(blueThemeBarItem);
            toolsSubMenu.Add(themeSubMenu);
        }

        public override IEnumerable<IBarComponent> GetExtendApplicationMenuItems(IBarComponent container, object context)
        {
            return new List<IBarComponent>() { toolsSubMenu };
        }
    }
}
