using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Timers;
using EasyDriverPlugin;

namespace EasyDriver.ModbusRTU
{
    public class ModbusRTUDriver : IEasyDriverPlugin
    {
        static readonly List<IDataType> supportDataTypes;
        static ModbusRTUDriver()
        {
            supportDataTypes = new List<IDataType>
            {
                new Bool(),
                new Word(),
                new DWord(),
                new LWord(),
                new Int(),
                new DInt(),
                new LInt(),
                new Real(),
                new EasyDriverPlugin.Char(),
            };
        }

        public IChannel Channel { get; set; }

        public int OpenPortFailCount { get; set; } = 0;

        readonly ModbusSerialRTU mbMaster;
        readonly Timer scanTimer;
        
        public ModbusRTUDriver()
        {
            mbMaster = new ModbusSerialRTU();
            scanTimer = new Timer(1);
            scanTimer.Elapsed += ScanTimer_Elapsed;
        }

        public bool Connect()
        {
            if (Channel.ParameterContainer.Parameters.Count < 5)
                return false;
            try
            {
                InitializeSerialPort();
                bool result = mbMaster.Open();
                scanTimer.Enabled = true;
                return result;
            }
            catch { return false; }
        }

        private void InitializeSerialPort()
        {
            if (Channel.ParameterContainer.Parameters.Count >= 5)
            {
                var port = Channel.ParameterContainer.Parameters["Port"].ToString();
                var baudRate = (int)Channel.ParameterContainer.Parameters["Baudrate"];
                var dataBits = (int)Channel.ParameterContainer.Parameters["DataBits"];
                var stopBits = (StopBits)Channel.ParameterContainer.Parameters["StopBits"];
                var parity = (Parity)Channel.ParameterContainer.Parameters["Parity"];
                mbMaster.Init(port, baudRate, dataBits, parity, stopBits);
            }
        }

        private void ScanTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                scanTimer.Enabled = false;
                InitializeSerialPort();

