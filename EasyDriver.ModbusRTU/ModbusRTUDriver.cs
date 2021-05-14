using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using EasyDriverPlugin;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using DevExpress.Mvvm.POCO;
using System.Collections.Concurrent;

namespace EasyDriver.ModbusRTU
{
    /// <summary>
    /// Driver thực hiện việc đọc ghi giá trị
    /// </summary>
    public class ModbusRTUDriver : EasyDriverPluginBase
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
                new LWord(),
                new Int() { Name = "Short" },
                new DInt() { Name = "Long" },
                new LInt() {Name = "LInt"},
                new Real() { Name = "Float" },
                new LReal() { Name = "Double" },
                new BCD(),
                new LBCD(),
                new ModbusRTU.String() { Name = "String" },
            };
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public ModbusRTUDriver()
        {
            DefaultPiorityWriteCommands = new ConcurrentDictionary<WriteCommand, WriteCommand>();
            mbMaster = new ModbusSerialRTU();
            locker = new SemaphoreSlim(1, 1);
            stopwatch = new Stopwatch();
            WriteQueue.Enqueued += OnWriteQueueAdded;

            BaudRateSource = new List<int>()
            {
                300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 38400, 56000, 57600, 115200, 128000, 256000
            };

            DataBitsSource = new List<int>()
            {
                5, 6, 7, 8
            };

            ComPortSource = new List<string>();
            for (int i = 1; i <= 100; i++)
            {
                ComPortSource.Add($"COM{i}");
            }

            ParitySource = Enum.GetValues(typeof(Parity)).Cast<Parity>().ToList();
            StopBitsSource = Enum.GetValues(typeof(StopBits)).Cast<StopBits>().ToList();
            AddressTypeSource = Enum.GetValues(typeof(AddressType)).Cast<AddressType>().ToList();
            ByteOrderSource = Enum.GetValues(typeof(ByteOrder)).Cast<ByteOrder>().ToList();
            ReadModeSource = Enum.GetValues(typeof(ReadMode)).Cast<ReadMode>().ToList();
            AccessPermissionSource = Enum.GetValues(typeof(AccessPermission)).Cast<AccessPermission>().ToList();
        }

        #endregion

        #region Members

        readonly ModbusSerialRTU mbMaster;
        private Task refreshTask;
        readonly object locker = new object();
        readonly Stopwatch stopwatch;

        public List<string> ComPortSource { get; set; }
        public List<int> BaudRateSource { get; set; }
        public List<int> DataBitsSource { get; set; }
        public List<Parity> ParitySource { get; set; }
        public List<StopBits> StopBitsSource { get; set; }
        public List<ByteOrder> ByteOrderSource { get; set; }
        public List<AddressType> AddressTypeSource { get; set; }
        public List<ReadMode> ReadModeSource { get; set; }
        public List<AccessPermission> AccessPermissionSource { get; set; }
        public ConcurrentDictionary<WriteCommand, WriteCommand> DefaultPiorityWriteCommands { get; set; } 
        public Channel Channel { get; set; }
        public override List<IDataType> SupportDataTypes 
        { 
            get => supportDataTypes;
            protected set => base.SupportDataTypes = value; 
        }

        /// <summary>
        /// Bit xác định driver này đã bị dispose
        /// </summary>
        public bool IsDisposed { get; private set; }
        #endregion

        #region Events
        public override event EventHandler Refreshed;
        public override event EventHandler Disposed;
        #endregion

        #region Methods
        /// <summary>
        /// Hàm bắt đầu kết nối
        /// </summary>
        /// <returns></returns>
        public override bool Start(IChannelCore channel)
        {
            Channel = (Channel)channel;

            // Đảm bảo channel có các thông số cần thiết để thực hiện việc kết nối
            if (Channel.ParameterContainer.Parameters.Count < 5)
                return false;

            // Đợi semaphore rảnh thì bắt đầu kết nối
            lock (locker)
            {
                try
                {
                    // Khởi tạo cổng serial
                    if (Channel.ParameterContainer.Parameters.Count >= 5)
                        mbMaster.Init(Channel.Port, Channel.Baudrate, Channel.DataBits, Channel.Parity, Channel.StopBits);

                    // Mở cổng serial
                    return mbMaster.Open();
                }
                catch { return false; }
                finally
                {
                    // Giải phóng semaphore
                    // Nếu task đang null thì khởi tạo task refresh để thực hiện việc đọc dữ liệu
                    if (refreshTask == null)
                        refreshTask = Task.Factory.StartNew(Refresh, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                }
            }
        }

        /// <summary>
        /// Hàm ngắt kết nối
        /// </summary>
        /// <returns></returns>
        public override bool Stop()
        {
            lock (locker)
            {
                try
                {
                    return mbMaster.Close();
                }
                catch { return false; }
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
                    bool setTagBad = false;
                    if (Channel.Enabled)
                    {
                        // Khởi động lại đồng hồ
                        stopwatch.Restart();
                        // Khởi tạo lại cổng serial nếu như thông số serial thay đổi
                        if (Channel.ParameterContainer.Parameters.Count >= 5)
                            mbMaster.Init(Channel.Port, Channel.Baudrate, Channel.DataBits, Channel.Parity, Channel.StopBits);

                        // Đảm bảo cổng serial đã mở
                        if (mbMaster.Open())
                        {
                            // Lặp qua tất cả các Device có trong Channel
                            foreach (var channelChild in Channel.GetAllDevices().ToArray())
                            {
                                // Đảm bảo đổi tượng con của Channel là Device
                                if (!(channelChild is Device device))
                                    continue; // Nếu không phải thì bỏ qua vòng lặp này
                                if (!device.Enabled)
                                    continue;

                                byte deviceId = (byte)device.DeviceId;

                                // Cài thời gian time out cho serial port
                                mbMaster.ResponseTimeOut = device.Timeout;
                                mbMaster.SerialPort.ReadTimeout = device.Timeout;
                                mbMaster.SerialPort.WriteTimeout = device.Timeout;

                                #region READ BLOCK FIRST

                                foreach (ReadBlockSetting block in device.GetAllReadBlockSettings())
                                {
                                    block.ReadResult = false;
                                    if (block.IsValid && block.Enabled && block.AddressType != AddressType.Undefined)
                                    {
                                        if (block.RegisterTags.Count > 0)
                                        {
                                            int readCount = 0;
                                            while (!block.ReadResult)
                                            {
                                                switch (block.AddressType)
                                                {
                                                    case AddressType.InputContact:
                                                    case AddressType.OutputCoil:
                                                        block.ReadResult = ReadBits(deviceId, block.AddressType, block.StartOffset, block.BufferCount, ref block.BoolBuffer);
                                                        if (!block.ReadResult)
                                                        {
                                                            if (!mbMaster.SerialPort.IsOpen)
                                                            {
                                                                setTagBad = true;
                                                                goto SET_TAG_BAD;
                                                            }
                                                        }
                                                        break;
                                                    case AddressType.InputRegister:
                                                    case AddressType.HoldingRegister:
                                                        block.ReadResult = ReadRegisters(deviceId, block.AddressType, block.StartOffset, block.BufferCount, ref block.ByteBuffer);
                                                        if (!block.ReadResult)
                                                        {
                                                            if (!mbMaster.SerialPort.IsOpen)
                                                            {
                                                                setTagBad = true;
                                                                goto SET_TAG_BAD;
                                                            }
                                                        }
                                                        break;
                                                    default:
                                                        break;
                                                }
                                                readCount++;
                                                if (readCount >= device.TryReadWriteTimes)
                                                    break;
                                            }

                                            if (block.ReadResult)
                                            {
                                                switch (block.AddressType)
                                                {
                                                    case AddressType.OutputCoil:
                                                    case AddressType.InputContact:
                                                        {
                                                            foreach (var tag in block.RegisterTags.ToArray())
                                                            {
                                                                if (tag.Enabled)
                                                                {
                                                                    tag.Quality = Quality.Good;
                                                                    tag.TimeStamp = DateTime.Now;
                                                                    tag.RefreshInterval = (int)(DateTime.Now - tag.TimeStamp).TotalMilliseconds;
                                                                    tag.Value = block.BoolBuffer[tag.IndexOfDataInBlockSetting] ? (tag.Gain + tag.Offset).ToString() : tag.Offset.ToString();
                                                                }
                                                            }
                                                            break;
                                                        }
                                                    case AddressType.InputRegister:
                                                    case AddressType.HoldingRegister:
                                                        {
                                                            foreach (var tag in block.RegisterTags.ToArray())
                                                            {
                                                                if (tag.Enabled)
                                                                {
                                                                    tag.Quality = Quality.Good;
                                                                    tag.TimeStamp = DateTime.Now;
                                                                    tag.RefreshInterval = (int)(DateTime.Now - tag.TimeStamp).TotalMilliseconds;
                                                                    tag.Value = tag.DataType.ConvertToValue(block.ByteBuffer, tag.Gain, tag.Offset, tag.IndexOfDataInBlockSetting, 0, device.ByteOrder);
                                                                }
                                                            }
                                                            break;
                                                        }
                                                    default:
                                                        break;
                                                }
                                            }
                                            else
                                            {
                                                foreach (var tag in block.RegisterTags.ToArray())
                                                {
                                                    if (tag.Enabled)
                                                    {
                                                        tag.Quality = Quality.Bad;
                                                        tag.TimeStamp = DateTime.Now;
                                                        tag.RefreshInterval = (int)(DateTime.Now - tag.TimeStamp).TotalMilliseconds;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                #endregion

                                foreach (var childDevice in device.UndefinedTags.ToArray())
                                {
                                    // Đảm bảo tag phải là modbus rtu tag
                                    if (!(childDevice is Tag tag))
                                        continue;

                                    // Bỏ qua nếu tag là một internal tag hoặc không enabled
                                    if (tag.IsInternalTag)
                                    {
                                        tag.Quality = Quality.Good;
                                        tag.TimeStamp = DateTime.Now;
                                        tag.RefreshInterval = (int)(DateTime.Now - tag.TimeStamp).TotalMilliseconds;
                                        continue;
                                    }

                                    if (!tag.Enabled)
                                        continue;

                                    // Nếu tag có kiểu địa chỉ xác định thì đọc nếu không thì set Quality thành Bad
                                    if (tag.AddressType != AddressType.Undefined)
                                    {
                                        if ((DateTime.Now - tag.TimeStamp).TotalMilliseconds >= tag.RefreshRate)
                                        {
                                            bool readSuccess = false;
                                            switch (tag.AddressType)
                                            {
                                                case AddressType.InputContact:
                                                    {
                                                        bool[] inputs = new bool[1];
                                                        readSuccess = ReadBits(
                                                            deviceId,
                                                            AddressType.InputContact,
                                                            (ushort)tag.AddressOffset,
                                                            1, ref inputs);
                                                        if (readSuccess)
                                                            tag.Value = inputs[0] ?
                                                                (tag.Gain + tag.Offset).ToString() : tag.Offset.ToString();
                                                        if (!readSuccess)
                                                        {
                                                            if (!mbMaster.SerialPort.IsOpen)
                                                            {
                                                                setTagBad = true;
                                                                goto SET_TAG_BAD;
                                                            }
                                                        }
                                                        break;
                                                    }
                                                case AddressType.OutputCoil:
                                                    {
                                                        bool[] outputs = new bool[1];
                                                        readSuccess = ReadBits(
                                                            deviceId,
                                                            AddressType.OutputCoil,
                                                            (ushort)tag.AddressOffset,
                                                            1, ref outputs);
                                                        if (readSuccess)
                                                            tag.Value = outputs[0] ?
                                                                (tag.Gain + tag.Offset).ToString() : tag.Offset.ToString();
                                                        if (!readSuccess)
                                                        {
                                                            if (!mbMaster.SerialPort.IsOpen)
                                                            {
                                                                setTagBad = true;
                                                                goto SET_TAG_BAD;
                                                            }
                                                        }
                                                        break;
                                                    }
                                                case AddressType.InputRegister:
                                                    {
                                                        byte[] inputRegs = new byte[tag.RequireByteLength];
                                                        readSuccess = ReadRegisters(
                                                            deviceId,
                                                            AddressType.InputRegister,
                                                            (ushort)tag.AddressOffset,
                                                            (ushort)(tag.RequireByteLength / 2),
                                                            ref inputRegs);
                                                        if (readSuccess)
                                                            tag.Value = tag.DataType.ConvertToValue(inputRegs, tag.Gain, tag.Offset, 0, 0, device.ByteOrder);
                                                        if (!readSuccess)
                                                        {
                                                            if (!mbMaster.SerialPort.IsOpen)
                                                            {
                                                                setTagBad = true;
                                                                goto SET_TAG_BAD;
                                                            }
                                                        }
                                                        break;
                                                    }
                                                case AddressType.HoldingRegister:
                                                    {
                                                        byte[] holdingRegs = new byte[tag.RequireByteLength];
                                                        readSuccess = ReadRegisters(
                                                            deviceId,
                                                            AddressType.HoldingRegister,
                                                            (ushort)tag.AddressOffset,
                                                            (ushort)(tag.RequireByteLength / 2),
                                                            ref holdingRegs);
                                                        if (readSuccess)
                                                            tag.Value = tag.DataType.ConvertToValue(holdingRegs, tag.Gain, tag.Offset, 0, 0, device.ByteOrder);
                                                        if (!readSuccess)
                                                        {
                                                            if (!mbMaster.SerialPort.IsOpen)
                                                            {
                                                                setTagBad = true;
                                                                goto SET_TAG_BAD;
                                                            }
                                                        }
                                                        break;
                                                    }
                                                default:
                                                    break;
                                            }

                                            tag.Quality = readSuccess ? Quality.Good : Quality.Bad;
                                            tag.TimeStamp = DateTime.Now;
                                            tag.RefreshInterval = (int)(DateTime.Now - tag.TimeStamp).TotalMilliseconds;
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
                            setTagBad = true;
                        }

                        SET_TAG_BAD:
                        if (setTagBad)
                        {
                            foreach (var device in Channel.GetAllDevices().ToArray())
                            {
                                if (device.Enabled)
                                {
                                    foreach (var tag in device.GetAllTags().ToArray())
                                    {
                                        if (!tag.IsInternalTag && tag.Enabled)
                                        {
                                            tag.TimeStamp = DateTime.Now;
                                            tag.Quality = Quality.Bad;
                                            tag.RefreshInterval = (int)(DateTime.Now - tag.TimeStamp).TotalMilliseconds;
                                        }
                                    }
                                }
                            }
                        }

                        Refreshed?.Invoke(this, EventArgs.Empty);
                    }

                    // Thực hiện lệnh ghi tag
                    for (int i = 0; i < Channel.MaxWritesCount; i++)
                    {
                        if (DefaultPiorityWriteCommands.Count > 0)
                        {
                            var cmd = DefaultPiorityWriteCommands.Keys.FirstOrDefault();
                            DefaultPiorityWriteCommands.TryRemove(cmd, out WriteCommand removeCmd);

                            if (cmd != null)
                            {
                                WriteResponse response = ExecuteWriteCommand(cmd);
                                CommandExecutedEventArgs args = new CommandExecutedEventArgs(cmd, response);
                                cmd.OnExecuted(args);
                            }
                        }
                        else { break; }
                    }
                }
                catch { }
                finally
                {
                    stopwatch.Stop();
                    Debug.WriteLine($"Scan interval = {stopwatch.ElapsedMilliseconds}");
                    int delay = (int)(Channel.ScanRate - stopwatch.ElapsedMilliseconds);
                    if (delay < 1)
                        delay = 1;
                    Thread.Sleep(delay);
                }
            }
            // Nếu đã bị dispose thì khởi tạo event dispose
            Disposed?.Invoke(this, EventArgs.Empty);
            // Ngắt kết nối serial
            Stop();
            // Dispose đối tượng phụ trách đọc ghi modbus
            mbMaster.Dispose();
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
                lock (locker)
                {
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
            }
            catch { return false; }
            finally { Thread.Sleep(Channel.DelayBetweenPool); }
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
                lock (locker)
                {
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
            }
            catch { return false; }
            finally 
            { 
                Thread.Sleep(Channel.DelayBetweenPool); 
            }
        }

        public WriteResponse WriteCustom(WriteCommand cmd)
        {
            WriteResponse response = new WriteResponse()
            {
                WriteCommand = cmd,
                ExecuteTime = DateTime.Now
            };

            if (cmd == null)
            {
                response.Error = "The write command is null.";
            }
            else
            {
                string writeAddress = cmd.CustomWriteAddress?.Trim();
                if (string.IsNullOrWhiteSpace(writeAddress))
                {
                    response.Error = $"The write address {cmd.CustomWriteAddress} is not valid.";
                }
                else
                {
                    if (cmd.EquivalentDevice == null)
                    {
                        response.Error = $"The write address {cmd.CustomWriteAddress} is not valid.";
                    }
                    else
                    {
                        if (cmd.EquivalentDevice.Parent?.FindParent<IChannelCore>(x => x is IChannelCore) != Channel ||
                            !(cmd.EquivalentDevice is Device))
                        {
                            response.Error = $"The parent of tag doesn't match with driver.";
                        }
                        else
                        {
                            if (cmd.CustomWriteAddress.DecomposeAddress(out AddressType addressType, out ushort offset))
                            {
                                if (addressType == AddressType.Undefined)
                                {
                                    response.Error = $"The write address {cmd.CustomWriteAddress} is not valid.";
                                }
                                else
                                {
                                    if (cmd.CustomWriteValue == null)
                                    {
                                        response.Error = $"The write value is null or invaid";
                                    }
                                    else
                                    {
                                        byte deviceId = (byte)(cmd.EquivalentDevice as Device).DeviceId;
                                        switch (addressType)
                                        {
                                            case AddressType.InputContact:
                                            case AddressType.InputRegister:
                                                {
                                                    response.Error = $"The write address doesn't support write function";
                                                    break;
                                                }
                                            case AddressType.OutputCoil:
                                                {
                                                    if (cmd.CustomWriteValue.Length == 0 ||
                                                    cmd.CustomWriteValue.Length >= 125)
                                                    {
                                                        response.Error = $"The write value is not valid";
                                                        return response;
                                                    }
                                                    Thread.Sleep(cmd.Delay);

                                                    lock (locker)
                                                    {
                                                        try
                                                        {
                                                            bool[] writeValues = cmd.CustomWriteValue.Select(x =>
                                                            {
                                                                if (x > 0)
                                                                    return true;
                                                                return false;
                                                            }).ToArray();

                                                            response.IsSuccess = mbMaster.WriteMultipleCoils(deviceId, offset, (ushort)(writeValues.Length), writeValues);
                                                            if (!response.IsSuccess)
                                                                response.Error = mbMaster.modbusStatus;
                                                        }
                                                        catch { }
                                                    }
                                                    break;
                                                }
                                            case AddressType.HoldingRegister:
                                                {
                                                    if (cmd.CustomWriteValue.Length % 2 == 1 ||
                                                    cmd.CustomWriteValue.Length == 0 ||
                                                    cmd.CustomWriteValue.Length >= 125)
                                                    {
                                                        response.Error = $"The write value is not valid";
                                                        return response;
                                                    }
                                                    Thread.Sleep(cmd.Delay);
                                                    lock (locker)
                                                    {
                                                        try
                                                        {
                                                            response.IsSuccess = mbMaster.WriteHoldingRegisters(deviceId, offset, (ushort)(cmd.CustomWriteValue.Length / 2), cmd.CustomWriteValue);
                                                            if (!response.IsSuccess)
                                                                response.Error = mbMaster.modbusStatus;
                                                        }
                                                        catch { }
                                                    }
                                                    break;
                                                }
                                            default:
                                                break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                response.Error = $"The write address {cmd.CustomWriteAddress} is not valid.";
                            }
                        }
                    }
                }

                if (cmd.NextCommands != null)
                {
                    if (cmd.NextCommands.Count > 0)
                    {
                        if (!cmd.AllowExecuteNextCommandsWhenFail ||
                            cmd.AllowExecuteNextCommandsWhenFail && response.IsSuccess)
                        {
                            foreach (var nextCmd in cmd.NextCommands)
                            {
                                ExecuteWriteCommand(nextCmd);
                            }
                        }
                    }
                }

            }
            return response;
        }

        public WriteResponse WriteTag(WriteCommand cmd)
        {
            WriteResponse response = new WriteResponse()
            {
                WriteCommand = cmd,
                ExecuteTime = DateTime.Now
            };
            if (cmd == null)
            {
                response.Error = "The write command is null.";
            }
            else
            {
                if (cmd.EquivalentDevice == null)
                {
                    response.Error = $"Device not found";
                }
                else
                {
                    if (cmd.EquivalentDevice.Parent?.FindParent<IChannelCore>(x => x is IChannelCore) != Channel ||
                        !(cmd.EquivalentDevice is Device))
                    {
                        response.Error = $"The parent of tag doesn't match with driver.";
                    }
                    else
                    {
                        if (cmd.EquivalentTag != null && cmd.EquivalentTag is Tag tag)
                        {
                            if (tag.AddressType == AddressType.Undefined)
                            {
                                response.Error = $"The tag address {tag.Address} is not valid.";
                            }
                            else
                            {
                                if (tag.DataType == null)
                                {
                                    response.Error = $"The data type of tag is null";
                                }
                                else
                                {
                                    if (tag.DataType.TryParseToByteArray(cmd.Value, tag.Gain, tag.Offset, out byte[] writeBuffer, (cmd.EquivalentDevice as Device).ByteOrder))
                                    {
                                        byte deviceId = (byte)(cmd.EquivalentDevice as Device).DeviceId;
                                        switch (tag.AddressType)
                                        {
                                            case AddressType.InputContact:
                                            case AddressType.InputRegister:
                                                {
                                                    response.Error = $"The write address doesn't support write function";
                                                    break;
                                                }
                                            case AddressType.OutputCoil:
                                                {
                                                    Thread.Sleep(cmd.Delay);
                                                    lock (locker)
                                                    {
                                                        try
                                                        {
                                                            bool[] writeValues = new bool[1];

                                                            if (cmd.Value == "1")
                                                            {
                                                                writeValues[0] = true;
                                                            }
                                                            else if (cmd.Value == "0")
                                                            {
                                                                writeValues[0] = false;
                                                            }
                                                            else
                                                            {
                                                                response.IsSuccess = false;
                                                                response.Error = "Write value not valid";
                                                                return response;
                                                            }

                                                            bool bitResult = ByteHelper.GetBitAt(writeBuffer, 0, 0);
                                                            response.IsSuccess = mbMaster.WriteSingleCoil(deviceId, (ushort)tag.AddressOffset, bitResult);
                                                            if (!response.IsSuccess)
                                                                response.Error = mbMaster.modbusStatus;
                                                        }
                                                        catch { }
                                                        break;
                                                    }
                                                }
                                            case AddressType.HoldingRegister:
                                                {
                                                    Thread.Sleep(cmd.Delay);
                                                    lock (locker)
                                                    {
                                                        try
                                                        {
                                                            response.IsSuccess = mbMaster.WriteHoldingRegisters(deviceId, (ushort)tag.AddressOffset, (ushort)(writeBuffer.Length / 2), writeBuffer);
                                                            if (!response.IsSuccess)
                                                                response.Error = mbMaster.modbusStatus;
                                                        }
                                                        catch { }
                                                    }
                                                    break;
                                                }
                                            default:
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        response.Error = $"The write value is not valid";
                                    }
                                }
                            }
                        }
                        else
                        {
                            response.Error = $"Tag not found";
                        }
                    }
                }

                if (cmd.NextCommands != null)
                {
                    if (cmd.NextCommands.Count > 0)
                    {
                        if (!cmd.AllowExecuteNextCommandsWhenFail ||
                            cmd.AllowExecuteNextCommandsWhenFail && response.IsSuccess)
                        {
                            foreach (var nextCmd in cmd.NextCommands)
                            {
                                ExecuteWriteCommand(nextCmd);
                            }
                        }
                    }
                }
            }
            return response;
        }

        public WriteResponse ExecuteWriteCommand(WriteCommand cmd)
        {
            if (cmd == null)
            {
                return new WriteResponse()
                {
                    WriteCommand = cmd,
                    ExecuteTime = DateTime.Now,
                    Error = "The write command is null."
                };
            }
            else
            {
                if (cmd.IsCustomWrite)
                    return WriteCustom(cmd);
                else
                    return WriteTag(cmd);
            }
        }

        private async void OnWriteQueueAdded(object sender, EventArgs e)
        {
            WriteCommand cmd = WriteQueue.GetCommand();
            if (cmd != null)
            {
                switch (cmd.WritePiority)
                {
                    case WritePiority.Default:
                        cmd.Expired += OnWriteCommandExpired;
                        DefaultPiorityWriteCommands.TryAdd(cmd, cmd);
                        break;
                    case WritePiority.High:
                        await Task.Run(() =>
                        {
                            WriteResponse response = ExecuteWriteCommand(cmd);
                            CommandExecutedEventArgs args = new CommandExecutedEventArgs(cmd, response);
                            cmd.OnExecuted(args);
                        });
                        break;
                    default:
                        break;
                }
            }
        }

        private void OnWriteCommandExpired(object sender, EventArgs e)
        {
            if (sender is WriteCommand cmd)
            {
                cmd.Expired -= OnWriteCommandExpired;
                
                if (DefaultPiorityWriteCommands.ContainsKey(cmd))
                {
                    DefaultPiorityWriteCommands.TryRemove(cmd, out WriteCommand removeCmd);
                }
            }
        }

        public override void Dispose()
        {
            IsDisposed = true;
            Stop();
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        public override object GetCreateChannelControl(IGroupItem parent, IChannelCore templateItem = null)
        {
            CreateChannelView view = new CreateChannelView()
            {
                DataContext = ViewModelSource.Create(() => new AddChannelViewModel(this, parent, templateItem))
            };
            return view;
        }

        public override object GetCreateDeviceControl(IGroupItem parent, IDeviceCore templateItem = null)
        {
            CreateDeviceView view = new CreateDeviceView()
            {
                DataContext = ViewModelSource.Create(() => new AddDeviceViewModel(this, parent, templateItem))
            };
            return view;
        }

        public override object GetCreateTagControl(IGroupItem parent, ITagCore templateItem = null)
        {
            CreateTagView view = new CreateTagView()
            {
                DataContext = ViewModelSource.Create(() => new AddTagViewModel(this, parent, templateItem))
            };
            return view;
        }

        public override object GetEditChannelControl(IChannelCore channel)
        {
            EditChannelView view = new EditChannelView()
            {
                DataContext = ViewModelSource.Create(() => new EditChannelViewModel(this, channel))
            };
            return view;
        }

        public override object GetEditDeviceControl(IDeviceCore device)
        {
            EditDeviceView view = new EditDeviceView()
            {
                DataContext = ViewModelSource.Create(() => new EditDeviceViewModel(this, device))
            };
            return view;
        }

        public override object GetEditTagControl(ITagCore tag)
        {
            EditTagView view = new EditTagView()
            {
                DataContext = ViewModelSource.Create(() => new EditTagViewModel(this, tag))
            };
            return view;
        }

        public override IChannelCore CreateChannel(IGroupItem parent)
        {
            return new Channel(parent);
        }

        public override IDeviceCore CreateDevice(IGroupItem parent)
        {
            return new Device(parent);
        }

        public override ITagCore CreateTag(IGroupItem parent)
        { 
            return new Tag(parent);
        }
        #endregion
    }
}
