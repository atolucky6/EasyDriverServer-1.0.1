using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core
{
    public class DiscreteAlarm : AlarmSettingItemBase
    {
        public string TriggerValue { get; set; }

        public override event EventHandler<AlarmStateChangedEventArgs> AlarmStateChanged;

        public override void Refresh()
        {
            bool isVailid = false;
            string triggerValueStr = TriggerValue;
            string triggerTagValueStr = TriggerTag?.Value;

            if (UseDirectTriggerValue)
            {
                if (GetTriggerValueFunc != null)
                {
                    isVailid = true;
                    triggerTagValueStr = GetTriggerValueFunc();
                }
            }
            else
            {
                if (TriggerTag != null &&
                    TriggerTag.Quality == Quality.Good)  
                {
                    isVailid = true;
                }
            }

            if (isVailid && 
                double.TryParse(triggerValueStr, out double triggerValue) &&
                double.TryParse(triggerTagValueStr, out double triggerTagValue))
            {
                if (!IsNormal.HasValue)
                {
                    if (triggerTagValue == triggerValue)
                    {
                        IsNormal = false;
                        var oldState = AlarmState;
                        AlarmState = AlarmState.Incoming;
                        AlarmStateChanged?.Invoke(this, new AlarmStateChangedEventArgs(this, oldState, AlarmState, triggerTagValue.ToString(), triggerValue.ToString(), "Equal"));
                    }
                    else
                    {
                        IsNormal = true;
                        var oldState = AlarmState;
                        AlarmState = AlarmState.Outgoing;
                        AlarmStateChanged?.Invoke(this, new AlarmStateChangedEventArgs(this, oldState, AlarmState, triggerTagValue.ToString(), triggerValue.ToString(), "Equal"));
                    }
                }
                else
                {
                    if (IsNormal == true)
                    {
                        if (triggerTagValue == triggerValue)
                        {
                            IsNormal = false;
                            var oldState = AlarmState;
                            AlarmState = AlarmState.Incoming;
                            AlarmStateChanged?.Invoke(this, new AlarmStateChangedEventArgs(this, oldState, AlarmState, triggerTagValue.ToString(), triggerValue.ToString(), "Equal"));
                        }
                    }
                    else if (IsNormal == false)
                    {
                        if (triggerTagValue != triggerValue)
                        {
                            IsNormal = true;
                            var oldState = AlarmState;
                            AlarmState = AlarmState.Outgoing;
                            AlarmStateChanged?.Invoke(this, new AlarmStateChangedEventArgs(this, oldState, AlarmState, triggerTagValue.ToString(), triggerValue.ToString(), "Equal"));
                        }
                    }
                }
            }
        }
    }
}
