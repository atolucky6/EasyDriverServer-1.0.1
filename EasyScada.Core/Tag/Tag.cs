using EasyDriverPlugin;
using EasyScada.Api.Interfaces;
using System;
using System.Collections.Generic;

namespace EasyScada.Core
{
    [Serializable]
    public class Tag : GroupItemBase, ITagCore, ITag
    {
        #region ITagCore

        public Tag(IGroupItem parent, bool isReadOnly = false) : base(parent, isReadOnly)
        {
            SyncObject = new object();
            ParameterContainer = new ParameterContainer();
            Tags = new Indexer<ITagCore>(this);
            Gain = 1;
            Offset = 0;
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

        [field: NonSerialized]
        string value;
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

        DateTime timeStamp;
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

        TimeSpan refreshInterval;
        public TimeSpan RefreshInterval
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

        public object SyncObject { get; protected set; }

        public Indexer<ITagCore> Tags { get; protected set; }

        public IParameterContainer ParameterContainer { get; set; }

        public double Gain
        {
            get => GetProperty<double>();
            set => SetProperty(value);
        }

        public double Offset
        {
            get => GetProperty<double>();
            set => SetProperty(value);
        }

        public string DataTypeName => DataType?.Name;

        public string CommunicationError { get; set; }

        public IDataType DataType
        {
            get => GetProperty<IDataType>();
            set
            {
                SetProperty(value);
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

        string ITag.Name => Name;

        string ITag.Address => Address;

        string ITag.DataType => DataTypeName;

        string ITag.Value => Value;

        string ITag.Quality => Quality.ToString();

        int ITag.RefreshRate => RefreshRate;

        int ITag.RefreshInterval => (int)RefreshInterval.TotalMilliseconds;

        string ITag.Error => CommunicationError;

        DateTime ITag.LastRefreshTime => TimeStamp;

        Dictionary<string, object> ITag.Parameters => throw new NotImplementedException();

        #endregion
    }
}
