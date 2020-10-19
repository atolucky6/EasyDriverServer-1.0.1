using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EasyScada.Core
{
    public class AlarmItem : INotifyPropertyChanged
    {
        DateTime incommingTime;
        string name;
        string alarmClass;
        string alarmGroup;
        string triggerTag;
        string value;
        string limit;
        string compareMode;
        string state;
        DateTime outgoingTime;
        DateTime ackTime;
        string alarmType;
        string alarmText;

        public DateTime IncommingTime
        {
            get => incommingTime;
            set
            {
                if (incommingTime != value)
                {
                    incommingTime = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string AlarmClass
        {
            get => alarmClass;
            set
            {
                if (alarmClass != value)
                {
                    alarmClass = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string AlarmGroup
        {
            get => alarmGroup;
            set
            {
                if (alarmGroup != value)
                {
                    alarmGroup = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string TriggerTag
        {
            get => triggerTag;
            set
            {
                if (triggerTag != value)
                {
                    triggerTag = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string Value
        {
            get => value;
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string Limit
        {
            get => limit;
            set
            {
                if (limit != value)
                {
                    limit = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string CompareMode
        {
            get => compareMode;
            set
            {
                if (compareMode != value)
                {
                    compareMode = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string State
        {
            get => state;
            set
            {
                if (state != value)
                {
                    state = value;
                    RaisePropertyChanged();
                }
            }
        }
        public DateTime OutgoingTime
        {
            get => outgoingTime;
            set
            {
                if (outgoingTime != value)
                {
                    outgoingTime = value;
                    RaisePropertyChanged();
                }
            }
        }
        public DateTime AckTime
        {
            get => ackTime;
            set
            {
                if (ackTime != value)
                {
                    ackTime = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string AlarmType
        {
            get => alarmType;
            set
            {
                if (alarmType != value)
                {
                    alarmType = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string AlarmText
        {
            get => alarmText;
            set
            {
                if (alarmText != value)
                {
                    alarmText = value;
                    RaisePropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string propertyname = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
