using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Connector
{
    public interface ITag : IPath, INotifyPropertyChanged
    {
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
        Dictionary<string, object> Parameters { get; }
        Quality Write(string value);
        Task<Quality> WriteAsync(string value);

        event EventHandler<TagValueChangedEventArgs> ValueChanged;
        event EventHandler<TagQualityChangedEventArgs> QualityChanged;
    }

    [Serializable]
    public sealed class Tag : ITag
    {
        internal EasyDriverConnector connector;

        public string Name { get; internal set; }

        public string Address { get; internal set; }

        public string DataType { get; internal set; }

        internal string value;
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
                        RaisePropertyChanged();
                        ValueChanged?.Invoke(this, new TagValueChangedEventArgs(oldValue, value));
                    }
                }
                catch { }
            }
        }

        internal Quality quality;
        public Quality Quality
        {
            get { return quality; }
            internal set
            {
                try
                {
                    if (quality != value)
                    {
                        Quality oldValue = quality;
                        quality = value;
                        RaisePropertyChanged();
                        QualityChanged?.Invoke(this, new TagQualityChangedEventArgs(oldValue, value));
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
            internal set
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
            internal set
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
            internal set
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

        public Dictionary<string, object> Parameters { get; internal set; }

        public string Path { get; set; }

        public bool Checked { get; internal set; }

        T IPath.GetItem<T>(string pathToObject)
        {
            if (Path == pathToObject)
                return this as T;
            return null;
        }

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

        public Quality Write(string value)
        {
            return connector.WriteTag(Path, value);
        }

        public Task<Quality> WriteAsync(string value)
        {
            return connector.WriteTagAsync(Path, value);
        }
    }
}
