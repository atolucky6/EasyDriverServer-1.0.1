using DevExpress.Xpf.Core;
using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Text;
using EasyDriver.Core;
using System.Windows.Input;

namespace EasyDriver.DPA870
{
    /// <summary>
    /// Interaction logic for CreateDeviceView.xaml
    /// </summary>
    public partial class CreateDeviceView : UserControl
    {
        #region Public members

        public IEasyDriverPlugin Driver { get; set; }
        public IGroupItem ParentItem { get; set; }
        public List<ByteOrder> ByteOrderSource { get; set; }

        #endregion

        #region Constructors

        public CreateDeviceView(IEasyDriverPlugin driver, IGroupItem parent, IDeviceCore templateItem)
        {
            Driver = driver;
            ParentItem = parent;

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

                cobByteOrder.SelectedItem = templateItem.ByteOrder;
            }

            KeyDown += OnKeyDown;

            btnOk.Click += BtnOk_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        #endregion

        #region Event handlers

        private void OnKeyDown(object sender, KeyEventArgs e)
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
                DXMessageBox.Show(validateResult, "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var childDevices = ParentItem.GetAllDevices();

            if (ParentItem.Childs.FirstOrDefault(x => (x as ICoreItem).Name == txbName.Text?.Trim()) != null)
            {
                DXMessageBox.Show($"The device name '{txbName.Text?.Trim()}' is already in use.", "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IDeviceCore device = new DeviceCore(ParentItem);
            device.Name = txbName.Text?.Trim();
            device.ParameterContainer.DisplayName = "DPA870 Device Parameter";
            device.ParameterContainer.DisplayParameters = "DPA870 Device Parameter";

            device.ByteOrder = (ByteOrder)Enum.Parse(typeof(ByteOrder), cobByteOrder.SelectedItem.ToString());

            Tag = device;
            ((Parent as FrameworkElement).Parent as Window).Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ((Parent as FrameworkElement).Parent as Window).Close();
        }

        #endregion
    }
}
