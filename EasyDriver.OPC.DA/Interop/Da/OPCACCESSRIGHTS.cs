using System;

namespace EasyDriver.Opc.DA.Client.Interop.Da
{
    [Flags]
    internal enum OPCACCESSRIGHTS
    {
        OPC_READABLE = 1,
        OPC_WRITEABLE = 2
    }
}