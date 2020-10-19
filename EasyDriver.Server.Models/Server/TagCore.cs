using EasyDriverPlugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EasyDriver.Core
{
    [Serializable]
    public class TagCore : GroupItemBase, ITagCore, IClientObject, IHaveTag, IClientTag
    {
        #region ITagCore

        public TagCore(IGroupItem parent, bool isReadOnly = false) : base(parent, isReadOnly)
        {
            SyncObject = new object();
            Tags = new TagCollection(this);
            ParameterContainer = new ParameterContainer();
            HaveTags = true;
            Tags = new TagCollection(this);
            Gain = 1;
            Offset = 0;
        }

        public bool IsDisposed { get; set; }

        private string name;
        public override string Name
        {
            get => name?.Trim();
            set
            {
                if (name != value)
                {
                    string oldName = name;
                    name = value;
                    RaisePropertyChanged();
                    NameChanged?.Invoke(this, new NameChangedEventArgs(oldName, value));
                }
            }
        }

        [JsonIgnore]
        public bool IsInternalTag
        {
            get => GetProperty<bool>();
            set => SetProperty(value);
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
                        var eventArgs = new TagValueChangedEventArgs(oldValue, value);
                        RaiseTagValueChanged(this, eventArgs);
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
                        var eventArgs = new TagQualityChangedEventArgs(oldQuality, quality);
                        RaiseTagQualityChanged(this, eventArgs);
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

        [JsonIgnore]
        public ConnectionStatus ConnectionStatus { get; set; }

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

        public override void ChildCollectionChangedCallback(NotifyCollectionChangedEventArgs e)
        {
            base.ChildCollectionChangedCallback(e);
            if (e.OldItems.Contains(this))
            {
                IsDisposed = true;
                Disposed?.Invoke(this, EventArgs.Empty);
            }
        }

        [field: NonSerialized]
        public event EventHandler Disposed;

        [field: NonSerialized]
        public event EventHandler<NameChangedEventArgs> NameChanged;

        #endregion

        #region IHaveTags

        [JsonIgnore]
        public bool HaveTags { get; set; }

        [JsonIgnore]
        public TagCollection Tags { get; protected set; }

        #endregion

        #region IClientObject

        [JsonProperty("Name")]
        string IClientObject.Name => Name;

        [JsonProperty("Path")]
        string IClientObject.Path => Path;

        [JsonProperty("Description")]
        string IClientObject.Description => Description;

        [JsonProperty("Error")]
        string IClientObject.Error => CommunicationError;

        [JsonProperty("ItemType")]
        ItemType IClientObject.ItemType => ItemType.Tag;

        [JsonProperty("Childs")]
        List<IClientObject> IClientObject.Childs => this.GetClientObjects();

        [JsonProperty("DisplayInfo")]
        string IClientObject.DisplayInfo => "";

        [JsonProperty("ConnectionStatus")]
        ConnectionStatus IClientObject.ConnectionStatus => ConnectionStatus;

        [field: NonSerialized]
        private Dictionary<string, string> properties;
        public Dictionary<string, string> Properties
        {
            get
            {
                if (properties == null)
                    properties = new Dictionary<string, string>();
                properties["Value"] = Value;
                properties["Quality"] = Quality.ToString();
                properties["DataType"] = DataTypeName;
                properties["AccessPermission"] = AccessPermission.ToString();
                properties["TimeStamp"] = TimeStamp.ToString();
                properties["IsInternalTag"] = IsInternalTag.ToString();
                return properties;
            }
        }

        #endregion

        #region IClientTag

        string IClientTag.Name => Name;

        string IClientTag.Path => Path;

        string IClientTag.Error => CommunicationError;

        List<IClientTag> IClientTag.Childs => this.GetClientTags();

        string IClientTag.Value => Value;

        Quality IClientTag.Quality => Quality;

        DateTime IClientTag.TimeStamp => TimeStamp;

        #endregion

    }
}
