using DevExpress.Xpf.Core;
using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace EasyDriver.OmronHostLink
{
    /// <summary>
    /// Interaction logic for EditDeviceView.xaml
    /// </summary>
    public partial class EditDeviceView : UserControl
    {
        #region Public members

        public IEasyDriverPlugin Driver { get; set; }
        public List<ByteOrder> ByteOrderSource { get; set; }
        public IDeviceCore Device { get; set; }
        public IChannelCore Channel => Device.FindParent<IChannelCore>(x => x is IChannelCore);

        public ObservableCollection<ReadBlockSetting> ReadBlockSettings { get; set; }

        #endregion

        #region Constructors

        public EditDeviceView(IEasyDriverPlugin driver, IDeviceCore device)
        {
            Driver = driver;
            Device = device;

            InitializeComponent();

            ByteOrderSource = Enum.GetValues(typeof(ByteOrder)).Cast<ByteOrder>().ToList();
            cobByteOrder.ItemsSource = ByteOrderSource;
            cobByteOrder.SelectedItem = ByteOrder.ABCD;

            KeyDown += OnKeyDown;
            btnOk.Click += BtnOk_Click;
            btnCancel.Click += BtnCancel_Click;
            Loaded += EditDeviceView_Loaded;
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

        private void EditDeviceView_Loaded(object sender, RoutedEventArgs e)
        {
            if (Device != null)
            {
                txbName.Text = Device.Name;
                if (decimal.TryParse(Device.ParameterContainer.Parameters["Timeout"], out decimal timeout))
                    spnTimeout.Value = timeout;
                if (decimal.TryParse(Device.ParameterContainer.Parameters["TryReadWriteTimes"], out decimal tryReadWriteTimes))
                    spnTryReadWrite.Value = tryReadWriteTimes;
                if (decimal.TryParse(Device.ParameterContainer.Parameters["UnitNo"], out decimal unitNo))
                    spnUnitNo.Value = unitNo;
                cobByteOrder.SelectedItem = Device.ByteOrder;

                if (Device.ParameterContainer.Parameters.ContainsKey("ReadBlockSettings"))
                {
                    ObservableCollection<ReadBlockSetting> blockSettings = new ObservableCollection<ReadBlockSetting>();
                    string readBlockStr = Device.ParameterContainer.Parameters["ReadBlockSettings"].ToString();
                    if (!string.IsNullOrWhiteSpace(readBlockStr))
                    {
                        string[] blockSplit = readBlockStr.Split('|');
                        foreach (var item in blockSplit)
                        {
                            ReadBlockSetting block = ReadBlockSetting.Convert(item);
                            if (block != null)
                            {
                                blockSettings.Add(block);
                            }
                        }
                    }
                    ReadBlockSettings = blockSettings;
                }
                else
                {
                    ReadBlockSettings = new ObservableCollection<ReadBlockSetting>();
                }

                blockSettingView.InitBlockSettings(ReadBlockSettings);
            }

        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (Validate())
            {
                Device.Name = txbName.Text?.Trim();
                Device.ParameterContainer.DisplayName = "Omron Host Link Device Parameter";
                Device.ParameterContainer.DisplayParameters = "Omron Host Link Device Parameter";

                Device.ParameterContainer.SetValue("Timeout", spnTimeout.Value.ToString());
                Device.ParameterContainer.SetValue("TryReadWriteTimes", spnTryReadWrite.Value.ToString());
                Device.ParameterContainer.SetValue("UnitNo", spnUnitNo.Value.ToString());
                Device.ByteOrder = (ByteOrder)(Enum.Parse(typeof(ByteOrder), cobByteOrder.SelectedItem.ToString()));
                DisableErrorBlockSettings(blockSettingView.ReadBlockSettings);

                Device.ParameterContainer.SetValue("ReadBlockSettings", ConvertBlockSettingsIntoString(blockSettingView.ReadBlockSettings));
                ((Parent as FrameworkElement).Parent as Window).Tag = Device;
                ((Parent as FrameworkElement).Parent as Window).Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ((Parent as FrameworkElement).Parent as Window).Close();
        }

        #endregion

        #region Methods

        private bool Validate()
        {
            string validateResult = txbName.Text?.Trim().ValidateFileName("Device");
            if (!string.IsNullOrWhiteSpace(validateResult))
            {
                DXMessageBox.Show(validateResult, "Message", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (Device.Parent.Childs.FirstOrDefault(x => x != Device && (x as ICoreItem).Name == txbName.Text?.Trim()) != null)
            {
                DXMessageBox.Show($"The device name '{txbName.Text?.Trim()}' is already in use.", "Message", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            var deviceChilds = Channel.GetAllDevices();
            if (deviceChilds.FirstOrDefault(x => x != Device && (x as IDeviceCore).ParameterContainer.Parameters["DeviceId"] == spnUnitNo.Value.ToString()) != null)
            {
                DXMessageBox.Show($"The unit no'{spnUnitNo.Value}' is already used in this device.", "Message", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
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
