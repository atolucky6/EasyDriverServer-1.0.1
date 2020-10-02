using EasyDriver.Core;
using System.Collections.Generic;
using System.ComponentModel;

namespace EasyScada.ServerApplication
{
    public class HubModel : ICheckable, INotifyPropertyChanged
    {
        public string RemoteAddress { get; set; }
        public string Port { get; set; }
        public string CommunicationMode { get; set; }
        public string StationName { get; set; }
        public string Name => $"{RemoteAddress}:{Port}";
        public List<IClientObject> Childs { get; set; }
        private bool? isChecked;
        public bool? IsChecked
        {
            get => isChecked;
            set
            {
                if (isChecked != value)
                {
                    isChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsChecked"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
