using EasyDriverPlugin;
using EasyScada.Api.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace EasyScada.Core
{
    [Serializable]
    public class TagCore : GroupItemBase, ITagCore, ITag
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
        string value;
        [JsonIgnore]
        public string Value
        {
            get => value;
            set
            {
                if (this.value != value)
                {
                    string oldValue = this.value;
                    this.value = value;
                    RaisePropertyChanged();
                    ValueChanged?.Invoke(this, new TagValueChangedEventArgs(oldValue, value));
                }
            }
        }

        [field: NonSerialized]
        Quality quality = Quality.Uncertain;
        [JsonIgnore]
        public Quality Quality
        {
            get => quality;
            set
            {
                if (quality != value)
                {
                    Quality oldQuality = quality;
                    quality = value;
                    RaisePropertyChanged();
                    QualityChanged?.Invoke(this, new TagQualityChangedEventArgs(oldQuality, quality));
                }
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
        DateTime timeStamp;
        [JsonIgnore]
        public DateTime TimeStamp
        {
            get => timeStamp;
            set
            {
                if (timeStamp != value)
                {
                    timeStamp = value;
                    RaisePropertyChanged();
                }
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
                if (refreshInterval != value)
                {
                    refreshInterval = value;
                    RaisePropertyChanged();
                }
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

        [field:NonSerialized]
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
        string ITag.Name => Name;

        [JsonProperty("Path")]
        string IPath.Path => Path;

        [JsonProperty("Address")]
        string ITag.Address => Address;

        [JsonProperty("DataType")]
        string ITag.DataType => DataTypeName;

        [JsonProperty("Value")]
        string ITag.Value => Value;

        [JsonProperty("Quality")]
        Quality ITag.Quality => Quality;

        [JsonProperty("AccessPermission")]
        AccessPermission ITag.AccessPermission => AccessPermission;

        [JsonProperty("RefreshRate")]
        int ITag.RefreshRate => RefreshRate;

        [JsonProperty("RefreshInterval")]
        int ITag.RefreshInterval => RefreshInterval;

        [JsonProperty("Error")]
        string ITag.Error => CommunicationError;

        [JsonProperty("LastRefreshTime")]
        DateTime ITag.TimeStamp => TimeStamp;

        [JsonProperty("Parameters")]
        Dictionary<string, object> ITag.Parameters => ParameterContainer?.Parameters;

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
