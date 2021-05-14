using EasyDriverPlugin;
using System;
using System.Text.RegularExpressions;

namespace EasyDriver.ModbusTCP
{
    static class Extensions
    {
        public static string REGEX_ValidFileName = @"^[\w\-. ]+$";

        public static string ValidateFileName(this string str, string name)
        {
            str = str?.Trim();
            if (string.IsNullOrEmpty(str))
                return $"The {name} name can't be empty.";
            if (!Regex.IsMatch(str, REGEX_ValidFileName))
                return $"The {name} name was not in correct format.";
            return string.Empty;
        }

        public static string IsValidAddress(this string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                if (uint.TryParse(str, out uint adrNumber))
                {
                    int type = (int)(adrNumber / 100000);
                    int offset = (int)(adrNumber % 100000) - 1;
                    if ((ushort)offset <= 0xFFFFU && offset >= 0)
                    {
                        if (type == (int)AddressType.InputContact ||
                            type == (int)AddressType.OutputCoil ||
                            type == (int)AddressType.InputRegister ||
                            type == (int)AddressType.HoldingRegister)
                        {
                            return string.Empty;
                        }
                    }
                }
            }
            return "The tag address was not in correct format.";
        }

        public static string IsValidAddress(this string str, IDataType dataType, AccessPermission accessPermission, out bool isBitAddress)
        {
            isBitAddress = false;
            if (!string.IsNullOrWhiteSpace(str))
            {
                if (dataType.GetType() == typeof(EasyDriver.ModbusTCP.String))
                {
                    string[] splitStr = str.Split('.');
                    if (splitStr.Length == 2)
                    {
                        str = splitStr[0];
                        if (uint.TryParse(str, out uint adrNumber))
                        {
                            int type = (int)(adrNumber / 100000);
                            int offset = (int)(adrNumber % 100000) - 1;
                            if ((ushort)offset <= 0xFFFFU && offset >= 0)
                            {
                                if (type == (int)AddressType.InputRegister ||
                                    type == (int)AddressType.HoldingRegister)
                                {
                                    isBitAddress = false;
                                    if ((type == (int)AddressType.InputContact || type == (int)AddressType.InputRegister) && accessPermission == AccessPermission.ReadAndWrite)
                                        return "The current tag address doesn't support write function.";

                                    if (byte.TryParse(splitStr[1], out byte length))
                                    {
                                        if (length <= 250)
                                        {
                                            return string.Empty;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (uint.TryParse(str, out uint adrNumber))
                    {
                        int type = (int)(adrNumber / 100000);
                        int offset = (int)(adrNumber % 100000) - 1;
                        if ((ushort)offset <= 0xFFFFU && offset >= 0)
                        {
                            if (type == (int)AddressType.InputContact ||
                                type == (int)AddressType.OutputCoil ||
                                type == (int)AddressType.InputRegister ||
                                type == (int)AddressType.HoldingRegister)
                            {
                                isBitAddress = type == (int)AddressType.InputContact || type == (int)AddressType.OutputCoil;
                                if ((type == (int)AddressType.InputContact || type == (int)AddressType.InputRegister) && accessPermission == AccessPermission.ReadAndWrite)
                                    return "The current tag address doesn't support write function.";
                                return string.Empty;
                            }
                        }
                    }
                }
            }
            return "The tag address was not in correct format.";
        }

        public static string IsValidAddress(this string address, AddressType addressType)
        {
            if (!string.IsNullOrWhiteSpace(address))
            {
                if (uint.TryParse(address, out uint adrNumber))
                {
                    int type = (int)(adrNumber / 100000);
                    int offset = (int)(adrNumber % 100000) - 1;
                    if ((ushort)offset <= 0xFFFFU && offset >= 0)
                    {
                        if (type == (int)addressType)
                            return string.Empty;
                    }
                }
            }
            return "The tag address was not in correct format.";
        }

        public static bool DecomposeAddress(this string address, out AddressType addressType, out ushort offset)
        {
            addressType = AddressType.Undefined;
            offset = 0;
            try
            {
                if (!string.IsNullOrWhiteSpace(address))
                {
                    if (uint.TryParse(address, out uint adrNumber))
                    {
                        int type = (int)(adrNumber / 100000);
                        int odd = (int)(adrNumber % 100000) - 1;

                        if (Enum.TryParse(type.ToString(), out addressType) && odd >= 0 && (ushort)odd <= (ushort)0xFFFFU)
                        {
                            offset = (ushort)odd;
                            return true;
                        }
                    }
                }
                return false;
            }
            catch { return false; }
        }
    }
}
