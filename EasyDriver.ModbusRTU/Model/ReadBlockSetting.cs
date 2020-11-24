using EasyDriverPlugin;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EasyDriver.ModbusRTU
{
    [Serializable]
    public class ReadBlockSetting : INotifyPropertyChanged, IDataErrorInfo
    {
        #region Public properties
        public AddressType AddressType { get; set; }

        [JsonIgnore]
        bool enabled;
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    InitializeAddress();
                    RaisePropertyChanged(null);
                }
            }
        }

        [JsonIgnore]
        string startAddress;
        public string StartAddress
        {
            get { return startAddress; }
            set
            {
                if (startAddress != value)
                {
                    startAddress = value;
                    RaisePropertyChanged(null);
                    InitializeAddress();
                }
            }
        }

        [JsonIgnore]
        string endAddress;
        public string EndAddress
        {
            get { return endAddress; }
            set
            {
                if (endAddress != value)
                {
                    endAddress = value;
                    RaisePropertyChanged(null);
                    InitializeAddress();
                }
            }
        }

        [JsonIgnore]
        private ushort startOffset;
        [JsonIgnore]
        public ushort StartOffset { get { return startOffset; } }

        [JsonIgnore]
        private ushort endOffset;
        [JsonIgnore]
        public ushort EndOffset { get { return endOffset; } }

        [JsonIgnore]
        public ushort BufferCount { get; private set; }

        [JsonIgnore]
        public bool ReadResult { get; set; }

        [JsonIgnore]
        public DateTime LastReadTime { get; set; }

        [JsonIgnore]
        public bool IsValid { get; set; }

        [JsonIgnore]
        private Device device;
        [JsonIgnore]
        public Device Device
        {
            get => device;
            set
            {
                if (device != value)
                {
                    device = value;
                }
            }
        }

        [JsonIgnore]
        public List<Tag> RegisterTags { get; set; }
        #endregion

        #region Members
        [JsonIgnore]
        private bool isEditing;
        [JsonIgnore]
        public bool[] BoolBuffer;
        [JsonIgnore]
        public byte[] ByteBuffer;
        #endregion

        #region Constructors
        public ReadBlockSetting()
        {
            RegisterTags = new List<Tag>();
        }
        #endregion

        #region Methods
        public void InitializeAddress()
        {
            if (!isEditing)
            {
                bool isValid = Enabled && string.IsNullOrEmpty(this["StartAddress"]) && string.IsNullOrEmpty(this["EndAddress"]);
                StartAddress.DecomposeAddress(out AddressType startAddressType, out startOffset);
                EndAddress.DecomposeAddress(out AddressType endAddressType, out endOffset);
                IsValid = isValid && AddressType == startAddressType && AddressType == endAddressType;

                BufferCount = (ushort)(endOffset - startOffset + 1);

                if (AddressType == AddressType.InputContact ||
                    AddressType == AddressType.OutputCoil)
                {
                    BoolBuffer = new bool[BufferCount];
                }
                else
                {
                    ByteBuffer = new byte[BufferCount * 2];
                }

                if (RegisterTags.Count > 0)
                {
                    foreach (var tag in RegisterTags.ToArray())
                    {
                        if (IsTagInRange(tag, out int index))
                            tag.IndexOfDataInBlockSetting = index;
                        else
                        {
                            tag.UnregisterReadBlock();
                            tag.AddToUndefinedTags();
                        }
                    }
                }

                if (!IsValid || !Enabled)
                {
                    foreach (var item in RegisterTags)
                    {
                        item.UnregisterReadBlock();
                        item.AddToUndefinedTags();
                    }
                }
            }
        }
        
        public bool IsTagInRange(Tag tag, out int index)
        {
            index = 0;
            bool result = false;
            if (Enabled && IsValid && tag != null && tag.DataType != null)
            {
                if (tag.AddressType == AddressType)
                {
                    if (AddressType == AddressType.InputContact || AddressType == AddressType.OutputCoil)
                    {
                        result = tag.AddressOffset <= EndOffset && tag.AddressOffset >= StartOffset;
                        if (result)
                        {
                            if (tag.DataType.BitLength == 1)
                                index = tag.AddressOffset - StartOffset;
                            else result = false;
                        }
                    }
                    else
                    {
                        result = (tag.AddressOffset + (tag.RequireByteLength / 2) - 1) <= EndOffset && tag.AddressOffset >= startOffset;
                        if (result)
                        {
                            if (tag.DataType.BitLength != 1)
                                index = (tag.AddressOffset - startOffset) * 2;
                            else
                                result = false;
                        }
                    }
                }
            }
            return result;
        }

        public void Clear()
        {
            foreach (var item in RegisterTags)
            {
                item.UnregisterReadBlock();
                item.AddToUndefinedTags();
            }
        }

        public void BeginEdit()
        {
            isEditing = true;
        }

        public void EndEdit()
        {
            isEditing = false;
            InitializeAddress();
        }
        #endregion

        #region INotifyPropertyChanged
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region IDataErrorInfo
        [JsonIgnore]
        public string Error { get; set; }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "StartAddress":
                        if (Enabled)
                        {
                            Error = StartAddress.IsValidAddress(AddressType);
                            if (string.IsNullOrEmpty(Error))
                            {
                                if (uint.TryParse(StartAddress, out uint start) && uint.TryParse(EndAddress, out uint end))
                                {
                                    if (end <= start)
                                    {
                                        Error = "The start address must be smaller than end address.";
                                    }
                                    else
                                    {
                                        if (AddressType == AddressType.InputContact || AddressType == AddressType.OutputCoil)
                                        {
                                            if (end - start >= 2000)
                                            {
                                                string name = AddressType == AddressType.InputContact ? "input contacts" : "output coils";
                                                Error = $"The maximum {name} can read per request is 2000.";
                                            }
                                            else { Error = string.Empty; }
                                        }
                                        else
                                        {
                                            if (end - start >= 125)
                                            {
                                                string name = AddressType == AddressType.InputRegister ? "input registers" : "holding registers";
                                                Error = $"The maximum {name} can read per request is 125.";
                                            }
                                            else { Error = string.Empty; }
                                        }
                                    }
                                }
                            }
                        }
                        else { Error = string.Empty; }
                        break;
                    case "EndAddress":
                        if (Enabled)
                        {
                            Error = EndAddress.IsValidAddress(AddressType);
                            if (string.IsNullOrEmpty(Error))
                            {
                                if (uint.TryParse(StartAddress, out uint start) && uint.TryParse(EndAddress, out uint end))
                                {
                                    if (end <= start)
                                    {
                                        Error = "The end address must be larger than start address.";
                                    }
                                    else
                                    {
                                        if (AddressType == AddressType.InputContact || AddressType == AddressType.OutputCoil)
                                        {
                                            if (end - start >= 2000)
                                            {
                                                string name = AddressType == AddressType.InputContact ? "input contacts" : "output coils";
                                                Error = $"The maximum {name} can read per request is 2000.";
                                            }
                                            else { Error = string.Empty; }
                                        }
                                        else
                                        {
                                            if (end - start >= 125)
                                            {
                                                string name = AddressType == AddressType.InputRegister ? "input registers" : "holding registers";
                                                Error = $"The maximum {name} can read per request is 125.";
                                            }
                                            else { Error = string.Empty; }
                                        }
                                    }
                                }
                            }
                        }
                        else { Error = string.Empty; }
                        break;
                    default:
                        Error = string.Empty;
                        break;
                }
                return Error;
            }
        }
        #endregion
    }
}
