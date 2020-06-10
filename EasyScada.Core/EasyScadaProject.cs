using System;
using System.Collections.Generic;
using System.Linq;
using EasyDriverPlugin;

namespace EasyScada.Core
{
    [Serializable]
    public class EasyScadaProject : GroupItemBase, IEasyScadaProject
    {
        public EasyScadaProject() : base(null, false)
        {
            Stations = new Indexer<IStation>(this);
            LocalStation = new LocalStation(this);
            Add(LocalStation);
        }

        public string ProjectPath { get; set; }

        public object SyncObject { get; set; }

        public IParameterContainer ParameterContainer { get; set; }

        public LocalStation LocalStation { get; private set; }

        public IReadOnlyCollection<RemoteStation> RemoteStations => Find(x => x is RemoteStation).Select(x => x as RemoteStation).ToList();

        public Indexer<IStation> Stations { get; private set; }

        Indexer<IStation> IEasyScadaProject.Stations => throw new NotImplementedException();

        IParameterContainer ISupportParameters.ParameterContainer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override string GetErrorOfProperty(string propertyName)
        {
            throw new NotImplementedException();
        }

        public override void GetErrors(ref IErrorInfo errorInfo)
        {
            throw new NotImplementedException();
        }
    }
}
