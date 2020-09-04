using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyDriver.ModbusTCP
{
    class ModbusDeviceReader : IDisposable
    {
        public ModbusDeviceReader(IDeviceCore device)
        {
            Device = device;
            semaphore = new SemaphoreSlim(1, 1);
            stopwatch = new Stopwatch();
            ReadBlockSettings = new List<ReadBlockSetting>();
            ReadBlockString = new List<string>() { "", "", "", "" };
            refreshTask = Task.Factory.StartNew(Refresh, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public IDeviceCore Device { get; private set; }
        private ModbusTCPClient mbClient;
        readonly Task refreshTask;
        readonly SemaphoreSlim semaphore;
        readonly Stopwatch stopwatch;

        /// <summary>
        /// Bit xác định đối tượng này đã bị dispose hay chưa
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Chu kì quét đọc giá trị của Device
        /// </summary>
        public int ScanRate
        {
            get
            {
                if (Device != null && Device.ParameterContainer != null && Device.ParameterContainer.Parameters.ContainsKey("ScanRate"))
                    return int.Parse(Device.ParameterContainer.Parameters["ScanRate"].ToString());
                return 1000;
            }
        }

        public byte UnitId
        {
            get
            {
                if (Device != null && Device.ParameterContainer != null && Device.ParameterContainer.Parameters.ContainsKey("UnitId"))
                    return (byte)Device.ParameterContainer.Parameters["UnitId"];
                return 0;
            }
        }

        public string IpAddress { get; set; }
        
        public ushort Port { get; set; }

        public int TryCount { get; private set; }

        /// <summary>
        /// Thời gian trễ giữa 2 lần request tới thiết bị thực
        /// </summary>
        public int DelayBetweenPool
        {
            get
            {
                if (Device != null)
                {
                    if (Device.ParameterContainer != null && Device.ParameterContainer.Parameters != null)
                    {
                        if (Device.ParameterContainer.Parameters.ContainsKey("DelayBetweenPool"))
                        {
                            if (int.TryParse(Device.ParameterContainer.Parameters["DelayBetweenPool"].ToString(), out int delay))
                                return delay;
                        }
                    }
                }
                return 10;
            }
        }

        /// <summary>
        /// ReadBlockSetting của Device
        /// </summary>
        public List<ReadBlockSetting> ReadBlockSettings { get; set; }

        /// <summary>
        /// Chuổi cấu hình ReadBlockSetting của Device
        /// </summary>
        public List<string> ReadBlockString { get; set; }

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

                    if (Device != null && Device.ParameterContainer != null)
                    {
                        if (Device.ParameterContainer.Parameters.ContainsKey("IpAddress"))
                        {
                            if (Device.Parent is IChannelCore channel)
                            {
                                if (channel != null && channel.ParameterContainer != null)
                                {
                                    if (channel.ParameterContainer.Parameters.ContainsKey("Port"))
                                    {
                                        string ipAddress = (string)Device.ParameterContainer.Parameters["IpAddress"];
                                        ushort port = Convert.ToUInt16(channel.ParameterContainer.Parameters["Port"].ToString());
                                        if (Port != port || IpAddress != ipAddress)
                                        {
                                            semaphore.Wait();
                                            try
                                            {
                                                IpAddress = ipAddress;
                                                Port = port;
                                                if (mbClient != null)
                                                    mbClient.Dispose();
                                                mbClient = new ModbusTCPClient();
                                                mbClient.Connect(ipAddress, port, false);
                                            }
                                            catch { }
                                            finally { semaphore.Release(); }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (mbClient != null)
                    {
                        try
                        {
                            semaphore.Wait();
                            if (!mbClient.Connected)
                                mbClient.Connect(IpAddress, Port, false);
                        }
                        catch { }
                        finally { semaphore.Release(); }

                        try
                        {
                            if (mbClient.IsSocketNull)
                                InitMBClient();
                        }
                        catch { }

                        if (mbClient.Connected)
                        {
                            byte deviceId = byte.Parse(Device.ParameterContainer.Parameters["UnitId"].ToString());
                            ByteOrder byteOrder = Device.ByteOrder;
                            // Lấy số lần thử đọc tối đa nếu đọc không thành công
                            int maxTryTimes = int.Parse(Device.ParameterContainer.Parameters["TryReadWriteTimes"].ToString());
                            // Thời gian timeout của việc đọc ghi
                            int timeout = int.Parse(Device.ParameterContainer.Parameters["Timeout"].ToString());

                            // Set lại thời gian timeout
                            mbClient.SetTimeout(timeout);

                            #region READ BLOCK FIRST

                            InitReadBlockSetting(Device, "ReadInputContactsBlockSetting", AddressType.InputContact);
                            InitReadBlockSetting(Device, "ReadOutputCoilsBlockSetting", AddressType.OutputCoil);
                            InitReadBlockSetting(Device, "ReadInputRegistersBlockSetting", AddressType.InputRegister);
                            InitReadBlockSetting(Device, "ReadHoldingRegistersBlockSetting", AddressType.HoldingRegister);

                            foreach (ReadBlockSetting block in ReadBlockSettings)
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

                            foreach (var childDevice in Device.Childs.ToArray())
                            {
                                if (!(childDevice is ITagCore tag))
                                    continue;

                                if (mbClient.IsSocketNull)
                                    break;

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
                                        foreach (var block in ReadBlockSettings)
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
                                                            tag.Value = tag.DataType.ConvertToValue(inputRegs, tag.Gain, tag.Offset, 0, 0, Device.ByteOrder);
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
                                                            tag.Value = tag.DataType.ConvertToValue(holdingRegs, tag.Gain, tag.Offset, 0, 0, Device.ByteOrder);
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

                            if (mbClient.IsSocketNull)
                            {
                                SetAllTagBad();
                            }
                        }

                        if (mbClient.IsSocketNull)
                        {
                            SetAllTagBad();
                        }
                    }
                }
                catch { }
                finally
                {
                    stopwatch.Stop();
                    //Debug.WriteLine($"Scan interval = {stopwatch.ElapsedMilliseconds}");
                    int delay = (int)(ScanRate - stopwatch.ElapsedMilliseconds);
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
                if (mbClient.IsSocketNull)
                    InitMBClient();

                if (mbClient == null || mbClient.IsSocketNull)
                    return false;

                semaphore.Wait();
                byte[] result = null;
                switch (addressType)
                {
                    case AddressType.InputContact:
                        mbClient.ReadDiscreteInputs(deviceId, deviceId, offset, count, ref result);
                        break;
                    case AddressType.OutputCoil:
                        mbClient.ReadCoils(deviceId, deviceId, offset, count, ref result);
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
                if (mbClient.IsSocketNull)
                    InitMBClient();

                if (mbClient == null || mbClient.IsSocketNull)
                {
                    return false;
                }

                semaphore.Wait();
                byte[] result = null;
                switch (addressType)
                {
                    case AddressType.InputRegister:
                        mbClient.ReadInputRegister(deviceId, deviceId, offset, count, ref result);
                        break;
                    case AddressType.HoldingRegister:
                        mbClient.ReadHoldingRegister(deviceId, deviceId, offset, count, ref result);
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
            catch { return false; }
            finally { Thread.Sleep(DelayBetweenPool); semaphore.Release(); }
        }

        /// <summary>
        /// Hàm thực hiện việc ghi giá trị
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Quality Write(ITagCore tag, object value)
        {
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
                                // Lấy thông tin của device như UnitId, MaxReadWriteTimes, Timeout...
                                byte unitId = byte.Parse(device.ParameterContainer.Parameters["UnitId"].ToString());
                                int maxTryTimes = int.Parse(device.ParameterContainer.Parameters["TryReadWriteTimes"].ToString());
                                int timeout = int.Parse(device.ParameterContainer.Parameters["Timeout"].ToString());

                                // Cài timeout cho serial port
                                mbClient.SetTimeout(timeout);

                                if (mbClient.IsSocketNull)
                                    InitMBClient();

                                // Kiểm tra xem serial port đã mở mới cho phép ghi
                                if (mbClient.Connected)
                                {
                                    // Khởi tạo biến thể hiện kết quả của việc ghi
                                    bool writeResult = false;
                                    semaphore.Wait();
                                    // Lặp việc ghi cho đến khi thành công hoặc đã vượt quá số lần cho phép ghi
                                    try
                                    {
                                        for (int i = 0; i < maxTryTimes; i++)
                                        {
                                            if (mbClient.IsSocketNull || !mbClient.Connected)
                                                break;
                                            try
                                            {
                                                if ((AddressType)tag.ParameterContainer.Parameters["AddressType"] == AddressType.OutputCoil)
                                                {
                                                    byte[] result = null;
                                                    bool bitResult = ByteHelper.GetBitAt(writeBuffer, 0, 0);
                                                    mbClient.WriteSingleCoils(
                                                        unitId,
                                                        unitId,
                                                        (ushort)tag.ParameterContainer.Parameters["Offset"],
                                                        bitResult,
                                                        ref result);
                                                    writeResult = result != null;
                                                }
                                                else if ((AddressType)tag.ParameterContainer.Parameters["AddressType"] == AddressType.HoldingRegister)
                                                {
                                                    byte[] result = null;
                                                    bool bitResult = ByteHelper.GetBitAt(writeBuffer, 0, 0);
                                                    mbClient.WriteMultipleRegister(
                                                        unitId,
                                                        unitId,
                                                        (ushort)tag.ParameterContainer.Parameters["Offset"],
                                                        writeBuffer,
                                                        ref result);
                                                    writeResult = result != null;
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
                catch { return Quality.Bad; }
            }
            return Quality.Bad;
        }

        /// <summary>
        /// Hàm set tất cả các tag trong device về trạng thái bad
        /// </summary>
        private void SetAllTagBad()
        {
            foreach (var childDevice in Device.Childs.ToArray())
            {
                if (childDevice is ITagCore tag)
                {
                    tag.TimeStamp = DateTime.Now;
                    tag.Quality = Quality.Bad;
                    tag.RefreshInterval = (int)(DateTime.Now - tag.TimeStamp).TotalMilliseconds;
                }
            }
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

                if (ReadBlockString[index] != readBlockStr)
                {
                    foreach (var readBlock in ReadBlockSettings.Where(x => x.AddressType == addressType))
                    {
                        ReadBlockSettings.Remove(readBlock);
                    }

                    ReadBlockString[index] = readBlockStr;
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
                                    ReadBlockSettings.Add(newBlock);
                            }
                        }
                    }
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
                semaphore.Wait();
                try
                {
                    if (Device != null && Device.ParameterContainer != null)
                    {
                        if (Device.ParameterContainer.Parameters.ContainsKey("IpAddress"))
                        {
                            if (Device.Parent is IChannelCore channel)
                            {
                                if (channel != null && channel.ParameterContainer != null)
                                {
                                    if (channel.ParameterContainer.Parameters.ContainsKey("Port"))
                                    {
                                        if (mbClient != null)
                                            mbClient.Dispose();
                                        string ipAddress = (string)Device.ParameterContainer.Parameters["IpAddress"];
                                        ushort port = Convert.ToUInt16(channel.ParameterContainer.Parameters["Port"].ToString());
                                        IpAddress = ipAddress;
                                        Port = port;
                                        mbClient = new ModbusTCPClient();
                                        mbClient.Connect(ipAddress, port, false);
                                    }
                                }
                            }
                        }
                    }
                }
                catch { }
                finally { semaphore.Release(); }
            }
        }

        /// <summary>
        /// Hủy đối tượng này
        /// </summary>
        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
