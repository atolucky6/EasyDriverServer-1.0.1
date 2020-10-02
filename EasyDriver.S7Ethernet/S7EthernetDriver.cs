using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDriver.S7Ethernet
{
    public class S7EthernetDriver : IEasyDriverPlugin
    {
        #region Static

        /// <summary>
        /// Danh sách các kiểu dữ liệu mà driver này hỗ trợ
        /// </summary>
        static readonly List<IDataType> supportDataTypes;

        /// <summary>
        /// Static constructor khởi tạo 1 lần khi load driver
        /// </summary>
        static S7EthernetDriver()
        {
            // Khởi tạo các kiểu dữ liệu
            supportDataTypes = new List<IDataType>
            {
                new Bool(),
                new EasyDriverPlugin.Byte(),
                new Word(),
                new DWord(),
                new LWord(),
                new SInt(),
                new Int(),
                new DInt(),
                new LInt(),
                new USInt(),
                new UInt(),
                new UDInt(),
                new ULInt(),
                new Real(),
                new LReal(),
                new EasyDriverPlugin.Char(),
                new EasyDriverPlugin.String()
            };
        }

        #endregion

        public S7EthernetDriver()
        {
            DeviceReaders = new List<S7DeviceReader>();
        }

        public IChannelCore Channel { get; set; }
        public bool IsConnected { get; private set; }
        internal List<S7DeviceReader> DeviceReaders { get; set; }
        /// <summary>
        /// Bit xác định driver này đã bị dispose
        /// </summary>
        public bool IsDisposed { get; private set; }
        public event EventHandler Disposed;
        public event EventHandler Refreshed;

        public bool Connect()
        {
            try
            {
                if (IsConnected)
                    return true;

                if (Channel != null)
                {
                    IsConnected = true;
                    foreach (var item in Channel.Childs)
                    {
                        if (item is IDeviceCore device)
                            DeviceReaders.Add(new S7DeviceReader(device));
                    }

                    Channel.Childs.CollectionChanged += OnChildCollectionChanged;
                    return true;
                }
            }
            catch { }
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
                            DeviceReaders.Add(new S7DeviceReader(device));
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (var item in e.OldItems)
                    {
                        if (item is IDeviceCore device)
                        {
                            S7DeviceReader deviceReader = DeviceReaders.FirstOrDefault(x => x.Device == device);
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
                            S7DeviceReader deviceReader = DeviceReaders.FirstOrDefault(x => x.Device == device);
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
                            DeviceReaders.Add(new S7DeviceReader(device));
                    }
                }
            }
            catch { }
        }


        public IDeviceCore ConverrtToDevice(IDeviceCore baseDevice)
        {
            return baseDevice;
        }

        public IChannelCore ConvertToChannel(IChannelCore baseChannel)
        {
            return baseChannel;
        }

        public ITagCore ConvertToTag(ITagCore tagCore)
        {
            return tagCore;
        }

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
            catch { }
            return false;
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
            return new CreateDeviceView(this, parent, templateItem);
        }

        public object GetCreateTagControl(IGroupItem parent, ITagCore templateItem = null)
        {
            return new CreateTagView(this, parent, templateItem);
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
                    if (tag.FindParent<IDeviceCore>(x => x is IDeviceCore) is IDeviceCore device)
                    {
                        S7DeviceReader deviceReader = DeviceReaders.FirstOrDefault(x => x.Device == device);
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
    }
}
