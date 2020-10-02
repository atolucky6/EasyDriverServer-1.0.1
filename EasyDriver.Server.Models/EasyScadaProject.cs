using System;
using System.Collections.Generic;
using System.Linq;
using EasyDriverPlugin;
using Newtonsoft.Json;

namespace EasyDriver.Core
{
    [JsonConverter(typeof(EasyScadaProjectJsonConverter))]
    [Serializable]
    public class EasyScadaProject : GroupItemBase, IEasyScadaProject
    {
        public EasyScadaProject() : base(null, false)
        {
            Stations = new Indexer<IStationCore>(this);
            LocalStation = new LocalStation(this);
            Add(LocalStation);
        }

        [JsonIgnore]
        public override string Path => string.Empty;

        public string ProjectPath { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        [JsonIgnore]
        public object SyncObject { get; set; }

        [JsonIgnore]
        public IParameterContainer ParameterContainer { get; set; }

        public LocalStation LocalStation { get; private set; }

        public IReadOnlyCollection<RemoteStation> RemoteStations => Find(x => x is RemoteStation).Select(x => x as RemoteStation).ToList();

        public Indexer<IStationCore> Stations { get; private set; }

        public override string GetErrorOfProperty(string propertyName)
        {
            return string.Empty;
        }

        public override void GetErrors(ref IErrorInfo errorInfo)
        {

        }
    }
}
