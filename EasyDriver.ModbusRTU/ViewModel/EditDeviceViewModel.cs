using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriverPlugin;
using Newtonsoft.Json;
using System;
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
        public virtual int Timeout { get; set; } = 1000;
        public virtual int TryReadWriteTimes { get; set; } = 3;
        public virtual int DeviceId { get; set; } = 1;
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
                DeviceId = Device.DeviceId;
                Timeout = Device.Timeout;
                TryReadWriteTimes = Device.TryReadWriteTimes;
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
            Device.DeviceId = DeviceId;
            Device.Timeout = Timeout;
            Device.TryReadWriteTimes = TryReadWriteTimes;
            Device.ByteOrder = ByteOrder;
            Device.Description = Description;
            Device.ReadMode = ReadMode;

            UpdateReadBlockSettings(Device.InputContactReadBlockSettings, InputContactReadBlockSettings);
            UpdateReadBlockSettings(Device.OutputCoilsReadBlockSettings, OutputCoilsReadBlockSettings);
            UpdateReadBlockSettings(Device.InputRegisterReadBlockSettings, InputRegisterReadBlockSettings);
            UpdateReadBlockSettings(Device.HoldingRegisterReadBlockSettings, HoldingRegisterReadBlockSettings);

            Device.SaveBlockSetting();
            CurrentWindowService.Close();
        }
        public bool CanSave()
        {
            return true;
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
        #endregion

        #region Methods 
        private void UpdateReadBlockSettings(ObservableCollection<ReadBlockSetting> target, ObservableCollection<ReadBlockSetting> source)
        {
            int index = -1;
            int maxCount = source.Count > target.Count ? source.Count : target.Count;
            for (int i = 0; i < maxCount; i++)
            {
                index = i;

                if (i < target.Count && i < source.Count)
                {
                    ReadBlockSetting settingTarget = target[i];
                    ReadBlockSetting settingSource = source[i];

                    if (settingTarget.Enabled != settingSource.Enabled ||
                        settingTarget.StartAddress != settingSource.StartAddress ||
                        settingTarget.EndAddress != settingSource.EndAddress)
                    {
                        settingTarget.BeginEdit();
                        settingTarget.Enabled = settingSource.Enabled;
                        settingTarget.StartAddress = settingSource.StartAddress;
                        settingTarget.EndAddress = settingSource.EndAddress;
                        settingTarget.EndEdit();
                    }
                }
                else
                    break;
            }

            if (index < 0)
                target.Clear();
            else
            {
                if (source.Count < target.Count)
                {
                    if (index < target.Count)
                    {
                        for (int i = target.Count - 1; i >= index; i--)
                        {
                            target.RemoveAt(i);
                        }
                    }
                }
                else if (source.Count > target.Count)
                {
                    for (int i = index; i < source.Count; i++)
                    {
                        target.Add(source[i]);
                    }
                }
            }
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
                        if (DeviceId < 0 || DeviceId > 254)
                            Error = "The device id is out of range. The valid range is 0 - 254";
                        else
                        {
                            if (Parent.Childs.FirstOrDefault(x => {
                                if (x is Device device)
                                    return device.DeviceId == DeviceId && device != Device;
                                return false;
                            }) is Device anotherDevice)
                            {
                                Error = $"The device id '{DeviceId}' is already used in device '{anotherDevice.Name}'";
                            }
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
