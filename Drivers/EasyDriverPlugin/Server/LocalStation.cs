using EasyDriverPlugin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace EasyDriverPlugin
{
    [Serializable]
    public class LocalStation : GroupItemBase, IStationCore
    {
        #region IStationCore

        public LocalStation(IGroupItem parent) : base(parent, true)
        {
            Name = "Local Station";
            SyncObject = new object();
            StationType = "Local";
            ParameterContainer = new ParameterContainer();            
        }

        public override bool Enabled { get => enabled; set => base.Enabled = value; }

        public override ItemType ItemType { get; set; } = ItemType.LocalStation;

        public string RemoteAddress
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        public string ConnectionString { get; set; }

        public CommunicationMode CommunicationMode
        {
            get { return GetProperty<CommunicationMode>(); }
            set { SetProperty(value); }
        }

        [field: NonSerialized]
        ConnectionStatus connectionStatus;
        public ConnectionStatus ConnectionStatus
        {
            get => ConnectionStatus.Connected;
            set { connectionStatus = value; }
        }

        public string StationType { get; set; }

        public ushort Port
        {
            get { return GetProperty<ushort>(); }
            set { SetProperty(value); }
        }

        public int RefreshRate { get; set; }

        public DateTime LastRefreshTime { get; set; }

        [Category(PropertyCategory.General), DisplayName("Parameters")]
        public IParameterContainer ParameterContainer { get; set; }

        [Browsable(false)]
        public object SyncObject { get; set; }

        public override string GetErrorOfProperty(string propertyName)
        {
            return string.Empty;
        }
        #endregion
    }
}
    
