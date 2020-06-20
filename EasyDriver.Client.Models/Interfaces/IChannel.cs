using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EasyDriver.Client.Models
{
    public interface IChannel : IPath, INotifyPropertyChanged
    {
        string Name { get; }
        string DriverName { get; }
        ConnectionType ConnectionType { get; }
        string Error { get; }
        List<IDevice> Devices { get; }
        DateTime LastRefreshTime { get; }
        Dictionary<string, object> Parameters { get; }
    }
}
