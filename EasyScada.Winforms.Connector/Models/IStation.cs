using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace EasyScada.Winforms.Connector
{
    public interface IStation : IPath, INotifyPropertyChanged
    {
        string Name { get; }
        bool IsLocalStation { get; }
        string RemoteAddress { get; }
        CommunicationMode CommunicationMode { get; }
        ConnectionStatus ConnectionStatus { get; }
        int RefreshRate { get; }
        ushort Port { get; }
        string Error { get; }
        DateTime LastRefreshTime { get; }
        Dictionary<string, object> Parameters { get; }
        List<IChannel> Channels { get; }
        List<IStation> RemoteStations { get; }
    }

    [Serializable]
    public sealed class Station : IStation
    {
        public string Name { get; set; }

        public bool IsLocalStation { get; set; }

        public string RemoteAddress { get; set; }

        public ushort Port { get; set; }

        public string Error { get; set; }

        public CommunicationMode CommunicationMode { get; set; }

        public ConnectionStatus ConnectionStatus { get; set; }

        public int RefreshRate { get; set; }

        public DateTime LastRefreshTime { get; set; }

        public Dictionary<string, object> Parameters { get; set; }

        public List<Channel> Channels { get; set; }

        public List<Station> RemoteStations { get; set; }

        public string Path { get; set; }

        public bool Checked { get; set; }

        List<IChannel> IStation.Channels => Channels?.Select(x => x as IChannel).ToList();

        List<IStation> IStation.RemoteStations => RemoteStations?.Select(x => x as IStation).ToList();

        T IPath.GetItem<T>(string pathToObject)
        {
            if (string.IsNullOrWhiteSpace(pathToObject))
                return null;
            if (Path == pathToObject)
                return this as T;
            if (pathToObject.StartsWith(Path))
            {
                foreach (var child in RemoteStations)
                {
                    if (child is IPath item)
                    {
                        if (pathToObject.StartsWith(item.Path))
                            return item.GetItem<T>(pathToObject);
                    }
                }

                foreach (var child in Channels)
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
