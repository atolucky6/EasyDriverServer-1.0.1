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
        public IGroupItem ParentItem { get; set; }
        public IHaveTag HaveTagObj => ParentItem as IHaveTag;
        public List<AccessPermission> AccessPermissionSource { get; set; }
        public List<IDataType> DataTypeSource { get; set; }

        #endregion

        #region Constructors

        public CreateTagView(IEasyDriverPlugin driver, IGroupItem parent, ITagCore templateItem)
        {
            Driver = driver;
            ParentItem = parent;

            InitializeComponent();

            AccessPermissionSource = Enum.GetValues(typeof(AccessPermission)).Cast<AccessPermission>().ToList();
            cobPermission.ItemsSource = AccessPermissionSource;
            cobPermission.SelectedItem = AccessPermission.ReadAndWrite;

            DataTypeSource = driver.SupportDataTypes;
            cobDataType.ItemsSource = DataTypeSource;
            cobDataType.DisplayMember = "Name";
            cobDataType.SelectedIndex = 0;

            if (templateItem == null)
            {
                txbName.Text = HaveTagObj.GetUniqueNameInGroupTags("Tag1");
            }
            else
            {
                txbName.Text = HaveTagObj.GetUniqueNameInGroupTags(templateItem.Name);
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
                DXMessageBox.Show(validateResult, "Message", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (HaveTagObj.Tags.FirstOrDefault(x => (x as ICoreItem).Name == txbName.Text?.Trim()) != null)
            {
                DXMessageBox.Show($"The tag name '{txbName.Text?.Trim()}' is already in use.", "Message", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string address = txbAddress.Text?.Trim();
            AccessPermission accessPermission = (AccessPermission)cobPermission.SelectedItem;
            string error = address.IsValidAddress(out bool isBitAddress);
            if (!string.IsNullOrEmpty(error))
            {
                DXMessageBox.Show(error, "Message", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (isBitAddress)
            {
                if (cobDataType.SelectedItem != DataTypeSource.FirstOrDefault(x => x.Name == "Bool"))
                {
                    DXMessageBox.Show($"The current address only support read and write Bool data type.", "Message", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
            {
                if (cobDataType.SelectedItem == DataTypeSource.FirstOrDefault(x => x.Name == "Bool"))
                {
                    DXMessageBox.Show($"The current address doesn't support read and write Bool data type.", "Message", MessageBoxButton.OK, MessageBoxImage.Warning);
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

                ITagCore tag = new TagCore(ParentItem)
                {
                    Name = HaveTagObj.GetUniqueNameInGroupTags(string.Format(nameFormat, number)),
                    AccessPermission = accessPermission,
                    DataType = (IDataType)cobDataType.SelectedItem,
                    Address = adrString,
                    RefreshRate = (int)spnRefreshRate.Value,
                    Gain = (double)spnGain.Value,
                    Offset = (double)spnOffset.Value,
                    ByteOrder = (ParentItem.FindParent<IDeviceCore>(x => x is IDeviceCore)).ByteOrder
                };

                if (i == 0 && !hasValue && spnAutoCreateCount.Value == 1)
                    tag.Name = currentTagName;

                tag.ParameterContainer.DisplayName = "Tag Parameters";
                tag.ParameterContainer.DisplayParameters = "Tag Parameters";
                currentTagName = tag.Name;
                createTags.Add(tag);
                number++;
            }

            this.Tag = createTags;
            ((Parent as FrameworkElement).Parent as Window).Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ((Parent as FrameworkElement).Parent as Window).Close();
        }

        #endregion
    }
}
