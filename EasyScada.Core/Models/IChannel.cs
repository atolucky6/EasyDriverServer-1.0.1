using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace EasyScada.Core
{
    public interface IChannel : IPath, INotifyPropertyChanged, IComposite
    {
        IStation StationParent { get; }
        string StationPath { get; }
        string Name { get; }
        string DriverName { get; }
        string Error { get; }
        List<IDevice> Devices { get; }
        DateTime LastRefreshTime { get; }
        Dictionary<string, string> Parameters { get; }
        string Description { get; }

        event EventHandler<TagValueChangedEventArgs> TagValueChanged;
        event EventHandler<TagQualityChangedEventArgs> TagQualityChanged;
    }

    [Serializable]
    public sealed class Channel : IChannel
    {
        [field: NonSerialized]
        [JsonIgnore]
        public IStation StationParent { get; internal set; }

        [JsonIgnore]
        public string StationPath
        {
            get => StationParent?.Name;
        }

        public string Name { get; set; }

        public string DriverName { get; set; }

        public DateTime LastRefreshTime { get; set; }

        public string Error { get; set; }

        public List<Device> Devices { get; set; }

        public Dictionary<string, string> Parameters { get; set; }

        public string Description { get; set; }

        public string Path { get; set; }

        public bool Checked { get; set; }

        List<IDevice> IChannel.Devices => Devices?.Select(x => x as IDevice)?.ToList();

        [JsonIgnore]
        public List<object> Childs
        {
            get
            {
                var res = new List<object>();
                res.AddRange(Devices);
                return res;
            }
        }

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
            if (StationParent is Station station)
                station.RaiseTagValueChanged(sender, e);
        }

        internal void RaiseTagQualityChanged(object sender, TagQualityChangedEventArgs e)
        {
            TagQualityChanged?.Invoke(sender, e);
            if (StationParent is Station station)
                station.RaiseTagQualityChanged(sender, e);
        }

        internal void SetParentForChild(IStation parent)
        {
            StationParent = parent;
            if (Devices != null)
            {
                foreach (var item in Devices)
                {
                    item.SetParentForChild(this);
                }
            }
        }
    }
}
