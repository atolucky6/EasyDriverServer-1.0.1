using System.Collections.Generic;
using EasyDriverPlugin;
using System.Windows.Controls;
using System;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;
using EasyDriver.Server.Models;
using EasyDriver.Client.Models;

namespace EasyDriver.ModbusRTU
{
    /// <summary>
    /// Interaction logic for CreateTagView.xaml
    /// </summary>
    public partial class CreateTagView : UserControl
    {
        #region Public members

        public IEasyDriverPlugin Driver { get; set; }
        public IDeviceCore Device { get; set; }
        public List<AccessPermission> AccessPermissionSource { get; set; }
        public List<IDataType> DataTypeSource { get; set; }

        #endregion

        #region Constructors

        public CreateTagView(IEasyDriverPlugin driver, IDeviceCore device)
        {
            Driver = driver;
            Device = device;

            InitializeComponent();

            AccessPermissionSource = Enum.GetValues(typeof(AccessPermission)).Cast<AccessPermission>().ToList();
            cobPermission.ItemsSource = AccessPermissionSource;
            cobPermission.SelectedItem = AccessPermission.ReadAndWrite;

            DataTypeSource = driver.GetSupportDataTypes().ToList();
            cobDataType.ItemsSource = DataTypeSource;
            cobDataType.DisplayMember = "Name";
            cobDataType.SelectedIndex = 0;

            txbName.Text = device.GetUniqueNameInGroup("Tag1");

            btnOk.Click += BtnOk_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        #endregion

        #region Event handlers

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            string validateResult = txbName.Text?.Trim().ValidateFileName("Tag");
            if (!string.IsNullOrWhiteSpace(validateResult))
            {
                DXMessageBox.Show(validateResult, "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Device.Childs.FirstOrDefault(x => (x as ICoreItem).Name == txbName.Text?.Trim()) != null)
            {
                DXMessageBox.Show($"The tag name '{txbName.Text?.Trim()}' is already in use.", "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool isBitAddress = false;
            AccessPermission accessPermission = (AccessPermission)cobPermission.SelectedItem;
            if (uint.TryParse(txbAddress.Text?.Trim(), out uint address))
            {
                if (address > 0 && address < 10000)
                {
                    isBitAddress = true;
                }
                else if (address > 10000 && address < 20000)
                {
                    isBitAddress = true;
                    accessPermission = AccessPermission.ReadOnly;
                }
                else if (address > 30000 && address < 40000)
                {
                    isBitAddress = false;
                    accessPermission = AccessPermission.ReadOnly;
                }
                else if (address > 40000 && address < 50000)
                {
                    isBitAddress = false;
                }
                else
                {
                    DXMessageBox.Show($"The tag address was not in correct format.", "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            if (isBitAddress)
            {
                cobDataType.SelectedItem = DataTypeSource.FirstOrDefault(x => x.Name == "Bool");
            }
            else
            {
                if (cobDataType.SelectedItem == DataTypeSource.FirstOrDefault(x => x.Name == "Bool"))
                {
                    DXMessageBox.Show($"The current address doesn't support read and write Bool data type.", "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            ITagCore tag = new TagCore(Device);
            tag.Name = txbName.Text?.Trim();
            tag.AccessPermission = accessPermission;
            tag.DataType = (IDataType)cobDataType.SelectedItem;
            tag.Address = address.ToString();
            tag.RefreshRate = (int)spnScanRate.Value;
            tag.Gain = isBitAddress ? 1 : (double)spnGain.Value;
            tag.Offset = isBitAddress ? 0 : (double)spnOffset.Value;
            tag.ByteOrder = Device.ByteOrder;
            tag.ParameterContainer.DisplayName = "Tag Parameters";
            tag.ParameterContainer.DisplayParameters = "Tag Parameters";
            Device.Add(tag);
            ((Parent as FrameworkElement).Parent as Window).Tag = tag;
            ((Parent as FrameworkElement).Parent as Window).Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ((Parent as FrameworkElement).Parent as Window).Close();
        }

        #endregion
    }
}
