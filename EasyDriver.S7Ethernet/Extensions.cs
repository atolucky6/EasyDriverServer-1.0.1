using EasyDriverPlugin;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace EasyDriver.S7Ethernet
{
    static class Extensions
    {
        static Extensions()
        {
            //AddressTypeSource = Enum.GetValues(typeof(AddressType)).Cast<AddressType>().Select(x => x.ToString()).ToArray();
        }

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

        public static string[] AddressTypeSource;

        public static string IsValidAddress(this string address, out bool isBitAddress, out int byteOffset, out int bitOffset, out int dbNumber, out AddressType addressType)
        {
            isBitAddress = false;
            byteOffset = 0;
            bitOffset = 0;
            dbNumber = 0;
            addressType = AddressType.Input;
            if (string.IsNullOrWhiteSpace(address))
                return $"The address can't be null or empty.";

            if (address.StartsWith("DB"))
            {
                addressType = AddressType.Datablock;
                string[] splitAddress = address.Split('.');
                if (splitAddress.Length >= 2)
                {
                    if (int.TryParse(splitAddress[0].Remove(0, 2), out dbNumber))
                    {
                        if (dbNumber >= 1 && dbNumber <= 65535)
                        {
                            if (splitAddress[1].StartsWith("DB"))
                            {
                                if (splitAddress[1].Length > 3)
                                {
                                    char adrtype = splitAddress[1][2];
                                    string addressOffset = splitAddress[1].Remove(0, 3);
                                    switch (adrtype)
                                    {
                                        case 'X':
                                            if (splitAddress.Length == 3)
                                            {
                                                isBitAddress = true;
                                                if (int.TryParse(addressOffset, out byteOffset))
                                                {
                                                    if (byteOffset >= 0 && byteOffset <= 65535)
                                                    {
                                                        if (int.TryParse(splitAddress[2], out bitOffset))
                                                        {
                                                            if (bitOffset >= 0 && bitOffset <= 7)
                                                                return string.Empty;
                                                        }
                                                    }
                                                }
                                            }
                                            break;
                                        case 'B':
                                        case 'W':
                                        case 'D':
                                            if (splitAddress.Length == 2)
                                            {
                                                if (int.TryParse(addressOffset, out byteOffset))
                                                {
                                                    if (byteOffset >= 0 && byteOffset <= 65535)
                                                        return string.Empty;
                                                }
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                var match = Regex.Match(address, @"^[IQM][BWD]?");
                if (match.Success)
                {
                    if (address.StartsWith("I"))
                        addressType = AddressType.Input;
                    else if (address.StartsWith("Q"))
                        addressType = AddressType.Output;
                    else
                        addressType = AddressType.Marker;

                    var offsetString = Regex.Replace(address, @"^[IQM][BWD]?", "");

                    if (!string.IsNullOrEmpty(offsetString))
                    {
                        string[] splitStr = offsetString.Split('.');

                        bool offsetByteValid = false;
                        bool offsetBitValid = true;
                        if (splitStr.Length >= 1)
                        {
                            if (int.TryParse(splitStr[0], out byteOffset))
                            {
                                if (byteOffset >= 0 && byteOffset <= 65535)
                                    offsetByteValid = true;
                            }
                        }


                        if (splitStr.Length >= 2)
                        {
                            isBitAddress = true;
                            offsetBitValid = false;
                            if (int.TryParse(splitStr[1], out bitOffset))
                            {
                                if (bitOffset >= 0 && bitOffset <= 7)
                                    offsetBitValid = true;
                            }
                        }

                        if (!offsetByteValid || !offsetBitValid)
                            return "The address was not in correct format.";
                        return string.Empty;
                    }
                }
            }
            return "The address was not in correct format.";
        }
    }
}
