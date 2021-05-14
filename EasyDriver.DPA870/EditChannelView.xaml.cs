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
using System.Windows.Input;

namespace EasyDriver.DPA870
{
    /// <summary>
    /// Interaction logic for EditChannelView.xaml
    /// </summary>
    public partial class EditChannelView : UserControl
    {
        #region Public members

        public IEasyDriverPlugin Driver { get; set; }

        public List<string> ComPortSource { get; set; }
        public List<int> BaudRateSource { get; set; }
        public List<int> DataBitsSource { get; set; }
        public List<Parity> ParitySource { get; set; }
        public List<StopBits> StopBitsSource { get; set; }
        public IChannelCore Channel { get; set; }

        #endregion

        #region Constructors

        public EditChannelView(IEasyDriverPlugin driver, IChannelCore channel)
        {
            Driver = driver;
            Channel = channel;

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

            KeyDown += OnKeyDown;

            btnOk.Click += BtnOk_Click;
            btnCancel.Click += BtnCancel_Click;
            Loaded += EditChannelView_Loaded;
        }

        #endregion

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnOk_Click(null, null);
            }
            else if (e.Key == Key.Escape)
            {
                BtnCancel_Click(null, null);
            }
        }

        private void EditChannelView_Loaded(object sender, RoutedEventArgs e)
        {
            if (Channel != null)
            {
                txbName.Text = Channel.Name;
                cobPort.Text = Channel.ParameterContainer.Parameters["Port"];
                cobBaudrate.Text = Channel.ParameterContainer.Parameters["Baudrate"];
                cobDataBits.Text = Channel.ParameterContainer.Parameters["DataBits"];
                cobParity.Text = Channel.ParameterContainer.Parameters["Parity"];
                cobStopBits.Text = Channel.ParameterContainer.Parameters["StopBits"];
                spnDelayPool.Text = Channel.ParameterContainer.Parameters["DelayBetweenPool"];
                spnScanRate.Text = Channel.ParameterContainer.Parameters["ScanRate"];
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            string validateResult = txbName.Text?.Trim().ValidateFileName("Channel");
            if (!string.IsNullOrWhiteSpace(validateResult))
            {
                DXMessageBox.Show(validateResult, "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Channel.Parent.Childs.FirstOrDefault(x => x != Channel && (x as ICoreItem).Name == txbName.Text?.Trim()) != null)
            {
                DXMessageBox.Show($"The channel name '{txbName.Text?.Trim()}' is already in use.", "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string currentPort = Channel.ParameterContainer.Parameters["Port"].ToString();
            if (cobPort.SelectedItem?.ToString() == currentPort || CheckPortNotIsInUse(cobPort.SelectedItem?.ToString()))
            {
                Channel.Name = txbName.Text?.Trim();

                Channel.ParameterContainer.DisplayName = "DPA870 Comunication Parameters";
                Channel.ParameterContainer.DisplayParameters = "DPA870 Comunication Parameters";
                Channel.ParameterContainer.SetValue("Port", cobPort.SelectedItem.ToString());
                Channel.ParameterContainer.SetValue("Baudrate", cobBaudrate.SelectedItem.ToString());
                Channel.ParameterContainer.SetValue("Parity", cobParity.SelectedItem.ToString());
                Channel.ParameterContainer.SetValue("DataBits", cobDataBits.SelectedItem.ToString());
                Channel.ParameterContainer.SetValue("StopBits", cobStopBits.SelectedItem.ToString());
                Channel.ParameterContainer.SetValue("ScanRate", spnScanRate.Value.ToString());
                Channel.ParameterContainer.SetValue("DelayBetweenPool", spnDelayPool.Value.ToString());

                Tag = Channel;
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
