using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDriverPlugin
{
    public class ParameterChangedEventArgs : EventArgs
    {
        public KeyValuePair<string, string> KeyValue { get; private set; }
        public ParameterChangedEventArgs(KeyValuePair<string, string> keyValue)
        {
            KeyValue = keyValue;
        }
    }
}
