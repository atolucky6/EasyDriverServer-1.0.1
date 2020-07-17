using EasyDriverPlugin;
using System.Collections.Generic;

namespace EasyDriver.Core
{
    public interface IEasyScadaProject : IGroupItem, ISupportParameters, ISupportSynchronization, IPath
    {
        string ProjectPath { get; set; }
        LocalStation LocalStation { get; }
        IReadOnlyCollection<RemoteStation> RemoteStations { get; }
        Indexer<IStationCore> Stations { get; }
    }
}
