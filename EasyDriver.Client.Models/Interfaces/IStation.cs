using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EasyDriver.Client.Models
{
    public interface IStation : IPath, INotifyPropertyChanged
    {
        string Name { get; }
        bool IsLocalStation { get; }
        string RemoteAddress { get; }
        CommunicationMode CommunicationMode { get; }
        int RefreshRate { get; }
        ushort Port { get; }
        string Error { get; }
        DateTime LastRefreshTime { get; }
        Dictionary<string, object> Parameters { get; }
        List<IChannel> Channels { get; }
        List<IStation> RemoteStations { get; }
    }
}
