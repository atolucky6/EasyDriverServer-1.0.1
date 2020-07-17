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

        static ModbusRTUDriver()
        {
            // Khởi tạo các kiểu dữ liệu
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

        #endregion

        #region Constructors

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

        public Dictionary<IDeviceCore, List<ReadBlockSetting>> DeviceToReadBlockSettings { get; set; }
        public Dictionary<IDeviceCore, List<string>> DeviceToReadBlockString { get; set; }
        public IChannelCore Channel { get; set; }
        public bool IsDisposed { get; private set; }

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

        public bool Connect()
        {
            if (Channel.ParameterContainer.Parameters.Count < 5)
                return false;
            semaphore.Wait();
            try
            {
                InitializeSerialPort();
                return mbMaster.Open();
            }
            catch { return false; }
            finally
            {
                semaphore.Release();
                if (refreshTask == null)
                    refreshTask = Task.Factory.StartNew(Refresh, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
        }

        private void InitializeSerialPort()
        {
            if (Channel.ParameterContainer.Parameters.Count >= 6)
            {
                var port = Channel.ParameterContainer.Parameters["Port"].ToString();
                var baudRate = (int)Channel.ParameterContainer.Parameters["Baudrate"];
                var dataBits = (int)Channel.ParameterContainer.Parameters["DataBits"];
                var stopBits = (StopBits)Channel.ParameterContainer.Parameters["StopBits"];
                var parity = (Parity)Channel.ParameterContainer.Parameters["Parity"];
                mbMaster.Init(port, baudRate, dataBits, parity, stopBits);
            }
        }

        private void Refresh()
        {
            while (!IsDisposed)
            {
                try
                {
                    stopwatch.Restart();
                    InitializeSerialPort();

                    if (mbMaster.Open())
                    {
                        foreach (var channelChild in Channel.Childs.ToArray())
                        {
                            IDeviceCore device = channelChild as IDeviceCore;

                            if (device == null)
                                continue;

                            byte deviceId = byte.Parse(device.ParameterContainer.Parameters["DeviceId"].ToString());
                            ByteOrder byteOrder = (ByteOrder)device.ParameterContainer.Parameters["ByteOrder"];
                            int maxTryTimes = int.Parse(device.ParameterContainer.Parameters["TryReadWriteTimes"].ToString());
                            int timeout = int.Parse(device.ParameterContainer.Parameters["Timeout"].ToString());

                            mbMaster.ResponseTimeOut = timeout;
                            mbMaster.SerialPort.ReadTimeout = timeout;
                            mbMaster.SerialPort.WriteTimeout = timeout;

                            #region READ BLOCK FIRST

                            if (!DeviceToReadBlockSettings.ContainsKey(device))
                                DeviceToReadBlockSettings[device] = new List<ReadBlockSetting>();

                            if (!DeviceToReadBlockString.ContainsKey(device))
                                DeviceToReadBlockString[device] = new List<string> { "", "", "", "" };

                            if (device.ParameterContainer.Parameters.ContainsKey("ReadInputContactsBlockSetting"))
                            {
                                string readBlockStr = device.ParameterContainer.Parameters["ReadInputContactsBlockSetting"].ToString();
                                if (DeviceToReadBlockString[device][0] != readBlockStr)
                                {
                                    DeviceToReadBlockSettings[device].Clear();
                                    DeviceToReadBlockString[device][0] = readBlockStr;
                                    string[] readBlockSplit = readBlockStr.Split('|');
                                    if (readBlockSplit.Length > 0)
                                    {
                                        foreach (var item in readBlockSplit)
                                        {
                                            if (item.StartsWith("True"))
                                            {
                                                ReadBlockSetting newBlock = ReadBlockSetting.Convert(item);
                                                newBlock.AddressType = AddressType.InputContact;
                                                if (newBlock.IsValid)
                                                    DeviceToReadBlockSettings[device].Add(newBlock);
                                            }
                                        }
                                    }
                                }
                            }

                            if (device.ParameterContainer.Parameters.ContainsKey("ReadOutputCoilsBlockSetting"))
                            {
                                string readBlockStr = device.ParameterContainer.Parameters["ReadOutputCoilsBlockSetting"].ToString();
                                if (DeviceToReadBlockString[device][1] != readBlockStr)
                                {
                                    DeviceToReadBlockSettings[device].Clear();
                                    DeviceToReadBlockString[device][1] = readBlockStr;
                                    string[] readBlockSplit = readBlockStr.Split('|');
                                    if (readBlockSplit.Length > 0)
                                    {
                                        foreach (var item in readBlockSplit)
                                        {
                                            if (item.StartsWith("True"))
                                            {
                                                ReadBlockSetting newBlock = ReadBlockSetting.Convert(item);
                                                newBlock.AddressType = AddressType.OutputCoil;
                                                if (newBlock.IsValid)
                                                    DeviceToReadBlockSettings[device].Add(newBlock);
                                            }
                                        }
                                    }
                                }
                            }

                            if (device.ParameterContainer.Parameters.ContainsKey("ReadInputRegistersBlockSetting"))
                            {
                                string readBlockStr = device.ParameterContainer.Parameters["ReadInputRegistersBlockSetting"].ToString();
                                if (DeviceToReadBlockString[device][2] != readBlockStr)
                                {
                                    DeviceToReadBlockSettings[device].Clear();
                                    DeviceToReadBlockString[device][2] = readBlockStr;
                                    string[] readBlockSplit = readBlockStr.Split('|');
                                    if (readBlockSplit.Length > 0)
                                    {
                                        foreach (var item in readBlockSplit)
                                        {
                                            if (item.StartsWith("True"))
                                            {
                                                ReadBlockSetting newBlock = ReadBlockSetting.Convert(item);
                                                newBlock.AddressType = AddressType.InputRegister;
                                                if (newBlock.IsValid)
                                                    DeviceToReadBlockSettings[device].Add(newBlock);
                                            }
                                        }
                                    }
                                }
                            }

                            if (device.ParameterContainer.Parameters.ContainsKey("ReadHoldingRegistersBlockSetting"))
                            {
                                string readBlockStr = device.ParameterContainer.Parameters["ReadHoldingRegistersBlockSetting"].ToString();
                                if (DeviceToReadBlockString[device][3] != readBlockStr)
                                {
                                    DeviceToReadBlockSettings[device].Clear();
                                    DeviceToReadBlockString[device][3] = readBlockStr;
                                    string[] readBlockSplit = readBlockStr.Split('|');
                                    if (readBlockSplit.Length > 0)
                                    {
                                        foreach (var item in readBlockSplit)
                                        {
                                            if (item.StartsWith("True"))
                                            {
                                                ReadBlockSetting newBlock = ReadBlockSetting.Convert(item);
                                                newBlock.AddressType = AddressType.HoldingRegister;
                                                if (newBlock.IsValid)
                                                    DeviceToReadBlockSettings[device].Add(newBlock);
                                            }
                                        }
                                    }
                                }
                            }

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
                                ITagCore tag = childDevice as ITagCore;

                                if (tag == null)
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
                                                                tag.Value = block.BoolBuffer[index] ? (tag.Gain + tag.Offset).ToString() : tag.Offset.ToString();
                                                                break;
                                                            case AddressType.InputRegister:
                                                            case AddressType.HoldingRegister:
                                                                tag.Value = tag.DataType.ConvertToValue(block.ByteBuffer, tag.Gain, tag.Offset, index, 0, device.ByteOrder);
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
                                                    bool[] inputs = new bool[1];
                                                    readSuccess = ReadBits(deviceId, AddressType.InputContact, (ushort)tag.ParameterContainer.Parameters["Offset"], 1, ref inputs);
                                                    if (readSuccess)
                                                        tag.Value = inputs[0] ? (tag.Gain + tag.Offset).ToString() : tag.Offset.ToString();
                                                    break;
                                                case AddressType.OutputCoil:
                                                    bool[] outputs = new bool[1];
                                                    readSuccess = ReadBits(deviceId, AddressType.OutputCoil, (ushort)tag.ParameterContainer.Parameters["Offset"], 1, ref outputs);
                                                    if (readSuccess)
                                                        tag.Value = outputs[0] ? (tag.Gain + tag.Offset).ToString() : tag.Offset.ToString();
                                                    break;
                                                case AddressType.InputRegister:
                                                    byte[] inputRegs = new byte[tag.DataType.RequireByteLength];
                                                    readSuccess = ReadRegisters(deviceId, AddressType.InputRegister, (ushort)tag.ParameterContainer.Parameters["Offset"], 1, ref inputRegs);
                                                    if (readSuccess)
                                                        tag.Value = tag.DataType.ConvertToValue(inputRegs, tag.Gain, tag.Offset, 0, 0, device.ByteOrder);
                                                    break;
                                                case AddressType.HoldingRegister:
                                                    byte[] holdingRegs = new byte[tag.DataType.RequireByteLength];
                                                    readSuccess = ReadRegisters(deviceId, AddressType.HoldingRegister, (ushort)tag.ParameterContainer.Parameters["Offset"], 1, ref holdingRegs);
                                                    if (readSuccess)
                                                        tag.Value = tag.DataType.ConvertToValue(holdingRegs, tag.Gain, tag.Offset, 0, 0, device.ByteOrder);
                                                    break;
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

            Disposed?.Invoke(this, EventArgs.Empty);
            Disconnect();
            mbMaster.Dispose();
        }

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

        public IEnumerable<IDataType> GetSupportDataTypes()
        {
            return supportDataTypes;
        }

        public Quality Write(ITagCore tag, string value)
        {
            // Nếu driver đã bị dispose thì không thực hiện việc ghi này
            if (!IsDisposed)
            {
                try
                {
                    // Kiểm tra các điều kiện cần để thực hiện việc ghi
                    if (tag != null && tag.DataType != null && tag.Parent is IDeviceCore device && tag.AccessPermission == AccessPermission.ReadAndWrite)
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
                            }
                            tag.ParameterContainer.Parameters["IsValid"] = false;
                        }
                        if ((bool)tag.ParameterContainer.Parameters["IsValid"])
                        {
                            // Lấy thông tin byte order của device parent
                            ByteOrder byteOrder = (ByteOrder)device.ParameterContainer.Parameters["ByteOrder"];
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
                                    // Lặp việc ghi cho đến khi thành công hoặc đã vượt quá số lần cho phép ghi
                                    for (int i = 0; i < maxTryTimes; i++)
                                    {
                                        if ((AddressType)tag.ParameterContainer.Parameters["AddressType"] == AddressType.OutputCoil)
                                        {
                                            bool bitResult = ByteHelper.GetBitAt(writeBuffer, 0, 0);
                                            try
                                            {
                                                semaphore.Wait();
                                                writeResult = mbMaster.WriteSingleCoil(deviceId, (ushort)tag.ParameterContainer.Parameters["Offset"], bitResult);
                                            }
                                            finally { Thread.Sleep(DelayBetweenPool); semaphore.Release(); }
                                        }
                                        else if ((AddressType)tag.ParameterContainer.Parameters["AddressType"] == AddressType.HoldingRegister)
                                        {
                                            try
                                            {
                                                semaphore.Wait();
                                                writeResult = mbMaster.WriteHoldingRegisters(deviceId, (ushort)tag.ParameterContainer.Parameters["Offset"], (ushort)(writeBuffer.Length / 2), writeBuffer);
                                            }
                                            finally { Thread.Sleep(DelayBetweenPool); semaphore.Release(); }
                                        }
                                        else { break; }
                                        if (writeResult) // Nếu ghi thành công thì thoát khỏi vòng lặp
                                            break;

                                    }
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
