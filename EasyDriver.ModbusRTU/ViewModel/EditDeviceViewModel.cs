using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriverPlugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace EasyDriver.ModbusRTU
{
    public class EditDeviceViewModel : IDataErrorInfo
    {
        #region Public properties
        public virtual ModbusRTUDriver Driver { get; set; }
        public virtual Device Device { get; set; }
        public virtual IGroupItem Parent { get => Device.Parent; }

        public virtual string Name { get; set; }
        public virtual string Timeout { get; set; } = "1000";
        public virtual string TryReadWriteTimes { get; set; } = "3";
        public virtual string DeviceId { get; set; } = "1";
        public virtual ByteOrder ByteOrder { get; set; } = ByteOrder.ABCD;
        public virtual ReadMode ReadMode { get; set; } = ReadMode.Block;
        public virtual ObservableCollection<ReadBlockSetting> InputContactReadBlockSettings { get; set; }
        public virtual ObservableCollection<ReadBlockSetting> OutputCoilsReadBlockSettings { get; set; }
        public virtual ObservableCollection<ReadBlockSetting> InputRegisterReadBlockSettings { get; set; }
        public virtual ObservableCollection<ReadBlockSetting> HoldingRegisterReadBlockSettings { get; set; }
        public virtual string Description { get; set; }

        public virtual ReadBlockSettingViewModel InputContactReadBlockSettingsViewModel { get; set; }
        public virtual ReadBlockSettingViewModel OutputCoilsReadBlockSettingsViewModel { get; set; }
        public virtual ReadBlockSettingViewModel InputRegisterReadBlockSettingsViewModel { get; set; }
        public virtual ReadBlockSettingViewModel HoldingRegisterReadBlockSettingsViewModel { get; set; }
        #endregion

        #region Services
        protected ICurrentWindowService CurrentWindowService { get => this.GetService<ICurrentWindowService>(); }
        protected IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }
        protected ISaveFileDialogService SaveFileDialogService { get => this.GetService<ISaveFileDialogService>(); }
        protected IOpenFileDialogService OpenFileDialogService { get => this.GetService<IOpenFileDialogService>(); }
        #endregion

        #region Constructors
        public EditDeviceViewModel(ModbusRTUDriver driver, IDeviceCore deviceCore)
        {
            Device = deviceCore as Device;
            Driver = driver;

            InputContactReadBlockSettings= CloneReadBlockSettings(Device.InputContactReadBlockSettings);
            OutputCoilsReadBlockSettings = CloneReadBlockSettings(Device.OutputCoilsReadBlockSettings);
            InputRegisterReadBlockSettings = CloneReadBlockSettings(Device.InputRegisterReadBlockSettings);
            HoldingRegisterReadBlockSettings=  CloneReadBlockSettings(Device.HoldingRegisterReadBlockSettings);

            InputContactReadBlockSettingsViewModel = ViewModelSource.Create(() => new ReadBlockSettingViewModel(AddressType.InputContact, InputContactReadBlockSettings)).SetParentViewModel(this);
            OutputCoilsReadBlockSettingsViewModel = ViewModelSource.Create(() => new ReadBlockSettingViewModel(AddressType.OutputCoil, OutputCoilsReadBlockSettings)).SetParentViewModel(this);
            InputRegisterReadBlockSettingsViewModel = ViewModelSource.Create(() => new ReadBlockSettingViewModel(AddressType.InputRegister, InputRegisterReadBlockSettings)).SetParentViewModel(this);
            HoldingRegisterReadBlockSettingsViewModel = ViewModelSource.Create(() => new ReadBlockSettingViewModel(AddressType.HoldingRegister, HoldingRegisterReadBlockSettings)).SetParentViewModel(this);

            if (Device != null)
            {
                Name = Device.Name;
                DeviceId = Device.DeviceId.ToString();
                Timeout = Device.Timeout.ToString();
                TryReadWriteTimes = Device.TryReadWriteTimes.ToString();
                ByteOrder = Device.ByteOrder;
                ReadMode = Device.ReadMode;
                Description = Device.Description;
            }
        }
        #endregion

        #region Commands
        public void Save()
        {
            Device.Name = Name;
            Device.DeviceId = int.Parse(DeviceId);
            Device.Timeout = int.Parse(Timeout);
            Device.TryReadWriteTimes = int.Parse(TryReadWriteTimes);
            Device.ByteOrder = ByteOrder;
            Device.Description = Description;
            Device.ReadMode = ReadMode;

            Device.UpdateReadBlockSettings(Device.InputContactReadBlockSettings, InputContactReadBlockSettings);
            Device.UpdateReadBlockSettings(Device.OutputCoilsReadBlockSettings, OutputCoilsReadBlockSettings);
            Device.UpdateReadBlockSettings(Device.InputRegisterReadBlockSettings, InputRegisterReadBlockSettings);
            Device.UpdateReadBlockSettings(Device.HoldingRegisterReadBlockSettings, HoldingRegisterReadBlockSettings);

            foreach (var item in Device.UndefinedTags.ToArray())
            {
                item.FindAndRegisterReadBlock();
            }

            Device.SaveBlockSetting();
            CurrentWindowService.Close();
        }
        public bool CanSave()
        {
            return string.IsNullOrWhiteSpace(Error);
        }

        public void Cancel()
        {
            CurrentWindowService.Close();
        }

        public void ImportSettings()
        {
            try
            {
                OpenFileDialogService.Title = "Import";
                OpenFileDialogService.Filter = "Json file (*.json)|*.json";
                if (OpenFileDialogService.ShowDialog())
                {
                    string jsonImport = File.ReadAllText(OpenFileDialogService.File.GetFullName());
                    DeviceBlockSetting deviceBlockSetting = JsonConvert.DeserializeObject<DeviceBlockSetting>(jsonImport);
                    if (deviceBlockSetting != null)
                    {
                        if (deviceBlockSetting.InputContactReadBlockSettings != null)
                        {
                            InputContactReadBlockSettings.Clear();
                            foreach (var item in deviceBlockSetting.InputContactReadBlockSettings)
                                InputContactReadBlockSettings.Add(item);
                        }

                        if (deviceBlockSetting.OutputCoilsReadBlockSettings != null)
                        {
                            OutputCoilsReadBlockSettings.Clear();
                            foreach (var item in deviceBlockSetting.OutputCoilsReadBlockSettings)
                                OutputCoilsReadBlockSettings.Add(item);
                        }

                        if (deviceBlockSetting.InputRegisterReadBlockSettings != null)
                        {
                            InputRegisterReadBlockSettings.Clear();
                            foreach (var item in deviceBlockSetting.InputRegisterReadBlockSettings)
                                InputRegisterReadBlockSettings.Add(item);
                        }

                        if (deviceBlockSetting.HoldingRegisterReadBlockSettings != null)
                        {
                            HoldingRegisterReadBlockSettings.Clear();
                            foreach (var item in deviceBlockSetting.HoldingRegisterReadBlockSettings)
                                HoldingRegisterReadBlockSettings.Add(item);
                        }

                        MessageBoxService.ShowMessage($"Import read block settings success!", "Message", MessageButton.OK, MessageIcon.Information);
                    }
                    else
                    {
                        MessageBoxService.ShowMessage($"Can't convert content file into read block settings.", "Error", MessageButton.OK, MessageIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowMessage($"Error when import read block settings. '{ex.Message}'", "Error", MessageButton.OK, MessageIcon.Error);
            }
        }

        public void ExportSettings()
        {
            try
            {
                SaveFileDialogService.Title = "Export";
                SaveFileDialogService.DefaultFileName = $"device_{Name}_modbus_rtu_readblocksettings.json";
                SaveFileDialogService.Filter = "Json file (*.json)|*.json";
                if (SaveFileDialogService.ShowDialog())
                {
                    DeviceBlockSetting deviceBlockSetting = new DeviceBlockSetting()
                    {
                        InputContactReadBlockSettings = InputContactReadBlockSettings,
                        OutputCoilsReadBlockSettings = OutputCoilsReadBlockSettings,
                        InputRegisterReadBlockSettings = InputRegisterReadBlockSettings,
                        HoldingRegisterReadBlockSettings = HoldingRegisterReadBlockSettings
                    };
                    string jsonExport = JsonConvert.SerializeObject(deviceBlockSetting, Formatting.Indented);
                    File.WriteAllText(SaveFileDialogService.File.GetFullName(), jsonExport);
                    MessageBoxService.ShowMessage($"Export read block settings success!", "Message", MessageButton.OK, MessageIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowMessage($"Error when export read block settings. '{ex.Message}'", "Error", MessageButton.OK, MessageIcon.Error);
            }
        }

        public void AutoDetectSetting()
        {
            try
            {
                List<Tag> inputContactTags = new List<Tag>();
                List<Tag> outputCoilTags = new List<Tag>();
                List<Tag> inputRegisterTags = new List<Tag>();
                List<Tag> holdingRegisterTags = new List<Tag>();
                foreach (var item in Device.GetAllTags())
                {
                    if (item is Tag tag)
                    {
                        switch (tag.AddressType)
                        {
                            case AddressType.OutputCoil:
                                outputCoilTags.Add(tag);
                                break;
                            case AddressType.InputContact:
                                inputContactTags.Add(tag);
                                break;
                            case AddressType.InputRegister:
                                inputRegisterTags.Add(tag);
                                break;
                            case AddressType.HoldingRegister:
                                holdingRegisterTags.Add(tag);
                                break;
                            default:
                                break;
                        }
                    }
                }

                ObservableCollection<ReadBlockSetting> inputContactSettings = 
                    new ObservableCollection<ReadBlockSetting>(
                        CreateReadBlockSettings(inputContactTags.OrderBy(x => x.AddressOffset).ToList()));

                ObservableCollection<ReadBlockSetting> outputCoilSettings = 
                    new ObservableCollection<ReadBlockSetting>(
                        CreateReadBlockSettings(outputCoilTags.OrderBy(x => x.AddressOffset).ToList()));

                ObservableCollection<ReadBlockSetting> inputRegisterSettings = 
                    new ObservableCollection<ReadBlockSetting>(
                        CreateReadBlockSettings(inputRegisterTags.OrderBy(x => x.AddressOffset).ToList()));

                ObservableCollection<ReadBlockSetting> holdingRegisterSettings = 
                    new ObservableCollection<ReadBlockSetting>(
                        CreateReadBlockSettings(holdingRegisterTags.OrderBy(x => x.AddressOffset).ToList()));

                Device.UpdateReadBlockSettings(InputContactReadBlockSettings, inputContactSettings);
                Device.UpdateReadBlockSettings(OutputCoilsReadBlockSettings, outputCoilSettings);
                Device.UpdateReadBlockSettings(InputRegisterReadBlockSettings, inputRegisterSettings);
                Device.UpdateReadBlockSettings(HoldingRegisterReadBlockSettings, holdingRegisterSettings);
            }
            catch { }
        }

        public bool CanAutoDetectSetting()
        {
            return Device != null;
        }
        #endregion

        #region Methods 
        private List<ReadBlockSetting> CreateReadBlockSettings(List<Tag> tags)
        {
            List<ReadBlockSetting> settings = new List<ReadBlockSetting>();
            if (tags.Count > 1)
            {
                AddressType addressType = tags[0].AddressType;
                int maxOffset = 0;
                switch (addressType)
                {
                    case AddressType.OutputCoil:
                    case AddressType.InputContact:
                        maxOffset = 2000;
                        break;
                    case AddressType.InputRegister:
                    case AddressType.HoldingRegister:
                        maxOffset = 125;
                        break;
                    default:
                        break;
                }
                int startAddress = tags[0].AddressOffset;
                int endAddressAndByteLength = tags[0].AddressOffset + (tags[0].RequireByteLength / 2) - 1;
                int endAddress = tags[0].AddressOffset;
                int lastEndAddress = tags[0].AddressOffset + (tags[0].RequireByteLength / 2) - 1;
                ReadBlockSetting currentSetting = null;
                for (int i = 1; i < tags.Count; i++)
                {
                    endAddress = tags[i].AddressOffset;
                    endAddressAndByteLength = tags[i].AddressOffset + (tags[i].RequireByteLength / 2) - 1;
                    int offset = endAddressAndByteLength - startAddress;
                    
                    if (offset < maxOffset)
                    {
                        if (currentSetting == null)
                        {
                            currentSetting = new ReadBlockSetting() { AddressType = addressType };
                            currentSetting.BeginEdit();
                        }

                        if (i == tags.Count - 1)
                        {
                            currentSetting.StartAddress = (100000 * (int)addressType + startAddress + 1).ToString();
                            currentSetting.EndAddress = (100000 * (int)addressType + endAddressAndByteLength + 1).ToString();
                            currentSetting.Enabled = true;
                            currentSetting.EndEdit();
                            settings.Add(currentSetting);
                            currentSetting = null;
                        }

                        lastEndAddress = endAddressAndByteLength;
                    }
                    else
                    {    
                        if (currentSetting != null)
                        {
                            currentSetting.StartAddress = (100000 * (int)addressType + startAddress + 1).ToString();
                            currentSetting.EndAddress = (100000 * (int)addressType + lastEndAddress + 1).ToString();
                            currentSetting.Enabled = true;
                            currentSetting.EndEdit();
                            settings.Add(currentSetting);
                            currentSetting = null;
                        }
                        startAddress = endAddress;
                    }
                }
            }
            return settings;
        }

        private ObservableCollection<ReadBlockSetting> CloneReadBlockSettings(ObservableCollection<ReadBlockSetting> source)
        {
            var target = new ObservableCollection<ReadBlockSetting>();
            foreach (var item in source)
            {
                ReadBlockSetting setting = new ReadBlockSetting();
                setting.BeginEdit();
                setting.Enabled = item.Enabled;
                setting.StartAddress = item.StartAddress;
                setting.EndAddress = item.EndAddress;
                setting.AddressType = item.AddressType;
                setting.EndEdit();
                target.Add(setting);
            }
            return target;
        }
        #endregion

        #region IDataErrorInfo
        public string Error { get; set; }

        public string this[string columnName]
        {
            get
            {
                Error = string.Empty;
                switch (columnName)
                {
                    case nameof(Name):
                        Error = Name?.Trim().ValidateFileName("Device");
                        if (string.IsNullOrEmpty(Error))
                        {
                            if (Parent.Childs.Any(x => x != Device && (x as ICoreItem).Name == Name?.Trim()))
                                Error = $"The device name '{Name}' is already in use.";
                        }
                        break;
                    case nameof(DeviceId):
                        if (int.TryParse(DeviceId, out int deviceId))
                        {
                            if (deviceId < 0 || deviceId > 255)
                                Error = $"Value is out of range.";
                            //else
                            //{
                            //    var channel = Parent.FindParent<Channel>(x => x is Channel);
                            //    foreach (var item in channel.GetAllDevices())
                            //    {
                            //        if (item is Device device)
                            //        {
                            //            if (device != Device && device.DeviceId == deviceId)
                            //            {
                            //                Error = $"The device id '{DeviceId}' is already used by device '{device.Name}'";
                            //                break;
                            //            }
                            //        }
                            //    }
                            //}
                        }
                        else
                        {
                            Error = $"Value must be a number.";
                        }
                        break;
                    case nameof(Timeout):
                        if (int.TryParse(Timeout, out int timeout))
                        {
                            if (!timeout.IsInRange(1000, int.MaxValue))
                                Error = $"Value is out of range.";
                        }
                        else
                        {
                            Error = $"Value must be a number.";
                        }
                        break;
                    case nameof(TryReadWriteTimes):
                        if (int.TryParse(TryReadWriteTimes, out int tryRead))
                        {
                            if (!tryRead.IsInRange(1, 99))
                                Error = $"Value is out of range.";
                        }
                        else
                        {
                            Error = $"Value must be a number.";
                        }
                        break;
                    default:
                        break;
                }
                return Error;
            }
        }
        #endregion
    }
}
