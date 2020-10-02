using System.Windows;

namespace EasyDriver.ContextWindow
{
    public interface IContextWindowService
    {
        object Show(object context, string title = "", SizeToContent sizeToContent = SizeToContent.WidthAndHeight);
        object Tag { get; }
    }
}
