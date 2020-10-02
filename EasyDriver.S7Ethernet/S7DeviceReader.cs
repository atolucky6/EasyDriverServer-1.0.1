using EasyDriverPlugin;
using Sharp7;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyDriver.S7Ethernet
{
    class S7DeviceReader : IDisposable
    {
        public S7DeviceReader(IDeviceCore device)
        {
            Device = device;
            semaphore = new SemaphoreSlim(1, 1);
            stopwatch = new Stopwatch();
            refreshTask = Task.Factory.StartNew(Refresh, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public IDeviceCore Device { get; set; }
        private S7Client s7Client;
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

        public byte Rack
        {
            get
            {
                if (Device != null && Device.ParameterContainer != null && Device.ParameterContainer.Parameters.ContainsKey("Rack"))
                    return byte.Parse(Device.ParameterContainer.Parameters["Rack"]);
                return 0;
            }
        }

        public byte Slot
        {
            get
            {
                if (Device != null && Device.ParameterContainer != null && Device.ParameterContainer.Parameters.ContainsKey("Slot"))
                    return byte.Parse(Device.ParameterContainer.Parameters["Slot"]);
                return 0;
            }
        }

        public ushort Port
        {
            get
            {
                if (Device != null && Device.ParameterContainer != null && Device.ParameterContainer.Parameters.ContainsKey("Port"))
                    return byte.Parse(Device.ParameterContainer.Parameters["Port"]);
                return 0;
            }
        }

        public string IpAddress { get; set; }

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

        private void Refresh()
        {
            while (!IsDisposed)
            {
                try
                {
                    // Khởi động lại đồng hồ
                    stopwatch.Restart();

                    if (Device != null && Device.ParameterContainer != null)
                    {
                        if (Device.ParameterContainer.Parameters.ContainsKey("IpAddress") &&
                            Device.ParameterContainer.Parameters.ContainsKey("Rack") &&
                            Device.ParameterContainer.Parameters.ContainsKey("Slot") &&
                            Device.ParameterContainer.Parameters.ContainsKey("Port"))
                        {
                            string ipAddress = Device.ParameterContainer.Parameters["IpAddress"];
                            if (IpAddress != ipAddress)
                            {
                                semaphore.Wait();
                                try
                                {
                                    IpAddress = ipAddress;
                                    if (s7Client != null)
                                        s7Client.Disconnect();
                                    s7Client = new S7Client();
                                    s7Client.PLCPort = Port;
                                    s7Client.ConnectTo(IpAddress, Rack, Slot);
                                }
                                catch { }
                                finally { semaphore.Release(); }
                            }
                        }
                    }

                    if (s7Client != null)
                    {
                        try
                        {
                            semaphore.Wait();
                            if (!s7Client.Connected)
                            {
                                s7Client.PLCPort = Port;
                                s7Client.ConnectTo(IpAddress, Rack, Slot);
                            }

                            if (s7Client.Connected)
                            {
                                ByteOrder byteOrder = Device.ByteOrder;
                                // Lấy số lần thử đọc tối đa nếu đọc không thành công
                                int maxTryTimes = int.Parse(Device.ParameterContainer.Parameters["TryReadWriteTimes"].ToString());
                                // Thời gian timeout của việc đọc ghi
                                int timeout = int.Parse(Device.ParameterContainer.Parameters["Timeout"].ToString());

                                s7Client.SendTimeout = timeout;
                                s7Client.RecvTimeout = timeout;

                                foreach (var childDevice in Device.GetAllTags().ToArray())
                                {
                                    if (!(childDevice is ITagCore tag))
                                        continue;

                                    bool isTagValid = string.IsNullOrEmpty(tag.Address.IsValidAddress(out bool isBitAddress, out int byteOffset, out int bitOffset, out int dbNumber, out AddressType addressType));

                                    if (!tag.ParameterContainer.Parameters.ContainsKey("LastAddress"))
                                        tag.ParameterContainer.Parameters["LastAddress"] = tag.Address;

                                    if (!tag.ParameterContainer.Parameters.ContainsKey("AddressType") ||
                                        tag.Address != tag.ParameterContainer.Parameters["LastAddress"].ToString())
                                    {
                                        if (isTagValid)
                                        {
                                            tag.ParameterContainer.Parameters["AddressType"] = addressType.ToString();
                                            tag.ParameterContainer.Parameters["ByteOffset"] = byteOffset.ToString();
                                            tag.ParameterContainer.Parameters["bitOffset"] = bitOffset.ToString();
                                            tag.ParameterContainer.Parameters["IsValid"] = bool.TrueString;
                                            tag.ParameterContainer.Parameters["LastAddress"] = tag.Address;
                                            tag.ParameterContainer.Parameters["IsBitAddress"] = isBitAddress.ToString();
                                        }
                                        else
                                        {
                                            tag.ParameterContainer.Parameters["IsValid"] = bool.FalseString;
                                        }
                                    }

                                    if (tag.ParameterContainer.Parameters["IsValid"] == bool.TrueString &&
                                        (DateTime.Now - tag.TimeStamp).TotalMilliseconds >= tag.RefreshRate)
                                    {
                                        bool readSuccess = false;
                                        bool containInBlock = false;

                                        if (!containInBlock)
                                        {
                                            int amount = tag.DataType.RequireByteLength;
                                            byte[] buffer = new byte[amount];
                                            int result = -1;
                                            switch (addressType)
                                            {
                                                case AddressType.Datablock:
                                                    result = s7Client.ReadArea(S7Consts.S7AreaDB, dbNumber, byteOffset, amount, S7Consts.S7WLByte, buffer);
                                                    break;
                                                case AddressType.Input:
                                                    result = s7Client.ReadArea(S7Consts.S7AreaPE, dbNumber, byteOffset, amount, S7Consts.S7WLByte, buffer);
                                                    break;
                                                case AddressType.Output:
                                                    result = s7Client.ReadArea(S7Consts.S7AreaPA, dbNumber, byteOffset, amount, S7Consts.S7WLByte, buffer);
                                                    break;
                                                case AddressType.Marker:
                                                    result = s7Client.ReadArea(S7Consts.S7AreaMK, dbNumber, byteOffset, amount, S7Consts.S7WLByte, buffer);
                                                    break;
                                                default:
                                                    break;
                                            }

                                            if (result == 0)
                                            {
                                                readSuccess = true;
                                                tag.Value = tag.DataType.ConvertToValue(buffer, tag.Gain, tag.Offset, 0, bitOffset, byteOrder);
                                            }
                                            else
                                            {
                                                Debug.WriteLine(s7Client.ErrorText(result));
                                            }
                                        }

                                        tag.Quality = readSuccess ? Quality.Good : Quality.Bad;
                                        tag.TimeStamp = DateTime.Now;
                                    }
                                    else
                                    {
                                        tag.TimeStamp = DateTime.Now;
                                        tag.Quality = Quality.Bad;
                                        tag.RefreshInterval = (int)(DateTime.Now - tag.TimeStamp).TotalMilliseconds;
                                    }
                                }
                            }
                            else
                            {
                                SetAllTagBad();
                            }
                        }
                        catch { }
                        finally { semaphore.Release(); }
                    }
                }
                catch { }
                finally
                {
                    stopwatch.Stop();
                    int delay = (int)(ScanRate - stopwatch.ElapsedMilliseconds);
                    if (delay < 1)
                        delay = 1;
                    Thread.Sleep(delay);
                }
            }
            s7Client?.Disconnect();
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
                        tag.FindParent<IDeviceCore>(x => x is IDeviceCore) is IDeviceCore device && tag.AccessPermission == AccessPermission.ReadAndWrite)
                    {
                        // Lấy thông tin của địa chỉ Tag như kiểu địa chỉ và vị trí bắt đầu của thanh ghi
                        if (!tag.ParameterContainer.Parameters.ContainsKey("LastAddress"))
                            tag.ParameterContainer.Parameters["LastAddress"] = tag.Address;

                        bool isTagValid = string.IsNullOrEmpty(tag.Address.IsValidAddress(out bool isBitAddress, out int byteOffset, out int bitOffset, out int dbNumber, out AddressType addressType));

                        if (!tag.ParameterContainer.Parameters.ContainsKey("AddressType") ||
                            tag.Address != tag.ParameterContainer.Parameters["LastAddress"].ToString())
                        {
                            if (isTagValid)
                            {
                                tag.ParameterContainer.Parameters["AddressType"] = addressType.ToString();
                                tag.ParameterContainer.Parameters["ByteOffset"] = byteOffset.ToString();
                                tag.ParameterContainer.Parameters["bitOffset"] = bitOffset.ToString();
                                tag.ParameterContainer.Parameters["IsValid"] = bool.TrueString;
                                tag.ParameterContainer.Parameters["LastAddress"] = tag.Address;
                                tag.ParameterContainer.Parameters["IsBitAddress"] = isBitAddress.ToString();
                            }
                            else
                            {
                                tag.ParameterContainer.Parameters["IsValid"] = bool.FalseString;
                            }
                        }
                        if (tag.ParameterContainer.Parameters["IsValid"] == bool.TrueString)
                        {
                            // Lấy thông tin byte order của device parent
                            ByteOrder byteOrder = device.ByteOrder;
                            // Chuyển đổi giá trị cần ghi thành chuỗi byte nếu thành công thì mới ghi
                            if (tag.DataType.TryParseToByteArray(value, tag.Gain, tag.Offset, out byte[] writeBuffer, byteOrder))
                            {
                                int maxTryTimes = int.Parse(device.ParameterContainer.Parameters["TryReadWriteTimes"].ToString());
                                int timeout = int.Parse(device.ParameterContainer.Parameters["Timeout"].ToString());

                                s7Client.SendTimeout = timeout;
                                s7Client.RecvTimeout = timeout;

                                if (s7Client == null)
                                    InitS7Client();

                                // Kiểm tra xem serial port đã mở mới cho phép ghi
                                if (s7Client.Connected)
                                {
                                    // Khởi tạo biến thể hiện kết quả của việc ghi
                                    bool writeResult = false;
                                    semaphore.Wait();
                                    // Lặp việc ghi cho đến khi thành công hoặc đã vượt quá số lần cho phép ghi
                                    try
                                    {
                                        for (int i = 0; i < maxTryTimes; i++)
                                        {
                                            if (!s7Client.Connected)
                                                break;
                                            try
                                            {
                                                int amount = tag.DataType.RequireByteLength;
                                                tag.DataType.TryParseToByteArray(value, tag.Gain, tag.Offset, out byte[] buffer, byteOrder);
                                                int start = tag.DataType.BitLength == 1 ? byteOffset * 8 : byteOffset;
                                                int wordLen = tag.DataType.BitLength == 1 ? S7Consts.S7WLBit : S7Consts.S7WLByte;
                                                int result = -1;
                                                if (buffer.Length == amount)
                                                {
                                                    switch (addressType)
                                                    {
                                                        case AddressType.Datablock:
                                                            result = s7Client.WriteArea(S7Consts.S7AreaDB, dbNumber, start, amount, wordLen, buffer);
                                                            break;
                                                        case AddressType.Input:
                                                            result = s7Client.WriteArea(S7Consts.S7AreaPE, dbNumber, start, amount, wordLen, buffer);
                                                            break;
                                                        case AddressType.Output:
                                                            result = s7Client.WriteArea(S7Consts.S7AreaPA, dbNumber, start, amount, wordLen, buffer);
                                                            break;
                                                        case AddressType.Marker:
                                                            result = s7Client.WriteArea(S7Consts.S7AreaMK, dbNumber, start, amount, wordLen, buffer);
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                }

                                                if (result == 0)
                                                    return Quality.Good;

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
            foreach (var childDevice in Device.GetAllTags().ToArray())
            {
                if (childDevice is ITagCore tag)
                {
                    tag.TimeStamp = DateTime.Now;
                    tag.Quality = Quality.Bad;
                    tag.RefreshInterval = (int)(DateTime.Now - tag.TimeStamp).TotalMilliseconds;
                }
            }
        }

        public void InitS7Client()
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
                            if (Device.FindParent<IChannelCore>(x => x is IChannelCore) is IChannelCore channel)
                            {
                                if (channel != null && channel.ParameterContainer != null)
                                {
                                    if (channel.ParameterContainer.Parameters.ContainsKey("Port"))
                                    {
                                        if (s7Client != null)
                                            s7Client.Disconnect();
                                        string ipAddress = (string)Device.ParameterContainer.Parameters["IpAddress"];
                                        IpAddress = ipAddress;
                                        s7Client = new S7Client();
                                        s7Client.PLCPort = Port;
                                        s7Client.ConnectTo(ipAddress, Rack, Slot);
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

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
