using DevExpress.Xpf.Core;
using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace EasyDriver.ModbusRTU
{
    /// <summary>
    /// Interaction logic for CreateChannelView.xaml
    /// </summary>
    public partial class CreateChannelView : UserControl
    {
        #region Public members

        public IEasyDriverPlugin Driver { get; set; }

        public List<string> ComPortSource { get; set; }
        public List<int> BaudRateSource { get; set; }
        public List<int> DataBitsSource { get; set; }
        public List<Parity> ParitySource { get; set; }
        public List<StopBits> StopBitsSource { get; set; }

        #endregion

        #region Constructors

        public CreateChannelView(IEasyDriverPlugin driver) 
        {
            Driver = driver;

            InitializeComponent();

            BaudRateSource = new List<int>()
            {
                300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 38400, 56000, 57600, 115200, 128000, 256000
            };
            cobBaudrate.ItemsSource = BaudRateSource;
            cobBaudrate.SelectedItem = 9600;

            DataBitsSource = new List<int>()
            {
                5, 6, 7, 8
            };
            cobDataBits.ItemsSource = DataBitsSource;
            cobDataBits.SelectedItem = 8;

            ComPortSource = new List<string>();
            for (int i = 1; i <= 100; i++)
            {
                ComPortSource.Add($"COM{i}");
            }
            cobPort.ItemsSource = ComPortSource;

            ParitySource = Enum.GetValues(typeof(Parity)).Cast<Parity>().ToList();

            cobParity.ItemsSource = ParitySource;
            cobParity.SelectedItem = Parity.None;

            StopBitsSource = Enum.GetValues(typeof(StopBits)).Cast<StopBits>().ToList();
            cobStopBits.ItemsSource = StopBitsSource;
            cobStopBits.SelectedItem = StopBits.One;

            btnOk.Click += BtnOk_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        #endregion

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {   
            if (CheckPortNotIsInUse(cobPort.SelectedItem?.ToString()))
            {
                Driver.Channel.ParameterContainer.DisplayName = "ModbusRTU Comunication Parameters";
                Driver.Channel.ParameterContainer.DisplayParameters = "ModbusRTU Comunication Parameters";
                Driver.Channel.ParameterContainer.Parameters["Port"] = cobPort.SelectedItem;
                Driver.Channel.ParameterContainer.Parameters["Baudrate"] = cobBaudrate.SelectedItem;
                Driver.Channel.ParameterContainer.Parameters["Parity"] = cobParity.SelectedItem;
                Driver.Channel.ParameterContainer.Parameters["DataBits"] = cobDataBits.SelectedItem;
                Driver.Channel.ParameterContainer.Parameters["StopBits"] = cobStopBits.SelectedItem;

                Driver.Connect();
                ((Parent as FrameworkElement).Parent as Window).Tag = Driver.Channel;
                ((Parent as FrameworkElement).Parent as Window).Close();
            }
            else
            {
                DXMessageBox.Show($"{cobPort.SelectedItem?.ToString()} is already in use.", "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ((Parent as FrameworkElement).Parent as Window).Close();
        }

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
                return true;
            }
        }
    }
}
