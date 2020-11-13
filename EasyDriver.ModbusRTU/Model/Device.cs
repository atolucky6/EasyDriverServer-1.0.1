using EasyDriverPlugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDriver.ModbusRTU
{
    public class Device : DeviceCore
    {
        #region Public properties
        public int Timeout
        {
            get
            {
                if (ParameterContainer.TryGetValue(nameof(Timeout), out int value))
                    return value;
                else
                    ParameterContainer.SetValue(nameof(Timeout), "1000");
                return 1000;
            }
            set
            {
                ParameterContainer.SetValue(nameof(Timeout), value.ToString());
                RaisePropertyChanged();
            }
        }

        public int TryReadWriteTimes
        {
            get
            {
                if (ParameterContainer.TryGetValue(nameof(TryReadWriteTimes), out int value))
                    return value;
                else
                    ParameterContainer.SetValue(nameof(TryReadWriteTimes), "3");
                return 3;
            }
            set
            {
                ParameterContainer.SetValue(nameof(TryReadWriteTimes), value.ToString());
                RaisePropertyChanged();
            }
        }

        public int DeviceId
        {
            get
            {
                if (ParameterContainer.TryGetValue(nameof(DeviceId), out int value))
                    return value;
                else
                    ParameterContainer.SetValue(nameof(DeviceId), "1");
                return 1;
            }
            set
            {
                ParameterContainer.SetValue(nameof(DeviceId), value.ToString());
                RaisePropertyChanged();
            }
        }

        public ReadMode ReadMode
        {
            get
            {
                if (ParameterContainer.TryGetValue(nameof(ReadMode), out ReadMode value))
                    return value;
                else
                    ParameterContainer.SetValue(nameof(ReadMode), ReadMode.Block.ToString());
                return ReadMode.Block;
            }
            set
            {
                ParameterContainer.SetValue(nameof(ReadMode), value.ToString());
                RaisePropertyChanged();
            }
        }

        ObservableCollection<ReadBlockSetting> inputContactReadBlockSettings;
        public ObservableCollection<ReadBlockSetting> InputContactReadBlockSettings
        {
            get
            {
                if (inputContactReadBlockSettings == null)
                {
                    if (ParameterContainer.TryGetValue(nameof(InputContactReadBlockSettings), out string settingStr))
                    {
                        try
                        {
                            inputContactReadBlockSettings = JsonConvert.DeserializeObject<ObservableCollection<ReadBlockSetting>>(settingStr);
                        }
                        catch { }
                    }

                    if (inputContactReadBlockSettings == null)
                        inputContactReadBlockSettings = new ObservableCollection<ReadBlockSetting>();
                }
                return inputContactReadBlockSettings;
            }
        }

        ObservableCollection<ReadBlockSetting> outputCoilsReadBlockSettings;
        public ObservableCollection<ReadBlockSetting> OutputCoilsReadBlockSettings
        {
            get
            {
                if (outputCoilsReadBlockSettings == null)
                {
                    if (ParameterContainer.TryGetValue(nameof(OutputCoilsReadBlockSettings), out string settingStr))
                    {
                        try
                        {
                            outputCoilsReadBlockSettings = JsonConvert.DeserializeObject<ObservableCollection<ReadBlockSetting>>(settingStr);
                        }
                        catch { }
                    }

                    if (outputCoilsReadBlockSettings == null)
                        outputCoilsReadBlockSettings = new ObservableCollection<ReadBlockSetting>();
                }
                return outputCoilsReadBlockSettings;
            }
        }

        ObservableCollection<ReadBlockSetting> inputRegisterReadBlockSettings;
        public ObservableCollection<ReadBlockSetting> InputRegisterReadBlockSettings
        {
            get
            {
                if (inputRegisterReadBlockSettings == null)
                {
                    if (ParameterContainer.TryGetValue(nameof(InputRegisterReadBlockSettings), out string settingStr))
                    {
                        try
                        {
                            inputRegisterReadBlockSettings = JsonConvert.DeserializeObject<ObservableCollection<ReadBlockSetting>>(settingStr);
                        }
                        catch { }
                    }

                    if (inputRegisterReadBlockSettings == null)
                        inputRegisterReadBlockSettings = new ObservableCollection<ReadBlockSetting>();
                }
                return inputRegisterReadBlockSettings;
            }
        }

        ObservableCollection<ReadBlockSetting> holdingRegisterReadBlockSettings;
        public ObservableCollection<ReadBlockSetting> HoldingRegisterReadBlockSettings
        {
            get
            {
                if (holdingRegisterReadBlockSettings == null)
                {
                    if (ParameterContainer.TryGetValue(nameof(HoldingRegisterReadBlockSettings), out string settingStr))
                    {
                        try
                        {
                            holdingRegisterReadBlockSettings = JsonConvert.DeserializeObject<ObservableCollection<ReadBlockSetting>>(settingStr);
                        }
                        catch { }
                    }

                    if (holdingRegisterReadBlockSettings == null)
                        holdingRegisterReadBlockSettings = new ObservableCollection<ReadBlockSetting>();
                }
                return holdingRegisterReadBlockSettings;
            }
        }

        public List<Tag> UndefinedTags { get; set; } = new List<Tag>();
        #endregion

        #region Constructors
        public Device(IGroupItem parent, bool isReadOnly = false) : base(parent, isReadOnly)
        {
            InputContactReadBlockSettings.CollectionChanged += OnReadBlockSettingCollectionChanged;
            OutputCoilsReadBlockSettings.CollectionChanged += OnReadBlockSettingCollectionChanged;
            InputRegisterReadBlockSettings.CollectionChanged += OnReadBlockSettingCollectionChanged;
            HoldingRegisterReadBlockSettings.CollectionChanged += OnReadBlockSettingCollectionChanged;
        }

        private void OnReadBlockSettingCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                    {
                        foreach (var item in e.NewItems)
                        {
                            if (item is ReadBlockSetting setting && setting.IsValid && setting.Enabled)
                            {
                                foreach (Tag tag in UndefinedTags.ToArray())
                                {
                                    if (tag.AddressType == setting.AddressType)
                                        tag.FindAndRegisterReadBlock();
                                }
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems)
                        {
                            if (item is ReadBlockSetting setting)
                            {
                                setting.Clear();
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems)
                        {
                            if (item is ReadBlockSetting setting)
                            {
                                setting.Clear();
                            }
                        }
                    }
                    if (e.NewItems != null)
                    {
                        foreach (var item in e.NewItems)
                        {
                            if (item is ReadBlockSetting setting && setting.IsValid && setting.Enabled)
                            {
                                foreach (Tag tag in UndefinedTags.ToArray())
                                {
                                    if (tag.AddressType == setting.AddressType)
                                        tag.FindAndRegisterReadBlock();
                                }
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems)
                        {
                            if (item is ReadBlockSetting setting)
                            {
                                setting.Clear();
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Methods
        public override string GetErrorOfProperty(string propertyName)
        {
            return base.GetErrorOfProperty(propertyName);
        }

        public ObservableCollection<ReadBlockSetting> GetReadBlockSettings(AddressType addressType)
        {
            switch (addressType)
            {
                case AddressType.OutputCoil:
                    return OutputCoilsReadBlockSettings;
                case AddressType.InputContact:
                    return InputContactReadBlockSettings;
                case AddressType.InputRegister:
                    return InputRegisterReadBlockSettings;
                case AddressType.HoldingRegister:
                    return HoldingRegisterReadBlockSettings;
                default:
                    break;
            }
            return null;
        }

        public void SaveBlockSetting()
        {
            ParameterContainer.SetValue(nameof(InputContactReadBlockSettings), JsonConvert.SerializeObject(InputContactReadBlockSettings, Formatting.Indented));
            ParameterContainer.SetValue(nameof(OutputCoilsReadBlockSettings), JsonConvert.SerializeObject(OutputCoilsReadBlockSettings, Formatting.Indented));
            ParameterContainer.SetValue(nameof(InputRegisterReadBlockSettings), JsonConvert.SerializeObject(InputRegisterReadBlockSettings, Formatting.Indented));
            ParameterContainer.SetValue(nameof(HoldingRegisterReadBlockSettings), JsonConvert.SerializeObject(HoldingRegisterReadBlockSettings, Formatting.Indented));
        }
        #endregion
    }
}
