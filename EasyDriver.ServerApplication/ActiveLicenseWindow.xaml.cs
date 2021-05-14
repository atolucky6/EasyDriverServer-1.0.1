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
    /// Interaction logic for ActiveLicenseWindow.xaml
    /// </summary>
    public partial class ActiveLicenseWindow : Window
    {
        public bool IsBusy { get; protected set; }
        public string ComputerId { get; private set; }
        public string SerialKey { get; private set; }

        public ActiveLicenseWindow()
        {
            InitializeComponent();

            ComputerId = LicenseManager.GetComputerId();
            SerialKey = LicenseManager.GetStoredSerialKey("AHDScada");

            txbComputerId.Text = ComputerId;
            txbSerialKey.Text = SerialKey;

            btnActive.Click += BtnActive_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void BtnActive_Click(object sender, RoutedEventArgs e)
        {
            if (IsBusy)
                return;
            IsBusy = true;
            try
            {
                int result = await IoC.Instance.Get<ILicenseManagerService>().Authenticate(txbSerialKey.Text);
                if (result > 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"Activated success!", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                        Close();
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"Activated fail!", "Message", MessageBoxButton.OK, MessageBoxImage.Warning);
                    });
                }
            }
            catch { 
            }
            finally { IsBusy = false; }
        }
    }
}
