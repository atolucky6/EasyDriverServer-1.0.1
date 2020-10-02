using System.Windows;
using System.Windows.Controls;

namespace EasyDriver.MenuPlugin
{
    internal class BarItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate BarButtonTemplate { get; set; }
        public DataTemplate BarCheckTemplate { get; set; }
        public DataTemplate BarStaticTemplate { get; set; }
        public DataTemplate BarSeparatorTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is BarItem barItem)
            {
                switch (barItem.Type)
                {
                    case BarItemType.Button:
                        return BarButtonTemplate;
                    case BarItemType.CheckItem:
                        return BarCheckTemplate;
                    case BarItemType.Separator:
                        return BarSeparatorTemplate;
                    case BarItemType.Static:
                        return BarStaticTemplate;
                    default:
                        return null;
                }
            }
            return null;
        }
    }
}
