using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDriver.OmronHostLink
{
    public class Channel : ChannelCore
    {
        public Channel(IGroupItem parent, bool isReadOnly = false) : base(parent, isReadOnly)
        {
        }
    }
}
