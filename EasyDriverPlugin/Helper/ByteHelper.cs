using System;
using System.Runtime.InteropServices;
using System.Text;

namespace EasyDriverPlugin
{
    public static class ByteHelper
    {
        public static class S7Consts
        {
            #region [Exported Consts]
            // Error codes
            //------------------------------------------------------------------------------
            //                                     ERRORS                 
            //------------------------------------------------------------------------------
            public const int errTCPSocketCreation = 0x00000001;
            public const int errTCPConnectionTimeout = 0x00000002;
            public const int errTCPConnectionFailed = 0x00000003;
            public const int errTCPReceiveTimeout = 0x00000004;
            public const int errTCPDataReceive = 0x00000005;
            public const int errTCPSendTimeout = 0x00000006;
            public const int errTCPDataSend = 0x00000007;
            public const int errTCPConnectionReset = 0x00000008;
            public const int errTCPNotConnected = 0x00000009;
            public const int errTCPUnreachableHost = 0x00002751;

            public const int errIsoConnect = 0x00010000; // Connection error
            public const int errIsoInvalidPDU = 0x00030000; // Bad format
            public const int errIsoInvalidDataSize = 0x00040000; // Bad Datasize passed to send/recv : buffer is invalid

            public const int errCliNegotiatingPDU = 0x00100000;
            public const int errCliInvalidParams = 0x00200000;
            public const int errCliJobPending = 0x00300000;
            public const int errCliTooManyItems = 0x00400000;
            public const int errCliInvalidWordLen = 0x00500000;
            public const int errCliPartialDataWritten = 0x00600000;
            public const int errCliSizeOverPDU = 0x00700000;
            public const int errCliInvalidPlcAnswer = 0x00800000;
            public const int errCliAddressOutOfRange = 0x00900000;
            public const int errCliInvalidTransportSize = 0x00A00000;
            public const int errCliWriteDataSizeMismatch = 0x00B00000;
            public const int errCliItemNotAvailable = 0x00C00000;
            public const int errCliInvalidValue = 0x00D00000;
            public const int errCliCannotStartPLC = 0x00E00000;
            public const int errCliAlreadyRun = 0x00F00000;
            public const int errCliCannotStopPLC = 0x01000000;
            public const int errCliCannotCopyRamToRom = 0x01100000;
            public const int errCliCannotCompress = 0x01200000;
            public const int errCliAlreadyStop = 0x01300000;
            public const int errCliFunNotAvailable = 0x01400000;
            public const int errCliUploadSequenceFailed = 0x01500000;
            public const int errCliInvalidDataSizeRecvd = 0x01600000;
            public const int errCliInvalidBlockType = 0x01700000;
            public const int errCliInvalidBlockNumber = 0x01800000;
            public const int errCliInvalidBlockSize = 0x01900000;
            public const int errCliNeedPassword = 0x01D00000;
            public const int errCliInvalidPassword = 0x01E00000;
            public const int errCliNoPasswordToSetOrClear = 0x01F00000;
            public const int errCliJobTimeout = 0x02000000;
            public const int errCliPartialDataRead = 0x02100000;
            public const int errCliBufferTooSmall = 0x02200000;
            public const int errCliFunctionRefused = 0x02300000;
            public const int errCliDestroying = 0x02400000;
            public const int errCliInvalidParamNumber = 0x02500000;
            public const int errCliCannotChangeParam = 0x02600000;
            public const int errCliFunctionNotImplemented = 0x02700000;
            //------------------------------------------------------------------------------
            //        PARAMS LIST FOR COMPATIBILITY WITH Snap7.net.cs           
            //------------------------------------------------------------------------------
            public const Int32 p_u16_LocalPort = 1;  // Not applicable here
            public const Int32 p_u16_RemotePort = 2;
            public const Int32 p_i32_PingTimeout = 3;
            public const Int32 p_i32_SendTimeout = 4;
            public const Int32 p_i32_RecvTimeout = 5;
            public const Int32 p_i32_WorkInterval = 6;  // Not applicable here
            public const Int32 p_u16_SrcRef = 7;  // Not applicable here
            public const Int32 p_u16_DstRef = 8;  // Not applicable here
            public const Int32 p_u16_SrcTSap = 9;  // Not applicable here
            public const Int32 p_i32_PDURequest = 10;
            public const Int32 p_i32_MaxClients = 11; // Not applicable here
            public const Int32 p_i32_BSendTimeout = 12; // Not applicable here
            public const Int32 p_i32_BRecvTimeout = 13; // Not applicable here
            public const Int32 p_u32_RecoveryTime = 14; // Not applicable here
            public const Int32 p_u32_KeepAliveTime = 15; // Not applicable here
                                                         // Area ID
            public const byte S7AreaPE = 0x81;
            public const byte S7AreaPA = 0x82;
            public const byte S7AreaMK = 0x83;
            public const byte S7AreaDB = 0x84;
            public const byte S7AreaCT = 0x1C;
            public const byte S7AreaTM = 0x1D;
            // Word Length
            public const int S7WLBit = 0x01;
            public const int S7WLByte = 0x02;
            public const int S7WLChar = 0x03;
            public const int S7WLWord = 0x04;
            public const int S7WLInt = 0x05;
            public const int S7WLDWord = 0x06;
            public const int S7WLDInt = 0x07;
            public const int S7WLReal = 0x08;
            public const int S7WLCounter = 0x1C;
            public const int S7WLTimer = 0x1D;
            // PLC Status
            public const int S7CpuStatusUnknown = 0x00;
            public const int S7CpuStatusRun = 0x08;
            public const int S7CpuStatusStop = 0x04;

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct S7Tag
            {
                public Int32 Area;
                public Int32 DBNumber;
                public Int32 Start;
                public Int32 Elements;
                public Int32 WordLen;
            }
            #endregion
        }

