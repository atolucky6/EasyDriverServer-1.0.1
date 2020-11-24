using EasyDriverPlugin;
using Newtonsoft.Json;
using System;
using EasyDriver.ServicePlugin;
using System.Collections.Generic;
using System.Linq;

namespace EasyDriver.Service.ProjectManager
{
    public class ProjectItem : GroupItemBase, IProjectItem
    {
        public ProjectItem() : base(null, false)
        {
            LocalStation = new LocalStation(this);
            Childs.Add(LocalStation);
        }

        public override bool Enabled { get => true; set => base.Enabled = value; }

        public override ItemType ItemType { get; set; } = ItemType.ConnectionSchema;

        public override string Path => string.Empty;

        public string ProjectPath { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public object SyncObject { get; set; }

        public IParameterContainer ParameterContainer { get; set; }

        public IStationCore LocalStation { get; private set; }

        public IReadOnlyCollection<IStationCore> RemoteStations => Find(x => x.ItemType == ItemType.RemoteStation).Select(x => x as IStationCore).ToList();

        public override string GetErrorOfProperty(string propertyName)
        {
            return string.Empty;
        }
    }
}
