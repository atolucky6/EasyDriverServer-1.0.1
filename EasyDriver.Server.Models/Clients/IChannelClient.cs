using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace EasyDriver.Core
{
    public interface IChannelClient : IPath, INotifyPropertyChanged
    {
        string Name { get; }
        string DriverName { get; }
        string Error { get; }
        List<IDeviceClient> Devices { get; }
        DateTime LastRefreshTime { get; }
        Dictionary<string, object> Parameters { get; }
    }

    [Serializable]
    public sealed class ChannelClient : IChannelClient
    {
        public string Name { get; set; }

        public string DriverName { get; set; }

        public DateTime LastRefreshTime { get; set; }

        public string Error { get; set; }

        public List<DeviceClient> Devices { get; set; }

        public Dictionary<string, object> Parameters { get; set; }

        public string Path { get; set; }

        public bool Checked { get; set; }

        List<IDeviceClient> IChannelClient.Devices => Devices?.Select(x => x as IDeviceClient)?.ToList();

        T IPath.GetItem<T>(string pathToObject)
        {
            if (string.IsNullOrWhiteSpace(pathToObject))
                return null;
            if (Path == pathToObject)
                return this as T;
            if (pathToObject.StartsWith(Path))
            {
                foreach (var child in Devices)
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
