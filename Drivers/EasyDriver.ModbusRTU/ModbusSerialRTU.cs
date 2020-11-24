using System;
using System.IO.Ports;

namespace EasyDriver.ModbusRTU
{
    class ModbusSerialRTU : IDisposable
    {
        public SerialPort SerialPort { get; private set; } = new SerialPort();
        public string modbusStatus;
        public int ResponseTimeOut { get; set; } = 300;

        #region Constructor / Deconstructor
        public ModbusSerialRTU()
        {
        }
        #endregion

        #region Open / Close Procedures
        public void Init(string portName, int baudRate, int databits, Parity parity, StopBits stopBits)
        {
            //Assign desired settings to the serial port:
            if (!SerialPort.IsOpen)
                SerialPort.PortName = portName;
            else
            {
                if (portName != SerialPort.PortName)
                {
                    SerialPort.Close();
                    SerialPort.PortName = portName;
                }
            }
            SerialPort.BaudRate = baudRate;
            SerialPort.DataBits = databits;
            SerialPort.Parity = parity;
            if (stopBits != StopBits.None)
                SerialPort.StopBits = stopBits;
        }

        public bool Open()
        {
            //Ensure port isn't already opened:
            if (!SerialPort.IsOpen)
            {
                //These timeouts are default and cannot be editted through the class at this point:
                SerialPort.ReadTimeout = ResponseTimeOut;
                SerialPort.WriteTimeout = ResponseTimeOut;

                try
                {
                    SerialPort.Open();
                }
                catch (Exception err)
                {
                    modbusStatus = "Error opening " + SerialPort.PortName + ": " + err.Message;
                    return false;
                }
                modbusStatus = SerialPort.PortName + " opened successfully";
                return true;
            }
            else
            {
                modbusStatus = SerialPort.PortName + " already opened";
                return true;
            }
        }
        public bool Close()
        {
            //Ensure port is opened before attempting to close:
            if (SerialPort.IsOpen)
            {
                try
                {
                    SerialPort.Close();
                }
                catch (Exception err)
                {
                    modbusStatus = "Error closing " + SerialPort.PortName + ": " + err.Message;
                    return false;
                }
                modbusStatus = SerialPort.PortName + " closed successfully";
                return true;
            }
            else
            {
                modbusStatus = SerialPort.PortName + " is not open";
                return false;
            }
        }
        #endregion

