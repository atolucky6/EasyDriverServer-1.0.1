using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyDriver.DPA870
{
    public class DPA870Driver : IEasyDriverPlugin
    {
        #region Static

        /// <summary>
        /// Danh sách các kiểu dữ liệu mà driver này hỗ trợ
        /// </summary>
        static readonly List<IDataType> supportDataTypes;

        /// <summary>
        /// Static constructor khởi tạo 1 lần khi load driver
        /// </summary>
        static DPA870Driver()
        {
            // Khởi tạo các kiểu dữ liệu
            supportDataTypes = new List<IDataType>
            {
                new Real() { Name = "Float" },
            };
        }

        #endregion

        /// <summary>
        /// Channel chạy driver này
        /// </summary>
        public IChannelCore Channel { get; set; }

        /// <summary>
        /// Bit xác định driver này đã bị dispose
        /// </summary>
        public bool IsDisposed { get; private set; }

        private Serial serial = new Serial();
        private Task refreshTask;
        readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private string receivedLine;

        public event EventHandler Disposed;
        public event EventHandler Refreshed;

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
                serial.DataReceived += Serial_DataReceived;
                return serial.Open();
            }
            catch { return false; }
            finally
            {
                // Giải phóng semaphore
                semaphore.Release();
            }
        }

        private void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                byte[] buffer = new byte[serial.SerialPort.BytesToRead];
                serial.SerialPort.Read(buffer, 0, buffer.Length);

                receivedLine = Encoding.ASCII.GetString(buffer);
                //receivedLine = ASCIIEncoding.UTF8.GetString(buffer);
                //receivedLine = ASCIIEncoding.Default.GetString(buffer);
                //receivedLine = ASCIIEncoding.UTF7.GetString(buffer);
                //receivedLine = ASCIIEncoding.UTF32.GetString(buffer);
                //receivedLine = Encoding.GetEncoding(1252).GetString(buffer);

                //foreach (var item in Encoding.GetEncodings())
                //{
                //    var encode = item.GetEncoding();
                //    var res = encode.GetString(buffer);
                //    Debug.WriteLine($"{encode.CodePage} - {encode.HeaderName}: {res}");
                //}

                if (!string.IsNullOrEmpty(receivedLine))
                {
                    string[] splitData = receivedLine.Split(' ');
                    if (splitData.Length >= 2)
                    {
                        string radiationSTR = splitData[0]?.Replace(';', ' ')?.Trim();
                        string tempSTR = splitData[1]?.Replace(';', ' ')?.Trim();
                        var tags = Channel.GetAllTags()?.ToArray();

                        if (tags != null && tags.Length >= 2)
                        {
                            if (float.TryParse(radiationSTR, out float radiation))
                            {
                                tags[0].Value = radiation.ToString();
                                tags[0].Quality = Quality.Good;
                                tags[0].TimeStamp = DateTime.Now;
                            }
                            else
                            {
                                tags[0].Quality = Quality.Bad;
                                tags[0].TimeStamp = DateTime.Now;
                            }

                            if (float.TryParse(tempSTR, out float temp))
                            {
                                tags[1].Value = temp.ToString();
                                tags[1].Quality = Quality.Good;
                                tags[1].TimeStamp = DateTime.Now;
                            }
                            else
                            {
                                tags[1].Quality = Quality.Bad;
                                tags[1].TimeStamp = DateTime.Now;
                            }
                        }
                    }
                }

            }
            catch
            {
                var tags = Channel.GetAllTags()?.ToArray();
                if (tags != null)
                {
                    foreach (var tag in tags)
                    {
                        tag.Quality = Quality.Bad;
                        tag.TimeStamp = DateTime.Now;
                    }
                }
            }
            finally
            {
                Refreshed?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Hàm khởi tạo cổng serial
        /// </summary>
        private void InitializeSerialPort()
        {
            if (Channel.ParameterContainer.Parameters.Count >= 5)
            {
                var port = Channel.ParameterContainer.Parameters["Port"].ToString();
                if (int.TryParse(Channel.ParameterContainer.Parameters["Baudrate"], out int baudRate) &&
                    int.TryParse(Channel.ParameterContainer.Parameters["DataBits"], out int dataBits) &&
                    Enum.TryParse(Channel.ParameterContainer.Parameters["StopBits"], out StopBits stopBits) &&
                    Enum.TryParse(Channel.ParameterContainer.Parameters["Parity"], out Parity parity))
                {
                    serial.Init(port, baudRate, dataBits, parity, stopBits);
                }
            }
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
                semaphore.Wait();
                serial.DataReceived -= Serial_DataReceived;
                return serial.Close();
            }
            catch { return false; }
            finally { semaphore.Release(); }
        }

        public void Dispose()
        {
            IsDisposed = true;
            Disposed?.Invoke(this, EventArgs.Empty);
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
            return Quality.Bad;
        }
    }
}
