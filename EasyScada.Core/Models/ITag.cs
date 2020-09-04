using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace EasyScada.Core
{
    public interface ITag : IPath, INotifyPropertyChanged, IComposite
    {
        IDevice DeviceParent { get; }
        string StationPath { get; }
        string ChannelName { get; }
        string DeviceName { get; }
        string Name { get; }
        string Address { get; }
        string DataType { get; }
        string Value { get; set; }
        Quality Quality { get; }
        int RefreshRate { get; }
        int RefreshInterval { get; }
        AccessPermission AccessPermission { get; }
        string Error { get; }
        DateTime TimeStamp { get; }
        Dictionary<string, string> Parameters { get; }
        string Description { get; }
        WriteResponse Write(string value);
        Task<WriteResponse> WriteAsync(string value);

        event EventHandler<TagValueChangedEventArgs> ValueChanged;
        event EventHandler<TagQualityChangedEventArgs> QualityChanged;
    }

    [Serializable]
    public sealed class Tag : ITag
    {
        [field: NonSerialized]
        [JsonIgnore]
        public IDevice DeviceParent { get; internal set; }

        [JsonIgnore]
        public string StationPath
        {
            get => DeviceParent?.StationPath;
        }

        [JsonIgnore]
        public string ChannelName
        {
            get => DeviceParent?.ChannelName;
        }

        [JsonIgnore]
        public string DeviceName
        {
            get => DeviceParent?.Name;
        }

        public string Name { get; set; }

        public string Address { get; set; }

        public string DataType { get; set; }

        public string value;
        public string Value
        {
            get { return value; }
            set
            {
                try
                {
                    if (this.value != value)
                    {
                        string oldValue = this.value;
                        this.value = value;
                        var args = new TagValueChangedEventArgs(this, oldValue, value);
                        ValueChanged?.Invoke(this, args);
                        if (DeviceParent is Device device)
                            device.RaiseTagValueChanged(this, args);
                    }
                }
                catch { }
            }
        }

        public Quality quality;
        public Quality Quality
        {
            get { return quality; }
            set
            {
                try
                {
                    if (quality != value)
                    {
                        Quality oldValue = quality;
                        quality = value;
                        var args = new TagQualityChangedEventArgs(this, oldValue, value);
                        QualityChanged?.Invoke(this, args);
                        if (DeviceParent is Device device)
                            device.RaiseTagQualityChanged(this, args);
                    }
                }
                catch { }
            }
        }

        public int RefreshRate { get; set; }

        int refreshInterval;
        public int RefreshInterval
        {
            get { return refreshInterval; }
            set
            {
                try
                {
                    if (refreshInterval != value)
                    {
                        refreshInterval = value;
                        RaisePropertyChanged();
                    }
                }
                catch { }
            }
        }

        public AccessPermission AccessPermission { get; internal set; }

        string error;
        public string Error
        {
            get { return error; }
            set
            {
                if (error != value)
                {
                    error = value;
                    RaisePropertyChanged();
                }
            }
        }

        DateTime timeStamp;
        public DateTime TimeStamp
        {
            get { return timeStamp; }
            set
            {
                try
                {
                    if (timeStamp != value)
                    {
                        timeStamp = value;
                        RaisePropertyChanged();
                    }
                }
                catch { }
            }
        }

        public Dictionary<string, string> Parameters { get; internal set; }

        public string Description { get; set; }

        public string Path { get; set; }

        public bool Checked { get; set; }

        T IPath.GetItem<T>(string pathToObject)
        {
            if (Path == pathToObject)
                return this as T;
            return null;
        }

        [JsonIgnore]
        public List<object> Childs { get { return new List<object>(); } }

        [field: NonSerialized]
        public event EventHandler<TagValueChangedEventArgs> ValueChanged;

        [field: NonSerialized]
        public event EventHandler<TagQualityChangedEventArgs> QualityChanged;

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public WriteResponse Write(string value)
        {
            return EasyDriverConnectorProvider.GetEasyDriverConnector().WriteTag(Path, value);
        }

        public Task<WriteResponse> WriteAsync(string value)
        {
            return EasyDriverConnectorProvider.GetEasyDriverConnector().WriteTagAsync(Path, value);
        }

        internal void SetParentForChild(IDevice parent)
        {
            DeviceParent = parent;
        }
    }
}
