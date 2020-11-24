using System.Collections;
using System.Windows.Input;

namespace EasyDriver.Service.BarManager
{
    public class BarFactory
    {
        #region Singleton

        public static BarFactory Default { get; } = new BarFactory();

        #endregion

        #region Cache

        private readonly Hashtable CACHE;

        public BarFactory()
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

        public IBarComponent Clone(IBarComponent barItem)
        {
            return (IBarComponent)barItem.Clone();
        }

        public IBarComponent CreateSeparator() => GetItemFromCache<IBarComponent>(BarItemType.Separator);

        public IBarComponent CreateButton(
            bool hideWhenDisable = false,
            string displayName = null,
            ICommand command = null,
            object keyGesture = null,
            object imageSource = null)
        {
            var result = GetItemFromCache<BarItem>(BarItemType.Button);
            result.DisplayName = displayName;
            result.Command = command;
            result.Glyph = imageSource;
            result.KeyGesture = keyGesture;
            result.HideWhenDisable = hideWhenDisable;
            return result;
        }

        public IBarComponent CreateSubItem(
            bool hideWhenDisable = false,
            string displayName = null,
            ICommand command = null,
            object keyGesture = null,
            object imageSource = null)
        {
            var result = GetItemFromCache<BarSubItem>(BarItemType.SubItem);
            result.DisplayName = displayName;
            result.Command = command;
            result.KeyGesture = keyGesture;
            result.Glyph = imageSource;
            result.HideWhenDisable = hideWhenDisable;
            return result;
        }

        public IBarComponent CreateCheckItem(
            bool hideWhenDisable = false,
            string displayName = null,
            ICommand command = null,
            object imageSource = null)
        {
            var result = GetItemFromCache<BarItem>(BarItemType.CheckItem);
            result.DisplayName = displayName;
            result.Command = command;
            result.Glyph = imageSource;
            result.HideWhenDisable = hideWhenDisable;
            return result;
        }

        public IBarComponent CreateStaticItem(
            bool hideWhenDisable = false,
            string displayName = null,
            object imageSource = null)
        {
            var result = GetItemFromCache<BarItem>(BarItemType.Static);
            result.DisplayName = displayName;
            result.Glyph = imageSource;
            result.HideWhenDisable = hideWhenDisable;
            return result;
        }

        public IBarComponent CreateButtonSplitItem(
            bool hideWhenDisable = false,
            string displayName = null,
            ICommand command = null,
            object keyGesture = null,
            object imageSource = null)
        {
            var result = GetItemFromCache<BarSplitButtonItem>(BarItemType.ButtonSplitItem);
            result.DisplayName = displayName;
            result.Command = command;
            result.KeyGesture = keyGesture;
            result.Glyph = imageSource;
            result.HideWhenDisable = hideWhenDisable;
            return result;
        }

        public IBarComponent CreateBarItem(
            BarItemType barItemType,
            bool hideWhenDisable = false,
            string displayName = null,
            ICommand command = null,
            object keyGesture = null,
            object imageSource = null)
        {
            switch (barItemType)
            {
                case BarItemType.Button:
                    return CreateButton(hideWhenDisable, displayName, command, keyGesture, imageSource);
                case BarItemType.CheckItem:
                    return CreateCheckItem(hideWhenDisable, displayName, command, imageSource);
                case BarItemType.Separator:
                    return CreateSeparator();
                case BarItemType.Static:
                    return CreateStaticItem(hideWhenDisable, displayName, imageSource);
                case BarItemType.SubItem:
                    return CreateSubItem(hideWhenDisable, displayName, command, keyGesture, imageSource);
                case BarItemType.ButtonSplitItem:
                    return CreateButtonSplitItem(hideWhenDisable, displayName, command, keyGesture, imageSource);
                default:
                    return null;
            }
        }

        #endregion
    }
}
