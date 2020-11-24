using EasyDriver.ServicePlugin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace EasyDriver.Service.ContextWindow
{
    public interface IContextWindowService : IEasyServicePlugin
    {
        object ShowDialog(object context, string title = "", SizeToContent sizeToContent = SizeToContent.WidthAndHeight,
            double width = 800, double height = 400);
        object Tag { get; }
    }
}
