using System.Windows.Input;
using System.Windows.Media;

namespace EasyDriver.MenuPlugin
{
    public interface IBarFactory
    {
        IBarComponent CreateButton(string displayName = null, ICommand command = null, KeyGesture keyGesture = null, ImageSource imageSource = null);
        IBarComponent CreateCheckItem(string displayName = null, ICommand command = null, ImageSource imageSource = null);
        IBarComponent CreateMainMenu(string displayName = null);
        IBarComponent CreateSeparator();
        IBarComponent CreateStaticItem(string displayName = null, ImageSource imageSource = null);
        IBarComponent CreateStatusBar(string displayName = null);
        IBarComponent CreateSubItem(string displayName = null, ICommand command = null, KeyGesture keyGesture = null, ImageSource imageSource = null);
        IBarComponent CreateToolBar(string displayName = null);
        IBarComponent CreateBarItem(BarItemType barItemType, string displayName = null, ICommand command = null, KeyGesture keyGesture = null, ImageSource imageSource = null);
        IBarComponent CreateButtonSplitItem(string displayName, ICommand command = null, KeyGesture keyGesture = null, ImageSource imageSource = null);
    }
}