        #region [Help Functions]

        private static Int64 bias = 621355968000000000; // "decimicros" between 0001-01-01 00:00:00 and 1970-01-01 00:00:00

        private static int BCDtoByte(byte B)
        {
            return ((B >> 4) * 10) + (B & 0x0F);
        }

        private static byte ByteToBCD(int Value)
        {
            return (byte)(((Value / 10) << 4) | (Value % 10));
        }

        private static byte[] CopyFrom(byte[] Buffer, int Pos, int Size)
        {
            byte[] Result = new byte[Size];
            Array.Copy(Buffer, Pos, Result, 0, Size);
            return Result;
        }

        public static int DataSizeByte(int WordLength)
        {
            switch (WordLength)
            {
                case S7Consts.S7WLBit: return 1;  // S7 sends 1 byte per bit
                case S7Consts.S7WLByte: return 1;
                case S7Consts.S7WLChar: return 1;
                case S7Consts.S7WLWord: return 2;
                case S7Consts.S7WLDWord: return 4;
                case S7Consts.S7WLInt: return 2;
                case S7Consts.S7WLDInt: return 4;
                case S7Consts.S7WLReal: return 4;
                case S7Consts.S7WLCounter: return 2;
                case S7Consts.S7WLTimer: return 2;
                default: return 0;
            }
        }

        #region Get/Set the bit at Pos.Bit
        public static bool GetBitAt(byte[] Buffer, int Pos, int Bit)
        {
            byte[] Mask = { 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80 };
            if (Bit < 0) Bit = 0;
            if (Bit > 7) Bit = 7;
            return (Buffer[Pos] & Mask[Bit]) != 0;
        }
        public static void SetBitAt(ref byte[] Buffer, int Pos, int Bit, bool Value)
        {
            byte[] Mask = { 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80 };
            if (Bit < 0) Bit = 0;
            if (Bit > 7) Bit = 7;

            if (Value)
                Buffer[Pos] = (byte)(Buffer[Pos] | Mask[Bit]);
            else
                Buffer[Pos] = (byte)(Buffer[Pos] & ~Mask[Bit]);
        }
        #endregion

        #region Get/Set 8 bit signed value (S7 SInt) -128..127
        public static int GetSIntAt(byte[] Buffer, int Pos)
        {
            int Value = Buffer[Pos];
            if (Value < 128)
                return Value;
            else
                return (int)(Value - 256);
        }
        public static void SetSIntAt(byte[] Buffer, int Pos, int Value)
        {
            if (Value < -128) Value = -128;
            if (Value > 127) Value = 127;
            Buffer[Pos] = (byte)Value;
        }
        #endregion

        #region Get/Set 16 bit signed value (S7 int) -32768..32767
        public static short GetIntAt(byte[] Buffer, int Pos, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            switch (byteOrder)
            {
                case ByteOrder.ABCD:
                case ByteOrder.CDAB:
                    return (short)((Buffer[Pos] << 8) | Buffer[Pos + 1]);
                case ByteOrder.DCBA:
                case ByteOrder.BADC:
                    return (short)(Buffer[Pos] | Buffer[Pos + 1] << 8);
                default:
                    return default;
            }
        }
        public static void SetIntAt(byte[] Buffer, int Pos, Int16 Value, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            switch (byteOrder)
            {
                case ByteOrder.ABCD:
                case ByteOrder.CDAB:
                    Buffer[Pos] = (byte)(Value >> 8);
                    Buffer[Pos + 1] = (byte)(Value & 0x00FF);
                    break;
                case ByteOrder.DCBA:
                case ByteOrder.BADC:
                    Buffer[Pos + 1] = (byte)(Value >> 8);
                    Buffer[Pos] = (byte)(Value & 0x00FF);
                    break;
                default:
                    break;
            }   
        }
        #endregion

