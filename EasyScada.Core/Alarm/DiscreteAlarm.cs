using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core
{
    public class DiscreteAlarm : AlarmItemBase
    {
        public string TriggerValue { get; set; }
    }
}
