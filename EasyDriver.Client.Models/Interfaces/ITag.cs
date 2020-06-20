using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EasyDriver.Client.Models
{
    public interface ITag : IPath, INotifyPropertyChanged
    {
        string Name { get; }
        string Address { get; }
        string DataType { get; }
        string Value { get; }
        Quality Quality { get; }
        int RefreshRate { get; }
        int RefreshInterval { get; }
        AccessPermission AccessPermission { get; }
        string Error { get; }
        DateTime TimeStamp { get; }
        Dictionary<string, object> Parameters { get; }

        event EventHandler<TagValueChangedEventArgs> ValueChanged;
        event EventHandler<TagQualityChangedEventArgs> QualityChanged;
    }
}
