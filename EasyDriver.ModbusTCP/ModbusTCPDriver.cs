using System;
using System.Collections.Generic;
using EasyDriverPlugin;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Specialized;
using System.Text;
using DevExpress.Mvvm.POCO;
using System.Threading;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace EasyDriver.ModbusTCP
{
    /// <summary>
    /// Driver thực hiện việc đọc ghi giá trị
    /// </summary>
    public class ModbusTCPDriver : EasyDriverPluginBase
    {
        #region Static

        /// <summary>
        /// Danh sách các kiểu dữ liệu mà driver này hỗ trợ
        /// </summary>
        static readonly List<IDataType> supportDataTypes;

        /// <summary>
        /// Static constructor khởi tạo 1 lần khi load driver
        /// </summary>
        static ModbusTCPDriver()
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
                new ModbusTCP.String() { Name = "String" , RequireByteLength = 0},
            };
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public ModbusTCPDriver()
        {
            mbClient = new ModbusTCPClient();
            locker = new SemaphoreSlim(1, 1);
            stopwatch = new Stopwatch();
            WriteQueue.Enqueued += OnWriteQueueAdded;
            DefaultPiorityWriteCommands = new ConcurrentDictionary<WriteCommand, WriteCommand>();
            AddressTypeSource = Enum.GetValues(typeof(AddressType)).Cast<AddressType>().ToList();
            ByteOrderSource = Enum.GetValues(typeof(ByteOrder)).Cast<ByteOrder>().ToList();
            ReadModeSource = Enum.GetValues(typeof(ReadMode)).Cast<ReadMode>().ToList();
            AccessPermissionSource = Enum.GetValues(typeof(AccessPermission)).Cast<AccessPermission>().ToList();
        }

        #endregion

        #region Members

        /// <summary>
        /// Channel chạy driver này
        /// </summary>
        public Channel Channel { get; set; }
        public bool IsConnected { get; private set; }
        private ModbusTCPClient mbClient;
        public List<ByteOrder> ByteOrderSource { get; set; }
        public List<AddressType> AddressTypeSource { get; set; }
        public List<ReadMode> ReadModeSource { get; set; }
        public List<AccessPermission> AccessPermissionSource { get; set; }
        public ConcurrentDictionary<WriteCommand, WriteCommand> DefaultPiorityWriteCommands { get; set; } 
        public override List<IDataType> SupportDataTypes
        {
            get => supportDataTypes;
            protected set => base.SupportDataTypes = value;
        }
        private Task refreshTask;
        readonly object locker = new object();
        readonly Stopwatch stopwatch;
        /// <summary>
        /// Bit xác định driver này đã bị dispose
        /// </summary>
        public bool IsDisposed { get; private set; }

        public event EventHandler Disposed;
        public event EventHandler Refreshed;
        public void RaiseRefresedEvent(object sender)
        {
            Refreshed?.Invoke(sender, EventArgs.Empty);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Hàm bắt đầu kết nối
        /// </summary>
        /// <returns></returns>
        public override bool Start(IChannelCore channelCore)
        {
            try
            {
                if (channelCore is Channel channel)
                    Channel = channel;
                else
                    return false;

                if (IsConnected)
                    return true;

                if (Channel != null)
                {
                    IsConnected = true;
                    refreshTask = Task.Factory.StartNew(Refresh, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                    return true;
                }
            }
            catch {  }
            return false;
        }

        /// <summary>
        /// Hàm ngắt kết nối
        /// </summary>
        /// <returns></returns>
        public override bool Stop()
        {
            try
            {
                if (!IsConnected || Channel == null)
                    return true;

                if (IsConnected)
                {
                    mbClient?.Disconnect();
                    return true;
                }
            }
            catch {  }
            return false;
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

                    if (mbClient == null)
                    {
                        InitMBClient();
                    }

                    if (!mbClient.Connected)
                    {
                        lock (locker)
                        {
                            try
                            {
                                mbClient.Connect(Channel.IpAddress, Channel.Port, false);
                            }
                            catch { }
                        }
                    }

                    if (mbClient.IsSocketNull)
                    {
                        try
                        {
                            InitMBClient();
                        }
                        catch { }
                    }

                    if (Channel.Enabled)
                    {
                        foreach (var childOfChannel in Channel.Childs)
                        {
                            if (childOfChannel is Device device)
                            {
                                if (!device.Enabled)
                                    continue;

                                if (mbClient.Connected)
                                {
                                    byte unitId = (byte)device.UnitId;
                                    ByteOrder byteOrder = device.ByteOrder;
                                    // Lấy số lần thử đọc tối đa nếu đọc không thành công
                                    int maxTryTimes = device.TryReadWriteTimes;
                                    // Thời gian timeout của việc đọc ghi
                                    int timeout = device.Timeout;

                                    // Set lại thời gian timeout
                                    mbClient.SetTimeout(timeout);

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
                                                            block.ReadResult = ReadBits(unitId, block.AddressType, block.StartOffset, block.BufferCount, ref block.BoolBuffer);
                                                            break;
                                                        case AddressType.InputRegister:
                                                        case AddressType.HoldingRegister:
                                                            block.ReadResult = ReadRegisters(unitId, block.AddressType, block.StartOffset, block.BufferCount, ref block.ByteBuffer);
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
                                        // Đảm bảo tag phải là modbus tcp tag
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
                                                                unitId,
                                                                AddressType.InputContact,
                                                                (ushort)tag.AddressOffset,
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
                                                                unitId,
                                                                AddressType.OutputCoil,
                                                                (ushort)tag.AddressOffset,
                                                                1, ref outputs);
                                                            if (readSuccess)
                                                                tag.Value = outputs[0] ?
                                                                    (tag.Gain + tag.Offset).ToString() : tag.Offset.ToString();
                                                            break;
                                                        }
                                                    case AddressType.InputRegister:
                                                        {
                                                            byte[] inputRegs = new byte[tag.RequireByteLength];
                                                            readSuccess = ReadRegisters(
                                                                unitId,
                                                                AddressType.InputRegister,
                                                                (ushort)tag.AddressOffset,
                                                                (ushort)(tag.RequireByteLength / 2),
                                                                ref inputRegs);
                                                            if (readSuccess)
                                                                tag.Value = tag.DataType.ConvertToValue(inputRegs, tag.Gain, tag.Offset, 0, 0, device.ByteOrder);
                                                            break;
                                                        }
                                                    case AddressType.HoldingRegister:
                                                        {
                                                            byte[] holdingRegs = new byte[tag.RequireByteLength];
                                                            readSuccess = ReadRegisters(
                                                                unitId,
                                                                AddressType.HoldingRegister,
                                                                (ushort)tag.AddressOffset,
                                                                (ushort)(tag.RequireByteLength / 2),
                                                                ref holdingRegs);
                                                            if (readSuccess)
                                                                tag.Value = tag.DataType.ConvertToValue(holdingRegs, tag.Gain, tag.Offset, 0, 0, device.ByteOrder);
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

                                    if (mbClient.IsSocketNull)
                                    {
                                        SetAllTagBad(device);
                                    }

                                    // Thực hiện lệnh ghi tag
                                    for (int i = 0; i < device.MaxWritesCount; i++)
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
                                else
                                {
                                    SetAllTagBad(device);
                                }

                                if (mbClient.IsSocketNull)
                                {
                                    SetAllTagBad(device);
                                }
                            }
                        }
                    }
                }
                catch { }
                finally
                {

                    stopwatch.Stop();
                    int delay = (int)(Channel.ScanRate - stopwatch.ElapsedMilliseconds);
                    if (delay < 1)
                        delay = 1;
                    Thread.Sleep(delay);
                }
            }
            // Dispose đối tượng phụ trách đọc ghi modbus
            mbClient?.Dispose();
        }

        /// <summary>
        /// Hàm đọc input và output
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="addressType"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="boolBuffer"></param>
        /// <returns></returns>
        private bool ReadBits(byte unitId, AddressType addressType, ushort offset, ushort count, ref bool[] boolBuffer)
        {
            try
            {
                if (mbClient.IsSocketNull)
                    InitMBClient();

                if (mbClient != null && !mbClient.IsSocketNull)
                {
                    lock (locker)
                    {
                        byte[] result = null;
                        switch (addressType)
                        {
                            case AddressType.InputContact:
                                mbClient.ReadDiscreteInputs(unitId, unitId, offset, count, ref result);
                                break;
                            case AddressType.OutputCoil:
                                mbClient.ReadCoils(unitId, unitId, offset, count, ref result);
                                break;
                            default:
                                break;
                        }
                        if (result != null)
                        {
                            for (int i = 0; i < result.Length; i++)
                            {
                                for (int j = 0; j < 8; j++)
                                {
                                    if ((8 * i + j) < count)
                                        boolBuffer[8 * i + j] = Convert.ToBoolean((result[i] >> j) & 0x01);
                                }
                            }
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch { }
            finally { Thread.Sleep(Channel.DelayBetweenPool); }
            return false;
        }

        /// <summary>
        /// Hàm đọc input register và holding register
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="addressType"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="byteBuffer"></param>
        /// <returns></returns>
        private bool ReadRegisters(byte unitId, AddressType addressType, ushort offset, ushort count, ref byte[] byteBuffer)
        {
            try
            {
                if (mbClient.IsSocketNull)
                    InitMBClient();

                if (mbClient != null && !mbClient.IsSocketNull)
                {
                    lock (locker)
                    {
                        byte[] result = null;
                        switch (addressType)
                        {
                            case AddressType.InputRegister:
                                mbClient.ReadInputRegister(unitId, unitId, offset, count, ref result);
                                break;
                            case AddressType.HoldingRegister:
                                mbClient.ReadHoldingRegister(unitId, unitId, offset, count, ref result);
                                break;
                            default:
                                break;
                        }
                        if (result != null)
                        {
                            Array.Copy(result, byteBuffer, byteBuffer.Length);
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch {  }
            finally { Thread.Sleep(Channel.DelayBetweenPool); }
            return false;
        }

        /// <summary>
        /// Hàm set tất cả các tag trong device về trạng thái bad
        /// </summary>
        private void SetAllTagBad(Device device)
        {
            foreach (var childDevice in device.GetAllTags().ToArray())
            {
                if (childDevice is ITagCore tag)
                {
                    if (tag.IsInternalTag)
                        continue;
                    tag.TimeStamp = DateTime.Now;
                    tag.Quality = Quality.Bad;
                    tag.RefreshInterval = (int)(DateTime.Now - tag.TimeStamp).TotalMilliseconds;
                }
            }
        }

        /// <summary>
        /// Hàm khởi tạo Modbus TCP Client 
        /// </summary>
        public void InitMBClient()
        {
            if (!IsDisposed)
            {
                try
                {
                    if (Channel != null && Channel.ParameterContainer != null)
                    {
                        if (Channel.ParameterContainer.Contains("IpAddress"))
                        {
                            lock (locker)
                            {
                                if (mbClient != null)
                                    mbClient.Dispose();
                                string ipAddress = Channel.IpAddress;
                                ushort port = Channel.Port;
                                mbClient = new ModbusTCPClient();
                                mbClient.Connect(ipAddress, port, false);
                            }
                        }
                    }
                }
                catch { }
            }
        }

        public WriteResponse WriteCustom(WriteCommand cmd)
        {
            WriteResponse response = new WriteResponse()
            {
                WriteCommand = cmd,
                ExecuteTime = DateTime.Now
            };

            try
            {
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
                                            byte unitId = (byte)(cmd.EquivalentDevice as Device).UnitId;
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
                                                        try
                                                        {
                                                            lock (locker)
                                                            {
                                                                byte[] result = null;
                                                                mbClient.WriteMultipleCoils(
                                                                    unitId,
                                                                    unitId,
                                                                    offset,
                                                                    (ushort)cmd.CustomWriteValue.Length, cmd.CustomWriteValue, ref result);
                                                                response.IsSuccess = result != null;
                                                                if (!response.IsSuccess)
                                                                    response.Error = $"Write not success.";
                                                            }
                                                        }
                                                        catch { }
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
                                                        try
                                                        {
                                                            lock (locker)
                                                            {
                                                                byte[] result = null;
                                                                mbClient.WriteMultipleRegister(
                                                                    unitId,
                                                                    unitId,
                                                                    offset,
                                                                    cmd.CustomWriteValue, ref result);
                                                                response.IsSuccess = result != null;

                                                                if (!response.IsSuccess)
                                                                    response.Error = $"Write not success.";
                                                            }
                                                        }
                                                        catch { }
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
            }
            catch (Exception ex)
            {
                response.Error = $"Exception throw: {ex.ToString()}";
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
            try
            {
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
                                            byte unitId = (byte)(cmd.EquivalentDevice as Device).UnitId;
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
                                                        try
                                                        {
                                                            lock (locker)
                                                            {
                                                                bool? writeValue = null;
                                                                if (cmd.Value == "1")
                                                                {
                                                                    writeValue = true;
                                                                }
                                                                else if (cmd.Value == "0")
                                                                {
                                                                    writeValue = false;
                                                                }
                                                                else
                                                                {
                                                                    response.IsSuccess = false;
                                                                    response.Error = "Write value is not valid";
                                                                    return response;
                                                                }

                                                                byte[] result = null;
                                                                mbClient.WriteSingleCoils(
                                                                    unitId, unitId, (ushort)tag.AddressOffset, writeValue.Value, ref result);

                                                                response.IsSuccess = result != null;
                                                                if (!response.IsSuccess)
                                                                    response.Error = $"Write not success.";
                                                            }
                                                        }
                                                        catch { }
                                                        break;
                                                    }
                                                case AddressType.HoldingRegister:
                                                    {
                                                        Thread.Sleep(cmd.Delay);
                                                        try
                                                        {
                                                            lock (locker)
                                                            {
                                                                byte[] result = null;
                                                                mbClient.WriteMultipleRegister(
                                                                    unitId, unitId, (ushort)tag.AddressOffset, writeBuffer, ref result);
                                                                response.IsSuccess = result != null;
                                                                if (!response.IsSuccess)
                                                                    response.Error = $"Write not success.";
                                                            }
                                                        }
                                                        catch { }
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
            }
            catch (Exception ex)
            {
                response.Error = $"Exception throw: {ex.ToString()}";
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
