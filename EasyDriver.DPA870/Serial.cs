using System;
using System.IO.Ports;

namespace EasyDriver.DPA870
{
    class Serial : IDisposable
    {
        public SerialPort SerialPort { get; private set; } = new SerialPort();
        public string modbusStatus;
        public int ResponseTimeOut { get; set; } = 300;

        #region Constructor / Deconstructor
        public Serial()
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
                    SerialPort.DataReceived += SerialPort_DataReceived;
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
                    SerialPort.DataReceived -= SerialPort_DataReceived;
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

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            DataReceived?.Invoke(sender, e);
        }

        public event SerialDataReceivedEventHandler DataReceived;

        public void Dispose()
        {
            SerialPort?.Close();
            SerialPort?.Dispose();
            GC.SuppressFinalize(SerialPort);
        }
    }
}
