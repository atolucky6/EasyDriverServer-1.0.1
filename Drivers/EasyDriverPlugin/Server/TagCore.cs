using EasyDriverPlugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

        public bool IsInternalTag
        {
            get => GetProperty<bool>();
            set => SetProperty(value);
        }

        public string DisplayName
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        public string Address
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        public string DefaultValue
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        [field: NonSerialized]
        public bool NeedToUpdateValue;

        [field: NonSerialized]
        string value;
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
        public bool NeedToUpdateQuality;

        [field: NonSerialized]
        Quality quality = Quality.Uncertain;
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

        public int RefreshRate
        {
            get => GetProperty<int>();
            set => SetProperty(value);
        }

        public AccessPermission AccessPermission
        {
            get => GetProperty<AccessPermission>();
            set => SetProperty(value);
        }

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

        public string Unit
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        [JsonIgnore]
        public string CommunicationError { get; set; }

        public double WriteMaxLimit
        {
            get => GetProperty<double>();
            set => SetProperty(value);
        }

        public double WriteMinLimit
        {
            get => GetProperty<double>();
            set => SetProperty(value);
        }

        public bool EnabledWriteLimit
        {   
            get => GetProperty<bool>();
            set => SetProperty(value);
        }

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

        public override void PropertyChangedCallback(string propertyName, object oldValue, object newValue)
        {
            base.PropertyChangedCallback(propertyName, oldValue, newValue);
            switch (propertyName)
            {
                case nameof(IsInternalTag):
                case nameof(Retain):
                    {
                        if (this.IsInternalTag)
                        {
                            Address = Retain ? "Retain" : "Non-Retain";
                            DataTypeName = "String";
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        [field: NonSerialized]
        public event EventHandler Disposed;

        #endregion

        #region IHaveTags

        public bool HaveTags { get; set; }

        public NotifyCollection Tags { get; protected set; }

        #endregion
    }
}
