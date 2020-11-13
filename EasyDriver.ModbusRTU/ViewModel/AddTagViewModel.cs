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
    public class AddTagViewModel : IDataErrorInfo
    {
        #region Public properties
        public virtual ModbusRTUDriver Driver { get; set; }
        public virtual List<Tag> Tags { get; set; }
        public virtual IGroupItem Parent { get; set; }
        public virtual List<string> DataTypeSource { get => Driver.SupportDataTypes.Select(x => x.Name).ToList(); }
        public virtual IDataType DataTypeCore { get => Driver.SupportDataTypes.FirstOrDefault(x => x.Name == DataType); }

        public virtual string Name { get; set; }
        public virtual string Address { get; set; } = "400001";
        public virtual string DataType { get; set; } 
        public virtual int RefreshRate { get; set; } = 100;
        public virtual AccessPermission AccessPermission { get; set; } = AccessPermission.ReadAndWrite;
        public virtual double Gain { get; set; } = 1.0;
        public virtual double Offset { get; set; } = 0;
        public virtual int AutoCreateTagCount { get; set; } = 1;
        public virtual string Description { get; set; }
        #endregion

        #region Services
        protected ICurrentWindowService CurrentWindowService { get => this.GetService<ICurrentWindowService>(); }
        protected IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }
        #endregion

        #region Constructors
        public AddTagViewModel(ModbusRTUDriver driver, IGroupItem parent, ITagCore itemTemplate)
        {
            Driver = driver;
            Parent = parent;

            DataType = DataTypeSource.FirstOrDefault(x => x == "Word");

            if (itemTemplate is Tag tag)
            {
                Name = (parent as IHaveTag).GetUniqueNameInGroupTags(itemTemplate.Name);
                DataType = tag.DataType == null ? tag.DataTypeName : tag.DataType.Name;
                Address = tag.Address;
                RefreshRate = tag.RefreshRate;
                AccessPermission = tag.AccessPermission;
                Description = tag.Description;
                Gain = tag.Gain;
                Offset = tag.Offset;
            }
            else
            {
                Name = (parent as IHaveTag).GetUniqueNameInGroupTags("Tag1");
            }
        }
        #endregion

        #region Commands
        public void Save()
        {
            Tags = new List<Tag>();
            string currentTagName = Name?.Trim();
            uint number = currentTagName.ExtractLastNumberFromString(out bool hasValue, out bool hasBracket);
            if (!hasValue)
                number = 1;
            string name = currentTagName.RemoveLastNumberFromString();
            string nameFormat = "";

            if (hasBracket)
            {
                name = name.Remove(name.Length - 2, 2);
                nameFormat = name + "({0})";
            }
            else
            {
                nameFormat = name + "{0}";
            }

            IDataType tagDataType = DataTypeCore;
            uint adrNumber = 0;
            uint stringLength = 0;

            if (tagDataType is String)
            {
                string[] splitAdr = Address.Split('.');
                adrNumber = uint.Parse(splitAdr[0].Trim());
                stringLength = uint.Parse(splitAdr[1].Trim());
                if (stringLength % 2 == 1)
                    stringLength++;
            }
            else
            {
                adrNumber = uint.Parse(Address?.Trim());
            }

            ByteOrder byteOrder = Parent.FindParent<IDeviceCore>(x => x is IDeviceCore).ByteOrder;

            for (int i = 0; i < AutoCreateTagCount; i++)
            {
                string adrString = "";

                if (tagDataType is String)
                {
                    adrString = $"{adrNumber}.{stringLength}";
                }
                else
                {
                    adrString = adrNumber.ToString();
                }

                IHaveTag haveTagObject = Parent as IHaveTag;
                Tag tag = new Tag(Parent)
                {
                    Name = haveTagObject.GetUniqueNameInGroupTags(string.Format(nameFormat, number)),
                    Address = adrString,
                    DataType = tagDataType,
                    RefreshRate = RefreshRate,
                    Gain = Gain,
                    Offset = Offset,
                    ByteOrder = byteOrder,
                    Description = Description
                };

                if (validateAddressType == AddressType.InputContact ||
                    validateAddressType == AddressType.InputRegister)
                {
                    tag.AccessPermission = AccessPermission.ReadOnly;
                }
                else
                {
                    tag.AccessPermission = AccessPermission;
                }

                if (i == 0 && !hasValue && AutoCreateTagCount == 1)
                    tag.Name = currentTagName;

                number++;

                currentTagName = tag.Name;
                Tags.Add(tag);

                int dtByteLength = tag.RequireByteLength;
                if (dtByteLength.IsOddNumber())
                    dtByteLength++;
                dtByteLength = dtByteLength / 2;
                if (dtByteLength == 0)
                    dtByteLength++;

                adrNumber += (uint)dtByteLength;
            }

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
                            if (Parent.Childs.Any(x => (x as ICoreItem).Name == Name?.Trim()))
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
