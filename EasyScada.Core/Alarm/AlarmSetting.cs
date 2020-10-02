using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core
{
    public class AlarmSetting
    {
        public UniqueItemCollection<AlarmClass> AlarmClasses { get; set; }
        public UniqueItemCollection<AlarmGroup> AlarmGroups { get; set; }
        public UniqueItemCollection<DiscreteAlarm> DiscreteAlarms { get; set; }
        public UniqueItemCollection<AnalogAlarm> AnalogAlarms { get; set; }

        public AlarmSetting()
        {
            AlarmClasses = new UniqueItemCollection<AlarmClass>();
            AlarmGroups = new UniqueItemCollection<AlarmGroup>();
            DiscreteAlarms = new UniqueItemCollection<DiscreteAlarm>();
            AnalogAlarms = new UniqueItemCollection<AnalogAlarm>();
            DiscreteAlarms.CollectionChanged += DiscreteAlarms_CollectionChanged;
            AnalogAlarms.CollectionChanged += AnalogAlarms_CollectionChanged;
        }

        private void AnalogAlarms_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }

        private void DiscreteAlarms_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }
    }
}
