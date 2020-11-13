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
    public class EditTagViewModel : IDataErrorInfo
    {
        #region Public properties
        public virtual ModbusRTUDriver Driver { get; set; }
        public virtual Tag Tag { get; set; }
        public virtual IGroupItem Parent { get => Tag.Parent; }
        public virtual List<string> DataTypeSource { get => Driver.SupportDataTypes.Select(x => x.Name).ToList(); }
        public virtual IDataType DataTypeCore { get => Driver.SupportDataTypes.FirstOrDefault(x => x.Name == DataType); }

        public virtual string Name { get; set; }
        public virtual string Address { get; set; } = "400001";
        public virtual string DataType { get; set; }
        public virtual int RefreshRate { get; set; } = 100;
        public virtual AccessPermission AccessPermission { get; set; } = AccessPermission.ReadAndWrite;
        public virtual double Gain { get; set; } = 1.0;
        public virtual double Offset { get; set; } = 0;
        public virtual string Description { get; set; }
        #endregion

        #region Services
        protected ICurrentWindowService CurrentWindowService { get => this.GetService<ICurrentWindowService>(); }
        protected IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }
        #endregion

        #region Constructors
        public EditTagViewModel(ModbusRTUDriver driver, ITagCore tagCore)
        {
            Driver = driver;
            Tag = tagCore as Tag;

            DataType = DataTypeSource.FirstOrDefault(x => x == "Word");

            if (Tag != null)
            {
                Name = Tag.Name;
                DataType = Tag.DataType == null ? Tag.DataTypeName : Tag.DataType.Name;
                Address = Tag.Address;
                RefreshRate = Tag.RefreshRate;
                AccessPermission = Tag.AccessPermission;
                Description = Tag.Description;
                Gain = Tag.Gain;
                Offset = Tag.Offset;
            }
        }
        #endregion

        #region Commands
        public void Save()
        {
            string address = Address;
            if (DataTypeCore is String)
            {
                string[] splitAddress = Address.Split('.');
                ushort.TryParse(splitAddress[1], out ushort stringLength);
                if (stringLength % 2 == 1)
                    stringLength++;
                address = $"{splitAddress[0]}.{stringLength}";
            }
            else
            {
                uint.TryParse(Address, out uint adrNumber);
                address = adrNumber.ToString();
            }

            Tag.Name = Name?.Trim();
            Tag.Address = address?.Trim();
            Tag.DataType = DataTypeCore;
            Tag.RefreshRate = RefreshRate;
            Tag.Gain = Gain;
            Tag.Offset = Offset;
            Tag.Description = Description;
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
        #endregion

        #region Methods 

        #endregion

        #region IDataErrorInfo
        public string AddressError { get; set; }
        public string DataTypeError { get; set; }
        public string Error { get; set; }
        public AddressType validateAddressType = AddressType.Undefined;

        public string this[string columnName]
        {
            get
            {
                Error = string.Empty;
                switch (columnName)
                {
                    case nameof(Name):
                        Error = Name?.Trim().ValidateFileName("Tag");
                        if (string.IsNullOrEmpty(Error))
                        {
                            if (Parent.Childs.Any(x => Tag != x && (x as ICoreItem).Name == Name?.Trim()))
                                Error = $"The tag name '{Name}' is already in use.";
                        }
                        break;
                    case nameof(Address):
                        var dataType = Driver.SupportDataTypes.FirstOrDefault(x => x.Name == DataType);
                        if (dataType == null)
                        {
                            AddressError = $"The data type '{DataType}' is not valid.";
                        }
                        else
                        {
                            string oldAddressError = AddressError;
                            Tag.GetAddressTypeAndOffset(Address, dataType, out validateAddressType, out int offset);
                            if (offset == -1)
                            {
                                AddressError = "The address is not valid.";
                            }
                            else
                            {
                                if (validateAddressType == AddressType.Undefined)
                                {
                                    if (offset >= 0xFFFF)
                                        AddressError = $"The address is out of range of valid modbus address range.";
                                    else
                                        AddressError = $"The address doesn't match with data type '{DataType}'";
                                }
                                else
                                {
                                    AddressError = string.Empty;
                                }
                            }

                            Error = AddressError;
                            if (string.IsNullOrEmpty(Error))
                            {
                                if (!string.IsNullOrEmpty(oldAddressError))
                                    this.RaisePropertyChanged(x => x.DataType);
                            }
                        }
                        break;
                    case nameof(DataType):
                        if (!Driver.SupportDataTypes.Any(x => x.Name == DataType))
                        {
                            DataTypeError = $"The ModbusRTU driver doesn't support data type '{DataType}'.";
                            Error = DataTypeError;
                        }
                        else
                        {
                            DataTypeError = string.Empty;
                            this.RaisePropertyChanged(x => x.Address);
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
