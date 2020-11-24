using DevExpress.Xpf.Core;
using EasyDriver.ServicePlugin;
using System.Windows;
using System.Windows.Controls;

namespace EasyDriver.Service.ContextWindow
{
    [Service(0, false)]
    public class ContextWindowService : EasyServicePluginBase, IContextWindowService
    {
        public object Tag { get; set; }

        public object ShowDialog(
            object context,
            string title = "", 
            SizeToContent sizeToContent = SizeToContent.WidthAndHeight,
            double width = 800,
            double height = 400)
        {
            ThemedWindow window = new ThemedWindow();
            window.Title = title;
            window.SizeToContent = sizeToContent;
            window.ShowIcon = true;
            window.Height = height;
            window.Width = width;
            window.Owner = Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.WindowStyle = WindowStyle.ToolWindow;
            window.Content = context;
            window.ShowDialog();
            Tag = (context as UserControl).Tag;
            return Tag;
        }
    }
}
