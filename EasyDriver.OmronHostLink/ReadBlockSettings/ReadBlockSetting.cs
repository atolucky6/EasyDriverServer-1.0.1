using EasyDriverPlugin;
using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EasyDriver.OmronHostLink
{

    [Serializable]
    public class ReadBlockSetting : INotifyPropertyChanged, IDataErrorInfo
    {
        #region Constructor

        public ReadBlockSetting()
        {
            IsChanged = true;
            tagCache = new Hashtable();
        }

        #endregion

        #region Members

        readonly Hashtable tagCache;
        public byte[] ByteBuffer;

        public bool ReadResult { get; set; }
        public DateTime LastReadTime { get; set; }
        public bool IsChanged { get; set; }
        public ushort Count { get; private set; }

        bool isValid;
        public bool IsValid
        {
            get
            {
                if (IsChanged)
                {
                    isValid = Enabled && string.IsNullOrEmpty(this["StartAddress"]) && string.IsNullOrEmpty(this["EndAddress"]);
                    Count = (ushort)(EndAddress - StartAddress + 1);
                    ByteBuffer = new byte[Count * 2];
                    IsChanged = false;
                }
                return isValid;
            }
        }

        private AddressType addressType;
        public AddressType AddressType
        {
            get { return addressType; }
            set
            {
                if (addressType != value)
                {
                    addressType = value;
                    RaisePropertyChanged();
                }
            }
        }

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

        ushort startAddress;
        public ushort StartAddress
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

        ushort endAddress;
        public ushort EndAddress
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

        #endregion

        #region Methods

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "StartAddress":
                        if (Enabled)
                        {
                            if (StartAddress > 9999)
                            {
                                Error = "The start word address is out of range. The range of start word address is 0 - 9999;";
                            }
                            else
                            {
                                if (StartAddress >= EndAddress)
                                {
                                    Error = "The start word address must be smaller than end word address.";
                                }
                                else
                                {
                                    if (EndAddress - StartAddress >= 155)
                                    {
                                        Error = $"The maximum word can read per request is 155.";
                                    }
                                    else
                                    {
                                        Error = string.Empty;
                                    }
                                }
                            }
                        }
                        else { Error = string.Empty; }
                        break;
                    case "EndAddress":
                        if (Enabled)
                        {
                            if (EndAddress > 9999)
                            {
                                Error = "The end word address is out of range. The range of end word address is 0 - 9999;";
                            }
                            else
                            {
                                if (StartAddress >= EndAddress)
                                {
                                    Error = "The end word address must be bigger than start word address.";
                                }
                                else
                                {
                                    if (EndAddress - StartAddress >= 155)
                                    {
                                        Error = $"The maximum word can read per request is 155.";
                                    }
                                    else
                                    {
                                        Error = string.Empty;
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

        public bool CheckTagIsInReadBlockRange(ITagCore tag, AddressType addressType, ushort wordOffset, ushort bitOffset, int dtByteLength, out int byteIndex, out int bitIndex)
        {
            byteIndex = 0;
            bitIndex = 0;
            if (tagCache.Contains(tag))
            {
                byteIndex = (tagCache[tag] as int[])[0];
                bitIndex = (tagCache[tag] as int[])[1];
                return true;
            }
            else
            {
                bool result = false;
                if (AddressType == addressType)
                {
                    if (wordOffset >= StartAddress && (wordOffset + dtByteLength / 2) <= EndAddress + 1)
                    {
                        byteIndex = (wordOffset - StartAddress) * 2;

                        if (bitOffset / 8 >= 1)
                        {
                            byteIndex++;
                            bitIndex = bitOffset - 8;
                        }
                        else
                        {
                            bitIndex = bitOffset;
                        }
                        result = true;
                    }
                }
                if (result)
                    tagCache.Add(tag, new int[] { byteIndex, bitIndex });
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
            return $"{Enabled}-{AddressType.ToString()}-{StartAddress}-{EndAddress}";
        }

        public static ReadBlockSetting Convert(string value)
        {
            try
            {
                string[] split = value.Split('-');
                if (split.Length == 4)
                {
                    ReadBlockSetting readBlockSetting = new ReadBlockSetting
                    {
                        Enabled = bool.Parse(split[0]),
                        AddressType = (AddressType)Enum.Parse(typeof(AddressType), split[1]),
                        StartAddress = ushort.Parse(split[2]),
                        EndAddress = ushort.Parse(split[3]),
                    };
                    return readBlockSetting;
                }
                return null;
            }
            catch { return null; }
        }

        #endregion
    }
}
