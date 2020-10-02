﻿using System;

namespace EasyDriver.Opc.DA.Client.Interop.Common
{
    [Serializable]
    public enum RpcImpLevel
    {
        Default = 0,
        Anonymous = 1,
        Identify = 2,
        Impersonate = 3,
        Delegate = 4
    }
}
