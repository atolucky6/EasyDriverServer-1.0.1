using System;
using System.Collections.Generic;
using EasyDriverPlugin;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Specialized;

namespace EasyDriver.ModbusTCP
{
    /// <summary>
    /// Driver thực hiện việc đọc ghi giá trị
    /// </summary>
    public class ModbusTCPDriver : IEasyDriverPlugin
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
        public ModbusTCPDriver()
        {
            DeviceReaders = new List<ModbusDeviceReader>();
        }

        #endregion

        #region Members

        /// <summary>
        /// Channel chạy driver này
        /// </summary>
        public IChannelCore Channel { get; set; }

        public bool IsConnected { get; private set; }

        internal List<ModbusDeviceReader> DeviceReaders { get; set; }

        /// <summary>
        /// Bit xác định driver này đã bị dispose
        /// </summary>
        public bool IsDisposed { get; private set; }

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
            try
            {
                if (IsConnected)
                    return true;

                if (Channel != null)
                {
                    IsConnected = true;
                    Channel.Childs.CollectionChanged += OnChildCollectionChanged;
                    foreach (var item in Channel.Childs)
                    {
                        if (item is IDeviceCore device)
                            DeviceReaders.Add(new ModbusDeviceReader(device));
                    }
                    return true;
                }
            }
            catch {  }
            return false;
        }

        private void OnChildCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (var item in e.NewItems)
                    {
                        if (item is IDeviceCore device)
                            DeviceReaders.Add(new ModbusDeviceReader(device));
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (var item in e.OldItems)
                    {
                        if (item is IDeviceCore device)
                        {
                            ModbusDeviceReader deviceReader = DeviceReaders.FirstOrDefault(x => x.Device == device);
                            if (deviceReader != null)
                            {
                                DeviceReaders.Remove(deviceReader);
                                deviceReader.Dispose();
                            }
                        }
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Replace)
                {
                    foreach (var item in e.OldItems)
                    {
                        if (item is IDeviceCore device)
                        {
                            ModbusDeviceReader deviceReader = DeviceReaders.FirstOrDefault(x => x.Device == device);
                            if (deviceReader != null)
                            {
                                DeviceReaders.Remove(deviceReader);
                                deviceReader.Dispose();
                            }
                        }
                    }

                    foreach (var item in e.NewItems)
                    {
                        if (item is IDeviceCore device)
                            DeviceReaders.Add(new ModbusDeviceReader(device));
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Hàm ngắt kết nối
        /// </summary>
        /// <returns></returns>
        public bool Disconnect()
        {
            try
            {
                if (!IsConnected || Channel == null)
                    return true;

                if (IsConnected)
                {
                    Channel.Childs.CollectionChanged -= OnChildCollectionChanged;
                    foreach (var deviceReader in DeviceReaders)
                        deviceReader.Dispose();
                }
            }
            catch {  }
            return false;
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
                    if (tag.Parent is IDeviceCore device)
                    {
                        ModbusDeviceReader deviceReader = DeviceReaders.FirstOrDefault(x => x.Device == device);
                        if (deviceReader != null)
                        {
                            return deviceReader.Write(tag, value);
                        }
                    }
                }
                catch { return Quality.Bad; } // Khi có lỗi thì trả về 'Bad'
            }
            return Quality.Bad;
        }

        public async void NotifyPortChanged()
        {
            await Task.Run(() =>
            {
                foreach (var deviceReader in DeviceReaders)
                {
                    try
                    {
                        deviceReader.InitMBClient();
                    }
                    catch { }
                }
            }); 
        }

        public void Dispose()
        {
            IsDisposed = true;
            Disconnect();
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
