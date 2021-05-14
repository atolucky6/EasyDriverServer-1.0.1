using EasyDriverPlugin;
using System.IO.Ports;

namespace EasyDriver.ModbusTCP
{
    public class Channel : ChannelCore
    {
        #region Public properties

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

        public string IpAddress
        {
            get
            {
                if (ParameterContainer.TryGetValue(nameof(IpAddress), out string value))
                    return value;
                else
                    ParameterContainer.SetValue(nameof(IpAddress), "192.168.1.1");
                return "192.168.1.1";
            }
            set
            {
                ParameterContainer.SetValue(nameof(IpAddress), value.ToString());
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(DisplayInformation));
            }
        }

        public ushort Port
        {
            get
            {
                if (ParameterContainer.TryGetValue(nameof(Port), out ushort value))
                    return value;
                else
                    ParameterContainer.SetValue(nameof(Port), "502");
                return 502;
            }
            set
            {
                ParameterContainer.SetValue(nameof(Port), value.ToString());
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(DisplayInformation));
            }
        }

        public override string DisplayInformation { get => GetDisplayInformation(); set => base.DisplayInformation = value; }

        #endregion

        #region Constructors
        public Channel(IGroupItem parent, bool isReadOnly = false) : base(parent, isReadOnly)
        {
            DriverPath = "ModbusTCP";
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
            return $"ModbusTCP - {IpAddress}:{Port}";
        }
        #endregion
    }
}
