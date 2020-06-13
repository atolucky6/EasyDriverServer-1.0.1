using EasyDriverPlugin;
using System;
using System.Collections.Generic;

namespace EasyScada.Core
{
    public interface IEasyScadaProject : IGroupItem, ISupportParameters, ISupportSynchronization
    {
        string ProjectPath { get; set; }
        LocalStation LocalStation { get; }
        IReadOnlyCollection<RemoteStation> RemoteStations { get; }
        Indexer<IStationCore> Stations { get; }
    }
}
