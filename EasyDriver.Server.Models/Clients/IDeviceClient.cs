﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace EasyDriver.Core
{
    public interface IDeviceClient : IPath, INotifyPropertyChanged
    {
        string Name { get; }
        Dictionary<string, object> Parameters { get; }
        DateTime LastRefreshTime { get; }
        string Error { get; }
        List<ITagClient> Tags { get; }
    }

    [Serializable]
    public sealed class DeviceClient : IDeviceClient
    {
        public string Name { get; set; }

        public Dictionary<string, object> Parameters { get; set; }

        public DateTime LastRefreshTime { get; set; }

        public string Error { get; set; }

        public List<TagClient> Tags { get; set; }

        public string Path { get; set; }

        public bool Checked { get; set; }

        List<ITagClient> IDeviceClient.Tags => Tags?.Select(x => x as ITagClient)?.ToList();

        T IPath.GetItem<T>(string pathToObject)
        {
            if (string.IsNullOrWhiteSpace(pathToObject))
                return null;
            if (Path == pathToObject)
                return this as T;
            if (pathToObject.StartsWith(Path))
            {
                foreach (var child in Tags)
                {
                    if (child is IPath item)
                    {
                        if (pathToObject.StartsWith(item.Path))
                            return item.GetItem<T>(pathToObject);
                    }
                }
            }
            return null;
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}