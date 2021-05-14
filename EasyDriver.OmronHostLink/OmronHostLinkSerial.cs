using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyDriver.OmronHostLink
{
    #region Enums

    public enum ReadCommandCode
    {
        RR,     // CIO AREA READ
        RL,     // LR AREA READ
        RH,     // HR AREA READ
        RC,     // TIMER/COUNTER PV READ
        RG,     // TIMER/COUNTER STATUS READ
        RD,     // DM AREA READ
        RJ,     // AR AREA READ
        RE,     // EM AREA READ
    }

    public enum WriteCommandCode
    {
        #region WRITE
        WR,     // CIO AREA WRITE
        WL,     // LR AREA WRITE
        WH,     // HR AREA WRITE
        WC,     // TIMER/COUNTER PV WRITE
        WG,     // TIMER/COUNTER STATUS WRITE
        WD,     // DM AREA WRITE
        WJ,     // AR AREA WRITE
        WE,     // EM AREA WRITE
        #endregion

        #region FORCED SET/RESET BIT
        KS,     // FORCED SET
        KR,     // FORCED RESET
        FK,     // MULTIPE FORCED SET/RESET
        KC,     // FORCED SET/RESET CANCEL
        #endregion
    }

    public enum Area
    {
        CIO,
        LR,
        WR,
        HR
    }

    #endregion

    public class OmronHostLinkSerial : IDisposable
    {
        #region Members
        private const char CR = (char)(13);

        public SerialPort SerialPort { get; private set; } = new SerialPort();
        public int ResponseTimeOut { get; set; } = 300;
        public string Status { get; private set; }
        public string LastErrorCode { get; private set; }

        #endregion

        #region Constructor / Deconstructor
        public OmronHostLinkSerial()
        {
        }

        public void Dispose()
        {
            SerialPort?.Close();
            SerialPort?.Dispose();
            GC.SuppressFinalize(SerialPort);
        }
        #endregion

        #region Open / Close 
        /// <summary>
        /// Hàm khởi tạo các thông số của cổng Serial
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        /// <param name="databits"></param>
        /// <param name="parity"></param>
        /// <param name="stopBits"></param>
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

        /// <summary>
        /// Hàm mở cổng Serial để giao tiếp với PLC
        /// </summary>
        /// <returns></returns>
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
                    Status = "Error opening " + SerialPort.PortName + ": " + err.Message;
                    return false;
                }
                Status = SerialPort.PortName + " opened successfully";
                return true;
            }
            else
            {
                Status = SerialPort.PortName + " already opened";
                return true;
            }
        }

        /// <summary>
        /// Hàm đóng cổng Serial ngắt giao tiếp với PLC
        /// </summary>
        /// <returns></returns>
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
                    Status = "Error closing " + SerialPort.PortName + ": " + err.Message;
                    return false;
                }
                Status = SerialPort.PortName + " closed successfully";
                return true;
            }
            else
            {
                Status = SerialPort.PortName + " is not open";
                return false;
            }
        }
        #endregion

        #region Build FCS 
        /// <summary>
        /// Hàm tạo FCS
        /// </summary>
        /// <param name="frame">string to perform FCS operation</param>
        /// <returns>Return FCS string of given frame</returns>
        public string BuildFCS(string frame)                       
        {
            int fcs = 0;
            foreach (char chartemp in frame.ToCharArray())
                fcs = fcs ^ Convert.ToInt16(chartemp);
            return string.Format("{0:X2}", Convert.ToUInt32(fcs));
        }
        #endregion

        #region Check FCS
        /// <summary>
        /// Hàm kiểm tra FCS
        /// </summary>
        /// <param name="response">Phản hồi từ PLC</param>
        /// <param name="message">Nội dung không bao gồm FCS</param>
        /// <returns></returns>
        public bool CheckFCS(string response, out string message)
        {
            int length = response.Length;
            string fcs = string.Empty;
            message = string.Empty;
            if (response.EndsWith("*"))
            {
                fcs = response.Substring(length - 3, 2);
                message = response.Remove(length - 3, 3);
            }
            else
            {
                fcs = response.Substring(length - 2, 2);
                message = response.Remove(length - 2, 2);
            }

            string calFCS = BuildFCS(message);
            return calFCS == fcs;
        }
        #endregion

        #region Build frame
        /// <summary>
        /// Hàm tạo khung truyền để gửi xuống PLC
        /// </summary>
        /// <param name="unitNo"></param>
        /// <param name="cmd"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public string BuildFrame(byte unitNo, string cmd, string message)
        {
            // Create instance frame for return
            string frame;
            // Create frame with format: @ + UnitNo + Header Code (CommandType) + Message
            frame = $"@{unitNo.ToString("00")}{cmd}{message}";
            // Build FCS of frame we just create
            string fcs = BuildFCS(frame);
            // Add FCS into the frame
            frame += fcs;
            // Add the terminator code * + CR
            frame += $"*{CR}";
            return frame;
        }
        #endregion

        #region Get Response
        /// <summary>
        /// Hàm lấy tin phản hổi từ PLC
        /// </summary>
        /// <returns></returns>
        private string GetResponse()
        {
            string response = string.Empty;
            char readChar;
            do
            {
                readChar = (char)SerialPort.ReadChar();
                response += readChar;

                if (readChar == '*')
                {
                    // Read the last CR and break from loop
                    readChar = (char)SerialPort.ReadChar();
                    response += readChar;
                    break;
                }
                else if (readChar == CR)
                {
                    Thread.Sleep(10);
                    // Sent CR to the PLC to get next response frame
                    SerialPort.Write(CR.ToString());
                }
            }
            while (true);
            return response;
        }
        #endregion

        #region Read
        /// <summary>
        /// Hàm gửi lệnh đọc vùng nhớ xuống PLC
        /// </summary>
        /// <param name="unitNo">UnitNo của PLC</param>
        /// <param name="cmd">Loại lệnh đọc</param>
        /// <param name="startWordAddress">Địa chỉ bắt đầu đọc</param>
        /// <param name="wordLen">Số lượng word cần đọc</param>
        /// <param name="values">Giá trị đọc được từ PLC</param>
        /// <returns></returns>
        public bool Read(byte unitNo, ReadCommandCode cmd, ushort startWordAddress, ushort wordLen, ref byte[] values)
        {
            // Make sure serial port was opened
            if (SerialPort.IsOpen)
            {
                // Clear in/out buffers
                SerialPort.DiscardInBuffer();
                SerialPort.DiscardOutBuffer();

                // Build content message in frame
                string message = startWordAddress.ToString("0000") + wordLen.ToString("0000");
                // Build the frame to sent to plc
                string frame = BuildFrame(unitNo, cmd.ToString(), message);
                // Init response message
                string response = string.Empty;
                try
                {
                    // Sent frame to the plc
                    SerialPort.Write(frame);
                    // Wait little bit
                    Thread.Sleep(2);
                    // Get the response message from PLC
                    response = GetResponse();

                    if (!response.EndsWith($"*{CR}"))
                    {
                        Status = "Response error";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Status = $"Error in read serial port: {ex.Message}";
                    return false;
                }

                // Split response by CR if it has
                string[] responseSplit = response.Split(CR);

                if (responseSplit.Length < 2)
                {
                    Status = "Response error";
                    return false;
                }

                string result = string.Empty;

                for (int i = 0; i < responseSplit.Length - 1; i++)
                {
                    response = responseSplit[i];
                    if (CheckFCS(response, out string msg))
                    {
                        if (msg.StartsWith("@"))
                        {
                            string errCode = msg.Substring(5, 2);
                            if (errCode == "00")
                            {
                                msg = msg.Remove(0, 7);
                            }
                            else
                            {
                                Status = GetErrorInfo(errCode);
                                return false;
                            }
                        }
                        result += msg;
                    }
                    else
                    {
                        Status = "FCS error";
                        return false;
                    }
                }

                if (values.Length * 2 == result.Length && result.Length % 2 == 0)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = (byte)((GetHexValue(result[i << 1]) << 4) + (GetHexValue(result[(i << 1) + 1])));
                    }
                    Status = "Ok";
                    return true;
                }
                else
                {
                    Status = "Respone value wrong";
                    return false;
                }
            }
            else
            {
                Status = "Serial port don't open yet.";
                return false;
            }
        }
        #endregion

        #region Write
        /// <summary>
        /// Hàm gửi lệnh ghi xuống PLC
        /// </summary>
        /// <param name="unitNo">UnitNo của PLC</param>
        /// <param name="cmd">Loại lệnh ghi</param>
        /// <param name="startWordAddress">Địa chỉ bắt đầu ghi</param>
        /// <param name="data">Dữ liệu cần ghi xuống PLC</param>
        /// <returns></returns>
        public bool Write(byte unitNo, WriteCommandCode cmd, ushort startWordAddress, byte[] data)
        {
            // Make sure serial port was opened
            if (SerialPort.IsOpen)
            {
                // Clear in/out buffers
                SerialPort.DiscardInBuffer();
                SerialPort.DiscardOutBuffer();

                // Build content message in frame
                string message = startWordAddress.ToString("0000") + ToHex(data);
                // Build the frame to sent to plc
                string frame = BuildFrame(unitNo, cmd.ToString(), message);
                // Init response message
                string response = string.Empty;
                try
                {
                    // Sent frame to the plc
                    SerialPort.Write(frame);
                    // Wait little bit
                    Thread.Sleep(2);
                    // Get the response message from PLC
                    response = GetResponse();

                    if (!response.EndsWith($"*{CR}"))
                    {
                        Status = "Response error";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Status = $"Error in read serial port: {ex.Message}";
                    return false;
                }

                // Split response by CR if it has
                string[] responseSplit = response.Split(CR);

                if (responseSplit.Length < 2)
                {
                    Status = "Response error";
                    return false;
                }

                string result = string.Empty;

                for (int i = 0; i < responseSplit.Length - 1; i++)
                {
                    response = responseSplit[i];
                    if (CheckFCS(response, out string msg))
                    {
                        if (msg.StartsWith("@"))
                        {
                            string errCode = msg.Substring(5, 2);
                            if (errCode == "00")
                            {
                                Status = "Ok";
                                return true;
                            }
                            else
                            {
                                Status = GetErrorInfo(errCode);
                                return false;
                            }
                        }
                        result += msg;
                    }
                    else
                    {
                        Status = "FCS error";
                        return false;
                    }
                }

                Status = "Response message wrong";
                return false;
            }
            else
            {
                Status = "Serial port don't open yet.";
                return false;
            }
        }
        #endregion

        #region Forced Set Bit
        public bool ForcedSetBit(byte unitNo, Area area, ushort wordAddress, ushort bitAddress)
        {
            // Make sure serial port was opened
            if (SerialPort.IsOpen)
            {
                // Clear in/out buffers
                SerialPort.DiscardInBuffer();
                SerialPort.DiscardOutBuffer();

                // Build content message in frame
                string message = area.ToString().PadRight(4, ' ') + wordAddress.ToString("0000") + bitAddress.ToString("00");
                // Build the frame to sent to plc
                string frame = BuildFrame(unitNo, "KS", message);
                // Init response message
                string response = string.Empty;
                try
                {
                    // Sent frame to the plc
                    SerialPort.Write(frame);
                    // Wait little bit
                    Thread.Sleep(2);
                    // Get the response message from PLC
                    response = GetResponse();

                    if (!response.EndsWith($"*{CR}"))
                    {
                        Status = "Response error";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Status = $"Error in read serial port: {ex.Message}";
                    return false;
                }

                // Split response by CR if it has
                string[] responseSplit = response.Split(CR);

                if (responseSplit.Length < 2)
                {
                    Status = "Response error";
                    return false;
                }

                string result = string.Empty;

                for (int i = 0; i < responseSplit.Length - 1; i++)
                {
                    response = responseSplit[i];
                    if (CheckFCS(response, out string msg))
                    {
                        if (msg.StartsWith("@"))
                        {
                            string errCode = msg.Substring(5, 2);
                            if (errCode == "00")
                            {
                                Status = "Ok";
                                return true;
                            }
                            else
                            {
                                Status = GetErrorInfo(errCode);
                                return false;
                            }
                        }
                        result += msg;
                    }
                    else
                    {
                        Status = "FCS error";
                        return false;
                    }
                }

                Status = "Response message wrong";
                return false;
            }
            else
            {
                Status = "Serial port don't open yet.";
                return false;
            }
        }
        #endregion

        #region Forced Reset Bit
        public bool ForcedResetBit(byte unitNo, Area area, ushort wordAddress, ushort bitAddress)
        {
            // Make sure serial port was opened
            if (SerialPort.IsOpen)
            {
                // Clear in/out buffers
                SerialPort.DiscardInBuffer();
                SerialPort.DiscardOutBuffer();

                // Build content message in frame
                string message = area.ToString().PadRight(4, ' ') + wordAddress.ToString("0000") + bitAddress.ToString("00");
                // Build the frame to sent to plc
                string frame = BuildFrame(unitNo, "KR", message);
                // Init response message
                string response = string.Empty;
                try
                {
                    // Sent frame to the plc
                    SerialPort.Write(frame);
                    // Wait little bit
                    Thread.Sleep(2);
                    // Get the response message from PLC
                    response = GetResponse();

                    if (!response.EndsWith($"*{CR}"))
                    {
                        Status = "Response error";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Status = $"Error in read serial port: {ex.Message}";
                    return false;
                }

                // Split response by CR if it has
                string[] responseSplit = response.Split(CR);

                if (responseSplit.Length < 2)
                {
                    Status = "Response error";
                    return false;
                }

                string result = string.Empty;

                for (int i = 0; i < responseSplit.Length - 1; i++)
                {
                    response = responseSplit[i];
                    if (CheckFCS(response, out string msg))
                    {
                        if (msg.StartsWith("@"))
                        {
                            string errCode = msg.Substring(5, 2);
                            if (errCode == "00")
                            {
                                Status = "Ok";
                                return true;
                            }
                            else
                            {
                                Status = GetErrorInfo(errCode);
                                return false;
                            }
                        }
                        result += msg;
                    }
                    else
                    {
                        Status = "FCS error";
                        return false;
                    }
                }

                Status = "Response message wrong";
                return false;
            }
            else
            {
                Status = "Serial port don't open yet.";
                return false;
            }
        }
        #endregion

        #region Forced Set/Reset Cancel Bit
        public bool ForcedCancel(byte unitNo)
        {
            // Make sure serial port was opened
            if (SerialPort.IsOpen)
            {
                // Clear in/out buffers
                SerialPort.DiscardInBuffer();
                SerialPort.DiscardOutBuffer();

                // Build the frame to sent to plc
                string frame = BuildFrame(unitNo, "KC", "");
                // Init response message
                string response = string.Empty;
                try
                {
                    // Sent frame to the plc
                    SerialPort.Write(frame);
                    // Wait little bit
                    Thread.Sleep(2);
                    // Get the response message from PLC
                    response = GetResponse();

                    if (!response.EndsWith($"*{CR}"))
                    {
                        Status = "Response error";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Status = $"Error in read serial port: {ex.Message}";
                    return false;
                }

                // Split response by CR if it has
                string[] responseSplit = response.Split(CR);

                if (responseSplit.Length < 2)
                {
                    Status = "Response error";
                    return false;
                }

                string result = string.Empty;

                for (int i = 0; i < responseSplit.Length - 1; i++)
                {
                    response = responseSplit[i];
                    if (CheckFCS(response, out string msg))
                    {
                        if (msg.StartsWith("@"))
                        {
                            string errCode = msg.Substring(5, 2);
                            if (errCode == "00")
                            {
                                Status = "Ok";
                                return true;
                            }
                            else
                            {
                                Status = GetErrorInfo(errCode);
                                return false;
                            }
                        }
                        result += msg;
                    }
                    else
                    {
                        Status = "FCS error";
                        return false;
                    }
                }

                Status = "Response message wrong";
                return false;
            }
            else
            {
                Status = "Serial port don't open yet.";
                return false;
            }
        }
        #endregion

        #region  Utils methods
        /// <summary>
        /// Hàm lấy thông tin của mã lỗi
        /// </summary>
        /// <param name="responseCode"></param>
        /// <returns></returns>
        public string GetErrorInfo(string responseCode)
        {
            string result = "unknown mistake";
            switch (responseCode)
            {
                case "00":
                    result = "Normally completed!";
                    break;
                case "01":
                    result = "Not executable in RUN mode!";
                    break;
                case "02":
                    result = "Unable to execute in MONITOR mode";
                    break;
                case "04":
                    result = "Address beyond the border";
                    break;
                case "0B":
                    result = "Not executable in PROGRAM mode";
                    break;
                case "13":
                    result = "FCS error";
                    break;
                case "14":
                    result = "wrong format";
                    break;
                case "15":
                    result = "Import number data error";
                    break;
                case "16":
                    result = "Command does not support";
                    break;
                case "18":
                    result = "Frame length error";
                    break;
                case "19":
                    result = "not executable";
                    break;
                case "23":
                    result = "User Store Write Protection";
                    break;
                case "A3":
                    result = "Terminated due to FCS error in data transfer";
                    break;
                case "A4":
                    result = "Terminated due to a format error in data transfer";
                    break;
                case "A5":
                    result = "Terminated due to incorrect entry number data in data transfer";
                    break;
                case "A8":
                    result = "Terminated due to frame length error in data transfer";
                    break;
                default:
                    result = "unknown mistake";
                    break;
            }
            return result;
        }

        /// <summary>
        /// Hàm chuyển giá trị byte thành mã HEX
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string ToHex(byte data)
        {
            return string.Format("{0:X2}", data);
        }

        /// <summary>
        /// Hàm chuyển giá trị byte thành mã HEX
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string ToHex(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 2);
            foreach (byte b in data)
                sb.Append(ToHex(b));
            return sb.ToString();
        }

        /// <summary>
        /// Convert hex char into interger value
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        private int GetHexValue(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }
        #endregion
    }
}
