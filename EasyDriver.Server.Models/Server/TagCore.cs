using EasyDriverPlugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyDriver.Core
{
    [Serializable]
    public class TagCore : GroupItemBase, ITagCore, ITagClient
    {
        #region ITagCore

        public TagCore(IGroupItem parent, bool isReadOnly = false) : base(parent, isReadOnly)
        {
            SyncObject = new object();
            ParameterContainer = new ParameterContainer();
            Tags = new Indexer<ITagCore>(this);
            Gain = 1;
            Offset = 0;
        }

        [JsonIgnore]
        public string DisplayName
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        [JsonIgnore]
        public string Address
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        [field: NonSerialized]
        [JsonIgnore]
        public bool NeedToUpdateValue;

        [field: NonSerialized]
        string value;
        [JsonIgnore]
        public string Value
        {
            get => value;
            set
            {
                try
                {
                    if (this.value != value)
                    {
                        string oldValue = this.value;
                        this.value = value;
                        NeedToUpdateValue = true;
                        ValueChanged?.Invoke(this, new TagValueChangedEventArgs(oldValue, value));
                    }
                }
                catch { }
            }
        }

        [field: NonSerialized]
        [JsonIgnore]
        public bool NeedToUpdateQuality;

        [field: NonSerialized]
        Quality quality = Quality.Uncertain;
        [JsonIgnore]
        public Quality Quality
        {
            get => quality;
            set
            {
                try
                {
                    if (quality != value)
                    {
                        Quality oldQuality = quality;
                        quality = value;
                        NeedToUpdateQuality = true;
                        QualityChanged?.Invoke(this, new TagQualityChangedEventArgs(oldQuality, quality));
                    }
                }
                catch { }
            }
        }

        [JsonIgnore]
        public int RefreshRate
        {
            get => GetProperty<int>();
            set => SetProperty(value);
        }

        [JsonIgnore]
        public AccessPermission AccessPermission
        {
            get => GetProperty<AccessPermission>();
            set => SetProperty(value);
        }

        [JsonIgnore]
        public ByteOrder ByteOrder
        {
            get => GetProperty<ByteOrder>();
            set => SetProperty(value);
        }

        [field: NonSerialized]
        [JsonIgnore]
        public bool NeedToUpdateTimeStamp;

        [field: NonSerialized]
        DateTime timeStamp;
        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{}{0:HH:mm:ss}")]
        public DateTime TimeStamp
        {
            get => timeStamp;
            set
            {
                try
                {
                    if (timeStamp != value)
                    {
                        timeStamp = value;
                        NeedToUpdateTimeStamp = true;
                    }
                }
                catch { }
            }
        }

        [field: NonSerialized]
        int refreshInterval;
        [JsonIgnore]
        public int RefreshInterval
        {
            get => refreshInterval;
            set
            {
                try
                {
                    if (refreshInterval != value)
                    {
                        refreshInterval = value;
                    }
                }
                catch { }
            }
        }

        [JsonIgnore]
        public object SyncObject { get; protected set; }

        [JsonIgnore]
        public Indexer<ITagCore> Tags { get; protected set; }

        [JsonIgnore]
        public IParameterContainer ParameterContainer { get; set; }

        [JsonIgnore]
        public double Gain
        {
            get => GetProperty<double>();
            set => SetProperty(value);
        }

        [JsonIgnore]
        public double Offset
        {
            get => GetProperty<double>();
            set => SetProperty(value);
        }

        string dataTypeName;
        [JsonIgnore]
        public string DataTypeName
        {
            get
            {
                if (DataType != null)
                    return DataType.Name;
                return dataTypeName;
            }
            set
            {
                if (dataTypeName != value)
                {
                    dataTypeName = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonIgnore]
        public string CommunicationError { get; set; }

        [JsonIgnore]
        public IDataType DataType
        {
            get => GetProperty<IDataType>();
            set
            {
                SetProperty(value);
                RaisePropertyChanged();
                RaisePropertyChanged("DataTypeName");
            }
        }

        public IEnumerable<ITagCore> GetAllChildTag()
        {
            return null;
        }

        public override string GetErrorOfProperty(string propertyName)
        {
            return string.Empty;
        }

        public override void GetErrors(ref IErrorInfo errorInfo)
        {
        }

        public event EventHandler<TagValueChangedEventArgs> ValueChanged;
        public event EventHandler<TagQualityChangedEventArgs> QualityChanged;

        #endregion

        #region ITag

        [JsonProperty("Name")]
        string ITagClient.Name => Name;

        [JsonProperty("Path")]
        string IPath.Path => Path;

        [JsonProperty("Address")]
        string ITagClient.Address => Address;

        [JsonProperty("DataType")]
        string ITagClient.DataType => DataTypeName;

        [JsonProperty("Value")]
        string ITagClient.Value => Value;

        [JsonProperty("Quality")]
        Quality ITagClient.Quality => Quality;

        [JsonProperty("AccessPermission")]
        AccessPermission ITagClient.AccessPermission => AccessPermission;

        [JsonProperty("RefreshRate")]
        int ITagClient.RefreshRate => RefreshRate;

        [JsonProperty("RefreshInterval")]
        int ITagClient.RefreshInterval => RefreshInterval;

        [JsonProperty("Error")]
        string ITagClient.Error => CommunicationError;

        [JsonProperty("TimeStamp")]
        DateTime ITagClient.TimeStamp => TimeStamp;

        [JsonProperty("Parameters")]
        Dictionary<string, object> ITagClient.Parameters => ParameterContainer?.Parameters;

        public T GetItem<T>(string pathToObject) where T : class, IPath
        {
            if (string.IsNullOrWhiteSpace(pathToObject))
                return null;
            if (Path == pathToObject)
                return this as T;
            if (pathToObject.StartsWith(Path))
            {
                foreach (var child in Childs)
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

        #endregion
    }
}
