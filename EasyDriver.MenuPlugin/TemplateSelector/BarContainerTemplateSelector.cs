using System.Windows;
using System.Windows.Controls;

namespace EasyDriver.MenuPlugin
{
    internal class BarContainerTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MainMenuTemplate { get; set; }
        public DataTemplate ToolBarTemplate { get; set; }
        public DataTemplate StatusBarTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is BarContainer bar)
            {
                switch (bar.Type)
                {
                    case BarContainerType.MainMenu:
                        return MainMenuTemplate;
                    case BarContainerType.ToolBar:
                        return ToolBarTemplate;
                    case BarContainerType.StatusBar:
                        return StatusBarTemplate;
                    default:
                        return null;
                }
            }
            return null;
        }
    }
}