        #region CRC Computation
        private void GetCRC(byte[] message, ref byte[] CRC)
        {

            //SEE CRC.XLS DOCUMENTATION//

            //Function expects a modbus message of any length as well as a 2 byte CRC array in which to 
            //return the CRC values:

            ushort CRCFull = 0xFFFF;
            byte CRCHigh = 0xFF, CRCLow = 0xFF;
            char CRCLSB;

            for (int i = 0; i < (message.Length) - 2; i++)
            {
                //XOR CRCfull with 16bits message to compute it's CRC
                CRCFull = (ushort)(CRCFull ^ message[i]);

                for (int j = 0; j < 8; j++)
                {
                    //get LSB
                    CRCLSB = (char)(CRCFull & 0x0001);

                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }
            CRC[1] = CRCHigh = (byte)((CRCFull >> 8) & 0xFF);
            CRC[0] = CRCLow = (byte)(CRCFull & 0xFF);
        }
        #endregion

        #region Build Message
        private void BuildMessage(byte deviceId, byte type, ushort start, ushort registers, ref byte[] message)
        {
            //Array to receive CRC bytes:
            byte[] CRC = new byte[2];

            message[0] = deviceId;
            message[1] = type;
            message[2] = (byte)(start >> 8);
            message[3] = (byte)start;
            message[4] = (byte)(registers >> 8);
            message[5] = (byte)registers;

            GetCRC(message, ref CRC);
            message[message.Length - 2] = CRC[0];
            message[message.Length - 1] = CRC[1];

        }
        #endregion

        #region Check Response
        private bool CheckResponse(byte[] response)
        {
            //Perform a basic CRC check:
            byte[] CRC = new byte[2];
            GetCRC(response, ref CRC);
            if (CRC[0] == response[response.Length - 2] && CRC[1] == response[response.Length - 1])
                return true;
            else
                return false;
        }
        #endregion

        #region Get Response
        private void GetResponse(ref byte[] response)
        {
            //There is a bug in .Net 2.0 DataReceived Event that prevents people from using this
            //event as an interrupt to handle data (it doesn't fire all of the time).  Therefore
            //we have to use the ReadByte command for a fixed length as it's been shown to be reliable.
            for (int i = 0; i < response.Length; i++)
            {
                response[i] = (byte)(SerialPort.ReadByte());
            }
        }
        #endregion

        #region Function 16 - Write Multiple Registers
        public bool WriteHoldingRegisters(byte deviceId, ushort start, ushort registers, byte[] values)
        {
            //Ensure port is open:
            if (SerialPort.IsOpen)
            {
                //Clear in/out buffers:
                SerialPort.DiscardOutBuffer();
                SerialPort.DiscardInBuffer();
                //Message is 1 addr + 1 fcn + 2 start + 2 reg + 1 count + 2 * reg vals + 2 CRC
                byte[] message = new byte[9 + 2 * registers];
                //Function 16 response is fixed at 8 bytes
                byte[] response = new byte[8];

                //Add bytecount to message:
                message[6] = (byte)(registers * 2);
                //Put write values into message prior to sending:
                for (int i = 0; i < registers; i++)
                {
                    message[7 + 2 * i] = values[2 * i];
                    message[8 + 2 * i] = values[2 * i + 1];
                }
                //Build outgoing message:
                BuildMessage(deviceId, (byte)16, start, registers, ref message);

                //Send Modbus message to Serial Port:
                try
                {
                    SerialPort.Write(message, 0, message.Length);
                    GetResponse(ref response);
                }
                catch (Exception err)
                {
                    modbusStatus = "Error in write event: " + err.Message;
                    return false;
                }
                //Evaluate message:
                if (CheckResponse(response))
                {
                    modbusStatus = "Good";
                    return true;
                }
                else
                {
                    modbusStatus = "CRC error";
                    return false;
                }
            }
            else
            {
                modbusStatus = "Serial port not open";
                return false;
            }
        }
        #endregion

        #region Function 3 - Read Holding Registers
        public bool ReadHoldingRegisters(byte deviceId, ushort start, ushort registers, ref byte[] values)
        {
            //Ensure port is open:
            if (SerialPort.IsOpen)
            {
                //Clear in/out buffers:
                SerialPort.DiscardOutBuffer();
                SerialPort.DiscardInBuffer();
                //Function 3 request is always 8 bytes:
                byte[] message = new byte[8];
                //Function 3 response buffer:
                byte[] response = new byte[5 + 2 * registers];
                //Build outgoing modbus message:
                BuildMessage(deviceId, (byte)3, start, registers, ref message);
                //Send modbus message to Serial Port:
                try
                {
                    SerialPort.Write(message, 0, message.Length);
                    GetResponse(ref response);
                }
                catch (Exception err)
                {
                    modbusStatus = "Error in read event: " + err.Message;
                    return false;
                }
                //Evaluate message:
                if (CheckResponse(response))
                {
                    //Return requested register values:
                    for (int i = 0; i < (response.Length - 5) / 2; i++)
                    {
                        values[2 * i] = response[2 * i + 3];
                        //values[i] <<= 8;
                        values[2 * i + 1] = response[2 * i + 4];
                    }
                    modbusStatus = "Good";
                    return true;
                }
                else
                {
                    modbusStatus = "CRC error";
                    return false;
                }
            }
            else
            {
                modbusStatus = "Serial port not open";
                return false;
            }

        }
        #endregion

        #region Function 05 - Write Single Coil
        public bool WriteSingleCoil(byte deviceId, ushort start, bool value)
        {
            //Ensure port is open:
            if (SerialPort.IsOpen)
            {
                //Clear in/out buffers:
                SerialPort.DiscardOutBuffer();
                SerialPort.DiscardInBuffer();
                //Message is 1 addr + 1 fcn + 2 start + 2 status to write + 2 CRC
                byte[] message = new byte[8];
                //Function 16 response is fixed at 8 bytes
                byte[] response = new byte[8];

                //Array to receive CRC bytes:
                byte[] CRC = new byte[2];

                message[0] = deviceId;
                message[1] = 5;
                message[2] = (byte)(start >> 8);
                message[3] = (byte)start;

                if (value == true)
                    message[4] = (byte)(0xFF);
                else
                    message[4] = 0;

                message[5] = 0;

                GetCRC(message, ref CRC);
                message[message.Length - 2] = CRC[0];
                message[message.Length - 1] = CRC[1];

                //Send Modbus message to Serial Port:
                try
                {
                    SerialPort.Write(message, 0, message.Length);
                    GetResponse(ref response);
                }
                catch (Exception err)
                {
                    modbusStatus = "Error in write event: " + err.Message;
                    return false;
                }
                //Evaluate message:
                if (CheckResponse(response))
                {
                    for (int i = 0; i < response.Length; i++)
                    {
                        if (response[i] != message[i])
                        {
                            modbusStatus = "Wrong reSerialPortonse";
                            return false;
                        }
                    }
                    modbusStatus = "Good";
                    return true;
                }
                else
                {
                    modbusStatus = "CRC error";
                    return false;
                }
            }
            else
            {
                modbusStatus = "Serial port not open";
                return false;
            }
        }
        public bool WriteSingleCoil(byte deviceId, ushort start, byte value)
        {
            //Ensure port is open:
            if (SerialPort.IsOpen)
            {
                //Clear in/out buffers:
                SerialPort.DiscardOutBuffer();
                SerialPort.DiscardInBuffer();
                //Message is 1 addr + 1 fcn + 2 start + 2 status to write + 2 CRC
                byte[] message = new byte[8];
                //Function 16 response is fixed at 8 bytes
                byte[] response = new byte[8];

                //Array to receive CRC bytes:
                byte[] CRC = new byte[2];

                message[0] = deviceId;
                message[1] = 5;
                message[2] = (byte)(start >> 8);
                message[3] = (byte)start;
                message[4] = value;
                message[5] = 0;

                GetCRC(message, ref CRC);
                message[message.Length - 2] = CRC[0];
                message[message.Length - 1] = CRC[1];

                //Send Modbus message to Serial Port:
                try
                {
                    SerialPort.Write(message, 0, message.Length);
                    GetResponse(ref response);
                }
                catch (Exception err)
                {
                    modbusStatus = "Error in write event: " + err.Message;
                    return false;
                }
                //Evaluate message:
                if (CheckResponse(response))
                {
                    for (int i = 0; i < response.Length; i++)
                    {
                        if (response[i] != message[i])
                        {
                            modbusStatus = "Wrong reSerialPortonse";
                            return false;
                        }
                    }
                    modbusStatus = "Good";
                    return true;
                }
                else
                {
                    modbusStatus = "CRC error";
                    return false;
                }
            }
            else
            {
                modbusStatus = "Serial port not open";
                return false;
            }
        }
        #endregion

        #region Function 15 - Write Multiple Coils
        public bool WriteMultipleCoils(byte deviceId, ushort start, ushort coils, bool[] values)
        {
            //Ensure port is open:
            if (SerialPort.IsOpen)
            {
                //Clear in/out buffers:
                SerialPort.DiscardOutBuffer();
                SerialPort.DiscardInBuffer();

                //Message is 1 addr + 1 fcn + 2 startcoil + 2 coilsnum  + 1 dbytes + dbytes * bytevals + 2 CRC

                int dbytes = 0;
                if ((coils % 8) > 0)
                    dbytes = coils / 8 + 1;
                else
                    dbytes = coils / 8;

                byte[] message = new byte[9 + dbytes];
                //Function 15 response is fixed at 8 bytes
                byte[] response = new byte[8];

                //Array to receive CRC bytes:
                byte[] CRC = new byte[2];

                message[0] = deviceId;
                message[1] = 15;
                message[2] = (byte)(start >> 8);
                message[3] = (byte)start;
                message[4] = (byte)(coils >> 8);
                message[5] = (byte)coils;
                message[6] = (byte)dbytes;

                int k = 0;
                if ((coils / 8) < dbytes)
                    k = dbytes * 8 - coils;


                //lay byte chan truoc		

                for (int i = 0; i < coils / 8; i++)
                {
                    message[7 + i] = 0;
                    for (int j = 0; j < 8; j++)
                    {
                        message[7 + i] = Convert.ToByte(Convert.ToByte(values[8 * i + 7 - j]) | (message[7 + i] << 1));
                    }
                }

                //xu ly le
                for (int i = coils / 8; i < dbytes; i++)
                {

                    message[7 + i] = 0;

                    for (int j = k; j < 8; j++)
                    {
                        message[7 + i] = Convert.ToByte(Convert.ToByte(values[8 * i + 7 - j]) | (message[7 + i] << 1));
                    }
                }

                GetCRC(message, ref CRC);
                message[message.Length - 2] = CRC[0];
                message[message.Length - 1] = CRC[1];


                //Send Modbus message to Serial Port:
                try
                {
                    SerialPort.Write(message, 0, message.Length);
                    GetResponse(ref response);
                }
                catch (Exception err)
                {
                    modbusStatus = "Error in write event: " + err.Message;
                    return false;
                }
                //Evaluate message:
                if (CheckResponse(response))
                {
                    for (int i = 0; i < 6; i++)
                    {
                        if (response[i] != message[i])
                        {
                            modbusStatus = "Bad";
                            return false;
                        }
                    }
                    modbusStatus = "Good";
                    return true;
                }
                else
                {
                    modbusStatus = "CRC error";
                    return false;
                }
            }
            else
            {
                modbusStatus = "Serial port not open";
                return false;
            }
        }
        #endregion

        #region Function 1 - Read Coil Status
        public bool ReadCoils(byte deviceId, ushort start, ushort coils, ref bool[] values)
        {

            //Ensure port is open:
            if (SerialPort.IsOpen)
            {
                //Clear in/out buffers:
                SerialPort.DiscardOutBuffer();
                SerialPort.DiscardInBuffer();
                //Function 1 request is always 8 bytes:
                byte[] message = new byte[8];

                //Array to receive CRC bytes:
                byte[] CRC = new byte[2];

                message[0] = deviceId;
                message[1] = 1;
                message[2] = (byte)(start >> 8);
                message[3] = (byte)start;
                message[4] = (byte)(coils >> 8);
                message[5] = (byte)coils;

                GetCRC(message, ref CRC);
                message[message.Length - 2] = CRC[0];
                message[message.Length - 1] = CRC[1];

                //Function 1 response buffer:
                //Frame: 1add + 1func + 1dbytes + dbytes*data + 2CRC

                int dbytes = 0;
                if ((coils % 8) > 0)
                    dbytes = coils / 8 + 1;
                else
                    dbytes = coils / 8;
                byte[] response = new byte[5 + dbytes];
                //Send modbus message to Serial Port:
                try
                {
                    SerialPort.Write(message, 0, message.Length);
                    GetResponse(ref response);
                }
                catch (Exception err)
                {
                    modbusStatus = "Error in read event: " + err.Message;
                    return false;
                }
                //Evaluate message:
                if (CheckResponse(response))
                {
                    //Return requested register values:
                    for (int i = 0; i < (response.Length - 5); i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            if ((8 * i + j) < coils)
                                values[8 * i + j] = Convert.ToBoolean((response[3 + i] >> j) & 0x01);
                        }
                    }
                    modbusStatus = "Good";
                    return true;
                }
                else
                {
                    modbusStatus = "CRC error";
                    return false;
                }
            }
            else
            {
                modbusStatus = "Serial port not open";
                return false;
            }

        }
        #endregion

