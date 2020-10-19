using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core
{
    public class SentEmailMessage
    {
        public AlarmStateChangedEventArgs EventArgs { get; private set; }
        public int TryCount { get; set; }
        public SentEmailMessage(AlarmStateChangedEventArgs eventArgs)
        {
            EventArgs = eventArgs;
        }
    }
}
