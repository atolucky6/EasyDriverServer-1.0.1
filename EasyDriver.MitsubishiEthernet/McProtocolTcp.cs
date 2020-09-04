using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EasyDriver.MitsubishiEthernet
{
    public class McProtocolTcp : IDisposable
    {
        #region Members
        public bool IsConnected { get; protected set; }
        public McFrameType FrameType { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public int Device { get; set; }
        public int Timeout { get; set; }

        McCommandBuilder cmdBuilder;
        TcpClient tcpClient;
        NetworkStream stream;
        #endregion

        #region Constructors
        public McProtocolTcp(string ipAddress, int port, McFrameType frameType, int timeout = 1000)
        {
            IpAddress = ipAddress;
            Port = port;
            FrameType = frameType;
            Timeout = timeout;
            tcpClient = new TcpClient()
            {
                ReceiveTimeout = timeout,
                SendTimeout = timeout
            };
        }
        #endregion

        #region Open/Close connection
        public int Open()
        {
            try
            {
                if (!IsConnected)
                {
                    cmdBuilder = new McCommandBuilder(FrameType);
                    var keepAlive = new List<byte>(sizeof(uint) * 3);
                    keepAlive.AddRange(BitConverter.GetBytes(1U));
                    keepAlive.AddRange(BitConverter.GetBytes(4500U));
                    keepAlive.AddRange(BitConverter.GetBytes(5000U));
                    tcpClient.Client.IOControl(IOControlCode.KeepAliveValues, keepAlive.ToArray(), null);
                    var result = tcpClient.BeginConnect(IpAddress, Port, null, null);
                    var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(4000));
                    if (success)
                    {
                        stream = tcpClient.GetStream();
                        IsConnected = true;
                        return 0;
                    }
                    else
                    {
                        tcpClient = new TcpClient();
                        tcpClient.ReceiveTimeout = Timeout;
                        tcpClient.SendTimeout = Timeout;
                        IsConnected = false;
                        stream?.Dispose();
                    }
                }
                return -1;
            }
            catch (Exception)
            {
                tcpClient = new TcpClient();
                tcpClient.ReceiveTimeout = Timeout;
                tcpClient.SendTimeout = Timeout;
                IsConnected = false;
                return -1;
            }
        }

        public int Close()
        {
            try
            {
                tcpClient.Close();
                IsConnected = false;
            }
            catch { }
            return 0;
        }
        #endregion

        #region Read
        //public int 
        #endregion

        #region Write

        #endregion

        #region IDisposable
        public void Dispose()
        {
            try
            {
                Close();
                stream?.Dispose();
            }
            catch { }
        }
        #endregion

        #region Internals

        protected byte[] SendMessage(byte[] commandFrame)
        {
            try
            {
                NetworkStream ns = stream;
                ns.Write(commandFrame, 0, commandFrame.Length);
                ns.Flush();

                using(var ms = new MemoryStream())
                {
                    var buffer = new byte[256];
                    do
                    {
                        int sz = ns.Read(buffer, 0, buffer.Length);
                        ms.Write(buffer, 0, sz);
                    }
                    while (ns.DataAvailable);
                    IsConnected = true;
                    return ms.ToArray();
                }
            }
            catch
            {
                IsConnected = false;
                return null;
            }
        }

        #endregion

        #region Utils

        public bool CheckResponse(byte[] response, uint subHeader, McFrameType frameType)
        {
            // Get the minimum response length base on frame type
            int responseMinLen = 2;
            switch (frameType)
            {
                case McFrameType.MC1E:
                    responseMinLen = 2;
                    break;
                case McFrameType.MC3E:
                    responseMinLen = 11;
                    break;
                case McFrameType.MC4E:
                    responseMinLen = 15;
                    break;
                default:
                    break;
            }

            // Response invalid when response length less than mimimum response length
            if (response.Length <= responseMinLen)
                return false;

            // Get end code of response
            uint endCode = BitConverter.ToUInt16(new byte[]
            {
                response[responseMinLen - 2],
                response[responseMinLen - 1]
            }, 0);
            // Response valid when end code equal 0
            if (endCode != 0)
                return false;

            // Get the valid response sub header base on sub header we was sent to plc
            uint validSubHeader = GetValidResponseSubHeader(frameType, subHeader);
            // Get the sub header from response buffer
            uint resSubHeader = 0;
            switch (frameType)
            {
                case McFrameType.MC1E:
                case McFrameType.MC3E:
                    resSubHeader = BitConverter.ToUInt16(response, 0);
                    break;
                case McFrameType.MC4E:
                    resSubHeader = BitConverter.ToUInt16(response, 13);
                    break;
                default:
                    break;
            }
            // Response valid when response sub header equal with valid sub header
            return validSubHeader == resSubHeader;
        }

        public uint GetValidResponseSubHeader(McFrameType frameType, uint subHeader)
        {
            switch (frameType)
            {
                case McFrameType.MC3E:
                    return 208; // It means 0xD000 in PLC
                case McFrameType.MC4E:
                    return 212; // It means 0xD400 in PLC
                case McFrameType.MC1E:
                    if (subHeader == 0x00)
                        return 0x80;
                    if (subHeader == 0x01)
                        return 0x81;
                    if (subHeader == 0x02)
                        return 0x82;
                    if (subHeader == 0x03)
                        return 0x83;
                    if (subHeader == 0x04)
                        return 0x84;
                    if (subHeader == 0x05)
                        return 0x85;
                    if (subHeader == 0x06)
                        return 0x86;
                    if (subHeader == 0x07)
                        return 0x87;
                    if (subHeader == 0x08)
                        return 0x88;
                    if (subHeader == 0x09)
                        return 0x89;
                    if (subHeader == 0x17)
                        return 0x97;
                    if (subHeader == 0x18)
                        return 0x98;
                    if (subHeader == 0x19)
                        return 0x99;
                    if (subHeader == 0x1A)
                        return 0x9A;
                    if (subHeader == 0x1B)
                        return 0x9B;
                    if (subHeader == 0x3B)
                        return 0xBB;
                    if (subHeader == 0x3C)
                        return 0xBC;
                    if (subHeader == 0x0E)
                        return 0x8E;
                    if (subHeader == 0xF)
                        return 0x8F;
                    break;
                default:
                    break;
            }
            return 0xD000;
        }

        #endregion
    }

    public class McCommandBuilder
    {
        #region Members
        public uint SerialNumber { get; set; } = 0x0001U;
        public uint NetworkNumber { get; set; } = 0x0000U;
        public uint PCNumber { get; set; } = 0x00FFU;
        public uint IONumber { get; set; } = 0x03FFU;
        public uint ChannelNumber { get; set; } = 0x0000U;
        public uint CPUTimer { get; set; } = 0x0010U;

        public McFrameType FrameType { get; set; }
        public int ResultCode { get; set; }
        public byte[] Response { get; private set; }
        #endregion

        #region Constructors
        public McCommandBuilder(McFrameType frameType)
        {
            FrameType = frameType;
        }
        #endregion

        #region Methods
        public byte[] BuildCommandMC1E(byte subHeader, byte[] data)
        {
            byte[] buffer = new byte[data.Length + 4];
            buffer[0] = subHeader;
            buffer[1] = (byte)PCNumber;
            buffer[2] = (byte)CPUTimer;
            buffer[3] = (byte)(CPUTimer >> 8);
            Array.Copy(data, 0, buffer, 4, data.Length);
            return buffer;
        }

        public byte[] BuildCommandMC3E(uint mainCommand, uint subCommand, byte[] data)
        {
            byte[] buffer = new byte[data.Length + 26];
            // Set Subheader = 0x5000
            buffer[0] = (byte)0x0050U;
            buffer[1] = (byte)(0x0050U >> 8);

            // Set Access Route
            buffer[2] = (byte)NetworkNumber;
            buffer[3] = (byte)PCNumber;
            buffer[4] = (byte)(IONumber & 0xFF);
            buffer[5] = (byte)(IONumber >> 8);
            buffer[6] = (byte)ChannelNumber;

            // Set Request data length
            buffer[7] = (byte)(data.Length + 6);
            buffer[8] = (byte)((data.Length + 6) >> 8);

            // Set Monitoring timer
            buffer[9] = (byte)CPUTimer;
            buffer[10] = (byte)(CPUTimer >> 8);

            // Set Command
            buffer[11] = (byte)mainCommand;
            buffer[12] = (byte)(mainCommand >> 8);
            buffer[13] = (byte)subCommand;
            buffer[14] = (byte)(subCommand >> 8);

            Array.Copy(data, 0, buffer, 15, data.Length);
            return buffer;
        }
        #endregion
    }

    #region Enums
    public enum McFrameType
    {
        MC1E,
        MC3E,
        MC4E,
    }

    public enum PlcDeviceType
    {
        /// <summary>
        /// Internal Relays
        /// </summary>
        M = 0x90,

        /// <summary>
        /// Special Internal Relays
        /// </summary>
        SM = 0x91,

        /// <summary>
        /// Latch Relays
        /// </summary>
        L = 0x92,

        /// <summary>
        /// Annunciator Relays
        /// </summary>
        F = 0x93,

        /// <summary>
        /// Edge Relays
        /// </summary>
        V = 0x94,

        /// <summary>
        /// Step Relays
        /// </summary>
        S = 0x98,

        /// <summary>
        /// Inputs (Hex or Oct)
        /// </summary>
        X = 0x9C,

        /// <summary>
        /// Output (Hex or Oct)
        /// </summary>
        Y = 0x9D,

        /// <summary>
        /// Link Relays (Hex)
        /// </summary>
        B = 0xA0,

        /// <summary>
        /// Special Link Relays (Hex)
        /// </summary>
        SB = 0xA1,

        /// <summary>
        /// Direct Inputs (Hex or Oct)
        /// </summary>
        DX = 0xA2,

        /// <summary>
        /// Direct Outputs (Hex or Oct)
        /// </summary>
        DY = 0xA3,

        /// <summary>
        /// Data Registers
        /// </summary>
        D = 0xA8,   

        /// <summary>
        /// Special Data Registers
        /// </summary>
        SD = 0xA9,

        /// <summary>
        /// Index Registers
        /// </summary>
        Z = 0xCC,

        /// <summary>
        /// Link Registers
        /// </summary>
        W = 0xB4,

        /// <summary>
        /// Link Registers
        /// </summary>
        SW = 0xB5,

        /// <summary>
        /// Time Coils
        /// </summary>
        TC = 0xC0,

        /// <summary>
        /// Timer Contacts
        /// </summary>
        TS = 0xC1,

        /// <summary>
        /// Timer Value
        /// </summary>
        TN = 0xC2,

        /// <summary>
        /// Counter Coils
        /// </summary>
        CC = 0xC3,

        /// <summary>
        /// Counter Contacts
        /// </summary>
        CS = 0xC4,

        /// <summary>
        /// Counter Value
        /// </summary>
        CN = 0xC5,

        ///// <summary>
        ///// Intergrating Timer Coils
        ///// </summary>
        //SC = 0xC6,

        ///// <summary>
        ///// Intergrating Timer Contacts
        ///// </summary>
        //SS = 0xC7,

        ///// <summary>
        ///// Intergrating Timer Value
        ///// </summary>
        //SN = 0xC8,

        ///// <summary>
        ///// File Registers
        ///// </summary>
        //R = 0xAF,

        ///// <summary>
        ///// File Registers
        ///// </summary>
        //ZR = 0xB0,
    }
    #endregion
}
