using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace EasyScada.Core
{
    public interface IStation : IPath, INotifyPropertyChanged, IComposite
    {
        object Parent { get; }
        string Name { get; }
        StationType StationType { get; }
        string RemoteAddress { get; }
        CommunicationMode CommunicationMode { get; }
        ConnectionStatus ConnectionStatus { get; }
        int RefreshRate { get; }
        ushort Port { get; }
        string Error { get; }
        DateTime LastRefreshTime { get; }
        Dictionary<string, string> Parameters { get; }
        string Description { get; }
        List<IChannel> Channels { get; }
        List<IStation> RemoteStations { get; }
        event EventHandler<TagValueChangedEventArgs> TagValueChanged;
        event EventHandler<TagQualityChangedEventArgs> TagQualityChanged;
    }

    [Serializable]
    public sealed class Station : IStation
    {
        public object Parent { get; internal set; }

        public string Name { get; set; }

        public string RemoteAddress { get; set; }

        public ushort Port { get; set; }

        public string Error { get; set; }

        public StationType StationType { get; }

        public CommunicationMode CommunicationMode { get; set; }

        public ConnectionStatus ConnectionStatus { get; set; }

        public int RefreshRate { get; set; }

        public DateTime LastRefreshTime { get; set; }

        public Dictionary<string, string> Parameters { get; set; }

        public string Description { get; set; }

        public List<Channel> Channels { get; set; }

        public List<Station> RemoteStations { get; set; }

        public string Path { get; set; }

        public bool Checked { get; set; }

        [JsonIgnore]
        public List<object> Childs
        {
            get
            {
                var res = new List<object>();
                res.AddRange(Channels);
                res.AddRange(RemoteStations);
                return res;
            }
        }

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
        [field: NonSerialized]
        public event EventHandler<TagValueChangedEventArgs> TagValueChanged;
        [field: NonSerialized]
        public event EventHandler<TagQualityChangedEventArgs> TagQualityChanged;

        public void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void RaiseTagValueChanged(object sender, TagValueChangedEventArgs e)
        {
            TagValueChanged?.Invoke(sender, e);
            if (Parent is Station station)
                station.RaiseTagValueChanged(sender, e);
            else if (Parent is ConnectionSchema schema)
                schema.RaiseTagValueChanged(sender, e);
        }

        internal void RaiseTagQualityChanged(object sender, TagQualityChangedEventArgs e)
        {
            TagQualityChanged?.Invoke(sender, e);
            if (Parent is Station station)
                station.RaiseTagQualityChanged(sender, e);
            else if (Parent is ConnectionSchema schema)
                schema.RaiseTagQualityChanged(sender, e);
        }

        internal void SetParentForChild(object parent)
        {
            Parent = parent;
            if (Channels != null)
            {
                foreach (var item in Channels)
                {
                    item.SetParentForChild(this);
                }
            }
            if (RemoteStations != null)
            {
                foreach (var item in RemoteStations)
                {
                    item.SetParentForChild(this);
                }
            }
        }
    }
}
