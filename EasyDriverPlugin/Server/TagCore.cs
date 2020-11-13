using EasyDriverPlugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EasyDriverPlugin
{
    [Serializable]
    public class TagCore : GroupItemBase, ITagCore, IHaveTag
    {
        #region ITagCore

        public TagCore(IGroupItem parent, bool isReadOnly = false) : base(parent, isReadOnly)
        {
            SyncObject = new object();
            Tags = new NotifyCollection(this);
            ParameterContainer = new ParameterContainer();
            HaveTags = true;
            Gain = 1;
            Offset = 0;
        }

        public override ItemType ItemType { get; set; } = ItemType.Tag;

        public bool IsDisposed { get; set; }

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

        public bool Retain
        {
            get => GetProperty<bool>();
            set => SetProperty(value);
        }

        public string GUID
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        public string ExpressionString { get; set; }

        [field: NonSerialized]
        [JsonIgnore]
        public bool NeedToUpdateQuality;

        [field: NonSerialized]
        Quality quality = Quality.Uncertain;
        [JsonIgnore]
        public Quality Quality
        {
            get => IsInternalTag ? Quality.Good : quality;
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

        #endregion

        #region IHaveTags

        [JsonIgnore]
        public bool HaveTags { get; set; }

        [JsonIgnore]
        public NotifyCollection Tags { get; protected set; }

        #endregion
    }
}
