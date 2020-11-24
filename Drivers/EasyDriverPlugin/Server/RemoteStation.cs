using EasyDriverPlugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace EasyDriverPlugin
{
    [Serializable]
    public class RemoteStation : GroupItemBase, IStationCore
    {
        #region IStationCore

        public RemoteStation(IGroupItem parent) : base(parent, true)
        {
            SyncObject = new object();
            StationType = StationType = "Remote";
            ParameterContainer = new ParameterContainer();
        }

        public override ItemType ItemType { get; set; } = ItemType.RemoteStation;

        public string RemoteAddress
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        public override string DisplayInformation { get => ConnectionString; set => base.DisplayInformation = value; }

        public string CommunicationError { get; set; }

        public string StationType { get; set; }

        private string connectionString;
        public string ConnectionString
        {
            get => connectionString;
            set 
            { 
                if (connectionString != value)
                {
                    connectionString = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(DisplayInformation));
                }
            }
        }

        public ushort Port
        {
            get { return GetProperty<ushort>(); }
            set { SetProperty(value); }
        }

        public int RefreshRate { get; set; }

        public DateTime LastRefreshTime { get;set; }

        public CommunicationMode CommunicationMode
        {
            get { return GetProperty<CommunicationMode>(); }
            set { SetProperty(value); }
        }

        [field: NonSerialized]
        public ConnectionStatus ConnectionStatus { get; set; }

        [Category(PropertyCategory.General), DisplayName("Parameters")]
        public IParameterContainer ParameterContainer { get; set; }

        [Browsable(false)]
        public object SyncObject { get; private set; }

        public override string GetErrorOfProperty(string propertyName)
        {
            return string.Empty;
        }
        #endregion
    }
}
