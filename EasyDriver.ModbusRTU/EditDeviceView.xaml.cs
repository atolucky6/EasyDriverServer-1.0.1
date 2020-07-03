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
    /// Interaction logic for EditDeviceView.xaml
    /// </summary>
    public partial class EditDeviceView : UserControl
    {
        #region Public members

        public IEasyDriverPlugin Driver { get; set; }
        public List<ByteOrder> ByteOrderSource { get; set; }
        public IDeviceCore Device { get; set; }
        public IChannelCore Channel => Device?.Parent as IChannelCore;

        #endregion

        #region Constructors

        public EditDeviceView(IEasyDriverPlugin driver, IDeviceCore device)
        {
            Driver = driver;
            Device = device;

            InitializeComponent();

            ByteOrderSource = Enum.GetValues(typeof(ByteOrder)).Cast<ByteOrder>().ToList();
            cobByteOrder.ItemsSource = ByteOrderSource;
            cobByteOrder.SelectedItem = ByteOrder.ABCD;

            btnOk.Click += BtnOk_Click;
            btnCancel.Click += BtnCancel_Click;
            Loaded += EditDeviceView_Loaded;
        }

        #endregion

        #region Event handlers

        private void EditDeviceView_Loaded(object sender, RoutedEventArgs e)
        {
            if (Device != null)
            {
                txbName.Text = Device.Name;
                spnTimeout.Value = (decimal)Device.ParameterContainer.Parameters["Timeout"];
                cobByteOrder.SelectedItem = (ByteOrder)Device.ParameterContainer.Parameters["ByteOrder"];
                spnTryReadWrite.Value = (decimal)Device.ParameterContainer.Parameters["TryReadWriteTimes"];
                spnDeviceId.Value = (decimal)Device.ParameterContainer.Parameters["DeviceId"];
            }

        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (Validate())
            {
                Device.Name = txbName.Text?.Trim();
                Device.ParameterContainer.DisplayName = "ModbusRTU Device Parameter";
                Device.ParameterContainer.DisplayParameters = "ModbusRTU Device Parameter";

                Device.ParameterContainer.Parameters["Timeout"] = spnTimeout.Value;
                Device.ParameterContainer.Parameters["ByteOrder"] = cobByteOrder.SelectedItem;
                Device.ParameterContainer.Parameters["TryReadWriteTimes"] = spnTryReadWrite.Value;
                Device.ParameterContainer.Parameters["DeviceId"] = spnDeviceId.Value;

                ((Parent as FrameworkElement).Parent as Window).Tag = Device;
                ((Parent as FrameworkElement).Parent as Window).Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ((Parent as FrameworkElement).Parent as Window).Close();
        }

        #endregion

        #region Methods

        private bool Validate()
        {
            string validateResult = txbName.Text?.Trim().ValidateFileName("Device");
            if (!string.IsNullOrWhiteSpace(validateResult))
            {
                DXMessageBox.Show(validateResult, "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (Channel.Childs.FirstOrDefault(x => x != Device && (x as ICoreItem).Name == txbName.Text?.Trim()) != null)
            {
                DXMessageBox.Show($"The device name '{txbName.Text?.Trim()}' is already in use.", "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (Channel.Childs.FirstOrDefault(x => x != Device && (decimal)(x as IDeviceCore).ParameterContainer.Parameters["DeviceId"] == spnDeviceId.Value) != null)
            {
                DXMessageBox.Show($"The device id '{spnDeviceId.Value}' is already used in this device.", "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        #endregion
    }
}