        #region Function 2 - Read Discrete Input Contacts 1xxxx
        public bool ReadDiscreteInputContact(byte deviceId, ushort start, ushort inputs, ref bool[] values)
        {
            //Ensure port is open:
            if (SerialPort.IsOpen)
            {
                //Clear in/out buffers:
                SerialPort.DiscardOutBuffer();
                SerialPort.DiscardInBuffer();
                //Function 2 request is always 8 bytes:
                byte[] message = new byte[8];

                //Array to receive CRC bytes:
                byte[] CRC = new byte[2];

                message[0] = deviceId;
                message[1] = 2;
                message[2] = (byte)(start >> 8);
                message[3] = (byte)start;
                message[4] = (byte)(inputs >> 8);
                message[5] = (byte)inputs;

                GetCRC(message, ref CRC);
                message[message.Length - 2] = CRC[0];
                message[message.Length - 1] = CRC[1];

                //Function 2 response buffer:
                //Frame: 1add + 1func + 1dbytes + dbytes*data + 2CRC

                int dbytes = 0;
                if ((inputs % 8) > 0)
                    dbytes = inputs / 8 + 1;
                else
                    dbytes = inputs / 8;
                byte[] response = new byte[5 + dbytes];
                //Send modbus message to Serial Port:
                try
                {
                    SerialPort.Write(message, 0, message.Length);
                    GetResponse(ref response);
                }
                catch (Exception err)
                {
                    modbusStatus = "Error in read event: " + err.Message;
                    return false;
                }
                //Evaluate message:
                if (CheckResponse(response))
                {
                    //Return requested register values:
                    for (int i = 0; i < (response.Length - 5); i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            if ((8 * i + j) < inputs)
                                values[8 * i + j] = Convert.ToBoolean((response[3 + i] >> j) & 0x01);
                        }
                    }
                    modbusStatus = "Good";
                    return true;
                }
                else
                {
                    modbusStatus = "CRC error";
                    return false;
                }
            }
            else
            {
                modbusStatus = "Serial port not open";
                return false;
            }


        }
        #endregion

