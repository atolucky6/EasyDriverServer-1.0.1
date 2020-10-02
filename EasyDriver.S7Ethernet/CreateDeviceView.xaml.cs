using DevExpress.Xpf.Core;
using EasyDriver.Core;
using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ByteOrder = EasyDriverPlugin.ByteOrder;

namespace EasyDriver.S7Ethernet
{
    /// <summary>
    /// Interaction logic for CreateDeviceView.xaml
    /// </summary>
    public partial class CreateDeviceView : UserControl
    {
        public CreateDeviceView()
        {
            InitializeComponent();
        }

        public CreateDeviceView(S7EthernetDriver driver, IGroupItem parent, IDeviceCore templateItem = null)
        {
            this.driver = driver;
            this.parent = parent;
            InitializeComponent();

            ByteOrderSource = Enum.GetValues(typeof(ByteOrder)).Cast<ByteOrder>().ToList();
            cobByteOrder.ItemsSource = ByteOrderSource;
            cobByteOrder.SelectedItem = ByteOrder.ABCD;

            if (templateItem == null)
            {
                txbName.Text = parent.GetUniqueNameInGroup("Device1");
            }
            else
            {
                txbName.Text = parent.GetUniqueNameInGroup(templateItem.Name);

                if (templateItem.ParameterContainer.Parameters.ContainsKey("IpAddress"))
                    txbIpAddress.Text = templateItem.ParameterContainer.Parameters["IpAddress"]?.ToString();
                if (templateItem.ParameterContainer.Parameters.ContainsKey("Timeout"))
                    spnTimeout.EditValue = templateItem.ParameterContainer.Parameters["Timeout"];
                if (templateItem.ParameterContainer.Parameters.ContainsKey("TryReadWriteTimes"))
                    spnTryReadWrite.EditValue = templateItem.ParameterContainer.Parameters["TryReadWriteTimes"];
                if (templateItem.ParameterContainer.Parameters.ContainsKey("DelayBetweenPool"))
                    spnDelayPool.EditValue = templateItem.ParameterContainer.Parameters["DelayBetweenPool"];
                if (templateItem.ParameterContainer.Parameters.ContainsKey("Port"))
                    spnPort.EditValue = templateItem.ParameterContainer.Parameters["Port"];
                if (templateItem.ParameterContainer.Parameters.ContainsKey("ScanRate"))
                    spnScanRate.EditValue = templateItem.ParameterContainer.Parameters["ScanRate"];

                cobByteOrder.SelectedItem = templateItem.ByteOrder;
            }

            KeyDown += OnKeyDown;

            btnOk.Click += BtnOk_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        readonly S7EthernetDriver driver;
        readonly IGroupItem parent;
        public List<ByteOrder> ByteOrderSource { get; set; }

        #region Event handlers

        private void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnOk_Click(null, null);
            }
            else if (e.Key == Key.Escape)
            {
                BtnCancel_Click(null, null);
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            string validateResult = txbName.Text?.Trim().ValidateFileName("Device");
            if (!string.IsNullOrWhiteSpace(validateResult))
            {
                DXMessageBox.Show(validateResult, "Message", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (parent.Childs.FirstOrDefault(x => (x as ICoreItem).Name == txbName.Text?.Trim()) != null)
            {
                DXMessageBox.Show($"The device name '{txbName.Text?.Trim()}' is already in use.", "Message", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!txbIpAddress.Text.IsIpAddress())
            {
                DXMessageBox.Show($"The ip address was not in correct format.", "Message", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IDeviceCore device = new DeviceCore(parent);
            device.Name = txbName.Text?.Trim();
            device.ParameterContainer.DisplayName = "S7Ethernet Device Parameter";
            device.ParameterContainer.DisplayParameters = "S7Ethernet Device Parameter";

            device.ParameterContainer.Parameters["IpAddress"] = txbIpAddress.Text?.Trim();
            device.ParameterContainer.Parameters["Timeout"] = spnTimeout.Value.ToString();
            device.ByteOrder = (ByteOrder)Enum.Parse(typeof(ByteOrder), cobByteOrder.SelectedItem.ToString());
            device.ParameterContainer.Parameters["TryReadWriteTimes"] = spnTryReadWrite.Value.ToString();
            device.ParameterContainer.Parameters["DelayBetweenPool"] = spnDelayPool.Value.ToString();
            device.ParameterContainer.Parameters["Port"] = spnPort.Value.ToString();
            device.ParameterContainer.Parameters["ScanRate"] = spnScanRate.Value.ToString();
            device.ParameterContainer.Parameters["Rack"] = spnRack.Value.ToString();
            device.ParameterContainer.Parameters["Slot"] = spnSlot.Value.ToString();
            ((Parent as FrameworkElement).Parent as Window).Tag = device;
            ((Parent as FrameworkElement).Parent as Window).Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ((Parent as FrameworkElement) as Window).Close();
        }
        #endregion
    }
}
