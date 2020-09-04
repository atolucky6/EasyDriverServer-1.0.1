using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core
{
    [Serializable]
    public enum StationType
    {
        Local,
        Remote,
        OPC_DA
    }
}
