using EasyDriverPlugin;
using System.IO.Ports;

namespace EasyDriver.ModbusRTU
{
    public class Channel : ChannelCore
    {
        #region Public properties

        public override string DisplayInformation { get => GetDisplayInformation(); set => base.DisplayInformation = value; }

        public int MaxWritesCount
        {
            get
            {
                if (ParameterContainer.TryGetValue(nameof(MaxWritesCount), out int value))
                    return value;
                else
                    ParameterContainer.SetValue(nameof(MaxWritesCount), "10");
                return 10;
            }
            set
            {
                ParameterContainer.SetValue(nameof(MaxWritesCount), value.ToString());
                RaisePropertyChanged();
            }
        }

        public string Port
        {
            get
            {
                if (ParameterContainer.TryGetValue(nameof(Port), out string value))
                    return value;
                else
                    ParameterContainer.SetValue(nameof(Port), "");
                return "";
            }
            set
            {
                ParameterContainer.SetValue(nameof(Port), value);
                RaisePropertyChanged();
            }
        }

        public int Baudrate
        {
            get
            {
                if (ParameterContainer.TryGetValue(nameof(Baudrate), out int value))
                    return value;
                else
                    ParameterContainer.SetValue(nameof(Baudrate), "9600");
                return 9600;
            }
            set
            {
                ParameterContainer.SetValue(nameof(Baudrate), value.ToString());
            }
        }

        public int DataBits
        {
            get
            {
                if (ParameterContainer.TryGetValue(nameof(DataBits), out int value))
                    return value;
                else
                    ParameterContainer.SetValue(nameof(DataBits), "8");
                return 8;
            }
            set
            {
                ParameterContainer.SetValue(nameof(DataBits), value.ToString());
            }
        }

        public Parity Parity
        {
            get
            {
                if (ParameterContainer.TryGetValue(nameof(Parity), out Parity value))
                    return value;
                else
                    ParameterContainer.SetValue(nameof(Parity), Parity.None.ToString());
                return Parity.None;
            }
            set
            {
                ParameterContainer.SetValue(nameof(Parity), value.ToString());
            }
        }

        public StopBits StopBits
        {
            get
            {
                if (ParameterContainer.TryGetValue(nameof(StopBits), out StopBits value))
                    return value;
                else
                    ParameterContainer.SetValue(nameof(StopBits), StopBits.One.ToString());
                return StopBits.One;
            }
            set
            {
                ParameterContainer.SetValue(nameof(StopBits), value.ToString());
            }
        }

        public int ScanRate
        {
            get
            {
                if (ParameterContainer.TryGetValue(nameof(ScanRate), out int value))
                    return value;
                else
                    ParameterContainer.SetValue(nameof(ScanRate), "1000");
                return 1000;
            }
            set
            {
                ParameterContainer.SetValue(nameof(ScanRate), value.ToString());
            }
        }

        public int DelayBetweenPool
        {
            get
            {
                if (ParameterContainer.TryGetValue(nameof(DelayBetweenPool), out int value))
                    return value;
                else
                    ParameterContainer.SetValue(nameof(DelayBetweenPool), "5");
                return 5;
            }
            set
            {
                ParameterContainer.SetValue(nameof(DelayBetweenPool), value.ToString());
            }
        }

        #endregion

        #region Constructors
        public Channel(IGroupItem parent, bool isReadOnly = false) : base(parent, isReadOnly)
        {
            DriverPath = "ModbusRTU";
            ParameterContainer.ParameterChanged += ParameterContainer_ParameterChanged;
        }
        #endregion

        #region Methods
        private void ParameterContainer_ParameterChanged(object sender, ParameterChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(DisplayInformation));
        }

        public override string GetErrorOfProperty(string propertyName)
        {
            return base.GetErrorOfProperty(propertyName);
        }

        private string GetDisplayInformation()
        {
            char parity = Parity.ToString()[0];
            int stopbit = (int)StopBits;
            return $"ModbusRTU {Port}.{Baudrate}.{DataBits}.{parity}.{stopbit}";
        }
        #endregion
    }
}
