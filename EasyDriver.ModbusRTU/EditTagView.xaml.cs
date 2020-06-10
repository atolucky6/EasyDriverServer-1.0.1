using System.Collections.Generic;
using EasyDriverPlugin;
using System.Windows.Controls;
using System;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;
using EasyScada.Core;

namespace EasyDriver.ModbusRTU
{
    /// <summary>
    /// Interaction logic for EditTagView.xaml
    /// </summary>
    public partial class EditTagView : UserControl
    {
        #region Public members

        public IEasyDriverPlugin Driver { get; set; }
        public IDevice Device => TagEdit?.Parent as IDevice;
        public ITag TagEdit { get; set; }
        public List<AccessPermission> AccessPermissionSource { get; set; }
        public List<IDataType> DataTypeSource { get; set; }

        #endregion

        #region Constructors

        public EditTagView(IEasyDriverPlugin driver, ITag tag)
        {
            Driver = driver;
            TagEdit = tag;

            InitializeComponent();

            AccessPermissionSource = Enum.GetValues(typeof(AccessPermission)).Cast<AccessPermission>().ToList();
            cobPermission.ItemsSource = AccessPermissionSource;
            cobPermission.SelectedItem = AccessPermission.ReadAndWrite;

            DataTypeSource = driver.GetSupportDataTypes().ToList();
            cobDataType.ItemsSource = DataTypeSource;
            cobDataType.DisplayMember = "Name";
            cobDataType.SelectedIndex = 0;

            btnOk.Click += BtnOk_Click;
            btnCancel.Click += BtnCancel_Click;
            Loaded += EditTagView_Loaded;
        }

        #endregion

        #region Event handlers

        private void EditTagView_Loaded(object sender, RoutedEventArgs e)
        {
            if (TagEdit != null)
            {
                txbName.Text = TagEdit.Name;
                cobPermission.SelectedItem = TagEdit.AccessPermission;
                cobDataType.SelectedItem = TagEdit.DataType;
                txbAddress.Text = TagEdit.Address;
                spnScanRate.EditValue = TagEdit.RefreshRate;
                spnGain.EditValue = TagEdit.Gain;
                spnOffset.EditValue = TagEdit.Offset;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            string validateResult = txbName.Text?.Trim().ValidateFileName("Tag");
            if (!string.IsNullOrWhiteSpace(validateResult))
            {
                DXMessageBox.Show(validateResult, "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Device.Childs.FirstOrDefault(x => x != TagEdit && x.Name == txbName.Text?.Trim()) != null)
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

            TagEdit.Name = txbName.Text?.Trim();
            TagEdit.AccessPermission = accessPermission;
            TagEdit.DataType = (IDataType)cobDataType.SelectedItem;
            TagEdit.Address = address.ToString();
            TagEdit.RefreshRate = (int)spnScanRate.Value;
            TagEdit.Gain = isBitAddress ? 1 : (double)spnGain.Value;
            TagEdit.Offset = isBitAddress ? 0 : (double)spnOffset.Value;
            TagEdit.ByteOrder = Device.ByteOrder;
            TagEdit.ParameterContainer.DisplayName = "Tag Parameters";
            TagEdit.ParameterContainer.DisplayParameters = "Tag Parameters";

            ((Parent as FrameworkElement).Parent as Window).Tag = TagEdit;
            ((Parent as FrameworkElement).Parent as Window).Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ((Parent as FrameworkElement).Parent as Window).Close();
        }

        #endregion
    }
}
