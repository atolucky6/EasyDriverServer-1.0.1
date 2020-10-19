using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core
{
    public class SentSMSMessage
    {
        public AlarmStateChangedEventArgs EventArgs { get; private set; }
        public int TryCount { get; set; }
        public SentSMSMessage(AlarmStateChangedEventArgs eventArgs)
        {
            EventArgs = eventArgs;
        }
    }
}
