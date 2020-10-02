using System.Windows;

namespace EasyDriver.CoreItemFactory
{
    public static class Helper
    {
        public static object ShowContextWindow(object context, string title = "", SizeToContent sizeToContent = SizeToContent.WidthAndHeight)
        {
            ContextWindow contextWindow = new ContextWindow(context)
            {
                Title = title,
                SizeToContent = sizeToContent,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            contextWindow.Owner = Application.Current.MainWindow;
            contextWindow.ShowDialog();
            return contextWindow.Tag;
        }
    }
}
