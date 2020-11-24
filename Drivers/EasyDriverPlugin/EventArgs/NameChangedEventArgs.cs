using System;

namespace EasyDriverPlugin
{
    public class NameChangedEventArgs : EventArgs
    {
        public string OldValue { get; private set; }
        public string NewValue { get; private set; }
        public NameChangedEventArgs(string oldValue, string newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
