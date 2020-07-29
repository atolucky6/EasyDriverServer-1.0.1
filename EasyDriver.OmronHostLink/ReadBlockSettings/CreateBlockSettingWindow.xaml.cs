using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DevExpress.Xpf.Core;


namespace EasyDriver.OmronHostLink
{
    /// <summary>
    /// Interaction logic for CreateBlockSettingWindow.xaml
    /// </summary>
    public partial class CreateBlockSettingWindow : ThemedWindow, IDataErrorInfo, INotifyPropertyChanged
    {
        private readonly ReadBlockSettingView readBlockSettingView;

        public CreateBlockSettingWindow()
        {
            InitializeComponent();
        }

        public CreateBlockSettingWindow(ReadBlockSettingView readBlockSettingView)
        {
            InitializeComponent();
            AddressTypeSource = new List<string>(Enum.GetValues(typeof(AddressType)).Cast<AddressType>().Select(x => x.ToString()));
            AddressType = AddressTypeSource[0];
            DataContext = this;
            btnOk.Click += BtnOk_Click;
            btnOkAndClose.Click += BtnOkAndClose_Click;
            btnCancel.Click += BtnCancel_Click;
            KeyDown += OnKeyDown;
            WindowStyle = WindowStyle.ToolWindow;
            ResizeMode = ResizeMode.NoResize;
            this.readBlockSettingView = readBlockSettingView;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnOkAndClose_Click(null, null);
            }
            else if (e.Key == Key.Escape)
            {
                BtnCancel_Click(null, null);
            }
        }

        private void BtnOkAndClose_Click(object sender, RoutedEventArgs e)
        {
            if (Error == string.Empty)
            {
                ReadBlockSetting readBlock = new ReadBlockSetting() { Enabled = true };
                readBlock.AddressType = (AddressType)Enum.Parse(typeof(AddressType), AddressType);
                readBlock.StartAddress = decimal.ToUInt16(StartAddress);
                readBlock.EndAddress = decimal.ToUInt16(EndAddress);

                readBlockSettingView.ReadBlockSettings.Add(readBlock);
                readBlockSettingView.grcBlock.SelectedItem = readBlock;
                int rowHandle = readBlockSettingView.grcBlock.GetRowHandleByListIndex(readBlockSettingView.ReadBlockSettings.IndexOf(readBlock));
                readBlockSettingView.grcBlock.ExpandGroupRow(rowHandle);
                readBlockSettingView.grcBlock.View.ScrollIntoView(rowHandle);
                Close();
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (Error == string.Empty)
            {
                ReadBlockSetting readBlock = new ReadBlockSetting() { Enabled = true };
                readBlock.AddressType = (AddressType)Enum.Parse(typeof(AddressType), AddressType);
                readBlock.StartAddress = decimal.ToUInt16(StartAddress);
                readBlock.EndAddress = decimal.ToUInt16(EndAddress);

                readBlockSettingView.ReadBlockSettings.Add(readBlock);
                readBlockSettingView.grcBlock.SelectedItem = readBlock;
                int rowHandle = readBlockSettingView.grcBlock.GetRowHandleByListIndex(readBlockSettingView.ReadBlockSettings.IndexOf(readBlock));
                readBlockSettingView.grcBlock.ExpandGroupRow(rowHandle);
                readBlockSettingView.grcBlock.View.ScrollIntoView(rowHandle);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public List<string> AddressTypeSource { get; set; }

        decimal startAddress = 0;
        public decimal StartAddress
        {
            get { return startAddress; }
            set
            {
                if (startAddress != value)
                {
                    startAddress = value;
                    RaisePropertyChanged();
                }
            }
        }

        decimal endAddress = 1;
        public decimal EndAddress
        {
            get { return endAddress; }
            set
            {
                if (endAddress != value)
                {
                    endAddress = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string AddressType { get; set; }

        public string Error { get; set; }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "StartAddress":
                        if (StartAddress > 9999)
                        {
                            Error = "The start word address is out of range. The range of start word address is 0 - 9999;";
                        }
                        else
                        {
                            if (StartAddress >= EndAddress)
                            {
                                Error = "The start word address must be smaller than end word address.";
                            }
                            else
                            {
                                if (EndAddress - StartAddress >= 155)
                                {
                                    Error = $"The maximum word can read per request is 155.";
                                }
                                else
                                {
                                    Error = string.Empty;
                                }
                            }
                        }
                        break;
                    case "EndAddress":
                        if (EndAddress > 9999)
                        {
                            Error = "The end word address is out of range. The range of end word address is 0 - 9999;";
                        }
                        else
                        {
                            if (StartAddress >= EndAddress)
                            {
                                Error = "The end word address must be bigger than start word address.";
                            }
                            else
                            {
                                if (EndAddress - StartAddress >= 155)
                                {
                                    Error = $"The maximum word can read per request is 155.";
                                }
                                else
                                {
                                    Error = string.Empty;
                                }
                            }
                        }
                        break;
                    default:
                        Error = string.Empty;
                        break;
                }
                return Error;
            }
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
