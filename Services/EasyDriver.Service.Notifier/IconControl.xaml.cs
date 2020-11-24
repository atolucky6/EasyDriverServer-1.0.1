using System;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.Core;

namespace EasyDriver.Service.Notifier
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class IconControl : UserControl
    {
        public IconControl()
        {
            InitializeComponent();
        }

        public IconControl(object dataContext)
        {
            InitializeComponent();
            DataContext = dataContext;
            IconNotificationService.Icon = Application.Current?.MainWindow?.Icon;

            if (DataContext is NotifyControlViewModel vm)
            {
                vm.notifyService.IconLeftClick += NotifyService_IconLeftClick;
                vm.notifyService.IconDoubleClick += NotifyService_IconLeftDoubleClick;
            }
        }

        private void NotifyService_IconLeftDoubleClick(object sender, EventArgs e)
        {

        }

        private void NotifyService_IconLeftClick(object sender, EventArgs e)
        {
            iconPopup.ShowPopup(this);
        }

        public NotifyIconService IconNotificationService { get => NotifyIcon; }
    }

    [POCOViewModel]
    public class NotifyControlViewModel
    {
        #region Members
        public virtual INotificationService NotifyMessageService => this.GetService<INotificationService>("NotifyMessage");
        public static string ApplicationId { get; set; } = "EasyDriverServer";
        public static string ApplicationName { get; set; }
        internal readonly NotifyService notifyService;
        #endregion

        #region Constructors
        public NotifyControlViewModel(NotifyService notifyService)
        {
            this.notifyService = notifyService;
        }
        #endregion

        #region Commands & Methods
        public void Show(INotification notification)
        {
            notification.ShowAsync();
        }

        public void MenuItemClick(object args)
        {
            notifyService.RaiseMenuItemClick(args?.ToString());
        }

        public void IconLeftClick()
        {
            notifyService.RaiseIconLeftClick();
        }

        public void IconDoubleClick()
        {
            notifyService.RaiseIconLeftDoubleClick();
        }
        #endregion
    }
}