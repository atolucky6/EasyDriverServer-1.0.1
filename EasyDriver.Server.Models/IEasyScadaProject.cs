using EasyDriverPlugin;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace EasyDriver.Core
{
    public interface IEasyScadaProject : IGroupItem, ISupportParameters, ISupportSynchronization
    { 
        string Username { get; set; }
        string Password { get; set; }
        string ProjectPath { get; set; }
        LocalStation LocalStation { get; }
        IReadOnlyCollection<RemoteStation> RemoteStations { get; }
        Indexer<IStationCore> Stations { get; }
    }
}
