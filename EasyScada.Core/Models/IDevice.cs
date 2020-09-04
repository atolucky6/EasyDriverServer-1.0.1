using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace EasyScada.Core
{
    public interface IDevice : IPath, INotifyPropertyChanged, IComposite
    {
        IChannel ChannelParent { get; }
        string StationPath { get; }
        string ChannelName { get; }
        string Name { get; }
        Dictionary<string, string> Parameters { get; }
        DateTime LastRefreshTime { get; }
        string Error { get; }
        List<ITag> Tags { get; }
        string Description { get; }
        event EventHandler<TagValueChangedEventArgs> TagValueChanged;
        event EventHandler<TagQualityChangedEventArgs> TagQualityChanged;
    }

    [Serializable]
    public sealed class Device : IDevice
    {
        [field: NonSerialized]
        [JsonIgnore]
        public IChannel ChannelParent { get; internal set; }

        [JsonIgnore]
        public string StationPath
        {
            get => ChannelParent?.StationPath;
        }

        [JsonIgnore]
        public string ChannelName
        {
            get => ChannelParent?.Name;
        }

        public string Name { get; set; }

        public Dictionary<string, string> Parameters { get; set; }

        public string Description { get; set; }

        public DateTime LastRefreshTime { get; set; }

        public string Error { get; set; }

        public List<Tag> Tags { get; set; }

        public string Path { get; set; }

        public bool Checked { get; set; }

        List<ITag> IDevice.Tags => Tags?.Select(x => x as ITag)?.ToList();

        [JsonIgnore]
        public List<object> Childs
        {
            get
            {
                var res = new List<object>();
                res.AddRange(Tags);
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
            if (ChannelParent is Channel channel)
                channel.RaiseTagValueChanged(sender, e);
        }

        internal void RaiseTagQualityChanged(object sender, TagQualityChangedEventArgs e)
        {
            TagQualityChanged?.Invoke(sender, e);
            if (ChannelParent is Channel channel)
                channel.RaiseTagQualityChanged(sender, e);
        }

        internal void SetParentForChild(IChannel parent)
        {
            ChannelParent = parent;
            if (Tags != null)
            {
                foreach (var item in Tags)
                {
                    item.SetParentForChild(this);
                }
            }
        }
    }
}
