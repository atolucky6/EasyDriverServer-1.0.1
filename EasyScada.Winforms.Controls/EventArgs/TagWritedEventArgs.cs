using EasyScada.Winforms.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Controls
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