        #region Get/Set 32 bit signed value (S7 DInt) -2147483648..2147483647
        public static int GetDIntAt(byte[] Buffer, int Pos, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            switch (byteOrder)
            {
                case ByteOrder.ABCD:
                    {
                        int Result;
                        Result = Buffer[Pos]; Result <<= 8;
                        Result += Buffer[Pos + 1]; Result <<= 8;
                        Result += Buffer[Pos + 2]; Result <<= 8;
                        Result += Buffer[Pos + 3];
                        return Result;
                    }
                case ByteOrder.BADC:
                    {
                        int Result;
                        Result = Buffer[Pos + 1]; Result <<= 8;
                        Result += Buffer[Pos]; Result <<= 8;
                        Result += Buffer[Pos + 3]; Result <<= 8;
                        Result += Buffer[Pos + 2];
                        return Result;
                    }
                case ByteOrder.CDAB:
                    {
                        int Result;
                        Result = Buffer[Pos + 2]; Result <<= 8;
                        Result += Buffer[Pos + 3]; Result <<= 8;
                        Result += Buffer[Pos]; Result <<= 8;
                        Result += Buffer[Pos + 1];
                        return Result;
                    }
                case ByteOrder.DCBA:
                    {
                        int Result;
                        Result = Buffer[Pos + 3]; Result <<= 8;
                        Result += Buffer[Pos + 2]; Result <<= 8;
                        Result += Buffer[Pos + 1]; Result <<= 8;
                        Result += Buffer[Pos ];
                        return Result;
                    }
                default:
                    return default;
            }
            
        }
        public static void SetDIntAt(byte[] Buffer, int Pos, int Value, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            switch (byteOrder)
            {
                case ByteOrder.ABCD:
                    Buffer[Pos + 3] = (byte)(Value & 0xFF);
                    Buffer[Pos + 2] = (byte)((Value >> 8) & 0xFF);
                    Buffer[Pos + 1] = (byte)((Value >> 16) & 0xFF);
                    Buffer[Pos] = (byte)((Value >> 24) & 0xFF);
                    break;
                case ByteOrder.BADC:
                    Buffer[Pos + 2] = (byte)(Value & 0xFF);
                    Buffer[Pos + 3] = (byte)((Value >> 8) & 0xFF);
                    Buffer[Pos] = (byte)((Value >> 16) & 0xFF);
                    Buffer[Pos + 1] = (byte)((Value >> 24) & 0xFF);
                    break;
                case ByteOrder.CDAB:
                    Buffer[Pos + 1] = (byte)(Value & 0xFF);
                    Buffer[Pos] = (byte)((Value >> 8) & 0xFF);
                    Buffer[Pos + 3] = (byte)((Value >> 16) & 0xFF);
                    Buffer[Pos + 2] = (byte)((Value >> 24) & 0xFF);
                    break;
                case ByteOrder.DCBA:
                    Buffer[Pos] = (byte)(Value & 0xFF);
                    Buffer[Pos + 1] = (byte)((Value >> 8) & 0xFF);
                    Buffer[Pos + 2] = (byte)((Value >> 16) & 0xFF);
                    Buffer[Pos + 3] = (byte)((Value >> 24) & 0xFF);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Get/Set 64 bit signed value (S7 LInt) -9223372036854775808..9223372036854775807
        public static Int64 GetLIntAt(byte[] Buffer, int Pos, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            switch (byteOrder)
            {
                case ByteOrder.ABCD:
                    {
                        Int64 Result;
                        Result = Buffer[Pos]; Result <<= 8;
                        Result += Buffer[Pos + 1]; Result <<= 8;
                        Result += Buffer[Pos + 2]; Result <<= 8;
                        Result += Buffer[Pos + 3]; Result <<= 8;
                        Result += Buffer[Pos + 4]; Result <<= 8;
                        Result += Buffer[Pos + 5]; Result <<= 8;
                        Result += Buffer[Pos + 6]; Result <<= 8;
                        Result += Buffer[Pos + 7];
                        return Result;
                    }
                case ByteOrder.DCBA:
                    {
                        Int64 Result;
                        Result = Buffer[Pos + 7]; Result <<= 8;
                        Result += Buffer[Pos + 6]; Result <<= 8;
                        Result += Buffer[Pos + 5]; Result <<= 8;
                        Result += Buffer[Pos + 4]; Result <<= 8;
                        Result += Buffer[Pos + 3]; Result <<= 8;
                        Result += Buffer[Pos + 2]; Result <<= 8;
                        Result += Buffer[Pos + 1]; Result <<= 8;
                        Result += Buffer[Pos];
                        return Result;
                    }
                default:
                    return default;
            }
        }
        public static void SetLIntAt(byte[] Buffer, int Pos, Int64 Value, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            switch (byteOrder)
            {
                // ABCD EFGH
                case ByteOrder.ABCD:
                    Buffer[Pos + 7] = (byte)(Value & 0xFF);
                    Buffer[Pos + 6] = (byte)((Value >> 8) & 0xFF);
                    Buffer[Pos + 5] = (byte)((Value >> 16) & 0xFF);
                    Buffer[Pos + 4] = (byte)((Value >> 24) & 0xFF);
                    Buffer[Pos + 3] = (byte)((Value >> 32) & 0xFF);
                    Buffer[Pos + 2] = (byte)((Value >> 40) & 0xFF);
                    Buffer[Pos + 1] = (byte)((Value >> 48) & 0xFF);
                    Buffer[Pos] = (byte)((Value >> 56) & 0xFF);
                    break;
                // GHEF CDAB
                case ByteOrder.CDAB:
                    Buffer[Pos + 1] = (byte)(Value & 0xFF);
                    Buffer[Pos + 0] = (byte)((Value >> 8) & 0xFF);
                    Buffer[Pos + 3] = (byte)((Value >> 16) & 0xFF);
                    Buffer[Pos + 2] = (byte)((Value >> 24) & 0xFF);
                    Buffer[Pos + 5] = (byte)((Value >> 32) & 0xFF);
                    Buffer[Pos + 4] = (byte)((Value >> 40) & 0xFF);
                    Buffer[Pos + 7] = (byte)((Value >> 48) & 0xFF);
                    Buffer[Pos + 6] = (byte)((Value >> 56) & 0xFF);
                    break;
                // BADC FEHG
                case ByteOrder.BADC:
                    Buffer[Pos + 6] = (byte)(Value & 0xFF);
                    Buffer[Pos + 7] = (byte)((Value >> 8) & 0xFF);
                    Buffer[Pos + 4] = (byte)((Value >> 16) & 0xFF);
                    Buffer[Pos + 5] = (byte)((Value >> 24) & 0xFF);
                    Buffer[Pos + 2] = (byte)((Value >> 32) & 0xFF);
                    Buffer[Pos + 3] = (byte)((Value >> 40) & 0xFF);
                    Buffer[Pos + 0] = (byte)((Value >> 48) & 0xFF);
                    Buffer[Pos + 1] = (byte)((Value >> 56) & 0xFF);
                    break;
                // HGFE DCBA
                case ByteOrder.DCBA:
                    Buffer[Pos] = (byte)(Value & 0xFF);
                    Buffer[Pos + 1] = (byte)((Value >> 8) & 0xFF);
                    Buffer[Pos + 2] = (byte)((Value >> 16) & 0xFF);
                    Buffer[Pos + 3] = (byte)((Value >> 24) & 0xFF);
                    Buffer[Pos + 4] = (byte)((Value >> 32) & 0xFF);
                    Buffer[Pos + 5] = (byte)((Value >> 40) & 0xFF);
                    Buffer[Pos + 6] = (byte)((Value >> 48) & 0xFF);
                    Buffer[Pos + 7] = (byte)((Value >> 56) & 0xFF);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Get/Set 8 bit unsigned value (S7 USInt) 0..255
        public static byte GetUSIntAt(byte[] Buffer, int Pos)
        {
            return Buffer[Pos];
        }
        public static void SetUSIntAt(byte[] Buffer, int Pos, byte Value)
        {
            Buffer[Pos] = Value;
        }
        #endregion

        #region Get/Set 16 bit unsigned value (S7 UInt) 0..65535
        public static UInt16 GetUIntAt(byte[] Buffer, int Pos, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            switch (byteOrder)
            {
                case ByteOrder.ABCD:
                case ByteOrder.CDAB:
                    return (UInt16)((Buffer[Pos] << 8) | Buffer[Pos + 1]);
                case ByteOrder.DCBA:
                case ByteOrder.BADC:
                    return (UInt16)((Buffer[Pos + 1] << 8) | Buffer[Pos]);
                default:
                    return default;
            }
        }
        public static void SetUIntAt(byte[] Buffer, int Pos, UInt16 Value, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            switch (byteOrder)
            {
                case ByteOrder.ABCD:
                case ByteOrder.CDAB:
                    Buffer[Pos] = (byte)(Value >> 8);
                    Buffer[Pos + 1] = (byte)(Value & 0x00FF);
                    break;
                case ByteOrder.DCBA:
                case ByteOrder.BADC:
                    Buffer[Pos + 1] = (byte)(Value >> 8);
                    Buffer[Pos] = (byte)(Value & 0x00FF);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Get/Set 32 bit unsigned value (S7 UDInt) 0..4294967296
        public static UInt32 GetUDIntAt(byte[] Buffer, int Pos, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            switch (byteOrder)
            {
                
                case ByteOrder.ABCD:
                    {
                        UInt32 Result;
                        Result = Buffer[Pos]; Result <<= 8;
                        Result |= Buffer[Pos + 1]; Result <<= 8;
                        Result |= Buffer[Pos + 2]; Result <<= 8;
                        Result |= Buffer[Pos + 3];
                        return Result;
                    }
                case ByteOrder.BADC:
                    {
                        UInt32 Result;
                        Result = Buffer[Pos + 1]; Result <<= 8;
                        Result |= Buffer[Pos]; Result <<= 8;
                        Result |= Buffer[Pos + 3]; Result <<= 8;
                        Result |= Buffer[Pos + 2];
                        return Result;
                    }
                case ByteOrder.CDAB:
                    {
                        UInt32 Result;
                        Result = Buffer[Pos + 2]; Result <<= 8;
                        Result |= Buffer[Pos + 3]; Result <<= 8;
                        Result |= Buffer[Pos + 0]; Result <<= 8;
                        Result |= Buffer[Pos + 1];
                        return Result;
                    }
                case ByteOrder.DCBA:
                    {
                        UInt32 Result;
                        Result = Buffer[Pos + 3]; Result <<= 8;
                        Result |= Buffer[Pos + 2]; Result <<= 8;
                        Result |= Buffer[Pos + 1]; Result <<= 8;
                        Result |= Buffer[Pos];
                        return Result;
                    }
                default:
                    return default;
            }          
        }
        public static void SetUDIntAt(byte[] Buffer, int Pos, UInt32 Value, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            switch (byteOrder)
            {
                case ByteOrder.ABCD:
                    Buffer[Pos + 3] = (byte)(Value & 0xFF);
                    Buffer[Pos + 2] = (byte)((Value >> 8) & 0xFF);
                    Buffer[Pos + 1] = (byte)((Value >> 16) & 0xFF);
                    Buffer[Pos] = (byte)((Value >> 24) & 0xFF);
                    break;
                case ByteOrder.BADC:
                    Buffer[Pos + 2] = (byte)(Value & 0xFF);
                    Buffer[Pos + 3] = (byte)((Value >> 8) & 0xFF);
                    Buffer[Pos] = (byte)((Value >> 16) & 0xFF);
                    Buffer[Pos + 1] = (byte)((Value >> 24) & 0xFF);
                    break;
                case ByteOrder.CDAB:
                    Buffer[Pos + 1] = (byte)(Value & 0xFF);
                    Buffer[Pos] = (byte)((Value >> 8) & 0xFF);
                    Buffer[Pos + 3] = (byte)((Value >> 16) & 0xFF);
                    Buffer[Pos + 2] = (byte)((Value >> 24) & 0xFF);
                    break;
                case ByteOrder.DCBA:
                    Buffer[Pos] = (byte)(Value & 0xFF);
                    Buffer[Pos + 1] = (byte)((Value >> 8) & 0xFF);
                    Buffer[Pos + 2] = (byte)((Value >> 16) & 0xFF);
                    Buffer[Pos + 3] = (byte)((Value >> 24) & 0xFF);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Get/Set 64 bit unsigned value (S7 ULint) 0..18446744073709551616
        public static UInt64 GetULIntAt(byte[] Buffer, int Pos, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            switch (byteOrder)
            {
                // ABCD EFGH
                case ByteOrder.ABCD:
                    {
                        UInt64 Result;
                        Result = Buffer[Pos]; Result <<= 8;
                        Result |= Buffer[Pos + 1]; Result <<= 8;
                        Result |= Buffer[Pos + 2]; Result <<= 8;
                        Result |= Buffer[Pos + 3]; Result <<= 8;
                        Result |= Buffer[Pos + 4]; Result <<= 8;
                        Result |= Buffer[Pos + 5]; Result <<= 8;
                        Result |= Buffer[Pos + 6]; Result <<= 8;
                        Result |= Buffer[Pos + 7];
                        return Result;
                    }
                // GHEF CDAB
                case ByteOrder.CDAB:
                    {
                        UInt64 Result;
                        Result = Buffer[Pos + 6]; Result <<= 8;
                        Result |= Buffer[Pos + 7]; Result <<= 8;
                        Result |= Buffer[Pos + 4]; Result <<= 8;
                        Result |= Buffer[Pos + 5]; Result <<= 8;
                        Result |= Buffer[Pos + 2]; Result <<= 8;
                        Result |= Buffer[Pos + 3]; Result <<= 8;
                        Result |= Buffer[Pos + 0]; Result <<= 8;
                        Result |= Buffer[Pos + 1];
                        return Result;
                    }
                // BADC FEHG
                case ByteOrder.BADC:
                    {
                        UInt64 Result;
                        Result = Buffer[Pos + 1]; Result <<= 8;
                        Result |= Buffer[Pos + 0]; Result <<= 8;
                        Result |= Buffer[Pos + 3]; Result <<= 8;
                        Result |= Buffer[Pos + 2]; Result <<= 8;
                        Result |= Buffer[Pos + 5]; Result <<= 8;
                        Result |= Buffer[Pos + 4]; Result <<= 8;
                        Result |= Buffer[Pos + 7]; Result <<= 8;
                        Result |= Buffer[Pos + 6];
                        return Result;
                    }
                // HGFE DCBA
                case ByteOrder.DCBA:
                    {
                        UInt64 Result;
                        Result = Buffer[Pos + 7]; Result <<= 8;
                        Result |= Buffer[Pos + 6]; Result <<= 8;
                        Result |= Buffer[Pos + 5]; Result <<= 8;
                        Result |= Buffer[Pos + 4]; Result <<= 8;
                        Result |= Buffer[Pos + 3]; Result <<= 8;
                        Result |= Buffer[Pos + 2]; Result <<= 8;
                        Result |= Buffer[Pos + 1]; Result <<= 8;
                        Result |= Buffer[Pos];
                        return Result;
                    }
                default:
                    return default;
            }
        }
        public static void SetULIntAt(byte[] Buffer, int Pos, UInt64 Value, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            switch (byteOrder)
            {
                // ABCD EFGH
                case ByteOrder.ABCD:
                    Buffer[Pos + 7] = (byte)(Value & 0xFF);
                    Buffer[Pos + 6] = (byte)((Value >> 8) & 0xFF);
                    Buffer[Pos + 5] = (byte)((Value >> 16) & 0xFF);
                    Buffer[Pos + 4] = (byte)((Value >> 24) & 0xFF);
                    Buffer[Pos + 3] = (byte)((Value >> 32) & 0xFF);
                    Buffer[Pos + 2] = (byte)((Value >> 40) & 0xFF);
                    Buffer[Pos + 1] = (byte)((Value >> 48) & 0xFF);
                    Buffer[Pos + 0] = (byte)((Value >> 56) & 0xFF);
                    break;
                // GHEF CDAB
                case ByteOrder.CDAB:
                    Buffer[Pos + 1] = (byte)(Value & 0xFF);
                    Buffer[Pos + 0] = (byte)((Value >> 8) & 0xFF);
                    Buffer[Pos + 3] = (byte)((Value >> 16) & 0xFF);
                    Buffer[Pos + 2] = (byte)((Value >> 24) & 0xFF);
                    Buffer[Pos + 5] = (byte)((Value >> 32) & 0xFF);
                    Buffer[Pos + 4] = (byte)((Value >> 40) & 0xFF);
                    Buffer[Pos + 7] = (byte)((Value >> 48) & 0xFF);
                    Buffer[Pos + 6] = (byte)((Value >> 56) & 0xFF);
                    break;
                // BADC FEHG
                case ByteOrder.BADC:
                    Buffer[Pos + 6] = (byte)(Value & 0xFF);
                    Buffer[Pos + 7] = (byte)((Value >> 8) & 0xFF);
                    Buffer[Pos + 4] = (byte)((Value >> 16) & 0xFF);
                    Buffer[Pos + 5] = (byte)((Value >> 24) & 0xFF);
                    Buffer[Pos + 2] = (byte)((Value >> 32) & 0xFF);
                    Buffer[Pos + 3] = (byte)((Value >> 40) & 0xFF);
                    Buffer[Pos + 0] = (byte)((Value >> 48) & 0xFF);
                    Buffer[Pos + 1] = (byte)((Value >> 56) & 0xFF);
                    break;
                // HGFE DCBA
                case ByteOrder.DCBA:
                    Buffer[Pos] = (byte)(Value & 0xFF);
                    Buffer[Pos + 1] = (byte)((Value >> 8) & 0xFF);
                    Buffer[Pos + 2] = (byte)((Value >> 16) & 0xFF);
                    Buffer[Pos + 3] = (byte)((Value >> 24) & 0xFF);
                    Buffer[Pos + 4] = (byte)((Value >> 32) & 0xFF);
                    Buffer[Pos + 5] = (byte)((Value >> 40) & 0xFF);
                    Buffer[Pos + 6] = (byte)((Value >> 48) & 0xFF);
                    Buffer[Pos + 7] = (byte)((Value >> 56) & 0xFF);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Get/Set 8 bit word (S7 Byte) 16#00..16#FF
        public static byte GetByteAt(byte[] Buffer, int Pos)
        {
            return Buffer[Pos];
        }
        public static void SetByteAt(byte[] Buffer, int Pos, byte Value)
        {
            Buffer[Pos] = Value;
        }
        #endregion

        #region Get/Set 16 bit word (S7 Word) 16#0000..16#FFFF
        public static UInt16 GetWordAt(byte[] Buffer, int Pos, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            return GetUIntAt(Buffer, Pos, byteOrder);
        }
        public static void SetWordAt(byte[] Buffer, int Pos, UInt16 Value, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            SetUIntAt(Buffer, Pos, Value, byteOrder);
        }
        #endregion

        #region Get/Set 32 bit word (S7 DWord) 16#00000000..16#FFFFFFFF
        public static UInt32 GetDWordAt(byte[] Buffer, int Pos, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            return GetUDIntAt(Buffer, Pos, byteOrder);
        }
        public static void SetDWordAt(byte[] Buffer, int Pos, UInt32 Value, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            SetUDIntAt(Buffer, Pos, Value, byteOrder);
        }
        #endregion

        #region Get/Set 64 bit word (S7 LWord) 16#0000000000000000..16#FFFFFFFFFFFFFFFF
        public static UInt64 GetLWordAt(byte[] Buffer, int Pos, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            return GetULIntAt(Buffer, Pos, byteOrder);
        }
        public static void SetLWordAt(byte[] Buffer, int Pos, UInt64 Value, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            SetULIntAt(Buffer, Pos, Value, byteOrder);
        }
        #endregion

        #region Get/Set 32 bit floating point number (S7 Real) (Range of Single)
        public static Single GetRealAt(byte[] Buffer, int Pos, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            UInt32 Value = GetUDIntAt(Buffer, Pos, byteOrder);
            byte[] bytes = BitConverter.GetBytes(Value);
            return BitConverter.ToSingle(bytes, 0);
        }
        public static void SetRealAt(byte[] Buffer, int Pos, Single Value, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            byte[] FloatArray = BitConverter.GetBytes(Value);
            switch (byteOrder)
            {
                case ByteOrder.ABCD:
                    Buffer[Pos] = FloatArray[3];
                    Buffer[Pos + 1] = FloatArray[2];
                    Buffer[Pos + 2] = FloatArray[1];
                    Buffer[Pos + 3] = FloatArray[0];
                    break;
                case ByteOrder.BADC:
                    Buffer[Pos + 1] = FloatArray[3];
                    Buffer[Pos] = FloatArray[2];
                    Buffer[Pos + 3] = FloatArray[1];
                    Buffer[Pos + 2] = FloatArray[0];
                    break;
                case ByteOrder.CDAB:
                    Buffer[Pos + 2] = FloatArray[3];
                    Buffer[Pos + 3] = FloatArray[2];
                    Buffer[Pos] = FloatArray[1];
                    Buffer[Pos + 1] = FloatArray[0];
                    break;
                case ByteOrder.DCBA:
                    Buffer[Pos + 3] = FloatArray[3];
                    Buffer[Pos + 2] = FloatArray[2];
                    Buffer[Pos + 1] = FloatArray[1];
                    Buffer[Pos] = FloatArray[0];
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Get/Set 64 bit floating point number (S7 LReal) (Range of Double)
        public static Double GetLRealAt(byte[] Buffer, int Pos, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            UInt64 Value = GetULIntAt(Buffer, Pos, byteOrder);
            byte[] bytes = BitConverter.GetBytes(Value);
            return BitConverter.ToDouble(bytes, 0);
        }
        public static void SetLRealAt(byte[] Buffer, int Pos, Double Value, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            byte[] FloatArray = BitConverter.GetBytes(Value);
            switch (byteOrder)
            {
                // ABCD EFGH
                case ByteOrder.ABCD:
                    Buffer[Pos + 0] = FloatArray[7];
                    Buffer[Pos + 1] = FloatArray[6];
                    Buffer[Pos + 2] = FloatArray[5];
                    Buffer[Pos + 3] = FloatArray[4];
                    Buffer[Pos + 4] = FloatArray[3];
                    Buffer[Pos + 5] = FloatArray[2];
                    Buffer[Pos + 6] = FloatArray[1];
                    Buffer[Pos + 7] = FloatArray[0];
                    break;
                // GHEF CDAB
                case ByteOrder.CDAB:
                    Buffer[Pos + 6] = FloatArray[7];
                    Buffer[Pos + 7] = FloatArray[6];
                    Buffer[Pos + 4] = FloatArray[5];
                    Buffer[Pos + 5] = FloatArray[4];
                    Buffer[Pos + 2] = FloatArray[3];
                    Buffer[Pos + 3] = FloatArray[2];
                    Buffer[Pos + 0] = FloatArray[1];
                    Buffer[Pos + 1] = FloatArray[0];
                    break;
                // BADC FEHG
                case ByteOrder.BADC:
                    Buffer[Pos + 1] = FloatArray[7];
                    Buffer[Pos + 0] = FloatArray[6];
                    Buffer[Pos + 3] = FloatArray[5];
                    Buffer[Pos + 2] = FloatArray[4];
                    Buffer[Pos + 5] = FloatArray[3];
                    Buffer[Pos + 4] = FloatArray[2];
                    Buffer[Pos + 7] = FloatArray[1];
                    Buffer[Pos + 6] = FloatArray[0];
                    break;
                // HGFE DCBA
                case ByteOrder.DCBA:
                    Buffer[Pos + 7] = FloatArray[7];
                    Buffer[Pos + 6] = FloatArray[6];
                    Buffer[Pos + 5] = FloatArray[5];
                    Buffer[Pos + 4] = FloatArray[4];
                    Buffer[Pos + 3] = FloatArray[3];
                    Buffer[Pos + 2] = FloatArray[2];
                    Buffer[Pos + 1] = FloatArray[1];
                    Buffer[Pos] = FloatArray[0];
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Get/Set DateTime (S7 DATE_AND_TIME)
        public static DateTime GetDateTimeAt(byte[] Buffer, int Pos)
        {
            int Year, Month, Day, Hour, Min, Sec, MSec;

            Year = BCDtoByte(Buffer[Pos]);
            if (Year < 90)
                Year += 2000;
            else
                Year += 1900;

            Month = BCDtoByte(Buffer[Pos + 1]);
            Day = BCDtoByte(Buffer[Pos + 2]);
            Hour = BCDtoByte(Buffer[Pos + 3]);
            Min = BCDtoByte(Buffer[Pos + 4]);
            Sec = BCDtoByte(Buffer[Pos + 5]);
            MSec = (BCDtoByte(Buffer[Pos + 6]) * 10) + (BCDtoByte(Buffer[Pos + 7]) / 10);
            try
            {
                return new DateTime(Year, Month, Day, Hour, Min, Sec, MSec);
            }
            catch (System.ArgumentOutOfRangeException)
            {
                return new DateTime(0);
            }
        }
        public static void SetDateTimeAt(byte[] Buffer, int Pos, DateTime Value)
        {
            int Year = Value.Year;
            int Month = Value.Month;
            int Day = Value.Day;
            int Hour = Value.Hour;
            int Min = Value.Minute;
            int Sec = Value.Second;
            int Dow = (int)Value.DayOfWeek + 1;
            // MSecH = First two digits of miliseconds 
            int MsecH = Value.Millisecond / 10;
            // MSecL = Last digit of miliseconds
            int MsecL = Value.Millisecond % 10;
            if (Year > 1999)
                Year -= 2000;

            Buffer[Pos] = ByteToBCD(Year);
            Buffer[Pos + 1] = ByteToBCD(Month);
            Buffer[Pos + 2] = ByteToBCD(Day);
            Buffer[Pos + 3] = ByteToBCD(Hour);
            Buffer[Pos + 4] = ByteToBCD(Min);
            Buffer[Pos + 5] = ByteToBCD(Sec);
            Buffer[Pos + 6] = ByteToBCD(MsecH);
            Buffer[Pos + 7] = ByteToBCD(MsecL * 10 + Dow);
        }
        #endregion

        #region Get/Set DATE (S7 DATE) 
        public static DateTime GetDateAt(byte[] Buffer, int Pos)
        {
            try
            {
                return new DateTime(1990, 1, 1).AddDays(GetIntAt(Buffer, Pos));
            }
            catch (System.ArgumentOutOfRangeException)
            {
                return new DateTime(0);
            }
        }
        public static void SetDateAt(byte[] Buffer, int Pos, DateTime Value)
        {
            SetIntAt(Buffer, Pos, (Int16)(Value - new DateTime(1990, 1, 1)).Days);
        }

        #endregion

        #region Get/Set TOD (S7 TIME_OF_DAY)
        public static DateTime GetTODAt(byte[] Buffer, int Pos)
        {
            try
            {
                return new DateTime(0).AddMilliseconds(ByteHelper.GetDIntAt(Buffer, Pos));
            }
            catch (System.ArgumentOutOfRangeException)
            {
                return new DateTime(0);
            }
        }
        public static void SetTODAt(byte[] Buffer, int Pos, DateTime Value)
        {
            TimeSpan Time = Value.TimeOfDay;
            SetDIntAt(Buffer, Pos, (Int32)Math.Round(Time.TotalMilliseconds));
        }
        #endregion

        #region Get/Set LTOD (S7 1500 LONG TIME_OF_DAY)
        public static DateTime GetLTODAt(byte[] Buffer, int Pos)
        {
            // .NET Tick = 100 ns, S71500 Tick = 1 ns
            try
            {
                return new DateTime(Math.Abs(GetLIntAt(Buffer, Pos) / 100));
            }
            catch (System.ArgumentOutOfRangeException)
            {
                return new DateTime(0);
            }
        }
        public static void SetLTODAt(byte[] Buffer, int Pos, DateTime Value)
        {
            TimeSpan Time = Value.TimeOfDay;
            SetLIntAt(Buffer, Pos, (Int64)Time.Ticks * 100);
        }
        #endregion

        #region GET/SET LDT (S7 1500 Long Date and Time)
        public static DateTime GetLDTAt(byte[] Buffer, int Pos)
        {
            try
            {
                return new DateTime((GetLIntAt(Buffer, Pos) / 100) + bias);
            }
            catch (System.ArgumentOutOfRangeException)
            {
                return new DateTime(0);
            }
        }
        public static void SetLDTAt(byte[] Buffer, int Pos, DateTime Value)
        {
            SetLIntAt(Buffer, Pos, (Value.Ticks - bias) * 100);
        }
        #endregion

        #region Get/Set DTL (S71200/1500 Date and Time)
        // Thanks to Johan Cardoen for GetDTLAt
        public static DateTime GetDTLAt(byte[] Buffer, int Pos)
        {
            int Year, Month, Day, Hour, Min, Sec, MSec;

            Year = Buffer[Pos] * 256 + Buffer[Pos + 1];
            Month = Buffer[Pos + 2];
            Day = Buffer[Pos + 3];
            Hour = Buffer[Pos + 5];
            Min = Buffer[Pos + 6];
            Sec = Buffer[Pos + 7];
            MSec = (int)GetUDIntAt(Buffer, Pos + 8) / 1000000;

            try
            {
                return new DateTime(Year, Month, Day, Hour, Min, Sec, MSec);
            }
            catch (System.ArgumentOutOfRangeException)
            {
                return new DateTime(0);
            }
        }
        public static void SetDTLAt(byte[] Buffer, int Pos, DateTime Value)
        {
            short Year = (short)Value.Year;
            byte Month = (byte)Value.Month;
            byte Day = (byte)Value.Day;
            byte Hour = (byte)Value.Hour;
            byte Min = (byte)Value.Minute;
            byte Sec = (byte)Value.Second;
            byte Dow = (byte)(Value.DayOfWeek + 1);

            Int32 NanoSecs = Value.Millisecond * 1000000;

            var bytes_short = BitConverter.GetBytes(Year);

            Buffer[Pos] = bytes_short[1];
            Buffer[Pos + 1] = bytes_short[0];
            Buffer[Pos + 2] = Month;
            Buffer[Pos + 3] = Day;
            Buffer[Pos + 4] = Dow;
            Buffer[Pos + 5] = Hour;
            Buffer[Pos + 6] = Min;
            Buffer[Pos + 7] = Sec;
            SetDIntAt(Buffer, Pos + 8, NanoSecs);
        }

        #endregion

        #region Get/Set String (S7 String)
        // Thanks to Pablo Agirre 
        public static string GetStringAt(byte[] Buffer, int Pos)
        {
            int size = (int)Buffer[Pos + 1];
            return Encoding.ASCII.GetString(Buffer, Pos + 2, size);
        }

        public static void SetStringAt(byte[] Buffer, int Pos, int MaxLen, string Value)
        {
            int size = Value.Length;
            Buffer[Pos] = (byte)MaxLen;
            Buffer[Pos + 1] = (byte)size;
            Encoding.ASCII.GetBytes(Value, 0, size, Buffer, Pos + 2);
        }
        #endregion

        #region Get/Set Array of char (S7 ARRAY OF CHARS)
        public static string GetCharsAt(byte[] Buffer, int Pos, int Size)
        {
            return Encoding.ASCII.GetString(Buffer, Pos, Size);
        }
        public static void SetCharsAt(byte[] Buffer, int Pos, string Value)
        {
            int MaxLen = Buffer.Length - Pos;
            // Truncs the string if there's no room enough        
            if (MaxLen > Value.Length) MaxLen = Value.Length;
            Encoding.ASCII.GetBytes(Value, 0, MaxLen, Buffer, Pos);
        }
        #endregion

        #region Get/Set Counter
        public static int GetCounter(ushort Value)
        {
            return BCDtoByte((byte)Value) * 100 + BCDtoByte((byte)(Value >> 8));
        }

        public static int GetCounterAt(ushort[] Buffer, int Index)
        {
            return GetCounter(Buffer[Index]);
        }

        public static ushort ToCounter(int Value)
        {
            return (ushort)(ByteToBCD(Value / 100) + (ByteToBCD(Value % 100) << 8));
        }

        public static void SetCounterAt(ushort[] Buffer, int Pos, int Value)
        {
            Buffer[Pos] = ToCounter(Value);
        }
        #endregion

        #endregion [Help Functions]
    }
}
