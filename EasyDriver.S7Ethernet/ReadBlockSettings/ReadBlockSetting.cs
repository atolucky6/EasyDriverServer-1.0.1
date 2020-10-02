using EasyDriverPlugin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EasyDriver.S7Ethernet
{
    public class ReadBlockSetting : INotifyPropertyChanged, IDataErrorInfo
    {
        #region Constructors

        public ReadBlockSetting()
        {

        }

        #endregion

        #region Members

        readonly Hashtable tagCache;
        public byte[] ByteBuffer;

        public bool ReadResult { get; set; }
        public DateTime LastReadTime { get; set; }
        public bool IsChanged { get; set; }
        public ushort Count { get; private set; }

        #endregion

        ushort dbNumber;
        public ushort DbNumber
        {
            get => dbNumber;
            set
            {
                if (dbNumber != value)
                {
                    dbNumber = value;
                    RaisePropertyChanged();
                }
            }
        }

        bool isValid;
        public bool IsValid
        {
            get
            {
                if (IsChanged)
                {
                    isValid = Enabled && string.IsNullOrEmpty(this["StartAddress"]) && string.IsNullOrEmpty(this["EndAddress"]);
                    Count = (ushort)(EndAddress - StartAddress + 1);
                    ByteBuffer = new byte[Count];
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
                            if (StartAddress > 65535)
                            {
                                Error = "The start word address is out of range. The range of start word address is 0 - 65535;";
                            }
                            else
                            {
                                if (StartAddress >= EndAddress)
                                {
                                    Error = "The start word address must be smaller than end word address.";
                                }
                                else
                                {
                                    if (EndAddress - StartAddress >= 460)
                                    {
                                        Error = $"The maximum word can read per request is 460.";
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
                            if (EndAddress > 65535)
                            {
                                Error = "The end word address is out of range. The range of end word address is 0 - 65535;";
                            }
                            else
                            {
                                if (StartAddress >= EndAddress)
                                {
                                    Error = "The end word address must be bigger than start word address.";
                                }
                                else
                                {
                                    if (EndAddress - StartAddress >= 460)
                                    {
                                        Error = $"The maximum word can read per request is 460.";
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

        public bool CheckTagIsInReadBlockRange(ITagCore tag, AddressType addressType, ushort byteOffset, ushort bitOffset, int dtByteLength, out int byteIndex, out int bitIndex)
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
                    if (byteOffset >= StartAddress && (byteOffset + dtByteLength) <= EndAddress + 1)
                    {
                        byteIndex = byteOffset - StartAddress;

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