                if (mbMaster.Open())
                {
                    OpenPortFailCount = 0;
                    for (int i = 0; i < Channel.Childs.Count; i++)
                    {
                        IDevice device = Channel.Childs[i] as IDevice;

                        byte deviceId = byte.Parse(device.ParameterContainer.Parameters["DeviceId"].ToString());
                        ByteOrder byteOrder = (ByteOrder)device.ParameterContainer.Parameters["ByteOrder"];
                        int maxTryTimes = int.Parse(device.ParameterContainer.Parameters["TryReadWriteTimes"].ToString());
                        int timeout = int.Parse(device.ParameterContainer.Parameters["Timeout"].ToString());

                        mbMaster.ResponseTimeOut = timeout;
                        mbMaster.SerialPort.ReadTimeout = timeout;
                        mbMaster.SerialPort.WriteTimeout = timeout;

                        for (int k = 0; k < device.Childs.Count; k++)
                        {
                            ITag tag = device.Childs[k] as ITag;

                            if (!tag.ParameterContainer.Parameters.ContainsKey("TryCount"))
                                tag.ParameterContainer.Parameters["TryCount"] = 0;

                            int currentTryCount = (int)tag.ParameterContainer.Parameters["TryCount"];
                            bool readSuccess = false;

                            if ((DateTime.Now - tag.TimeStamp).TotalMilliseconds >= tag.RefreshRate)
                            {
                                if (ushort.TryParse(tag.Address, out ushort address))
                                {
                                    if (address >= 0 && address < 10000)
                                    {
                                        address--;
                                        bool[] retVal = new bool[1];
                                        if (mbMaster.ReadCoils(deviceId, address, 1, ref retVal))
                                        {
                                            readSuccess = true;
                                            tag.ParameterContainer.Parameters["TryCount"] = 0;
                                            tag.Value = retVal[0] ? "1" : "0";
                                            tag.TimeStamp = DateTime.Now;
                                            tag.Quality = Quality.Good;
                                            tag.RefreshInterval = DateTime.Now - tag.TimeStamp;

                                        }

                                    }
                                    else if (address > 10000 && address < 20000)
                                    {
                                        address = (ushort)(address - 10001);
                                        bool[] retVal = new bool[1];
                                        if (mbMaster.ReadDiscreteInputContact(deviceId, address, 1, ref retVal))
                                        {
                                            readSuccess = true;
                                            tag.ParameterContainer.Parameters["TryCount"] = 0;
                                            tag.Value = retVal[0] ? "1" : "0";
                                            tag.TimeStamp = DateTime.Now;
                                            tag.Quality = Quality.Good;
                                            tag.RefreshInterval = DateTime.Now - tag.TimeStamp;

                                        }

                                    }
                                    else if (address > 30000 && address < 40000)
                                    {
                                        address = (ushort)(address - 30001);
                                        byte[] buffer = new byte[tag.DataType.RequireByteLength];
                                        ushort countRegister = (ushort)(tag.DataType.RequireByteLength / 2);
                                        countRegister = countRegister == 0 ? (ushort)1 : countRegister;
                                        if (mbMaster.ReadInputRegisters(deviceId, address, countRegister, ref buffer))
                                        {
                                            readSuccess = true;
                                            tag.ParameterContainer.Parameters["TryCount"] = 0;
                                            tag.Value = tag.DataType.ConvertToValue(buffer, 0, 0, byteOrder);
                                            tag.TimeStamp = DateTime.Now;
                                            tag.Quality = Quality.Good;
                                            tag.RefreshInterval = DateTime.Now - tag.TimeStamp;

                                        }
                                    }
                                    else if (address > 40000 && address < 50000)
                                    {
                                        address = (ushort)(address - 40001);
                                        byte[] buffer = new byte[tag.DataType.RequireByteLength];
                                        ushort countRegister = (ushort)(tag.DataType.RequireByteLength / 2);
                                        countRegister = countRegister == 0 ? (ushort)1 : countRegister;
                                        if (mbMaster.ReadHoldingRegisters(deviceId, address, countRegister, ref buffer))
                                        {
                                            readSuccess = true;
                                            tag.ParameterContainer.Parameters["TryCount"] = 0;
                                            tag.Value = tag.DataType.ConvertToValue(buffer, 0, 0, byteOrder);
                                            tag.TimeStamp = DateTime.Now;
                                            tag.Quality = Quality.Good;
                                            tag.RefreshInterval = DateTime.Now - tag.TimeStamp;

                                        }
                                    }
                                }

                                if (!readSuccess)
                                {
                                    currentTryCount++;
                                    tag.ParameterContainer.Parameters["TryCount"] = currentTryCount;
                                }
                                else
                                    currentTryCount = 0;

                                if (currentTryCount > maxTryTimes)
                                {
                                    tag.TimeStamp = DateTime.Now;
                                    tag.Quality = Quality.Bad;
                                    tag.RefreshInterval = DateTime.Now - tag.TimeStamp;
                                }

                            }
                        }
                    }
                }
                else
                {
                    OpenPortFailCount++;

                    if (OpenPortFailCount > 5)
                    {
                        for (int i = 0; i < Channel.Childs.Count; i++)
                        {
                            IDevice device = Channel.Childs[i] as IDevice;
                            for (int k = 0; k < device.Childs.Count; k++)
                            {
                                ITag tag = device.Childs[k] as ITag;

                                if ((DateTime.Now - tag.TimeStamp).TotalMilliseconds >= tag.RefreshRate)
                                {
                                    tag.TimeStamp = DateTime.Now;
                                    tag.Quality = Quality.Bad;
                                    tag.RefreshInterval = DateTime.Now - tag.TimeStamp;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally { scanTimer.Enabled = true; }
        }

        public bool Disconnect()
        { 
            try
            {
                return mbMaster.Close();
            }
            catch
            {
                return false;
            }
        }

        public object GetCreateChannelControl()
        {
            return new CreateChannelView(this);
        }

        public object GetCreateDeviceControl()
        {
            return new CreateDeviceView(this, Channel);
        }

        public object GetCreateTagControl(IDevice parent)
        {
            return new CreateTagView(this, parent);
        }

        public object GetEditChannelControl(IChannel channel)
        {
            return new EditChannelView(this, channel);
        }

        public object GetEditDeviceControl(IDevice device)
        {
            return new EditDeviceView(this, device);
        }

        public object GetEditTagControl(ITag tag)
        {
            return new EditTagView(this, tag);
        }

        public IEnumerable<IDataType> GetSupportDataTypes()
        {
            return supportDataTypes;
            
        }

        public void WriteMulti(ITag[] tags, string[] values)
        {
            throw new System.NotImplementedException();
        }

        public Quality WriteSingle(ITag tag, string value)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            Disconnect();
        }
    }

    public static class ValidateHelper
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
    }
}
