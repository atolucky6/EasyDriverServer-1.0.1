using EasyDriverPlugin;
using EasyScada.Core;
using System;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //float value = 123.456F;
            //Console.WriteLine(value.ToString());

            //byte[] bufferL = new byte[8];
            //byte[] bufferB = new byte[8];

            //SetRealAt(bufferL, 0, value, ByteOrder.LittleEndian);
            //SetRealAt(bufferB, 0, value, ByteOrder.BigEndian);

            //string s = string.Empty;
            //foreach (var item in bufferL)
            //{
            //    s += item.ToString("X");
            //}

            //Console.WriteLine($"Byte stored in Litte Endian: {s}");

            //s = string.Empty;
            //foreach (var item in bufferB)
            //{
            //    s += item.ToString("X");
            //}

            //Console.WriteLine($"Byte stored in Big Endian: {s}");

            //float result = 0;

            //result = GetRealAt(bufferL, 0, ByteOrder.LittleEndian);
            //Console.WriteLine($"Value get in Litte Endian: {result}");

            //result = GetRealAt(bufferB, 0, ByteOrder.BigEndian);
            //Console.WriteLine($"Value get in Litte Endian: {result}");

            try
            {
                EasyScadaServer server = new EasyScadaServer();
                server.Start();
                Console.WriteLine("Server running at port 9090");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.ReadLine();

        }

        public static int GetIntAt(byte[] Buffer, int Pos, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            switch (byteOrder)
            {
                case ByteOrder.ABCD:
                    return (int)((Buffer[Pos] << 8) | Buffer[Pos + 1]);
                case ByteOrder.DCAB:
                    return (int)(Buffer[Pos] | Buffer[Pos + 1] << 8);
                default:
                    return 0;
            }
        }
        public static void SetIntAt(byte[] Buffer, int Pos, Int16 Value, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            switch (byteOrder)
            {
                case ByteOrder.ABCD:
                    Buffer[Pos] = (byte)(Value >> 8);
                    Buffer[Pos + 1] = (byte)(Value & 0x00FF);
                    break;
                case ByteOrder.DCAB:
                    Buffer[Pos] = (byte)(Value & 0x00FF);
                    Buffer[Pos + 1] = (byte)(Value >> 8);
                    break;
                default:
                    break;
            }
        }

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
                case ByteOrder.DCAB:
                    {
                        int Result;
                        Result = Buffer[Pos + 3]; Result <<= 8;
                        Result += Buffer[Pos + 2]; Result <<= 8;
                        Result += Buffer[Pos + 1]; Result <<= 8;
                        Result += Buffer[Pos];
                        return Result;
                    }
                default:
                    return 0;
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
                case ByteOrder.DCAB:
                    Buffer[Pos] = (byte)(Value & 0xFF);
                    Buffer[Pos + 1] = (byte)((Value >> 8) & 0xFF);
                    Buffer[Pos + 2] = (byte)((Value >> 16) & 0xFF);
                    Buffer[Pos + 3] = (byte)((Value >> 24) & 0xFF);
                    break;
                default:
                    break;
            }
        }


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
                case ByteOrder.DCAB:
                    Buffer[Pos + 3] = FloatArray[3];
                    Buffer[Pos + 2] = FloatArray[2];
                    Buffer[Pos + 1] = FloatArray[1];
                    Buffer[Pos + 0] = FloatArray[0];
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
                case ByteOrder.DCAB:
                    {
                        UInt32 Result;
                        Result = Buffer[Pos + 3]; Result <<= 8;
                        Result |= Buffer[Pos + 2]; Result <<= 8;
                        Result |= Buffer[Pos + 1]; Result <<= 8;
                        Result |= Buffer[Pos];
                        return Result;
                    }
                default:
                    return 0;
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
                case ByteOrder.DCAB:
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

    }
}
