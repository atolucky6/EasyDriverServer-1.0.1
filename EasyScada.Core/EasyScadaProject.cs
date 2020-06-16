using System;
using System.Collections.Generic;
using System.Linq;
using EasyDriverPlugin;
using EasyScada.Api.Interfaces;

namespace EasyScada.Core
{
    [Serializable]
    public class EasyScadaProject : GroupItemBase, IEasyScadaProject, IPath
    {
        public EasyScadaProject() : base(null, false)
        {
            Stations = new Indexer<IStationCore>(this);
            LocalStation = new LocalStation(this);
            Add(LocalStation);
        }

        public override string Path => string.Empty;

        public string ProjectPath { get; set; }

        public object SyncObject { get; set; }

        public IParameterContainer ParameterContainer { get; set; }

        public LocalStation LocalStation { get; private set; }

        public IReadOnlyCollection<RemoteStation> RemoteStations => Find(x => x is RemoteStation).Select(x => x as RemoteStation).ToList();

        public Indexer<IStationCore> Stations { get; private set; }

        Indexer<IStationCore> IEasyScadaProject.Stations => throw new NotImplementedException();

        IParameterContainer ISupportParameters.ParameterContainer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override string GetErrorOfProperty(string propertyName)
        {
            return string.Empty;
        }

        public override void GetErrors(ref IErrorInfo errorInfo)
        {

        }

        T IPath.GetItem<T>(string pathToObject)
        {
            foreach (var child in Childs)
            {
                if (child is IPath item)
                {
                    T result = item.GetItem<T>(pathToObject);
                    if (result != null)
                        return result;
                }
            }
            return null;
        }
    }
}
