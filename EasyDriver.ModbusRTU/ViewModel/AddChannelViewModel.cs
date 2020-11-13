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
    public class AddChannelViewModel : IDataErrorInfo
    {
        #region Public properties
        public virtual Channel Channel { get; set; }
        public virtual ModbusRTUDriver Driver { get; set; }
        public virtual IGroupItem Parent { get; set; }
        public virtual List<string> ComPortSource { get; set; }

        public virtual string Port { get; set; } = "COM1";
        public virtual int ScanRate { get; set; } = 1000;
        public virtual int Baudrate { get; set; } = 9600;
        public virtual int DataBits { get; set; } = 8;
        public virtual Parity Parity { get; set; } = Parity.None;
        public virtual StopBits StopBits { get; set; } = StopBits.One;
        public virtual int DelayBetweenPool { get; set; } = 5;
        public virtual string Description { get; set; }
        #endregion

        #region Services
        protected ICurrentWindowService CurrentWindowService { get => this.GetService<ICurrentWindowService>(); }
        protected IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }
        #endregion

        #region Constructors
        public AddChannelViewModel(ModbusRTUDriver driver, IGroupItem parent, IChannelCore itemTemplate)
        {
            Driver = driver;
            Parent = parent;

            ComPortSource = new List<string>(driver.ComPortSource);

            foreach (var item in Parent.Childs)
            {
                if (item is ISupportParameters supportParameters)
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

            if (itemTemplate != null)
            {
                if (itemTemplate.ParameterContainer.TryGetValue(nameof(Port), out string port))
                    Port = port;
                if (itemTemplate.ParameterContainer.TryGetValue(nameof(ScanRate), out int scanRate))
                    ScanRate = scanRate;
                if (itemTemplate.ParameterContainer.TryGetValue(nameof(Baudrate), out int baudRate))
                    Baudrate = baudRate;
                if (itemTemplate.ParameterContainer.TryGetValue(nameof(DataBits), out int dataBits))
                    DataBits = dataBits;
                if (itemTemplate.ParameterContainer.TryGetValue(nameof(Parity), out Parity parity))
                    Parity = parity;
                if (itemTemplate.ParameterContainer.TryGetValue(nameof(StopBits), out StopBits stopBits))
                    StopBits = stopBits;
                if (itemTemplate.ParameterContainer.TryGetValue(nameof(DelayBetweenPool), out int delay))
                    DelayBetweenPool = delay;
                Description = itemTemplate.Description;
            }
        }
        #endregion

        #region Commands
        public void Save()
        {
            if (CheckPortNotIsInUse(Port))
            {
                Channel = new Channel(Parent)
                {
                    Port = Port?.Trim(),
                    ScanRate = ScanRate,
                    Baudrate = Baudrate,
                    Parity = Parity,
                    StopBits = StopBits,
                    DelayBetweenPool = DelayBetweenPool,
                    Description = Description
                };
                CurrentWindowService.Close();
            }
            else
            {
                MessageBoxService.ShowMessage($"{Port} is already in use.", "Easy Driver Server", MessageButton.OK, MessageIcon.Warning); 
            }
        }
        public bool CanSave()
        {
            return true;
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
                    if (item is ISupportParameters supportParameters)
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
                    case nameof(Port):
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
