using System.Collections;
using System.Windows.Input;
using System.Windows.Media;

namespace EasyDriver.MenuPlugin
{
    public class BarFactory : IBarFactory
    {
        #region Singleton

        public static BarFactory Default { get; } = new BarFactory();

        #endregion

        #region Cache

        private readonly Hashtable CACHE;

        internal BarFactory()
        {
            CACHE = new Hashtable
            {
                { BarContainerType.MainMenu, new BarContainer("MainMenu", BarContainerType.MainMenu) },
                { BarContainerType.ToolBar, new BarContainer("ToolBar", BarContainerType.ToolBar) },
                { BarContainerType.StatusBar, new BarContainer("StatusBar", BarContainerType.StatusBar) },

                { BarItemType.Button, new BarItem(BarItemType.Button) },
                { BarItemType.CheckItem, new BarItem(BarItemType.CheckItem) },
                { BarItemType.Separator, new BarItem(BarItemType.Separator) },
                { BarItemType.Static, new BarItem(BarItemType.Static) },
                { BarItemType.SubItem, new BarSubItem() },
                { BarItemType.ButtonSplitItem, new BarSplitButtonItem() },
            };
        }

        private T GetItemFromCache<T>(object key)
            where T : IBarComponent
        {
            return ((T)CACHE[key]).Clone<T>();
        }

        #endregion

        #region Create bar container methods

        public IBarComponent CreateMainMenu(string displayName = null)
        {
            var result = GetItemFromCache<BarContainer>(BarContainerType.MainMenu);
            result.DisplayName = displayName;
            return result;
        }

        public IBarComponent CreateToolBar(string displayName = null)
        {
            var result = GetItemFromCache<BarContainer>(BarContainerType.ToolBar);
            result.DisplayName = displayName;
            return result;
        }

        public IBarComponent CreateStatusBar(string displayName = null)
        {
            var result = GetItemFromCache<BarContainer>(BarContainerType.StatusBar);
            result.DisplayName = displayName;
            return result;
        }

        #endregion

        #region Create bar item methods

        public IBarComponent CreateSeparator() => GetItemFromCache<IBarComponent>(BarItemType.Separator);

        public IBarComponent CreateButton(
            string displayName = null,
            ICommand command = null,
            KeyGesture keyGesture = null,
            ImageSource imageSource = null)
        {
            var result = GetItemFromCache<BarItem>(BarItemType.Button);
            result.DisplayName = displayName;
            result.Command = command;
            result.Glyph = imageSource;
            result.KeyGesture = keyGesture;
            return result;
        }

        public IBarComponent CreateSubItem(
            string displayName = null,
            ICommand command = null,
            KeyGesture keyGesture = null,
            ImageSource imageSource = null)
        {
            var result = GetItemFromCache<BarSubItem>(BarItemType.SubItem);
            result.DisplayName = displayName;
            result.Command = command;
            result.KeyGesture = keyGesture;
            result.Glyph = imageSource;
            return result;
        }

        public IBarComponent CreateCheckItem(
            string displayName = null,
            ICommand command = null,
            ImageSource imageSource = null)
        {
            var result = GetItemFromCache<BarItem>(BarItemType.CheckItem);
            result.DisplayName = displayName;
            result.Command = command;
            result.Glyph = imageSource;
            return result;
        }

        public IBarComponent CreateStaticItem(
            string displayName = null,
            ImageSource imageSource = null)
        {
            var result = GetItemFromCache<BarItem>(BarItemType.Static);
            result.DisplayName = displayName;
            result.Glyph = imageSource;
            return result;
        }

        public IBarComponent CreateButtonSplitItem(
            string displayName,
            ICommand command = null,
            KeyGesture keyGesture = null,
            ImageSource imageSource = null)
        {
            var result = GetItemFromCache<BarSplitButtonItem>(BarItemType.ButtonSplitItem);
            result.DisplayName = displayName;
            result.Command = command;
            result.KeyGesture = keyGesture;
            result.Glyph = imageSource;
            return result;
        }

        public IBarComponent CreateBarItem(
            BarItemType barItemType,
            string displayName = null,
            ICommand command = null,
            KeyGesture keyGesture = null,
            ImageSource imageSource = null)
        {
            switch (barItemType)
            {
                case BarItemType.Button:
                    return CreateButton(displayName, command, keyGesture, imageSource);
                case BarItemType.CheckItem:
                    return CreateCheckItem(displayName, command, imageSource);
                case BarItemType.Separator:
                    return CreateSeparator();
                case BarItemType.Static:
                    return CreateStaticItem(displayName, imageSource);
                case BarItemType.SubItem:
                    return CreateSubItem(displayName, command, keyGesture, imageSource);
                case BarItemType.ButtonSplitItem:
                    return CreateButtonSplitItem(displayName, command, keyGesture, imageSource);
                default:
                    return null;
            }
        }

        #endregion
    }
}
