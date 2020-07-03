using DevExpress.Xpf.Core;
using EasyDriverPlugin;
using EasyDriver.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EasyDriver.ModbusRTU
{
    /// <summary>
    /// Interaction logic for CreateDeviceView.xaml
    /// </summary>
    public partial class CreateDeviceView : UserControl
    {
        #region Public members

        public IEasyDriverPlugin Driver { get; set; }
        public IChannelCore Channel { get; set; }
        public List<ByteOrder> ByteOrderSource { get; set; }

        #endregion

        #region Constructors

        public CreateDeviceView(IEasyDriverPlugin driver, IChannelCore channel)
        {
            Driver = driver;
            Channel = channel;

            InitializeComponent();

            ByteOrderSource = Enum.GetValues(typeof(ByteOrder)).Cast<ByteOrder>().ToList();
            cobByteOrder.ItemsSource = ByteOrderSource;
            cobByteOrder.SelectedItem = ByteOrder.ABCD;

            txbName.Text = channel.GetUniqueNameInGroup("Device1");

            btnOk.Click += BtnOk_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        #endregion

        #region Event handlers

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            string validateResult = txbName.Text?.Trim().ValidateFileName("Device");
            if (!string.IsNullOrWhiteSpace(validateResult))
            {
                DXMessageBox.Show(validateResult, "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Channel.Childs.FirstOrDefault(x => (x as ICoreItem).Name == txbName.Text?.Trim()) != null)
            {
                DXMessageBox.Show($"The device name '{txbName.Text?.Trim()}' is already in use.", "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Channel.Childs.FirstOrDefault(x => (decimal)(x as IDeviceCore).ParameterContainer.Parameters["DeviceId"] == spnDeviceId.Value) != null)
            {
                DXMessageBox.Show($"The device id '{spnDeviceId.Value}' is already used in this device.", "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IDeviceCore device = new DeviceCore(Channel);
            device.Name = txbName.Text?.Trim();
            device.ParameterContainer.DisplayName = "ModbusRTU Device Parameter";
            device.ParameterContainer.DisplayParameters = "ModbusRTU Device Parameter";

            device.ParameterContainer.Parameters["Timeout"] = spnTimeout.Value;
            device.ParameterContainer.Parameters["ByteOrder"] = cobByteOrder.SelectedItem;
            device.ParameterContainer.Parameters["TryReadWriteTimes"] = spnTryReadWrite.Value;
            device.ParameterContainer.Parameters["DeviceId"] = spnDeviceId.Value;

            Channel.Add(device);
            ((Parent as FrameworkElement).Parent as Window).Tag = device;
            ((Parent as FrameworkElement).Parent as Window).Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ((Parent as FrameworkElement).Parent as Window).Close();
        }

        #endregion
    }
}
