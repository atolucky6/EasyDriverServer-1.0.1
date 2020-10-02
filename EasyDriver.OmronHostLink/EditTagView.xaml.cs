using System.Collections.Generic;
using EasyDriverPlugin;
using System.Windows.Controls;
using System;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;
using System.Windows.Input;

namespace EasyDriver.OmronHostLink
{
    /// <summary>
    /// Interaction logic for EditTagView.xaml
    /// </summary>
    public partial class EditTagView : UserControl
    {
        #region Public members

        public IEasyDriverPlugin Driver { get; set; }
        public IDeviceCore Device => TagEdit.FindParent<IDeviceCore>(x => x is IDeviceCore);
        public ITagCore TagEdit { get; set; }
        public IHaveTag HaveTagObj => TagEdit.Parent as IHaveTag;
        public List<AccessPermission> AccessPermissionSource { get; set; }
        public List<IDataType> DataTypeSource { get; set; }

        #endregion

        #region Constructors

        public EditTagView(IEasyDriverPlugin driver, ITagCore tag)
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

            txbAddress.QuerySubmitted += OnAddressQuerySubmitted;
            txbAddress.ItemsSource = Extensions.AddressTypeSource;
            txbAddress.PopupClosed += (s, e) =>
            {
                txbAddress.SelectionStart = txbAddress.Text.Length;
                txbAddress.SelectionLength = 0;
            };

            KeyDown += OnKeyDown;
            btnOk.Click += BtnOk_Click;
            btnCancel.Click += BtnCancel_Click;
            Loaded += EditTagView_Loaded;
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

        private void OnAddressQuerySubmitted(object sender, DevExpress.Xpf.Editors.AutoSuggestEditQuerySubmittedEventArgs e)
        {
            txbAddress.ItemsSource = string.IsNullOrEmpty(e.Text) ? null :
                Extensions.AddressTypeSource.Where(x => x.StartsWith(e.Text, StringComparison.InvariantCultureIgnoreCase)).ToArray();
        }

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

                int dtIndex = DataTypeSource.FindIndex(x => x.Name == TagEdit.DataType.Name);
                if (dtIndex > -1)
                {
                    cobDataType.EditValue = DataTypeSource[dtIndex];
                }
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

            if (HaveTagObj.Tags.FirstOrDefault(x => x != TagEdit && (x as ICoreItem).Name == txbName.Text?.Trim()) != null)
            {
                DXMessageBox.Show($"The tag name '{txbName.Text?.Trim()}' is already in use.", "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string address = txbAddress.Text?.Trim();
            AccessPermission accessPermission = (AccessPermission)cobPermission.SelectedItem;
            string error = address.IsValidAddress( out bool isBitAddress);
            if (!string.IsNullOrEmpty(error))
            {
                DXMessageBox.Show(error, "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (isBitAddress)
            {
                if (cobDataType.SelectedItem != DataTypeSource.FirstOrDefault(x => x.Name == "Bool"))
                {
                    DXMessageBox.Show($"The current address only support read and write Bool data type.", "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
            {
                if (cobDataType.SelectedItem == DataTypeSource.FirstOrDefault(x => x.Name == "Bool"))
                {
                    DXMessageBox.Show($"The current address doesn't support read and write Bool data type.", "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            TagEdit.Address = address?.ToUpper();
            TagEdit.RefreshRate = (int)spnScanRate.Value;
            TagEdit.Gain = (double)spnGain.Value;
            TagEdit.Offset = (double)spnOffset.Value;
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
