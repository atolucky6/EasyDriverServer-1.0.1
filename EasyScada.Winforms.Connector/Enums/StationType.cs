﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Connector
{
    [Serializable]
    public enum StationType
    {
        Local,
        Remote,
        OPC_DA
    }
}