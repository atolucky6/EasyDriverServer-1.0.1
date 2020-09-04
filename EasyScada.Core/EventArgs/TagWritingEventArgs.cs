using System;

namespace EasyScada.Core
{
    public class TagWritingEventArgs : EventArgs
    {
        public ITag Tag { get; private set; }
        public string WriteValue { get; private set; }
        public TagWritingEventArgs(ITag tag, string writeValue)
        {
            Tag = tag;
            WriteValue = writeValue;
        }
    }
}
