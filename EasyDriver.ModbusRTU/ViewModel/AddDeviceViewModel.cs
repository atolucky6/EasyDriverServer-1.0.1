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
    public class AddDeviceViewModel : IDataErrorInfo
    {
        #region Public properties
        public virtual ModbusRTUDriver Driver { get; set; }
        public virtual Device Device { get; set; }
        public virtual IGroupItem Parent { get; set; }
        
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
        public AddDeviceViewModel(ModbusRTUDriver driver, IGroupItem parent, IDeviceCore itemTemplate)
        {
            Driver = driver;
            Parent = parent;

            InputContactReadBlockSettings = new ObservableCollection<ReadBlockSetting>();
            OutputCoilsReadBlockSettings = new ObservableCollection<ReadBlockSetting>();
            InputRegisterReadBlockSettings = new ObservableCollection<ReadBlockSetting>();
            HoldingRegisterReadBlockSettings = new ObservableCollection<ReadBlockSetting>();

            InputContactReadBlockSettingsViewModel = ViewModelSource.Create(() => new ReadBlockSettingViewModel(AddressType.InputContact, InputContactReadBlockSettings)).SetParentViewModel(this);
            OutputCoilsReadBlockSettingsViewModel = ViewModelSource.Create(() => new ReadBlockSettingViewModel(AddressType.OutputCoil, OutputCoilsReadBlockSettings)).SetParentViewModel(this);
            InputRegisterReadBlockSettingsViewModel = ViewModelSource.Create(() => new ReadBlockSettingViewModel(AddressType.InputRegister, InputRegisterReadBlockSettings)).SetParentViewModel(this);
            HoldingRegisterReadBlockSettingsViewModel = ViewModelSource.Create(() => new ReadBlockSettingViewModel(AddressType.HoldingRegister, HoldingRegisterReadBlockSettings)).SetParentViewModel(this);

            if (itemTemplate is Device device)
            {
                Name = Parent.GetUniqueNameInGroup(device.Name);
                DeviceId = device.DeviceId;
                Timeout = device.Timeout;
                TryReadWriteTimes = device.TryReadWriteTimes;
                ByteOrder = device.ByteOrder;
                ReadMode = device.ReadMode;
                Description = device.Description;

                foreach (var item in device.InputContactReadBlockSettings)
                    InputContactReadBlockSettings.Add(item);

                foreach (var item in device.OutputCoilsReadBlockSettings)
                    OutputCoilsReadBlockSettings.Add(item);

                foreach (var item in device.InputRegisterReadBlockSettings)
                    InputRegisterReadBlockSettings.Add(item);

                foreach (var item in device.HoldingRegisterReadBlockSettings)
                    HoldingRegisterReadBlockSettings.Add(item);
            }
            else
            {
                Name = parent.GetUniqueNameInGroup("Device1");
            }
        }
        #endregion

        #region Commands
        public void Save()
        {
            Device = new Device(Parent)
            {
                Name = Name,
                DeviceId = DeviceId,
                Timeout = Timeout,
                TryReadWriteTimes = TryReadWriteTimes,
                ByteOrder = ByteOrder,
                Description = Description,
                ReadMode = ReadMode,
            };

            foreach (var item in InputContactReadBlockSettings)
                Device.InputContactReadBlockSettings.Add(item);

            foreach (var item in OutputCoilsReadBlockSettings)
                Device.OutputCoilsReadBlockSettings.Add(item);

            foreach (var item in InputRegisterReadBlockSettings)
                Device.InputRegisterReadBlockSettings.Add(item);

            foreach (var item in HoldingRegisterReadBlockSettings)
                Device.HoldingRegisterReadBlockSettings.Add(item);

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
                                    return device.DeviceId == DeviceId;
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
