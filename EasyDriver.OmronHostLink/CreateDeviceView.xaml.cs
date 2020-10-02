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

namespace EasyDriver.OmronHostLink
{
    /// <summary>
    /// Interaction logic for CreateDeviceView.xaml
    /// </summary>
    public partial class CreateDeviceView : UserControl
    {
        #region Public members

        public IEasyDriverPlugin Driver { get; set; }
        public IGroupItem ParentItem { get; set; }
        public IGroupItem Channel => ParentItem.FindParent<IChannelCore>(x => x is IChannelCore) as IChannelCore;
        public List<ByteOrder> ByteOrderSource { get; set; }
        public ObservableCollection<ReadBlockSetting> ReadBlockSettings { get; set; }

        #endregion

        #region Constructors

        public CreateDeviceView(IEasyDriverPlugin driver, IGroupItem parentItem, IDeviceCore templateItem)
        {
            Driver = driver;
            ParentItem = parentItem;

            InitializeComponent();

            ByteOrderSource = Enum.GetValues(typeof(ByteOrder)).Cast<ByteOrder>().ToList();
            cobByteOrder.ItemsSource = ByteOrderSource;
            cobByteOrder.SelectedItem = ByteOrder.CDAB;

            if (templateItem == null)
            {
                txbName.Text = parentItem.GetUniqueNameInGroup("Device1");
                ReadBlockSettings = new ObservableCollection<ReadBlockSetting>();
            }
            else
            {
                txbName.Text = parentItem.GetUniqueNameInGroup(templateItem.Name);

                if (templateItem.ParameterContainer.Parameters.ContainsKey("Timeout"))
                    spnTimeout.EditValue = templateItem.ParameterContainer.Parameters["Timeout"];
                if (templateItem.ParameterContainer.Parameters.ContainsKey("TryReadWriteTimes"))
                    spnTryReadWrite.EditValue = templateItem.ParameterContainer.Parameters["TryReadWriteTimes"];
                if (templateItem.ParameterContainer.Parameters.ContainsKey("UnitNo"))
                    spnUnitNo.EditValue = templateItem.ParameterContainer.Parameters["UnitNo"];

                cobByteOrder.SelectedItem = templateItem.ByteOrder;

                if (templateItem.ParameterContainer.Parameters.ContainsKey("ReadBlockSettings"))
                {
                    ObservableCollection<ReadBlockSetting> blockSettings = new ObservableCollection<ReadBlockSetting>();
                    string readBlockStr = templateItem.ParameterContainer.Parameters["ReadBlockSettings"].ToString();
                    if (!string.IsNullOrWhiteSpace(readBlockStr))
                    {
                        string[] blockSplit = readBlockStr.Split('|');
                        foreach (var item in blockSplit)
                        {
                            ReadBlockSetting block = ReadBlockSetting.Convert(item);
                            if (block != null)
                                blockSettings.Add(block);
                        }
                    }
                    ReadBlockSettings = blockSettings;
                }
            }

            blockSettingView.InitBlockSettings(ReadBlockSettings);
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

            if (ParentItem.Childs.FirstOrDefault(x => (x as ICoreItem).Name == txbName.Text?.Trim()) != null)
            {
                DXMessageBox.Show($"The device name '{txbName.Text?.Trim()}' is already in use.", "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var childDevices = Channel.GetAllDevices();
            if (childDevices.FirstOrDefault(x => (x as IDeviceCore).ParameterContainer.Parameters["UnitNo"] == spnUnitNo.Value.ToString()) != null)
            {
                DXMessageBox.Show($"The unit no '{spnUnitNo.Value}' is already used in this device.", "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IDeviceCore device = new DeviceCore(ParentItem);
            device.Name = txbName.Text?.Trim();
            device.ParameterContainer.DisplayName = "Omron Host Link Device Parameter";
            device.ParameterContainer.DisplayParameters = "Omron Host Link Device Parameter";

            device.ParameterContainer.Parameters["Timeout"] = spnTimeout.Value.ToString();
            device.ParameterContainer.Parameters["TryReadWriteTimes"] = spnTryReadWrite.Value.ToString();
            device.ParameterContainer.Parameters["UnitNo"] = spnUnitNo.Value.ToString();

            device.ByteOrder = (ByteOrder)Enum.Parse(typeof(ByteOrder), cobByteOrder.SelectedItem.ToString());
            DisableErrorBlockSettings(blockSettingView.ReadBlockSettings);

            device.ParameterContainer.Parameters["ReadBlockSettings"] = ConvertBlockSettingsIntoString(blockSettingView.ReadBlockSettings);

            ((Parent as FrameworkElement).Parent as Window).Tag = device;
            ((Parent as FrameworkElement).Parent as Window).Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ((Parent as FrameworkElement).Parent as Window).Close();
        }

        private string ConvertBlockSettingsIntoString(ObservableCollection<ReadBlockSetting> readBlockSettings)
        {
            if (readBlockSettings.Count <= 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            foreach (var block in readBlockSettings)
            {
                sb.Append($"{block.ToString()}|");
            }
            string result = sb.ToString();
            if (result.EndsWith("|"))
                result = result.Remove(result.Length - 1, 1);
            return result;
        }

        private void DisableErrorBlockSettings(ObservableCollection<ReadBlockSetting> readBlockSettings)
        {
            foreach (var item in readBlockSettings)
            {
                if (!string.IsNullOrWhiteSpace(item.Error))
                    item.Enabled = false;
            }
        }

        #endregion
    }
}
