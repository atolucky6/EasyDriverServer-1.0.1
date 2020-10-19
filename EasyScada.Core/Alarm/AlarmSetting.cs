using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core
{
    public class AlarmSetting
    {
        [Category("Easy Scada")]
        public bool Enabled { get; set; } = true;

        [Category("Easy Scada"), TypeConverter(typeof(CollectionEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public UniqueItemCollection<AlarmClass> AlarmClasses { get; set; }

        [Category("Easy Scada"), TypeConverter(typeof(CollectionEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public UniqueItemCollection<AlarmGroup> AlarmGroups { get; set; }

        [Category("Easy Scada"), TypeConverter(typeof(CollectionEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public AlarmSettingItemCollection<DiscreteAlarm> DiscreteAlarms { get; set; }

        [Category("Easy Scada"), TypeConverter(typeof(CollectionEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public AlarmSettingItemCollection<AnalogAlarm> AnalogAlarms { get; set; }

        [Category("Easy Scada"), TypeConverter(typeof(CollectionEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public AlarmSettingItemCollection<QualityAlarm> QualityAlarms { get; set; }

        [Category("Easy Scada"), TypeConverter(typeof(CollectionEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public UniqueItemCollection<EmailSetting> EmailSettings { get; set; }

        [Category("Easy Scada"), TypeConverter(typeof(CollectionEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public UniqueItemCollection<SMSSetting> SMSSettings { get; set; }

        [JsonConstructor]
        public AlarmSetting()
        {
            EmailSettings = new UniqueItemCollection<EmailSetting>();
            SMSSettings = new UniqueItemCollection<SMSSetting>();

            AlarmClasses = new UniqueItemCollection<AlarmClass>();
            AlarmGroups = new UniqueItemCollection<AlarmGroup>();

            DiscreteAlarms = new AlarmSettingItemCollection<DiscreteAlarm>(this);
            AnalogAlarms = new AlarmSettingItemCollection<AnalogAlarm>(this);
            QualityAlarms = new AlarmSettingItemCollection<QualityAlarm>(this);
        }

        public event EventHandler<AlarmStateChangedEventArgs> AlarmStateChagned;

        internal void RaiseAlarmStateChanged(object sender, AlarmStateChangedEventArgs e)
        {
            AlarmStateChagned?.Invoke(sender, e);
        }
    }
}
