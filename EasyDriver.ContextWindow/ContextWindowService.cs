using DevExpress.Mvvm.UI;
using System.Windows;

namespace EasyDriver.ContextWindow
{
    public class ContextWindowService : ServiceBase, IContextWindowService
    {
        public object Tag { get; protected set; }
        public object Show(object context, string title = "", SizeToContent sizeToContent = SizeToContent.WidthAndHeight)
        {
            ContextWindow contextWindow = new ContextWindow(context)
            {
                Title = title,
                SizeToContent = sizeToContent,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            contextWindow.Owner = Application.Current.MainWindow;
            contextWindow.ShowDialog();
            Tag = contextWindow.Tag;
            return Tag;
        }
    }
}
