using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EasyDriver.Client.Models
{
    public interface IDevice : IPath, INotifyPropertyChanged
    {
        string Name { get; }
        Dictionary<string, object> Parameters { get; }
        DateTime LastRefreshTime { get; }
        string Error { get; }
        List<ITag> Tags { get; }
    }
}
