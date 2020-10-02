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

namespace EasyDriver.ModbusTCP
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
        public IChannelCore Channel => Device.FindParent<IChannelCore>(x => x is IChannelCore) as IChannelCore;

        public ObservableCollection<ReadBlockSetting> ReadInputContacts { get; set; }
        public ObservableCollection<ReadBlockSetting> ReadOutputCoils { get; set; }
        public ObservableCollection<ReadBlockSetting> ReadInputRegisters { get; set; }
        public ObservableCollection<ReadBlockSetting> ReadHoldingRegisters { get; set; }

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

        private void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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
                txbIpAddress.Text = Device.ParameterContainer.Parameters["IpAddress"]?.ToString();
                if (decimal.TryParse(Device.ParameterContainer.Parameters["Timeout"], out decimal timeout))
                    spnTimeout.Value = timeout;
                cobByteOrder.SelectedItem = Device.ByteOrder;
                if (decimal.TryParse(Device.ParameterContainer.Parameters["TryReadWriteTimes"], out decimal tryReadWriteTimes))
                    spnTryReadWrite.Value = tryReadWriteTimes;
                if (decimal.TryParse(Device.ParameterContainer.Parameters["UnitId"], out decimal unitId))
                    spnUnitId.Value = unitId;
                if (decimal.TryParse(Device.ParameterContainer.Parameters["DelayBetweenPool"], out decimal delayPool))
                    spnDelayPool.Value = delayPool;
                if (decimal.TryParse(Device.ParameterContainer.Parameters["ScanRate"], out decimal scanRate))
                    spnScanRate.Value = scanRate;

                if (Device.ParameterContainer.Parameters.ContainsKey("ReadInputContactsBlockSetting"))
                {
                    ObservableCollection<ReadBlockSetting> blockSettings = new ObservableCollection<ReadBlockSetting>();
                    string readBlockStr = Device.ParameterContainer.Parameters["ReadInputContactsBlockSetting"].ToString();
                    if (!string.IsNullOrWhiteSpace(readBlockStr))
                    {
                        string[] blockSplit = readBlockStr.Split('|');
                        foreach (var item in blockSplit)
                        {
                            ReadBlockSetting block = ReadBlockSetting.Convert(item);
                            if (block != null)
                            {
                                block.AddressType = AddressType.InputContact;
                                blockSettings.Add(block);
                            }
                        }
                    }
                    ReadInputContacts = blockSettings;
                }
                else
                {
                    ReadInputContacts = new ObservableCollection<ReadBlockSetting>();
                }

                if (Device.ParameterContainer.Parameters.ContainsKey("ReadOutputCoilsBlockSetting"))
                {
                    ObservableCollection<ReadBlockSetting> blockSettings = new ObservableCollection<ReadBlockSetting>();
                    string readBlockStr = Device.ParameterContainer.Parameters["ReadOutputCoilsBlockSetting"].ToString();
                    if (!string.IsNullOrWhiteSpace(readBlockStr))
                    {
                        string[] blockSplit = readBlockStr.Split('|');
                        foreach (var item in blockSplit)
                        {
                            ReadBlockSetting block = ReadBlockSetting.Convert(item);
                            if (block != null)
                            {
                                block.AddressType = AddressType.OutputCoil;
                                blockSettings.Add(block);
                            }
                        }
                    }
                    ReadOutputCoils = blockSettings;
                }
                else
                {
                    ReadOutputCoils = new ObservableCollection<ReadBlockSetting>();
                }

                if (Device.ParameterContainer.Parameters.ContainsKey("ReadInputRegistersBlockSetting"))
                {
                    ObservableCollection<ReadBlockSetting> blockSettings = new ObservableCollection<ReadBlockSetting>();
                    string readBlockStr = Device.ParameterContainer.Parameters["ReadInputRegistersBlockSetting"].ToString();
                    if (!string.IsNullOrWhiteSpace(readBlockStr))
                    {
                        string[] blockSplit = readBlockStr.Split('|');
                        foreach (var item in blockSplit)
                        {
                            ReadBlockSetting block = ReadBlockSetting.Convert(item);
                            if (block != null)
                            {
                                block.AddressType = AddressType.InputRegister;
                                blockSettings.Add(block);
                            }
                        }
                    }
                    ReadInputRegisters = blockSettings;
                }
                else
                {
                    ReadInputRegisters = new ObservableCollection<ReadBlockSetting>();
                }

                if (Device.ParameterContainer.Parameters.ContainsKey("ReadHoldingRegistersBlockSetting"))
                {
                    ObservableCollection<ReadBlockSetting> blockSettings = new ObservableCollection<ReadBlockSetting>();
                    string readBlockStr = Device.ParameterContainer.Parameters["ReadHoldingRegistersBlockSetting"].ToString();
                    if (!string.IsNullOrWhiteSpace(readBlockStr))
                    {
                        string[] blockSplit = readBlockStr.Split('|');
                        foreach (var item in blockSplit)
                        {
                            ReadBlockSetting block = ReadBlockSetting.Convert(item);
                            if (block != null)
                            {
                                block.AddressType = AddressType.HoldingRegister;
                                blockSettings.Add(block);
                            }
                        }
                    }
                    ReadHoldingRegisters = blockSettings;
                }
                else
                {
                    ReadHoldingRegisters = new ObservableCollection<ReadBlockSetting>();
                }

                blockInputContacts.InitBlockSettings(ReadInputContacts, AddressType.InputContact);
                blockOutputCoils.InitBlockSettings(ReadOutputCoils, AddressType.OutputCoil);
                blockInputRegisters.InitBlockSettings(ReadInputRegisters, AddressType.InputRegister);
                blockHoldingRegisters.InitBlockSettings(ReadHoldingRegisters, AddressType.HoldingRegister);
            }

        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (Validate())
            {
                Device.Name = txbName.Text?.Trim();
                Device.ParameterContainer.DisplayName = "ModbusRTU Device Parameter";
                Device.ParameterContainer.DisplayParameters = "ModbusRTU Device Parameter";

                Device.ParameterContainer.Parameters["IpAddress"] = txbIpAddress.Text?.Trim();
                Device.ParameterContainer.Parameters["Timeout"] = spnTimeout.Value.ToString();
                Device.ByteOrder = (ByteOrder)Enum.Parse(typeof(ByteOrder), cobByteOrder.SelectedItem.ToString());
                Device.ParameterContainer.Parameters["TryReadWriteTimes"] = spnTryReadWrite.Value.ToString();
                Device.ParameterContainer.Parameters["UnitId"] = spnUnitId.Value.ToString();
                Device.ParameterContainer.Parameters["DelayBetweenPool"] = spnDelayPool.Value.ToString();
                Device.ParameterContainer.Parameters["ScanRate"] = spnScanRate.Value.ToString();

                DisableErrorBlockSettings(blockInputContacts.ReadBlockSettings);
                DisableErrorBlockSettings(blockOutputCoils.ReadBlockSettings);
                DisableErrorBlockSettings(blockInputRegisters.ReadBlockSettings);
                DisableErrorBlockSettings(blockHoldingRegisters.ReadBlockSettings);

                Device.ParameterContainer.Parameters["ReadInputContactsBlockSetting"] = ConvertBlockSettingsIntoString(blockInputContacts.ReadBlockSettings);
                Device.ParameterContainer.Parameters["ReadOutputCoilsBlockSetting"] = ConvertBlockSettingsIntoString(blockOutputCoils.ReadBlockSettings);
                Device.ParameterContainer.Parameters["ReadInputRegistersBlockSetting"] = ConvertBlockSettingsIntoString(blockInputRegisters.ReadBlockSettings);
                Device.ParameterContainer.Parameters["ReadHoldingRegistersBlockSetting"] = ConvertBlockSettingsIntoString(blockHoldingRegisters.ReadBlockSettings);

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
                DXMessageBox.Show(validateResult, "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (Channel.Childs.FirstOrDefault(x => x != Device && (x as ICoreItem).Name == txbName.Text?.Trim()) != null)
            {
                DXMessageBox.Show($"The device name '{txbName.Text?.Trim()}' is already in use.", "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!txbIpAddress.Text.IsIpAddress())
            {
                DXMessageBox.Show($"The ip address was not in correct format.", "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
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
