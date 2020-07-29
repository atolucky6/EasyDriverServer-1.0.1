using System.Collections.Generic;
using EasyDriverPlugin;
using System.Windows.Controls;
using System;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;
using EasyDriver.Core;
using System.Windows.Input;

namespace EasyDriver.OmronHostLink
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
            string error = address.IsValidAddress(out bool isBitAddress);
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

            address.DecomposeAddress(out AddressType adrType, out ushort wordOffset, out ushort bitOffset);
            string adrTypeString = adrType.ToString();
            for (int i = 0; i < spnAutoCreateCount.Value; i++)
            {
                string adrString = adrTypeString;
                if (isBitAddress)
                {
                    adrString += (wordOffset + bitOffset / 16).ToString();
                    adrString += ".";
                    adrString += (bitOffset % 16).ToString();
                    bitOffset++;
                }
                else
                {
                    adrString += wordOffset;
                    wordOffset++;
                }


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
