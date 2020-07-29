using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using EasyDriverPlugin;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace EasyDriver.OmronHostLink
{
    /// <summary>
    /// Driver thực hiện việc đọc ghi giá trị
    /// </summary>
    public class OmronHostLinkDriver : IEasyDriverPlugin
    {
        #region Static

        /// <summary>
        /// Danh sách các kiểu dữ liệu mà driver này hỗ trợ
        /// </summary>
        static readonly List<IDataType> supportDataTypes;

        static OmronHostLinkDriver()
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
                new LReal { Name = "Double" },
                new EasyDriverPlugin.Char(),
            };
        }

        #endregion

        #region Constructors

        public OmronHostLinkDriver()
        {
            hostLink = new OmronHostLinkSerial();
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

        readonly OmronHostLinkSerial hostLink;
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
                return hostLink.Open();
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
                hostLink.Init(port, baudRate, dataBits, parity, stopBits);
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

                    if (hostLink.Open())
                    {
                        foreach (var channelChild in Channel.Childs.ToArray())
                        {
                            IDeviceCore device = channelChild as IDeviceCore;

                            if (device == null)
                                continue;

                            byte unitNo = byte.Parse(device.ParameterContainer.Parameters["UnitNo"].ToString());
                            ByteOrder byteOrder = device.ByteOrder;
                            int maxTryTimes = int.Parse(device.ParameterContainer.Parameters["TryReadWriteTimes"].ToString());
                            int timeout = int.Parse(device.ParameterContainer.Parameters["Timeout"].ToString());

                            hostLink.ResponseTimeOut = timeout;
                            hostLink.SerialPort.ReadTimeout = timeout;
                            hostLink.SerialPort.WriteTimeout = timeout;

                            #region READ BLOCK FIRST

                            if (!DeviceToReadBlockSettings.ContainsKey(device))
                                DeviceToReadBlockSettings[device] = new List<ReadBlockSetting>();

                            if (!DeviceToReadBlockString.ContainsKey(device))
                                DeviceToReadBlockString[device] = new List<string> { "", "", "", "" };

                            if (device.ParameterContainer.Parameters.ContainsKey("ReadBlockSettings"))
                            {
                                string readBlockStr = device.ParameterContainer.Parameters["ReadBlockSettings"].ToString();
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
                                semaphore.Wait();
                                try
                                {
                                    while (!block.ReadResult)
                                    {
                                        try
                                        {
                                            block.ReadResult = hostLink.Read(unitNo, Extensions.GetReadCommand(block.AddressType), block.StartAddress, block.Count, ref block.ByteBuffer);
                                        }
                                        finally { Thread.Sleep(DelayBetweenPool); }
                                        readCount++;
                                        if (readCount >= maxTryTimes)
                                            break;
                                    }
                                }
                                finally { semaphore.Release(); }
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
                                    if (tag.Address.DecomposeAddress(out AddressType addressType, out ushort wordOffset, out ushort bitOffset))
                                    {
                                        tag.ParameterContainer.Parameters["AddressType"] = (int)addressType;
                                        tag.ParameterContainer.Parameters["WordOffset"] = wordOffset;
                                        tag.ParameterContainer.Parameters["BitOffset"] = bitOffset;
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
                                                    (ushort)tag.ParameterContainer.Parameters["WordOffset"],
                                                    (ushort)tag.ParameterContainer.Parameters["BitOffset"],
                                                    tag.DataType.RequireByteLength,
                                                    out int byteIndex, 
                                                    out int bitIndex))
                                                {
                                                    containInBlock = true;
                                                    readSuccess = block.ReadResult;
                                                    if (readSuccess)
                                                    {
                                                        if (tag.DataType.BitLength == 1)
                                                        {
                                                            switch (byteOrder)
                                                            {
                                                                case ByteOrder.ABCD:
                                                                case ByteOrder.CDAB:
                                                                    if (byteIndex % 2 == 1)
                                                                        byteIndex--;
                                                                    else
                                                                        byteIndex++;
                                                                    break;
                                                                case ByteOrder.BADC:
                                                                case ByteOrder.DCBA:
                                                                    break;
                                                                default:
                                                                    break;
                                                            }
                                                        }

                                                        tag.Value = tag.DataType.ConvertToValue(
                                                            block.ByteBuffer, 
                                                            tag.Gain, 
                                                            tag.Offset, 
                                                            byteIndex, 
                                                            bitIndex, 
                                                            byteOrder);
                                                    }
                                                    break;
                                                }
                                            }
                                        }

                                        if (!containInBlock)
                                        {
                                            ushort wordLen = (ushort)(tag.DataType.RequireByteLength / 2);
                                            if (wordLen == 0)
                                                wordLen++;
                                            byte[] buffer = new byte[wordLen * 2];
                                            semaphore.Wait();
                                            try
                                            {
                                                readSuccess = hostLink.Read(
                                                unitNo,
                                                Extensions.GetReadCommand((AddressType)tag.ParameterContainer.Parameters["AddressType"]),
                                                (ushort)tag.ParameterContainer.Parameters["WordOffset"],
                                                wordLen,
                                                ref buffer);
                                            }
                                            finally { Thread.Sleep(DelayBetweenPool); semaphore.Release(); }
                                            if (readSuccess)
                                            {
                                                int byteIndex = 0;
                                                int bitIndex = (ushort)tag.ParameterContainer.Parameters["BitOffset"];
                                                if (bitIndex / 8 == 1)
                                                {
                                                    byteIndex++;
                                                    bitIndex = bitIndex - 8;
                                                }

                                                if (tag.DataType.BitLength == 1)
                                                {
                                                    switch (byteOrder)
                                                    {
                                                        case ByteOrder.ABCD:
                                                        case ByteOrder.CDAB:
                                                            if (byteIndex == 1)
                                                                byteIndex = 0;
                                                            else
                                                                byteIndex = 1;
                                                            break;
                                                        case ByteOrder.BADC:
                                                        case ByteOrder.DCBA:
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                }

                                                tag.Value = tag.DataType.ConvertToValue(
                                                    buffer,
                                                    tag.Gain,
                                                    tag.Offset,
                                                    byteIndex,
                                                    bitIndex,
                                                    byteOrder);
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
            hostLink.Dispose();
        }

        public bool Disconnect()
        {
            try
            {
                semaphore.Wait();
                return hostLink.Close();
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
                            if (tag.Address.DecomposeAddress(out AddressType addressType, out ushort wordOffset, out ushort bitOffset))
                            {
                                tag.ParameterContainer.Parameters["AddressType"] = addressType;
                                tag.ParameterContainer.Parameters["WordOffset"] = wordOffset;
                                tag.ParameterContainer.Parameters["BitOffset"] = bitOffset;
                                tag.ParameterContainer.Parameters["IsValid"] = true;
                                tag.ParameterContainer.Parameters["LastAddress"] = tag.Address;
                            }
                            tag.ParameterContainer.Parameters["IsValid"] = false;
                        }
                        if ((bool)tag.ParameterContainer.Parameters["IsValid"])
                        {
                            // Lấy thông tin byte order của device parent
                            ByteOrder byteOrder = device.ByteOrder;
                            // Chuyển đổi giá trị cần ghi thành chuỗi byte nếu thành công thì mới ghi
                            if (tag.DataType.TryParseToByteArray(value, tag.Gain, tag.Offset, out byte[] writeBuffer, byteOrder))
                            {
                                // Lấy thông tin của device như UnitNo, MaxReadWriteTimes, Timeout...
                                byte unitNo = byte.Parse(device.ParameterContainer.Parameters["UnitNo"].ToString());
                                int maxTryTimes = int.Parse(device.ParameterContainer.Parameters["TryReadWriteTimes"].ToString());
                                int timeout = int.Parse(device.ParameterContainer.Parameters["Timeout"].ToString());

                                // Cài timeout cho serial port
                                hostLink.SerialPort.ReadTimeout = timeout;
                                hostLink.SerialPort.WriteTimeout = timeout;

                                // Kiểm tra xem serial port đã mở mới cho phép ghis
                                if (hostLink.SerialPort.IsOpen)
                                {
                                    // Khởi tạo biến thể hiện kết quả của việc ghi
                                    bool writeResult = false;
                                    if ((tag.DataType.BitLength != 1))
                                    {
                                        semaphore.Wait();
                                        try
                                        {
                                            // Lặp việc ghi cho đến khi thành công hoặc đã vượt quá số lần cho phép ghi
                                            for (int i = 0; i < maxTryTimes; i++)
                                            {
                                                byte[] buffer = new byte[tag.DataType.RequireByteLength];
                                                tag.DataType.TryParseToByteArray(value, tag.Gain, tag.Offset, out buffer, byteOrder);
                                                WriteCommand writeCommand = Extensions.GetWriteCommand((AddressType)tag.ParameterContainer.Parameters["AddressType"]);
                                                try
                                                {

                                                    writeResult = hostLink.Write(
                                                        unitNo,
                                                        writeCommand,
                                                        (ushort)tag.ParameterContainer.Parameters["WordOffset"],
                                                        buffer);
                                                }
                                                finally { Thread.Sleep(DelayBetweenPool); }
                                                if (writeResult) // Nếu ghi thành công thì thoát khỏi vòng lặp
                                                    break;
                                            }
                                        }
                                        finally { semaphore.Release(); }   
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
