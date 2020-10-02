using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core
{
    public class AnalogAlarm : AlarmItemBase
    {
        public string Limit { get; set; }
        public LimitMode LimitMode { get; set; }
        public int Delay { get; set; }
        public TimeUnit TimeUnit { get; set; }
        public DeadbandMode DeadbandMode { get; set; }
        public string DeadbandValue { get; set; }
        public bool DeadbandInPercentage { get; set; }
    }
}
