﻿using DevExpress.Xpf.Core;
using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Text;
using EasyDriver.Core;

namespace EasyDriver.ModbusRTU
{
    /// <summary>
    /// Interaction logic for CreateDeviceView.xaml
    /// </summary>
    public partial class CreateDeviceView : UserControl
    {
        #region Public members

        public IEasyDriverPlugin Driver { get; set; }
        public IChannelCore Channel { get; set; }
        public List<ByteOrder> ByteOrderSource { get; set; }
        public ObservableCollection<ReadBlockSetting> ReadInputContacts { get; set; }
        public ObservableCollection<ReadBlockSetting> ReadOutputCoils { get; set; }
        public ObservableCollection<ReadBlockSetting> ReadInputRegisters { get; set; }
        public ObservableCollection<ReadBlockSetting> ReadHoldingRegisters { get; set; }

        #endregion

        #region Constructors

        public CreateDeviceView(IEasyDriverPlugin driver, IChannelCore channel, IDeviceCore templateItem)
        {
            Driver = driver;
            Channel = channel;

            InitializeComponent();

            ByteOrderSource = Enum.GetValues(typeof(ByteOrder)).Cast<ByteOrder>().ToList();
            cobByteOrder.ItemsSource = ByteOrderSource;
            cobByteOrder.SelectedItem = ByteOrder.ABCD;

            if (templateItem == null)
            {
                txbName.Text = channel.GetUniqueNameInGroup("Device1");
                ReadInputContacts = new ObservableCollection<ReadBlockSetting>();
                ReadOutputCoils = new ObservableCollection<ReadBlockSetting>();
                ReadInputRegisters = new ObservableCollection<ReadBlockSetting>();
                ReadHoldingRegisters = new ObservableCollection<ReadBlockSetting>();
            }
            else
            {
                txbName.Text = channel.GetUniqueNameInGroup(templateItem.Name);

                if (templateItem.ParameterContainer.Parameters.ContainsKey("Timeout"))
                    spnTimeout.EditValue = templateItem.ParameterContainer.Parameters["Timeout"];
                if (templateItem.ParameterContainer.Parameters.ContainsKey("ByteOrder"))
                    cobByteOrder.SelectedItem = templateItem.ParameterContainer.Parameters["ByteOrder"];
                if (templateItem.ParameterContainer.Parameters.ContainsKey("TryReadWriteTimes"))
                    spnTryReadWrite.EditValue = templateItem.ParameterContainer.Parameters["TryReadWriteTimes"];
                if (templateItem.ParameterContainer.Parameters.ContainsKey("DeviceId"))
                    spnDeviceId.EditValue = templateItem.ParameterContainer.Parameters["DeviceId"];

                if (templateItem.ParameterContainer.Parameters.ContainsKey("ReadInputContactsBlockSetting"))
                {
                    ObservableCollection<ReadBlockSetting> blockSettings = new ObservableCollection<ReadBlockSetting>();
                    string readBlockStr = templateItem.ParameterContainer.Parameters["ReadInputContactsBlockSetting"].ToString();
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

                if (templateItem.ParameterContainer.Parameters.ContainsKey("ReadOutputCoilsBlockSetting"))
                {
                    ObservableCollection<ReadBlockSetting> blockSettings = new ObservableCollection<ReadBlockSetting>();
                    string readBlockStr = templateItem.ParameterContainer.Parameters["ReadOutputCoilsBlockSetting"].ToString();
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

                if (templateItem.ParameterContainer.Parameters.ContainsKey("ReadInputRegistersBlockSetting"))
                {
                    ObservableCollection<ReadBlockSetting> blockSettings = new ObservableCollection<ReadBlockSetting>();
                    string readBlockStr = templateItem.ParameterContainer.Parameters["ReadInputRegistersBlockSetting"].ToString();
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

                if (templateItem.ParameterContainer.Parameters.ContainsKey("ReadHoldingRegistersBlockSetting"))
                {
                    ObservableCollection<ReadBlockSetting> blockSettings = new ObservableCollection<ReadBlockSetting>();
                    string readBlockStr = templateItem.ParameterContainer.Parameters["ReadHoldingRegistersBlockSetting"].ToString();
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
            }

            blockInputContacts.InitBlockSettings(ReadInputContacts, AddressType.InputContact);
            blockOutputCoils.InitBlockSettings(ReadOutputCoils, AddressType.OutputCoil);
            blockInputRegisters.InitBlockSettings(ReadInputRegisters, AddressType.InputRegister);
            blockHoldingRegisters.InitBlockSettings(ReadHoldingRegisters, AddressType.HoldingRegister);

            btnOk.Click += BtnOk_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        #endregion

        #region Event handlers

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            string validateResult = txbName.Text?.Trim().ValidateFileName("Device");
            if (!string.IsNullOrWhiteSpace(validateResult))
            {
                DXMessageBox.Show(validateResult, "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Channel.Childs.FirstOrDefault(x => (x as ICoreItem).Name == txbName.Text?.Trim()) != null)
            {
                DXMessageBox.Show($"The device name '{txbName.Text?.Trim()}' is already in use.", "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Channel.Childs.FirstOrDefault(x => (decimal)(x as IDeviceCore).ParameterContainer.Parameters["DeviceId"] == spnDeviceId.Value) != null)
            {
                DXMessageBox.Show($"The device id '{spnDeviceId.Value}' is already used in this device.", "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IDeviceCore device = new DeviceCore(Channel);
            device.Name = txbName.Text?.Trim();
            device.ParameterContainer.DisplayName = "ModbusRTU Device Parameter";
            device.ParameterContainer.DisplayParameters = "ModbusRTU Device Parameter";

            device.ParameterContainer.Parameters["Timeout"] = spnTimeout.Value;
            device.ParameterContainer.Parameters["ByteOrder"] = cobByteOrder.SelectedItem;
            device.ParameterContainer.Parameters["TryReadWriteTimes"] = spnTryReadWrite.Value;
            device.ParameterContainer.Parameters["DeviceId"] = spnDeviceId.Value;

            DisableErrorBlockSettings(blockInputContacts.ReadBlockSettings);
            DisableErrorBlockSettings(blockOutputCoils.ReadBlockSettings);
            DisableErrorBlockSettings(blockInputRegisters.ReadBlockSettings);
            DisableErrorBlockSettings(blockHoldingRegisters.ReadBlockSettings);

            device.ParameterContainer.Parameters["ReadInputContactsBlockSetting"] = ConvertBlockSettingsIntoString(blockInputContacts.ReadBlockSettings);
            device.ParameterContainer.Parameters["ReadOutputCoilsBlockSetting"] = ConvertBlockSettingsIntoString(blockOutputCoils.ReadBlockSettings);
            device.ParameterContainer.Parameters["ReadInputRegistersBlockSetting"] = ConvertBlockSettingsIntoString(blockInputRegisters.ReadBlockSettings);
            device.ParameterContainer.Parameters["ReadHoldingRegistersBlockSetting"] = ConvertBlockSettingsIntoString(blockHoldingRegisters.ReadBlockSettings);

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
