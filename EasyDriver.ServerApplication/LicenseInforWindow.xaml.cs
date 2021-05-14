using EasyScada.License;
using EasyScada.ServerApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EasyDriver.ServerApplication
{
    /// <summary>
    /// Interaction logic for LicenseInforWindow.xaml
    /// </summary>
    public partial class LicenseInforWindow : Window
    {
        public LicenseInforWindow()
        {
            InitializeComponent();

            lbComputerId.Content = $"- Computer Id: {LicenseManager.GetComputerId()}";
            lbSerialKey.Content = $"- Serial Key: {LicenseManager.GetStoredSerialKey("AHDScada")}";

            Loaded += LicenseInforWindow_Loaded;
        }

        private async void LicenseInforWindow_Loaded(object sender, RoutedEventArgs e)
        {
            int totalTags = await IoC.Instance.Get<ILicenseManagerService>().Authenticate(LicenseManager.GetStoredSerialKey("AHDScada"));
            if (totalTags > 0)
            {
                Dispatcher.Invoke(() =>
                {
                    activeLink.Visibility = Visibility.Hidden;
                    lbStatus.Content = "- Status: Activated";
                    lbInformation.Content = $"Information: Total activated tags '{totalTags}'";
                });
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    activeLink.Visibility = Visibility.Visible;
                    lbStatus.Content = "- Status: Unactive";
                    lbInformation.Content = "- Information: Trial";
                });
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            ActiveLicenseWindow window = new ActiveLicenseWindow();
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            if (window.ShowDialog() == true)
            {
                Task.Run(async () =>
                {
                    int totalTags = await IoC.Instance.Get<ILicenseManagerService>().Authenticate(LicenseManager.GetStoredSerialKey("AHDScada"));
                    if (totalTags > 0)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            activeLink.Visibility = Visibility.Hidden;
                            lbStatus.Content = "- Status: Activated";
                            lbInformation.Content = $"Information: Total activated tags '{totalTags}'";
                        });
                    }
                    else
                    {
                        Dispatcher.Invoke(() =>
                        {
                            activeLink.Visibility = Visibility.Visible;
                            lbStatus.Content = "- Status: Unactive";
                            lbInformation.Content = "- Information: Trial";
                        });
                    }
                });
            }
        }
    }
}
