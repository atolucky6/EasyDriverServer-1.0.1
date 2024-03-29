﻿using DevExpress.Mvvm;
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

namespace EasyDriver.ModbusRTU
{
    public class EditChannelViewModel : IDataErrorInfo
    {
        #region Public properties
        public virtual Channel Channel { get; set; }
        public virtual ModbusRTUDriver Driver { get; set; }
        public virtual IGroupItem Parent { get => Channel.Parent; }
        public virtual List<string> ComPortSource { get; set; }

        public virtual string Name { get; set; }
        public virtual string Port { get; set; } = "COM1";
        public virtual string MaxWritesCount { get; set; } = "10";
        public virtual string ScanRate { get; set; } = "1000";
        public virtual int Baudrate { get; set; } = 9600;
        public virtual int DataBits { get; set; } = 8;
        public virtual Parity Parity { get; set; } = Parity.None;
        public virtual StopBits StopBits { get; set; } = StopBits.One;
        public virtual string DelayBetweenPool { get; set; } = "5";
        public virtual string Description { get; set; }
        #endregion

        #region Services
        protected ICurrentWindowService CurrentWindowService { get => this.GetService<ICurrentWindowService>(); }
        protected IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }
        #endregion

        #region Constructors
        public EditChannelViewModel(ModbusRTUDriver driver, IChannelCore channelCore)
        {
            Channel = channelCore as Channel;
            Driver = driver;
            ComPortSource = new List<string>(driver.ComPortSource);

            foreach (var item in Parent.Childs)
            {
                if (item is ISupportParameters supportParameters && item != Channel)
                {
                    foreach (var kvp in supportParameters.ParameterContainer.Parameters)
                    {
                        if (kvp.Value.StartsWith("COM"))
                        {
                            ComPortSource.Remove(kvp.Value);
                        }
                    }
                }
            }
            Port = ComPortSource.FirstOrDefault();

            if (channelCore is Channel channel)
            {
                Channel = channel;
                if (Channel != null)
                {
                    if (Channel.ParameterContainer.TryGetValue(nameof(Port), out string port))
                        Port = port;
                    if (Channel.ParameterContainer.TryGetValue(nameof(ScanRate), out int scanRate))
                        ScanRate = scanRate.ToString();
                    if (Channel.ParameterContainer.TryGetValue(nameof(Baudrate), out int baudRate))
                        Baudrate = baudRate;
                    if (Channel.ParameterContainer.TryGetValue(nameof(DataBits), out int dataBits))
                        DataBits = dataBits;
                    if (Channel.ParameterContainer.TryGetValue(nameof(Parity), out Parity parity))
                        Parity = parity;
                    if (Channel.ParameterContainer.TryGetValue(nameof(StopBits), out StopBits stopBits))
                        StopBits = stopBits;
                    if (Channel.ParameterContainer.TryGetValue(nameof(DelayBetweenPool), out int delay))
                        DelayBetweenPool = delay.ToString();
                    if (Channel.ParameterContainer.TryGetValue(nameof(MaxWritesCount), out int writeCount))
                        MaxWritesCount = writeCount.ToString();
                    Description = Channel.Description;
                    Name = Channel.Name;
                }
            } 
        }
        #endregion

        #region Commands

        public void Save()
        {
            if (CheckPortNotIsInUse(Port))
            {
                Channel.Port = Port;
                Channel.Baudrate = Baudrate;
                Channel.DataBits = DataBits;
                Channel.StopBits = StopBits;
                Channel.Parity = Parity;
                Channel.ScanRate = int.Parse(ScanRate);
                Channel.DelayBetweenPool = int.Parse(DelayBetweenPool);
                Channel.Name = Name?.Trim();
                Channel.Description = Description?.Trim();
                Channel.MaxWritesCount = int.Parse(MaxWritesCount);
                CurrentWindowService.Close();
            }
            else
            {
                MessageBoxService.ShowMessage($"{Port} is already in use.", "Easy Driver Server", MessageButton.OK, MessageIcon.Warning);
            }
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
        private bool CheckPortNotIsInUse(string portName)
        {
            try
            {
                if (Channel.Port == portName)
                    return true;

                using (SerialPort port = new SerialPort(portName, 9600, Parity.Even, 8, StopBits.One))
                {
                    port.Open();
                    Thread.Sleep(10);
                    port.Close();
                    return true;
                }
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (IOException)
            {
                foreach (var item in Parent.Childs)
                {
                    if (item is ISupportParameters supportParameters && item != Channel)
                    {
                        foreach (var kvp in supportParameters.ParameterContainer.Parameters)
                        {
                            if (kvp.Value == Port)
                                return false;
                        }
                    }
                }
                return true;
            }
        }
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
                        if (Port != Channel.Port)
                        {
                            foreach (var item in Parent.Childs)
                            {
                                if (item is ISupportParameters supportParameters)
                                {
                                    foreach (var kvp in supportParameters.ParameterContainer.Parameters)
                                    {
                                        if (kvp.Value == Port)
                                        {
                                            Error = $"`{Port}` is already in use.";
                                            return Error;
                                        }
                                    }
                                }
                            }                            
                        }
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
                    case nameof(MaxWritesCount):
                        if (int.TryParse(MaxWritesCount, out int maxWrite))
                        {
                            if (!maxWrite.IsInRange(1, 99))
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
