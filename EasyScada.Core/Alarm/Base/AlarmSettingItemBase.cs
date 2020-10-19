using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core
{
    public abstract class AlarmSettingItemBase : IUniqueNameItem
    {
        private AlarmClass alarmClass;
        private AlarmGroup alarmGroup;
        private SMSSetting smsSetting;
        private EmailSetting emailSetting;

        [Browsable(false)]
        [JsonIgnore]
        public AlarmClass AlarmClass
        {
            get
            {
                if (AlarmSetting != null)
                {
                    if (alarmClass != null)
                    {
                        if (alarmClass.Name != AlarmClassName)
                            alarmClass = AlarmSetting.AlarmClasses.FirstOrDefault(x => x.Name == AlarmClassName);
                    }
                    else
                    {
                        alarmClass = AlarmSetting.AlarmClasses.FirstOrDefault(x => x.Name == AlarmClassName);
                    }
                }
                return alarmClass;
            }
        }

        [Browsable(false)]
        [JsonIgnore]
        public AlarmGroup AlarmGroup
        {
            get
            {
                if (AlarmSetting != null)
                {
                    if (alarmClass != null)
                    {
                        if (alarmGroup.Name != AlarmGroupName)
                            alarmGroup = AlarmSetting.AlarmGroups.FirstOrDefault(x => x.Name == AlarmGroupName);
                    }
                    else
                    {
                        alarmGroup = AlarmSetting.AlarmGroups.FirstOrDefault(x => x.Name == AlarmGroupName);
                    }
                }
                return alarmGroup;
            }
        }

        [Browsable(false)]
        [JsonIgnore]
        public SMSSetting SMSSetting
        {
            get
            {
                if (AlarmGroup != null && AlarmSetting != null)
                {
                    if (smsSetting != null)
                    {
                        if (smsSetting.Name != AlarmGroup.SMSSettingName)
                            smsSetting = AlarmSetting.SMSSettings.FirstOrDefault(x => x.Name == AlarmGroup.SMSSettingName);
                    }
                    else
                    {
                        smsSetting = AlarmSetting.SMSSettings.FirstOrDefault(x => x.Name == AlarmGroup.SMSSettingName);
                    }
                }
                return smsSetting;
            }
        }

        [Browsable(false)]
        [JsonIgnore]
        public EmailSetting EmailSetting
        {
            get
            {
                if (AlarmGroup != null && AlarmSetting != null)
                {
                    if (emailSetting != null)
                    {
                        if (emailSetting.Name != AlarmGroup.EmailSettingName)
                            emailSetting = AlarmSetting.EmailSettings.FirstOrDefault(x => x.Name == AlarmGroup.EmailSettingName);
                    }
                    else
                    {
                        emailSetting = AlarmSetting.EmailSettings.FirstOrDefault(x => x.Name == AlarmGroup.EmailSettingName);
                    }
                }
                return emailSetting;
            }
        }

        [JsonIgnore]
        public virtual bool UseDirectTriggerValue { get; set; }
        [JsonIgnore]
        public virtual Func<string> GetTriggerValueFunc { get; set; }

        public virtual string Name { get; set; }
        public virtual bool Enabled { get; set; }
        public virtual string AlarmText { get; set; }
        public virtual string AlarmClassName { get; set; }
        public virtual string AlarmGroupName { get; set; }
        public virtual string Description { get; set; }
        public virtual string TriggerTagPath { get; set; }

        [JsonIgnore]
        public virtual bool? IsNormal { get; protected set; }

        [JsonIgnore]
        [Browsable(false)]
        public virtual AlarmSetting AlarmSetting { get; set; }

        private AlarmState alarmState = AlarmState.Normal;
        [Browsable(false)]
        public virtual AlarmState AlarmState
        {
            get => alarmState;
            set
            {
                if (alarmState != value)
                {
                    AlarmState oldState = alarmState;
                    alarmState = value;
                }
            }
        }

        private ITag triggerTag;
        [JsonIgnore]
        [Browsable(false)]
        public virtual ITag TriggerTag
        {
            get
            {
                if (triggerTag != null)
                {
                    if (triggerTag.Path != TriggerTagPath)
                        triggerTag = DriverConnector.GetTag(TriggerTagPath);
                }
                else
                {
                    triggerTag = DriverConnector.GetTag(TriggerTagPath);
                }
                return triggerTag;
            }
        }

        [JsonIgnore]
        [Browsable(false)]
        public IEasyDriverConnector DriverConnector
        {
            get => EasyDriverConnectorProvider.GetEasyDriverConnector();
        }

        public virtual void Refresh()
        {
            
        }

        public virtual event EventHandler<AlarmStateChangedEventArgs> AlarmStateChanged;
    }
}
