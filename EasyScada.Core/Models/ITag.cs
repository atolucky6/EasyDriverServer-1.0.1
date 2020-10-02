using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace EasyScada.Core
{
    public interface ITag : INotifyPropertyChanged, ICoreItem
    {
        string Address { get; }
        string DataType { get; }
        string Value { get; set; }
        Quality Quality { get; }
        int RefreshRate { get; }
        int RefreshInterval { get; }
        AccessPermission AccessPermission { get; }
        DateTime TimeStamp { get; }
        WriteResponse Write(string value);
        Task<WriteResponse> WriteAsync(string value);

        bool GetValue<T>(out T value) where T : IConvertible;
    }

    [Serializable]
    internal class Tag : CoreItem, ITag
    {
        public Tag() : base()
        {
            ItemType = ItemType.Tag;
        }

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
                        RaiseValueChanged(this, args);
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
                        RaiseQualityChanged(this, args);
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

        public bool GetValue<T>(out T value) where T : IConvertible
        {
            bool result = false;
            value = default;
            try
            {
                value = (T)Convert.ChangeType(Value, typeof(T));
            }
            catch { }
            return result;
        }

        internal void UpdateValue(ClientTag clientTag)
        {
            if (clientTag == null)
            {
                Quality = Quality.Bad;
                TimeStamp = DateTime.Now;
                Error = "Tag was not found on server.";

                if (Childs != null)
                {
                    foreach (var item in Childs)
                    {
                        if (item is Tag childTag)
                            childTag.UpdateValue(null);
                    }
                }
            }
            else
            {
                Value = clientTag.Value;
                Quality = clientTag.Quality;
                TimeStamp = clientTag.TimeStamp;
                Error = clientTag.Error;

                if (clientTag.Childs != null && Childs != null)
                {
                    foreach (var item in Childs)
                    {
                        if (item is Tag childTag)
                        {
                            var childClientTag = clientTag.Childs.FirstOrDefault(x => x.Path == childTag.Path);
                            if (childClientTag != null)
                                childTag.UpdateValue(childClientTag);
                        }
                    }
                }
            }
        }
    }
}