        #region Function 4 - Read Input Register 3xxxx
        public bool ReadInputRegisters(byte deviceId, ushort start, ushort registers, ref byte[] values)
        {
            //Ensure port is open:
            if (SerialPort.IsOpen)
            {
                //Clear in/out buffers:
                SerialPort.DiscardOutBuffer();
                SerialPort.DiscardInBuffer();
                //Function 4 request is always 8 bytes:
                byte[] message = new byte[8];
                //Function 3 response buffer:
                byte[] response = new byte[5 + 2 * registers];
                //Build outgoing modbus message:
                BuildMessage(deviceId, (byte)4, start, registers, ref message);
                //Send modbus message to Serial Port:
                try
                {
                    SerialPort.Write(message, 0, message.Length);
                    GetResponse(ref response);
                }
                catch (Exception err)
                {
                    modbusStatus = "Error in read event: " + err.Message;
                    return false;
                }

                //Evaluate message:
                if (CheckResponse(response))
                {
                    //Return requested register values:
                    for (int i = 0; i < (response.Length - 5) / 2; i++)
                    {
                        values[2 * i] = response[2 * i + 3];
                        //values[i] <<= 8;
                        values[2 * i + 1] = response[2 * i + 4];
                    }
                    modbusStatus = "Good";
                    return true;
                }
                else
                {
                    modbusStatus = "CRC error";
                    return false;
                }
            }
            else
            {
                modbusStatus = "Serial port not open";
                return false;
            }

        }

        public void Dispose()
        {
            SerialPort?.Close();
            SerialPort?.Dispose();
            GC.SuppressFinalize(SerialPort);
        }
        #endregion
    }
}
