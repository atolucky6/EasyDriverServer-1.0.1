using System.Collections.Generic;
using EasyDriverPlugin;
using System.Windows.Controls;
using System;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;
using EasyDriver.Core;

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

        public CreateTagView(IEasyDriverPlugin driver, IDeviceCore device, ITagCore templateItem)
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

            if (templateItem == null)
            {
                txbName.Text = device.GetUniqueNameInGroup("Tag1");
                txbAddress.Text = "1";
            }
            else
            {
                txbName.Text = device.GetUniqueNameInGroup(templateItem.Name);
                cobDataType.SelectedItem = DataTypeSource.FirstOrDefault(x => x.Name == templateItem.DataTypeName);
                if (cobDataType.SelectedItem == null)
                    cobDataType.SelectedItem = DataTypeSource.FirstOrDefault();
                txbAddress.Text = templateItem.Address;
                spnGain.EditValue = templateItem.Gain;
                spnOffset.EditValue = templateItem.Offset;
                spnRefreshRate.EditValue = templateItem.RefreshRate;
                cobPermission.SelectedItem = templateItem.AccessPermission;
            }

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

            string address = txbAddress.Text?.Trim();
            AccessPermission accessPermission = (AccessPermission)cobPermission.SelectedItem;
            string error = address.IsValidAddress(accessPermission, out bool isBitAddress);
            if (!string.IsNullOrEmpty(error))
            {
                DXMessageBox.Show(error, "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
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

            List<ITagCore> createTags = new List<ITagCore>();
            string currentTagName = txbName.Text?.Trim();
            uint number = currentTagName.ExtractLastNumberFromString(out bool hasValue, out bool hasBracket);
            if (!hasValue)
                number = 1;
            string name = currentTagName.RemoveLastNumberFromString();
            string nameFormat = "";

            if (hasBracket)
            {
                name = name.Remove(name.Length - 2, 2);
                nameFormat = name + "({0})";
            }
            else
            {
                nameFormat = name + "{0}";
            }

            uint adrNumber = uint.Parse(txbAddress.Text?.Trim());
            
            for (int i = 0; i < spnAutoCreateCount.Value; i++)
            {
                string adrString = adrNumber.ToString();

                if (!string.IsNullOrEmpty(adrString.IsValidAddress()))
                    break;

                ITagCore tag = new TagCore(Device)
                {
                    Name = Device.GetUniqueNameInGroup(string.Format(nameFormat, number)),
                    AccessPermission = accessPermission,
                    DataType = (IDataType)cobDataType.SelectedItem,
                    Address = adrString,
                    RefreshRate = (int)spnRefreshRate.Value,
                    Gain = (double)spnGain.Value,
                    Offset = (double)spnOffset.Value,
                    ByteOrder = Device.ByteOrder
                };
                tag.ParameterContainer.DisplayName = "Tag Parameters";
                tag.ParameterContainer.DisplayParameters = "Tag Parameters";
                currentTagName = tag.Name;
                createTags.Add(tag);
                number++;
                adrNumber++;
            }

            ((Parent as FrameworkElement).Parent as Window).Tag = createTags;
            ((Parent as FrameworkElement).Parent as Window).Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ((Parent as FrameworkElement).Parent as Window).Close();
        }

        #endregion
    }
}
