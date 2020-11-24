using DevExpress.Mvvm.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EasyDriver.Service.Notifier
{
    /// <summary>
    /// Interaction logic for NotifyControl.xaml
    /// </summary>
    public partial class NotifyControl : UserControl
    {
        public NotifyControl()
        {
            InitializeComponent();
        }

        public NotifyControl(object dataContext)
        {
            InitializeComponent();
            DataContext = dataContext;
        }

        public NotificationService MessageNotificationService { get => NotifyMessage; }
    }
}
