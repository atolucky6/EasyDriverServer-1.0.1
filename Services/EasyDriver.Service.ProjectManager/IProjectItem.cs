using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyDriver.Service.ProjectManager
{
    public interface IProjectItem : IGroupItem, ISupportParameters, ISupportSynchronization
    {
        string Username { get; set; }
        string Password { get; set; }
        string ProjectPath { get; set; }
        IStationCore LocalStation { get; }
        IReadOnlyCollection<IStationCore> RemoteStations { get; }
    }
}
