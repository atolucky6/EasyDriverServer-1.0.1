using System;

namespace EasyScada.Core
{
    public class TagWritedEventArgs : EventArgs
    {
        public ITag Tag { get; private set; }
        public Quality WriteQuality { get; private set; } 
        public string WriteValue { get; private set; }

        public TagWritedEventArgs(ITag tag, Quality writeQuality, string writeValue)
        {
            Tag = tag;
            WriteQuality = writeQuality;
            WriteValue = writeValue;
        }
    }
}
