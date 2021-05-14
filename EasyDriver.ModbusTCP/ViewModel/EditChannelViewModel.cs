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
    public class EditChannelViewModel : IDataErrorInfo
    {
        #region Public properties
        public virtual Channel Channel { get; set; }
        public virtual ModbusTCPDriver Driver { get; set; }
        public virtual IGroupItem Parent { get => Channel.Parent; }
        public virtual List<string> ComPortSource { get; set; }

        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual string IpAddress { get; set; } = "192.168.1.1";
        public virtual string Port { get; set; } = "502";
        public virtual string ScanRate { get; set; } = "1000";
        public virtual string DelayBetweenPool { get; set; } = "5";
        #endregion

        #region Services
        protected ICurrentWindowService CurrentWindowService { get => this.GetService<ICurrentWindowService>(); }
        protected IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }
        #endregion

        #region Constructors
        public EditChannelViewModel(ModbusTCPDriver driver, IChannelCore channelCore)
        {
            Channel = channelCore as Channel;
            Driver = driver;

            if (channelCore is Channel channel)
            {
                Channel = channel;
                if (Channel != null)
                {
                    Description = Channel.Description;
                    Name = Channel.Name;
                    IpAddress = Channel.IpAddress;
                    Port = Channel.Port.ToString();
                    ScanRate = Channel.ScanRate.ToString();
                    DelayBetweenPool = Channel.DelayBetweenPool.ToString();
                }
            } 
        }
        #endregion

        #region Commands

        public void Save()
        {
            Channel.Name = Name?.Trim();
            Channel.Description = Description?.Trim();
            Channel.IpAddress = IpAddress;
            Channel.Port = ushort.Parse(Port);
            Channel.ScanRate = int.Parse(ScanRate);
            Channel.DelayBetweenPool = int.Parse(DelayBetweenPool);

            CurrentWindowService.Close();
        }
        public bool CanSave()
        {
            return Channel != null && string.IsNullOrWhiteSpace(Error);
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
                    case nameof(Name):
                        Error = Name?.Trim().ValidateFileName("Channel");
                        if (string.IsNullOrEmpty(Error))
                        {
                            if (Channel.Parent.Childs.Any(x => x != Channel && (x as ICoreItem).Name == Name?.Trim()) )
                                Error = $"The channel name '{Name}' is already in use.";
                        }
                        break;
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
