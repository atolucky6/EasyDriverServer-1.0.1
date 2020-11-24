using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDriver.ModbusRTU
{
    public class Tag : TagCore
    {
        #region Public properties
        private AddressType addressType = AddressType.Undefined;
        public AddressType AddressType { get => addressType; }

        private int addressOffset;
        public int AddressOffset { get => addressOffset; }

        public ReadBlockSetting ReadBlockSetting { get; set; }

        public int IndexOfDataInBlockSetting { get; set; }

        public int RequireByteLength
        {
            get
            {
                if (DataType != null)
                {
                    if (DataType.GetType() == typeof(String))
                        return stringLength;
                    return DataType.RequireByteLength;
                }
                return 0;
            }
        }
        #endregion

        #region Members
        private ushort stringLength;
        #endregion

        #region Constructors
        public Tag(IGroupItem parent, bool isReadOnly = false) : base(parent, isReadOnly)
        {
            Added += OnItemWasAddToParent;
            Removed += OnItemWasRemoveFromParent;
        }
        #endregion

        #region Methods
        public void UnregisterReadBlock()
        {
            if (ReadBlockSetting != null)
            {
                ReadBlockSetting.RegisterTags.Remove(this);
                ReadBlockSetting = null;
                IndexOfDataInBlockSetting = 0;
            }
        }

        public void RegisterReadBlock(Device deviceParent, ReadBlockSetting readBlockSetting, int indexOfData)
        {
            if (deviceParent.UndefinedTags.Contains(this))
                deviceParent.UndefinedTags.Remove(this);
            ReadBlockSetting = readBlockSetting;
            IndexOfDataInBlockSetting = indexOfData;
            if (!readBlockSetting.RegisterTags.Contains(this))
                readBlockSetting.RegisterTags.Add(this);
        }

        public void FindAndRegisterReadBlock()
        {
            if (this.FindParent<Device>(x => x is Device) is Device device)
            {
                if (ReadBlockSetting != null && AddressType != ReadBlockSetting.AddressType)
                    UnregisterReadBlock();

                if (addressType != AddressType.Undefined && RequireByteLength > 0)
                {
                    foreach (ReadBlockSetting setting in device.GetReadBlockSettings(AddressType).ToArray())
                    {
                        if (setting.IsTagInRange(this, out int index))
                        {
                            UnregisterReadBlock();
                            RegisterReadBlock(device, setting, index);
                            return;
                        }
                    }
                }

                UnregisterReadBlock();
                if (!device.UndefinedTags.Contains(this))
                    device.UndefinedTags.Add(this);
            }
        }

        public void AddToUndefinedTags()
        {
            if (this.FindParent<Device>(x => x is Device) is Device device)
                if (!device.UndefinedTags.Contains(this))
                    device.UndefinedTags.Add(this);
        }

        public void RemoveFromUndifinedTags()
        {
            if (this.FindParent<Device>(x => x is Device) is Device device)
                if (device.UndefinedTags.Contains(this))
                    device.UndefinedTags.Remove(this);
        }

        private void OnItemWasRemoveFromParent(object sender, EventArgs e)
        {
            UnregisterReadBlock();
            if (this.FindParent<Device>(x => x is Device) is Device device)
            {
                if (device.UndefinedTags.Contains(this))
                    device.UndefinedTags.Remove(this);
            }
        }

        private void OnItemWasAddToParent(object sender, EventArgs e)
        {
            FindAndRegisterReadBlock();
        }

        public override string GetErrorOfProperty(string propertyName)
        {
            return base.GetErrorOfProperty(propertyName);
        }

        public override void PropertyChangedCallback(string propertyName, object oldValue, object newValue)
        {
            base.PropertyChangedCallback(propertyName, oldValue, newValue);
            switch (propertyName)
            {
                case nameof(Address):
                    GetAddressTypeAndOffset(newValue?.ToString(), DataType, out addressType, out addressOffset);
                    FindAndRegisterReadBlock();
                    break;
                case nameof(DataType):
                    GetAddressTypeAndOffset(Address, DataType, out addressType, out addressOffset);
                    if (newValue is IDataType dataType)
                    {
                        if (dataType.GetType() == typeof(String))
                        {
                            string[] adrSplit = Address?.Split('.');
                            if (adrSplit != null && adrSplit.Length == 2)
                            {
                                if (ushort.TryParse(adrSplit[1], out ushort len))
                                {
                                    if (len > 1)
                                    {
                                        if (len % 2 == 1)
                                            len++;
                                        stringLength = len;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        stringLength = 0;
                    }
                    FindAndRegisterReadBlock();
                    RaisePropertyChanged(nameof(Address));
                    break;
                default:
                    break;
            }
        }

        public static void GetAddressTypeAndOffset(string address, IDataType dataType, out AddressType addressType, out int offset)
        {
            addressType = AddressType.Undefined;
            offset = -1;
            if (dataType != null)
            {
                if (dataType is String)
                {
                    string[] splitAdr = address?.Trim()?.Split('.');
                    if (splitAdr.Length == 2)
                    {
                        if (uint.TryParse(splitAdr[0], out uint adrNumber) &&
                            uint.TryParse(splitAdr[1], out uint byteLen))
                        {
                            if (byteLen <= 250 && byteLen > 0)
                            {
                                int type = (int)(adrNumber / 100000);
                                offset = (int)(adrNumber % 100000) - 1;
                                if ((ushort)offset <= 0xFFFFU && offset >= 0)
                                {
                                    if (type == (int)AddressType.InputRegister ||
                                        type == (int)AddressType.HoldingRegister)
                                    {
                                        addressType = (AddressType)type;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (uint.TryParse(address, out uint adrNumber))
                    {
                        int type = (int)(adrNumber / 100000);
                        offset = (int)(adrNumber % 100000) - 1;
                        if ((ushort)offset <= 0xFFFFU && offset >= 0)
                        {
                            if (dataType.BitLength == 1)
                            {
                                if (type == (int)AddressType.InputContact ||
                                    type == (int)AddressType.OutputCoil)
                                {
                                    addressType = (AddressType)type;
                                }
                            }
                            else
                            {
                                if (type == (int)AddressType.InputRegister ||
                                    type == (int)AddressType.HoldingRegister)
                                {
                                    addressType = (AddressType)type;
                                }
                            }
                        }
                    }

                }
            }
        }
        #endregion
    }
}
