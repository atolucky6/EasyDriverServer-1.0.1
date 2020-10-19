using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core
{
    public class AlarmStateChangedEventArgs : EventArgs
    {
        public AlarmSettingItemBase AlarmItem { get; private set; }
        public DateTime OccurTime { get; private set; }
        public AlarmState OldState { get; private set; }
        public AlarmState NewState { get; private set; }
        public string Value { get; private set; }
        public string Limit { get; private set; }
        public string CompareMode { get; private set; }

        public AlarmStateChangedEventArgs(
            AlarmSettingItemBase alarmItem, 
            AlarmState oldState, 
            AlarmState newState,
            string value,
            string limit,
            string compareMode)
        {
            AlarmItem = alarmItem;
            OldState = oldState;
            NewState = newState;
            Value = value;
            Limit = limit;
            OccurTime = DateTime.Now;
            CompareMode = compareMode;
        }
    }
}
