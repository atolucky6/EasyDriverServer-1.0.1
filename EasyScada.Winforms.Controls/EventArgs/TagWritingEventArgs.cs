using EasyScada.Winforms.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Controls
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
