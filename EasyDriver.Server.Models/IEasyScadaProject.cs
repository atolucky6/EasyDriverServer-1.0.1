using EasyDriverPlugin;
using EasyDriver.Client.Models;
using System;
using System.Collections.Generic;

namespace EasyDriver.Server.Models
{
    public interface IEasyScadaProject : IGroupItem, ISupportParameters, ISupportSynchronization, IPath
    {
        string ProjectPath { get; set; }
        LocalStation LocalStation { get; }
        IReadOnlyCollection<RemoteStation> RemoteStations { get; }
        Indexer<IStationCore> Stations { get; }
    }
}
