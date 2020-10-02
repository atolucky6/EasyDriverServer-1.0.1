using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Controls
{
    public interface ISupportWriteMultiTag
    {
        WriteTagCommandCollection WriteTagCommands { get; }
    }
}
