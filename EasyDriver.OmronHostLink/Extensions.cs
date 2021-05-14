using EasyDriverPlugin;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace EasyDriver.OmronHostLink
{
    static class Extensions
    {
        static Extensions()
        {
            AddressTypeSource = Enum.GetValues(typeof(AddressType)).Cast<AddressType>().Select(x => x.ToString()).ToArray();
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

        public static string IsValidAddress(this string adr)
        {
            if (!string.IsNullOrWhiteSpace(adr))
            {
                string prefix = string.Empty;
                string numberPart = string.Empty;

                for (int i = 0; i < adr.Length; i++)
                {
                    if (!char.IsDigit(adr[i]))
                    {
                        prefix += adr[i];
                    }
                    else
                    {
                        numberPart = adr.Substring(i, adr.Length - i);
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(numberPart))
                {
                    prefix = prefix.ToUpper();
                    bool isPrefixValid = false;
                    for (int i = 0; i < AddressTypeSource.Length; i++)
                    {
                        if (prefix == AddressTypeSource[i])
                        {
                            isPrefixValid = true;
                            break;
                        }
                    }

                    if (isPrefixValid)
                    {
                        string[] splitAdr = numberPart.Split('.');
                        string wordAdr = string.Empty;
                        string bitAdr = string.Empty;

                        if (splitAdr.Length == 2)
                        {
                            wordAdr = splitAdr[0];
                            bitAdr = splitAdr[1];
                        }
                        else if (splitAdr.Length == 1)
                        {
                            wordAdr = splitAdr[0];
                        }

                        if (!string.IsNullOrEmpty(wordAdr))
                        {
                            if (ushort.TryParse(wordAdr, out ushort wordAdrNum))
                            {
                                if (wordAdrNum > 9999)
                                {
                                    return "The word address is out of range. The range of word address is 0 - 9999";
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(bitAdr))
                                    {
                                        return string.Empty;
                                    }
                                    else
                                    {
                                        if (ushort.TryParse(bitAdr, out ushort bitAdrNum))
                                        {
                                            if (bitAdrNum > 15)
                                            {
                                                return "The bit address is out of range. The range of bit address is 0 - 15";
                                            }
                                            else
                                            {
                                                return string.Empty;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return "The tag address was not in correct format.";
        }

        public static string IsValidAddress(this string adr, out bool isBitAddress)
        {
            isBitAddress = false;
            if (!string.IsNullOrWhiteSpace(adr))
            {
                string prefix = string.Empty;
                string numberPart = string.Empty;

                for (int i = 0; i < adr.Length; i++)
                {
                    if (!char.IsDigit(adr[i]))
                    {
                        prefix += adr[i];
                    }
                    else
                    {
                        numberPart = adr.Substring(i, adr.Length - i);
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(numberPart))
                {
                    prefix = prefix.ToUpper();
                    bool isPrefixValid = false;
                    for (int i = 0; i < AddressTypeSource.Length; i++)
                    {
                        if (prefix == AddressTypeSource[i])
                        {
                            isPrefixValid = true;
                            break;
                        }
                    }

                    if (isPrefixValid)
                    {
                        string[] splitAdr = numberPart.Split('.');
                        string wordAdr = string.Empty;
                        string bitAdr = string.Empty;

                        if (splitAdr.Length == 2)
                        {
                            wordAdr = splitAdr[0];
                            bitAdr = splitAdr[1];
                            isBitAddress = true;
                        }
                        else if (splitAdr.Length == 1)
                        {
                            wordAdr = splitAdr[0];
                        }

                        if (!string.IsNullOrEmpty(wordAdr))
                        {
                            if (ushort.TryParse(wordAdr, out ushort wordAdrNum))
                            {
                                if (wordAdrNum > 9999)
                                {
                                    return "The word address is out of range. The range of word address is 0 - 9999";
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(bitAdr))
                                    {
                                        return string.Empty;
                                    }
                                    else
                                    {
                                        if (ushort.TryParse(bitAdr, out ushort bitAdrNum))
                                        {
                                            if (bitAdrNum > 15)
                                            {
                                                return "The bit address is out of range. The range of bit address is 0 - 15";
                                            }
                                            else
                                            {
                                                return string.Empty;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return "The tag address was not in correct format.";
        }

        public static bool DecomposeAddress(this string adr, out AddressType addressType, out ushort wordOffset, out ushort bitOffset)
        {
            addressType = AddressType.CIO;
            wordOffset = 0;
            bitOffset = 0;
            if (!string.IsNullOrWhiteSpace(adr))
            {
                string prefix = string.Empty;
                string numberPart = string.Empty;

                for (int i = 0; i < adr.Length; i++)
                {
                    if (!char.IsDigit(adr[i]))
                    {
                        prefix += adr[i];
                    }
                    else
                    {
                        numberPart = adr.Substring(i, adr.Length - i);
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(numberPart))
                {
                    prefix = prefix.ToUpper();
                    bool isPrefixValid = false;
                    for (int i = 0; i < AddressTypeSource.Length; i++)
                    {
                        if (prefix == AddressTypeSource[i])
                        {
                            isPrefixValid = true;
                            break;
                        }
                    }

                    if (isPrefixValid)
                    {
                        addressType = (AddressType)Enum.Parse(typeof(AddressType), prefix);
                        string[] splitAdr = numberPart.Split('.');
                        string wordAdr = string.Empty;
                        string bitAdr = string.Empty;

                        if (splitAdr.Length == 2)
                        {
                            wordAdr = splitAdr[0];
                            bitAdr = splitAdr[1];
                        }
                        else if (splitAdr.Length == 1)
                        {
                            wordAdr = splitAdr[0];
                        }

                        if (!string.IsNullOrEmpty(wordAdr))
                        {
                            if (ushort.TryParse(wordAdr, out wordOffset))
                            {
                                if (wordOffset > 9999)
                                {
                                    return false;
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(bitAdr))
                                    {
                                        if (ushort.TryParse(bitAdr, out bitOffset))
                                        {
                                            return bitOffset <= 15;
                                        }
                                    }
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public static ReadCommandCode GetReadCommand(AddressType addressType)
        {
            switch (addressType)
            {
                case AddressType.CIO:
                    return ReadCommandCode.RR;
                case AddressType.H:
                case AddressType.HR:
                    return ReadCommandCode.RH;
                case AddressType.A:
                case AddressType.AR:
                    return ReadCommandCode.RJ;
                case AddressType.D:
                case AddressType.DM:
                    return ReadCommandCode.RD;
                case AddressType.LR:
                    return ReadCommandCode.RL;
                case AddressType.EM:
                    return ReadCommandCode.RE;
                default:
                    throw new NotSupportedException();
            }
        }

        public static WriteCommandCode GetWriteCommand(AddressType addressType)
        {
            switch (addressType)
            {
                case AddressType.CIO:
                    return WriteCommandCode.WR;
                case AddressType.H:
                case AddressType.HR:
                    return WriteCommandCode.WH;
                case AddressType.A:
                case AddressType.AR:
                    return WriteCommandCode.WJ;
                case AddressType.D:
                case AddressType.DM:
                    return WriteCommandCode.WD;
                case AddressType.LR:
                    return WriteCommandCode.WL;
                case AddressType.EM:
                    return WriteCommandCode.WE;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
