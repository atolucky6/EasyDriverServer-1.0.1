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

        public override string DisplayInformation { get => $"{DeviceId}"; set => base.DisplayInformation = value; }

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
                RaisePropertyChanged(nameof(DisplayInformation));
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

        public Channel ChannelParent { get; set; }

        public ObservableCollection<Tag> ChildTags { get; set; } = new ObservableCollection<Tag>();

        public List<ReadBlockSetting> AutoReadBlockSettings { get; set; } = new List<ReadBlockSetting>();
        #endregion

        #region Constructors
        public Device(IGroupItem parent, bool isReadOnly = false) : base(parent, isReadOnly)
        {
            InputContactReadBlockSettings.CollectionChanged += OnReadBlockSettingCollectionChanged;
            OutputCoilsReadBlockSettings.CollectionChanged += OnReadBlockSettingCollectionChanged;
            InputRegisterReadBlockSettings.CollectionChanged += OnReadBlockSettingCollectionChanged;
            HoldingRegisterReadBlockSettings.CollectionChanged += OnReadBlockSettingCollectionChanged;

            ParameterContainer.ParameterChanged += OnParameterChanged;
            Added += OnRemovedFromParent;
            Removed += OnAddedToParent;
            ChildTags.CollectionChanged += OnChildTagsCollectionChanged;
        }

        private void OnChildTagsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is Tag tag)
                    {
                        tag.PropertyChanged += OnChildTagPropertyChanged;
                        CreateOrAddToReadBlockSetting(tag);
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is Tag tag)
                    {
                        tag.PropertyChanged -= OnChildTagPropertyChanged;
                        if (tag.AutoReadBlockSetting != null)
                        {
                            tag.AutoReadBlockSetting.RegisterTags.Remove(tag);
                            tag.AutoReadBlockSetting = null;
                            tag.IndexOfDataInAutoBlockSetting = 0;
                        }
                    }
                }
            }

            if (ChildTags.Count == 0)
                AutoReadBlockSettings.Clear();
        }

        private void OnChildTagPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Address" ||
                e.PropertyName == "DataType")
            {
                if (sender is Tag tag)
                {
                    if (tag.AutoReadBlockSetting != null)
                    {
                        if (tag.AutoReadBlockSetting.IsTagInRange(tag, out int index))
                        {
                            tag.IndexOfDataInAutoBlockSetting = index;
                        }
                        else
                        {
                            tag.AutoReadBlockSetting.RegisterTags.Remove(tag);
                            tag.AutoReadBlockSetting = null;
                            tag.IndexOfDataInAutoBlockSetting = 0;
                        }
                    }
                }
            }
        }

        private void CreateOrAddToReadBlockSetting(Tag tag)
        {
            if (tag.AddressType != AddressType.Undefined)
            {
                foreach (var setting in AutoReadBlockSettings)
                {
                    if (setting.AddressType == tag.AddressType)
                    {
                        if (setting.TryAddTag(tag, out int index))
                        {
                            tag.AutoReadBlockSetting = setting;
                            tag.IndexOfDataInAutoBlockSetting = index;
                            break;
                        }
                    }
                }

                if (int.TryParse(tag.Address, out int adrNum))
                {
                    ReadBlockSetting setting = new ReadBlockSetting();
                    setting.BeginEdit();
                    setting.Device = this;
                    setting.AddressType = tag.AddressType;
                    setting.StartAddress = tag.Address;
                    setting.EndAddress = (adrNum + tag.RequireByteLength - 1).ToString();
                    setting.EndEdit();
                    AutoReadBlockSettings.Add(setting);
                }
            }
            else
            {
                if (tag.AutoReadBlockSetting != null)
                {
                    tag.AutoReadBlockSetting.RegisterTags.Remove(tag);
                    tag.AutoReadBlockSetting = null;
                    tag.IndexOfDataInAutoBlockSetting = 0;
                }
            }
        }

        private void OnAddedToParent(object sender, EventArgs e)
        {
            ChannelParent = this.FindParent<Channel>(x => x is Channel);
            if (ChannelParent != null)
                ChannelParent.ChildDevices.Add(this);
        }

        private void OnRemovedFromParent(object sender, EventArgs e)
        {
            if (ChannelParent != null)
                ChannelParent.ChildDevices.Remove(this);
        }

        private void OnParameterChanged(object sender, ParameterChangedEventArgs e)
        {
            string key = e.KeyValue.Key;
            switch (key)
            {
                case nameof(InputContactReadBlockSettings):
                    {
                        ObservableCollection<ReadBlockSetting> inputContactSettings = JsonConvert.DeserializeObject<ObservableCollection<ReadBlockSetting>>(e.KeyValue.Value);
                        if (inputContactSettings != null)
                            UpdateReadBlockSettings(InputContactReadBlockSettings, inputContactSettings);
                        break;
                    }
                case nameof(OutputCoilsReadBlockSettings):
                    {
                        ObservableCollection<ReadBlockSetting> outputCoilsSettings = JsonConvert.DeserializeObject<ObservableCollection<ReadBlockSetting>>(e.KeyValue.Value);
                        if (outputCoilsSettings != null)
                            UpdateReadBlockSettings(OutputCoilsReadBlockSettings, outputCoilsSettings);
                        break;
                    }
                case nameof(InputRegisterReadBlockSettings):
                    ObservableCollection<ReadBlockSetting> inputRegisterSettings = JsonConvert.DeserializeObject<ObservableCollection<ReadBlockSetting>>(e.KeyValue.Value);
                    if (inputRegisterSettings != null)
                        UpdateReadBlockSettings(InputRegisterReadBlockSettings, inputRegisterSettings);
                    break;
                case nameof(HoldingRegisterReadBlockSettings):
                    ObservableCollection<ReadBlockSetting> holdingRegisterSettings = JsonConvert.DeserializeObject<ObservableCollection<ReadBlockSetting>>(e.KeyValue.Value);
                    if (holdingRegisterSettings != null)
                        UpdateReadBlockSettings(HoldingRegisterReadBlockSettings, holdingRegisterSettings);
                    break;
                default:
                    break;
            }
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
        public void UpdateReadBlockSettings(ObservableCollection<ReadBlockSetting> target, ObservableCollection<ReadBlockSetting> source)
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

        public List<ReadBlockSetting> GetAllReadBlockSettings()
        {
            List<ReadBlockSetting> settings = new List<ReadBlockSetting>();
            settings.AddRange(InputContactReadBlockSettings);
            settings.AddRange(OutputCoilsReadBlockSettings);
            settings.AddRange(InputRegisterReadBlockSettings);
            settings.AddRange(HoldingRegisterReadBlockSettings);
            return settings;
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
