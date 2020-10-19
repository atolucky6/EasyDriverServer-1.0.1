using System.Collections.Generic;
using EasyDriverPlugin;
using System.Windows.Controls;
using System;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;
using EasyDriver.Core;
using System.Windows.Input;

namespace EasyDriver.ModbusRTU
{
    /// <summary>
    /// Interaction logic for CreateTagView.xaml
    /// </summary>
    public partial class CreateTagView : UserControl
    {
        #region Public members

        public IEasyDriverPlugin Driver { get; set; }
        public IGroupItem ParentItem { get; set; }
        public IHaveTag HaveTagObj { get => ParentItem as IHaveTag; }
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

            DataTypeSource = driver.GetSupportDataTypes().ToList();
            cobDataType.ItemsSource = DataTypeSource;
            cobDataType.DisplayMember = "Name";
            cobDataType.SelectedIndex = 0;

            if (templateItem == null)
            {
                txbName.Text = HaveTagObj.GetUniqueNameInGroupTags("Tag1");
                txbAddress.Text = "1";
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
            string validateResult = txbName.Text?.Trim().ValidateFileName("Tag");
            if (!string.IsNullOrWhiteSpace(validateResult))
            {
                DXMessageBox.Show(validateResult, "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (HaveTagObj.Tags.FirstOrDefault(x => (x as ICoreItem).Name == txbName.Text?.Trim()) != null)
            {
                DXMessageBox.Show($"The tag name '{txbName.Text?.Trim()}' is already in use.", "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string address = txbAddress.Text?.Trim();
            AccessPermission accessPermission = (AccessPermission)cobPermission.SelectedItem;
            string error = address.IsValidAddress(cobDataType.SelectedItem as IDataType,  accessPermission, out bool isBitAddress);
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

                ITagCore tag = new TagCore(ParentItem)
                {
                    Name = HaveTagObj.GetUniqueNameInGroupTags(string.Format(nameFormat, number)),
                    AccessPermission = accessPermission,
                    DataType = (IDataType)cobDataType.SelectedItem,
                    Address = adrString,
                    RefreshRate = (int)spnRefreshRate.Value,
                    Gain = (double)spnGain.Value,
                    Offset = (double)spnOffset.Value,
                    ByteOrder = ParentItem.FindParent<IDeviceCore>(x => x is IDeviceCore).ByteOrder
                };

                if (i == 0 && !hasValue && spnAutoCreateCount.Value == 1)
                    tag.Name = currentTagName;

                number++;
                tag.ParameterContainer.DisplayName = "Tag Parameters";
                tag.ParameterContainer.DisplayParameters = "Tag Parameters";
                currentTagName = tag.Name;
                createTags.Add(tag);
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
