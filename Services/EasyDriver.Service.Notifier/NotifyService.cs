using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriver.ServicePlugin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace EasyDriver.Service.Notifier
{
    [Service(int.MaxValue, true)]
    public class NotifyService : EasyServicePluginBase, INotifyService
    {
        public string ApplicationId => notifyControl == null ? null : notifyControl.MessageNotificationService.ApplicationId;
        public string ApplicationName => notifyControl == null ? null : notifyControl.MessageNotificationService.ApplicationName;

        private NotifyControl notifyControl;
        private IconControl iconControl;
        private NotifyControlViewModel notifyControlViewModel;

        public event EventHandler IconLeftClick;
        public event EventHandler IconDoubleClick;
        public event Action<string> MenuItemClick;

        public NotifyService() : base()
        {
            notifyControlViewModel = ViewModelSource.Create(() => new NotifyControlViewModel(this));
            notifyControl = new NotifyControl(notifyControlViewModel);
        }

        public void Show(INotification notification)
        {
            notifyControlViewModel?.Show(notification);
        }

        public void Show(ImageSource image, params string[] contents)
        {
            string text1 = contents.Length > 0 ? contents[0] : "";
            string text2 = contents.Length > 1 ? contents[1] : "";
            string text3 = contents.Length > 2 ? contents[2] : "";
            INotification notification = notifyControlViewModel.NotifyMessageService.CreatePredefinedNotification(text1, text2, text3);
            Show(notification);
        }

        public void SetApplicationId(string applicationId)
        {
            notifyControl.MessageNotificationService.ApplicationId = applicationId;
        }

        public void SetApplicationName(string applicationName)
        {
            notifyControl.MessageNotificationService.ApplicationName = applicationName;
        }

        public void ShowApplicationIcon()
        {
            if (iconControl == null)
                iconControl = new IconControl(notifyControlViewModel);
        }

        internal void RaiseIconLeftClick()
        {
            IconLeftClick?.Invoke(this, EventArgs.Empty);
        }

        internal void RaiseIconLeftDoubleClick()
        {
            IconDoubleClick?.Invoke(this, EventArgs.Empty);
        }

        internal void RaiseMenuItemClick(string args)
        {
            MenuItemClick?.Invoke(args);
        }

        public override void EndInit()
        {
            ShowApplicationIcon();
        }
    }
}
