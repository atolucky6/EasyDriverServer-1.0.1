using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using EasyDriverPlugin;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace EasyDriver.ModbusRTU
{
    /// <summary>
    /// Driver thực hiện việc đọc ghi giá trị
    /// </summary>
    public class ModbusRTUDriver : IEasyDriverPlugin
    {
        #region Static
        
        /// <summary>
        /// Danh sách các kiểu dữ liệu mà driver này hỗ trợ
        /// </summary>
        static readonly List<IDataType> supportDataTypes;

        /// <summary>
        /// Static constructor khởi tạo 1 lần khi load driver
        /// </summary>
        static ModbusRTUDriver()
        {
            // Khởi tạo các kiểu dữ liệu
            supportDataTypes = new List<IDataType>
            {
                new Bool(),
                new Word(),
                new DWord(),
                new Int() { Name = "Short" },
                new DInt() { Name = "Long" },
                new Real() { Name = "Float" },
                new LReal() { Name = "Double" },
            };
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public ModbusRTUDriver()
        {
            mbMaster = new ModbusSerialRTU();
            DeviceToReadBlockSettings = new Dictionary<IDeviceCore, List<ReadBlockSetting>>();
            DeviceToReadBlockString = new Dictionary<IDeviceCore, List<string>>();
            semaphore = new SemaphoreSlim(1, 1);
            stopwatch = new Stopwatch();
        }

        #endregion

        #region Members

        /// <summary>
        /// Dictionary ReadBlockSetting của Device
        /// </summary>
        public Dictionary<IDeviceCore, List<ReadBlockSetting>> DeviceToReadBlockSettings { get; set; }

        /// <summary>
        /// Dictionary chuổi cấu hình ReadBlockSetting của Device
        /// </summary>
        public Dictionary<IDeviceCore, List<string>> DeviceToReadBlockString { get; set; }

        /// <summary>
        /// Channel chạy driver này
        /// </summary>
        public IChannelCore Channel { get; set; }

        /// <summary>
        /// Bit xác định driver này đã bị dispose
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Chu kỳ quét của driver
        /// </summary>
        public int ScanRate
        {
            get
            {
                if (Channel != null)
                {
                    if (Channel.ParameterContainer != null && Channel.ParameterContainer.Parameters != null)
                    {
                        if (Channel.ParameterContainer.Parameters.ContainsKey("ScanRate"))
                        {
                            if (int.TryParse(Channel.ParameterContainer.Parameters["ScanRate"].ToString(), out int scanRate))
                                return scanRate;
                        }
                    }
                }
                return 1000;
            }
        }

        /// <summary>
        /// Thời gian trễ giữa 2 lần request tới thiết bị thực
        /// </summary>
        public int DelayBetweenPool
        {
            get
            {
                if (Channel != null)
                {
                    if (Channel.ParameterContainer != null && Channel.ParameterContainer.Parameters != null)
                    {
                        if (Channel.ParameterContainer.Parameters.ContainsKey("DelayBetweenPool"))
                        {
                            if (int.TryParse(Channel.ParameterContainer.Parameters["DelayBetweenPool"].ToString(), out int delay))
                                return delay;
                        }
                    }
                }
                return 10;
            }
        }

        readonly ModbusSerialRTU mbMaster;
        private Task refreshTask;
        readonly SemaphoreSlim semaphore;
        readonly Stopwatch stopwatch;

        public event EventHandler Disposed;
        public event EventHandler Refreshed;

        #endregion

        #region Methods

        /// <summary>
        /// Hàm bắt đầu kết nối
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            // Đảm bảo channel có các thông số cần thiết để thực hiện việc kết nối
            if (Channel.ParameterContainer.Parameters.Count < 5)
                return false;

            // Đợi semaphore rảnh thì bắt đầu kết nối
            semaphore.Wait();
            try
            {
                // Khởi tạo cổng serial
                InitializeSerialPort();
                // Mở cổng serial
                return mbMaster.Open();
            }
            catch { return false; }
            finally
            {
                // Giải phóng semaphore
                semaphore.Release();
                // Nếu task đang null thì khởi tạo task refresh để thực hiện việc đọc dữ liệu
                if (refreshTask == null)
                    refreshTask = Task.Factory.StartNew(Refresh, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
        }

        /// <summary>
        /// Hàm khởi tạo cổng serial
        /// </summary>
        private void InitializeSerialPort()
        {
            if (Channel.ParameterContainer.Parameters.Count >= 5)
            {
                // Lấy các thông số kết nối từ Channel parameter container
                var port = Channel.ParameterContainer.Parameters["Port"].ToString();
                var baudRate = (int)Channel.ParameterContainer.Parameters["Baudrate"];
                var dataBits = (int)Channel.ParameterContainer.Parameters["DataBits"];
                var stopBits = (StopBits)Channel.ParameterContainer.Parameters["StopBits"];
                var parity = (Parity)Channel.ParameterContainer.Parameters["Parity"];
                // Khởi tạo cổng serial
                mbMaster.Init(port, baudRate, dataBits, parity, stopBits);
            }
        }

        /// <summary>
        /// Hàm thực hiện việc đọc các thiết bị liên tục
        /// </summary>
        private void Refresh()
        {
            // Đảm bảo driver này chưa bị dispose
            while (!IsDisposed)
            {
                try
                {
                    // Khởi động lại đồng hồ
                    stopwatch.Restart();
                    // Khởi tạo lại cổng serial nếu như thông số serial thay đổi
                    InitializeSerialPort();
                    // Đảm bảo cổng serial đã mở
                    if (mbMaster.Open())
                    {
                        // Lặp qua tất cả các Device có trong Channel
                        foreach (var channelChild in Channel.Childs.ToArray())
                        {
                            // Đảm bảo đổi tượng con của Channel là Device
                            if (!(channelChild is IDeviceCore device))
                                continue; // Nếu không phải thì bỏ qua vòng lặp này

                            byte deviceId = byte.Parse(device.ParameterContainer.Parameters["DeviceId"].ToString());
                            ByteOrder byteOrder = device.ByteOrder;
                            // Lấy số lần thử đọc tối đa nếu đọc không thành công
                            int maxTryTimes = int.Parse(device.ParameterContainer.Parameters["TryReadWriteTimes"].ToString());
                            // Thời gian timeout của việc đọc ghi
                            int timeout = int.Parse(device.ParameterContainer.Parameters["Timeout"].ToString());

                            // Cài thời gian time out cho serial port
                            mbMaster.ResponseTimeOut = timeout;
                            mbMaster.SerialPort.ReadTimeout = timeout;
                            mbMaster.SerialPort.WriteTimeout = timeout;

                            #region READ BLOCK FIRST

                            // Kiểm tra nếu như device chưa có ReadBlockSetting thì tạo cho nó
                            if (!DeviceToReadBlockSettings.ContainsKey(device))
                                DeviceToReadBlockSettings[device] = new List<ReadBlockSetting>();
                            if (!DeviceToReadBlockString.ContainsKey(device))
                                DeviceToReadBlockString[device] = new List<string> { "", "", "", "" };

                            InitReadBlockSetting(device, "ReadInputContactsBlockSetting", AddressType.InputContact);
                            InitReadBlockSetting(device, "ReadOutputCoilsBlockSetting", AddressType.OutputCoil);
                            InitReadBlockSetting(device, "ReadInputRegistersBlockSetting", AddressType.InputRegister);
                            InitReadBlockSetting(device, "ReadHoldingRegistersBlockSetting", AddressType.HoldingRegister);

                            foreach (ReadBlockSetting block in DeviceToReadBlockSettings[device])
                            {
                                block.ReadResult = false;
                                int readCount = 0;
                                while (!block.ReadResult)
                                {
                                    switch (block.AddressType)
                                    {
                                        case AddressType.InputContact:
                                        case AddressType.OutputCoil:
                                            block.ReadResult = ReadBits(deviceId, block.AddressType, block.StartOffset, block.Count, ref block.BoolBuffer);
                                            break;
                                        case AddressType.InputRegister:
                                        case AddressType.HoldingRegister:
                                            block.ReadResult = ReadRegisters(deviceId, block.AddressType, block.StartOffset, block.Count, ref block.ByteBuffer);
                                            break;
                                        default:
                                            break;
                                    }
                                    readCount++;
                                    if (readCount >= maxTryTimes)
                                        break;
                                }
                            }

                            #endregion

                            foreach (var childDevice in device.Childs.ToArray())
                            {
                                if (!(childDevice is ITagCore tag))
                                    continue;

                                if (!tag.ParameterContainer.Parameters.ContainsKey("LastAddress"))
                                    tag.ParameterContainer.Parameters["LastAddress"] = tag.Address;

                                if (!tag.ParameterContainer.Parameters.ContainsKey("AddressType") ||
                                    tag.Address != tag.ParameterContainer.Parameters["LastAddress"].ToString())
                                {
                                    if (tag.Address.DecomposeAddress(out AddressType addressType, out ushort offset))
                                    {
                                        tag.ParameterContainer.Parameters["AddressType"] = (int)addressType;
                                        tag.ParameterContainer.Parameters["Offset"] = offset;
                                        tag.ParameterContainer.Parameters["IsValid"] = true;
                                        tag.ParameterContainer.Parameters["LastAddress"] = tag.Address;
                                    }
                                    else
                                    {
                                        tag.ParameterContainer.Parameters["IsValid"] = false;
                                    }
                                }

                                if ((bool)tag.ParameterContainer.Parameters["IsValid"])
                                {
                                    if ((DateTime.Now - tag.TimeStamp).TotalMilliseconds >= tag.RefreshRate)
                                    {
                                        bool readSuccess = false;
                                        bool containInBlock = false;
                                        foreach (var block in DeviceToReadBlockSettings[device])
                                        {
                                            if (block.AddressType == (AddressType)tag.ParameterContainer.Parameters["AddressType"])
                                            {
                                                if (block.CheckTagIsInReadBlockRange(
                                                    tag,
                                                    block.AddressType,
                                                    (ushort)tag.ParameterContainer.Parameters["Offset"],
                                                    tag.DataType.RequireByteLength,
                                                    out int index))
                                                {
                                                    containInBlock = true;
                                                    readSuccess = block.ReadResult;
                                                    if (readSuccess)
                                                    {
                                                        switch (block.AddressType)
                                                        {
                                                            case AddressType.InputContact:
                                                            case AddressType.OutputCoil:
                                                                tag.Value = block.BoolBuffer[index] ? 
                                                                    (tag.Gain + tag.Offset).ToString() : tag.Offset.ToString();
                                                                break;
                                                            case AddressType.InputRegister:
                                                            case AddressType.HoldingRegister:
                                                                tag.Value = tag.DataType.ConvertToValue(
                                                                    block.ByteBuffer, tag.Gain, tag.Offset, index, 0, byteOrder);
                                                                break;
                                                            default:
                                                                break;
                                                        }
                                                    }
                                                    break;
                                                }
                                            }
                                        }

                                        if (!containInBlock)
                                        {
                                            switch ((AddressType)tag.ParameterContainer.Parameters["AddressType"])
                                            {
                                                case AddressType.InputContact:
                                                    {
                                                        bool[] inputs = new bool[1];
                                                        readSuccess = ReadBits(
                                                            deviceId,
                                                            AddressType.InputContact,
                                                            (ushort)tag.ParameterContainer.Parameters["Offset"],
                                                            1, ref inputs);
                                                        if (readSuccess)
                                                            tag.Value = inputs[0] ?
                                                                (tag.Gain + tag.Offset).ToString() : tag.Offset.ToString();
                                                        break;
                                                    }
                                                case AddressType.OutputCoil:
                                                    {
                                                        bool[] outputs = new bool[1];
                                                        readSuccess = ReadBits(
                                                            deviceId,
                                                            AddressType.OutputCoil,
                                                            (ushort)tag.ParameterContainer.Parameters["Offset"],
                                                            1, ref outputs);
                                                        if (readSuccess)
                                                            tag.Value = outputs[0] ?
                                                                (tag.Gain + tag.Offset).ToString() : tag.Offset.ToString();
                                                        break;
                                                    }
                                                case AddressType.InputRegister:
                                                    {
                                                        byte[] inputRegs = new byte[tag.DataType.RequireByteLength];
                                                        readSuccess = ReadRegisters(
                                                            deviceId,
                                                            AddressType.InputRegister,
                                                            (ushort)tag.ParameterContainer.Parameters["Offset"],
                                                            (ushort)(tag.DataType.RequireByteLength / 2),
                                                            ref inputRegs);
                                                        if (readSuccess)
                                                            tag.Value = tag.DataType.ConvertToValue(inputRegs, tag.Gain, tag.Offset, 0, 0, device.ByteOrder);
                                                        break;
                                                    }
                                                case AddressType.HoldingRegister:
                                                    {
                                                        byte[] holdingRegs = new byte[tag.DataType.RequireByteLength];
                                                        readSuccess = ReadRegisters(
                                                            deviceId,
                                                            AddressType.HoldingRegister, (
                                                            ushort)tag.ParameterContainer.Parameters["Offset"],
                                                            (ushort)(tag.DataType.RequireByteLength / 2),
                                                            ref holdingRegs);
                                                        if (readSuccess)
                                                            tag.Value = tag.DataType.ConvertToValue(holdingRegs, tag.Gain, tag.Offset, 0, 0, device.ByteOrder);
                                                        break;
                                                    }
                                                default:
                                                    break;
                                            }
                                        }

                                        tag.Quality = readSuccess ? Quality.Good : Quality.Bad;
                                        tag.TimeStamp = DateTime.Now;
                                    }
                                }
                                else
                                {
                                    tag.TimeStamp = DateTime.Now;
                                    tag.Quality = Quality.Bad;
                                    tag.RefreshInterval = (int)(DateTime.Now - tag.TimeStamp).TotalMilliseconds;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < Channel.Childs.Count; i++)
                        {
                            IDeviceCore device = Channel.Childs[i] as IDeviceCore;
                            for (int k = 0; k < device.Childs.Count; k++)
                            {
                                ITagCore tag = device.Childs[k] as ITagCore;
                                tag.TimeStamp = DateTime.Now;
                                tag.Quality = Quality.Bad;
                                tag.RefreshInterval = (int)(DateTime.Now - tag.TimeStamp).TotalMilliseconds;
                            }
                        }
                    }

                    Refreshed?.Invoke(this, EventArgs.Empty);
                }
                catch { }
                finally
                {
                    stopwatch.Stop();
                    Debug.WriteLine($"Scan interval = {stopwatch.ElapsedMilliseconds}");
                    int delay = (int)(ScanRate - stopwatch.ElapsedMilliseconds);
                    if (delay < 1)
                        delay = 1;
                    Thread.Sleep(delay);
                }
            }
            // Nếu đã bị dispose thì khởi tạo event dispose
            Disposed?.Invoke(this, EventArgs.Empty);
            // Ngắt kết nối serial
            Disconnect();
            // Dispose đối tượng phụ trách đọc ghi modbus
            mbMaster.Dispose();
        }

        /// <summary>
        /// Hàm khởi tạo các ReadBlockSetting cho Device
        /// </summary>
        /// <param name="device"></param>
        /// <param name="readBlocksettingKey"></param>
        /// <param name="addressType"></param>
        private void InitReadBlockSetting(IDeviceCore device, string readBlocksettingKey, AddressType addressType)
        {
            if (device.ParameterContainer.Parameters.ContainsKey(readBlocksettingKey))
            {
                string readBlockStr = device.ParameterContainer.Parameters[readBlocksettingKey].ToString();
                int index = 0;
                switch (addressType)
                {
                    case AddressType.OutputCoil:
                        index = 0;
                        break;
                    case AddressType.InputContact:
                        index = 1;
                        break;
                    case AddressType.InputRegister:
                        index = 2;
                        break;
                    case AddressType.HoldingRegister:
                        index = 3;
                        break;
                    default:
                        break;
                }

                if (DeviceToReadBlockString[device][index] != readBlockStr)
                {
                    foreach (var readBlock in DeviceToReadBlockSettings[device].Where(x => x.AddressType == addressType))
                        DeviceToReadBlockSettings[device].Remove(readBlock);

                    DeviceToReadBlockString[device][index] = readBlockStr;
                    string[] readBlockSplit = readBlockStr.Split('|');
                    if (readBlockSplit.Length > 0)
                    {
                        foreach (var item in readBlockSplit)
                        {
                            if (item.StartsWith("True"))
                            {
                                ReadBlockSetting newBlock = ReadBlockSetting.Convert(item);
                                newBlock.AddressType = addressType;
                                if (newBlock.IsValid)
                                    DeviceToReadBlockSettings[device].Add(newBlock);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Hàm đọc input và output
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="addressType"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="boolBuffer"></param>
        /// <returns></returns>
        private bool ReadBits(byte deviceId, AddressType addressType, ushort offset, ushort count, ref bool[] boolBuffer)
        {
            try
            {
                semaphore.Wait();
                switch (addressType)
                {
                    case AddressType.InputContact:
                        return mbMaster.ReadDiscreteInputContact(deviceId, offset, count, ref boolBuffer);
                    case AddressType.OutputCoil:
                        return mbMaster.ReadCoils(deviceId, offset, count, ref boolBuffer);
                    default:
                        break;
                }
                return false;
            }
            catch { return false; }
            finally { Thread.Sleep(DelayBetweenPool); semaphore.Release(); }
        }

        /// <summary>
        /// Hàm đọc input register và holding register
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="addressType"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="byteBuffer"></param>
        /// <returns></returns>
        private bool ReadRegisters(byte deviceId, AddressType addressType, ushort offset, ushort count, ref byte[] byteBuffer)
        {
            try
            {
                semaphore.Wait();
                switch (addressType)
                {
                    case AddressType.InputRegister:
                        return mbMaster.ReadInputRegisters(deviceId, offset, count, ref byteBuffer);
                    case AddressType.HoldingRegister:
                        return mbMaster.ReadHoldingRegisters(deviceId, offset, count, ref byteBuffer);
                    default:
                        break;
                }
                return false;
            }
            catch { return false; }
            finally { Thread.Sleep(DelayBetweenPool); semaphore.Release(); }
        }

        /// <summary>
        /// Hàm ngắt kết nối
        /// </summary>
        /// <returns></returns>
        public bool Disconnect()
        {
            try
            {
                semaphore.Wait();
                return mbMaster.Close();
            }
            catch { return false; }
            finally { semaphore.Release(); }
        }

        /// <summary>
        /// Hàm lấy các kiệu dự liệu mà driver này hỗ trợ
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IDataType> GetSupportDataTypes()
        {
            return supportDataTypes;
        }

        /// <summary>
        /// Hàm ghi giá trị
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Quality Write(ITagCore tag, string value)
        {
            // Nếu driver đã bị dispose thì không thực hiện việc ghi này
            if (!IsDisposed)
            {
                try
                {
                    // Kiểm tra các điều kiện cần để thực hiện việc ghi
                    if (tag != null && tag.DataType != null && 
                        tag.Parent is IDeviceCore device && tag.AccessPermission == AccessPermission.ReadAndWrite)
                    {
                        // Lấy thông tin của địa chỉ Tag như kiểu địa chỉ và vị trí bắt đầu của thanh ghi
                        if (!tag.ParameterContainer.Parameters.ContainsKey("LastAddress"))
                            tag.ParameterContainer.Parameters["LastAddress"] = tag.Address;

                        if (!tag.ParameterContainer.Parameters.ContainsKey("AddressType") ||
                            tag.Address != tag.ParameterContainer.Parameters["LastAddress"].ToString())
                        {
                            if (tag.Address.DecomposeAddress(out AddressType addressType, out ushort offset))
                            {
                                tag.ParameterContainer.Parameters["AddressType"] = addressType;
                                tag.ParameterContainer.Parameters["Offset"] = offset;
                                tag.ParameterContainer.Parameters["IsValid"] = true;
                                tag.ParameterContainer.Parameters["LastAddress"] = tag.Address;
                            }
                            else
                            {
                                tag.ParameterContainer.Parameters["IsValid"] = false;
                            }
                        }
                        if ((bool)tag.ParameterContainer.Parameters["IsValid"])
                        {
                            // Lấy thông tin byte order của device parent
                            ByteOrder byteOrder = device.ByteOrder;
                            // Chuyển đổi giá trị cần ghi thành chuỗi byte nếu thành công thì mới ghi
                            if (tag.DataType.TryParseToByteArray(value, tag.Gain, tag.Offset, out byte[] writeBuffer, byteOrder))
                            {
                                // Lấy thông tin của device như DeviceId, MaxReadWriteTimes, Timeout...
                                byte deviceId = byte.Parse(device.ParameterContainer.Parameters["DeviceId"].ToString());
                                int maxTryTimes = int.Parse(device.ParameterContainer.Parameters["TryReadWriteTimes"].ToString());
                                int timeout = int.Parse(device.ParameterContainer.Parameters["Timeout"].ToString());

                                // Cài timeout cho serial port
                                mbMaster.SerialPort.ReadTimeout = timeout;
                                mbMaster.SerialPort.WriteTimeout = timeout;

                                // Kiểm tra xem serial port đã mở mới cho phép ghis
                                if (mbMaster.SerialPort.IsOpen)
                                {
                                    // Khởi tạo biến thể hiện kết quả của việc ghi
                                    bool writeResult = false;
                                    semaphore.Wait();
                                    // Lặp việc ghi cho đến khi thành công hoặc đã vượt quá số lần cho phép ghi
                                    try
                                    {
                                        for (int i = 0; i < maxTryTimes; i++)
                                        {
                                            try
                                            {
                                                if ((AddressType)tag.ParameterContainer.Parameters["AddressType"] == AddressType.OutputCoil)
                                                {
                                                    bool bitResult = ByteHelper.GetBitAt(writeBuffer, 0, 0);
                                                    writeResult = mbMaster.WriteSingleCoil(
                                                        deviceId, 
                                                        (ushort)tag.ParameterContainer.Parameters["Offset"], 
                                                        bitResult);
                                                }
                                                else if ((AddressType)tag.ParameterContainer.Parameters["AddressType"] == AddressType.HoldingRegister)
                                                {
                                                    writeResult = mbMaster.WriteHoldingRegisters(
                                                        deviceId, 
                                                        (ushort)tag.ParameterContainer.Parameters["Offset"], 
                                                        (ushort)(writeBuffer.Length / 2), 
                                                        writeBuffer);
                                                }
                                                else { break; }
                                                if (writeResult) // Nếu ghi thành công thì thoát khỏi vòng lặp
                                                    break;
                                            }
                                            finally { Thread.Sleep(DelayBetweenPool); }
                                        }
                                    }
                                    finally { semaphore.Release(); }
                                    // Trả về kết quả 'Good' nếu ghi thành công, 'Bad' nếu không thành công
                                    return writeResult ? Quality.Good : Quality.Bad;
                                }
                            }

                        }
                    }
                }
                catch { return Quality.Bad; } // Khi có lỗi thì trả về 'Bad'
            }
            return Quality.Bad;
        }

        public void Dispose()
        {
            IsDisposed = true;
        }

        public object GetCreateChannelControl(IGroupItem parent, IChannelCore templateItem = null)
        {
            return new CreateChannelView(this, parent, templateItem);
        }

        public object GetCreateDeviceControl(IGroupItem parent, IDeviceCore templateItem = null)
        {
            return new CreateDeviceView(this, parent as IChannelCore, templateItem);
        }

        public object GetCreateTagControl(IGroupItem parent, ITagCore templateItem = null)
        {
            return new CreateTagView(this, parent as IDeviceCore, templateItem);
        }

        public object GetEditChannelControl(IChannelCore channel)
        {
            return new EditChannelView(this, channel);
        }

        public object GetEditDeviceControl(IDeviceCore device)
        {
            return new EditDeviceView(this, device);
        }

        public object GetEditTagControl(ITagCore tag)
        {
            return new EditTagView(this, tag);
        }

        #endregion
    }
}
