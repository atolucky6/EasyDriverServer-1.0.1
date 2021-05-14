using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyDriver.ModbusTCP
{
    public class AddChannelViewModel : IDataErrorInfo
    {
        #region Public properties
        public virtual Channel Channel { get; set; }
        public virtual ModbusTCPDriver Driver { get; set; }
        public virtual IGroupItem Parent { get; set; }
        public virtual string IpAddress { get; set; } = "192.168.1.1";
        public virtual string Port { get; set; } = "502";
        public virtual string Description { get; set; }
        public virtual string ScanRate { get; set; } = "1000";
        public virtual string DelayBetweenPool { get; set; } = "5";
        #endregion

        #region Services
        protected ICurrentWindowService CurrentWindowService { get => this.GetService<ICurrentWindowService>(); }
        protected IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }
        #endregion

        #region Constructors
        public AddChannelViewModel(ModbusTCPDriver driver, IGroupItem parent, IChannelCore itemTemplate)
        {
            Driver = driver;
            Parent = parent;

            if (itemTemplate is Channel channel)
            {
                Description = itemTemplate.Description;
                IpAddress = channel.IpAddress;
                Port = channel.Port.ToString();
                DelayBetweenPool = channel.DelayBetweenPool.ToString();
                ScanRate = channel.ScanRate.ToString();
            }
        }
        #endregion

        #region Commands
        public void Save()
        {
            Channel = new Channel(Parent)
            {
                Description = Description,
                IpAddress = IpAddress,
                Port = ushort.Parse(Port),
                DelayBetweenPool = int.Parse(DelayBetweenPool),
                ScanRate = int.Parse(ScanRate),
            };
            CurrentWindowService.Close();
        }
        public bool CanSave()
        {
            return string.IsNullOrWhiteSpace(Error);
        }

        public void Cancel()
        {
            CurrentWindowService.Close();
        }
        #endregion

        #region Methods
        #endregion

        #region IDataErrorInfo
        public string Error { get; set; }

        public string this[string columnName]
        {
            get
            {
                Error = string.Empty;
                switch (columnName)
                {
                    case nameof(Port):
                        if (int.TryParse(Port, out int tryRead))
                        {
                            if (!tryRead.IsInRange(1, 65535))
                                Error = $"Value is out of range.";
                        }
                        else
                        {
                            Error = $"Value must be a number.";
                        }
                        break;
                    case nameof(IpAddress):
                        if (!IpAddress.IsIpAddress())
                            Error = "Ip address was not in correct format";
                        break;
                    case nameof(ScanRate):
                        if (int.TryParse(ScanRate, out int scanRate))
                        {
                            if (!scanRate.IsInRange(0, int.MaxValue))
                                Error = $"Value is out of range.";
                        }
                        else
                        {
                            Error = $"Value must be a number.";
                        }
                        break;
                    case nameof(DelayBetweenPool):
                        if (int.TryParse(DelayBetweenPool, out int delay))
                        {
                            if (!delay.IsInRange(0, 100))
                                Error = $"Value is out of range.";
                        }
                        else
                        {
                            Error = $"Value must be a number.";
                        }
                        break;
                    default:
                        break;
                }
                return Error;
            }
        }
        #endregion
    }
}
