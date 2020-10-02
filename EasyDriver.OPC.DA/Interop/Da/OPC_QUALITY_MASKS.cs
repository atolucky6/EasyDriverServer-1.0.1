using System;

namespace EasyDriver.Opc.DA.Client.Interop.Da
{
    [Flags]
    internal enum OPC_QUALITY_MASKS : short
    {
        LIMIT_MASK = (short) 3,
        STATUS_MASK = (short) 252,
        MASTER_MASK = (short) 192
    }
}