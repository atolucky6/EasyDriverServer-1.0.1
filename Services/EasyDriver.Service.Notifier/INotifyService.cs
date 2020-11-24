using DevExpress.Mvvm;
using EasyDriver.ServicePlugin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace EasyDriver.Service.Notifier
{
    public interface INotifyService : IEasyServicePlugin
    {
        string ApplicationId { get; }
        string ApplicationName { get; }

        void Show(INotification notification);
        void Show(ImageSource image, params string[] contents);
        void SetApplicationId(string applicationId);
        void SetApplicationName(string applicationName);
        void ShowApplicationIcon();

        event EventHandler IconLeftClick;
        event EventHandler IconDoubleClick;
        event Action<string> MenuItemClick;
    }
}
