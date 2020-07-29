using EasyDriverPlugin;
using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EasyDriver.ModbusRTU
{
    [Serializable]
    public class ReadBlockSetting : INotifyPropertyChanged, IDataErrorInfo
    {
        public ReadBlockSetting()
        {
            IsChanged = true;
            tagCache = new Hashtable();
        }

        readonly Hashtable tagCache;

        public bool ReadResult { get; set; }

        public DateTime LastReadTime { get; set; }

        public bool[] BoolBuffer;
        public byte[] ByteBuffer;

        public bool IsChanged { get; set; }

        bool isValid;
        public bool IsValid
        {
            get
            {
                if (IsChanged)
                {
                    isValid = Enabled && string.IsNullOrEmpty(this["StartAddress"]) && string.IsNullOrEmpty(this["EndAddress"]);
                    isValid = StartAddress.DecomposeAddress(out AddressType startAddressType, out startOffset);
                    isValid = EndAddress.DecomposeAddress(out AddressType endAddressType, out endOffset);
                    Count = (ushort)(endOffset - startOffset + 1);

                    if (AddressType == AddressType.InputContact ||
                        AddressType == AddressType.OutputCoil)
                    {
                        BoolBuffer = new bool[Count];
                    }
                    else
                    {
                        ByteBuffer = new byte[Count * 2];
                    }
                    IsChanged = false;
                }
                return isValid;
            }
        }

        private ushort startOffset;
        public ushort StartOffset { get { return startOffset; } }

        private ushort endOffset;
        public ushort EndOffset { get { return endOffset; } }

        public ushort Count { get; private set; }

        public AddressType AddressType { get; set; }

        bool enabled;
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    IsChanged = true;
                    RaisePropertyChanged(null);
                }
            }
        }

        string startAddress;
        public string StartAddress
        {
            get { return startAddress; }
            set
            {
                if (startAddress != value)
                {
                    startAddress = value;
                    IsChanged = true;
                    RaisePropertyChanged(null);
                }
            }
        }

        string endAddress;
        public string EndAddress
        {
            get { return endAddress; }
            set
            {
                if (endAddress != value)
                {
                    endAddress = value;
                    IsChanged = true;
                    RaisePropertyChanged(null);
                }
            }
        }

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
                                            if (end - start >= 2008)
                                            {
                                                string name = AddressType == AddressType.InputContact ? "input contacts" : "output coils";
                                                Error = $"The maximum {name} can read per request is 2008.";
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
                                            if (end - start >= 2008)
                                            {
                                                string name = AddressType == AddressType.InputContact ? "input contacts" : "output coils";
                                                Error = $"The maximum {name} can read per request is 2008.";
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

        public bool CheckTagIsInReadBlockRange(ITagCore tag, AddressType addressType, ushort offset, int dtByteLength, out int index)
        {
            index = 0;
            if (tagCache.Contains(tag))
            {
                index = (int)tagCache[tag];
                return true;
            }
            else
            {
                bool result = false;
                if (AddressType == addressType)
                {
                    if (addressType == AddressType.InputContact ||
                        addressType == AddressType.OutputCoil)
                    {
                        index = offset - StartOffset;
                        result = offset <= EndOffset && offset >= StartOffset;
                    }
                    else
                    {
                        index = (offset - StartOffset) * 2;
                        result = (offset + (dtByteLength / 2) - 1) <= EndOffset && offset >= startOffset;
                    }
                }
                if (result)
                    tagCache.Add(tag, index);
                return result;
            }
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return $"{Enabled}-{StartAddress}-{EndAddress}";
        }

        public static ReadBlockSetting Convert(string value)
        {
            try
            {
                string[] split = value.Split('-');
                if (split.Length == 3)
                {
                    ReadBlockSetting readBlockSetting = new ReadBlockSetting();
                    readBlockSetting.Enabled = bool.Parse(split[0]);
                    readBlockSetting.StartAddress = split[1];
                    readBlockSetting.EndAddress = split[2];
                    return readBlockSetting;
                }
                return null;
            }
            catch { return null; }
        }
    }
}
