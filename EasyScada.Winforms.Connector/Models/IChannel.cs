using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace EasyScada.Winforms.Connector
{
    public interface IChannel : IPath, INotifyPropertyChanged
    {
        string Name { get; }
        string DriverName { get; }
        string Error { get; }
        List<IDevice> Devices { get; }
        DateTime LastRefreshTime { get; }
        Dictionary<string, object> Parameters { get; }
    }

    [Serializable]
    public sealed class Channel : IChannel
    {
        public string Name { get; set; }

        public string DriverName { get; set; }

        public DateTime LastRefreshTime { get; set; }

        public string Error { get; set; }

        public List<Device> Devices { get; set; }

        public Dictionary<string, object> Parameters { get; set; }

        public string Path { get; set; }

        public bool Checked { get; set; }

        List<IDevice> IChannel.Devices => Devices?.Select(x => x as IDevice)?.ToList();

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
