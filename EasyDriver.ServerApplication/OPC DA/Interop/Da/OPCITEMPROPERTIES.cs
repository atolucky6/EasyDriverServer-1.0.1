using System;
using System.Runtime.InteropServices;
using EasyDriver.Opc.Client.Common;

namespace EasyDriver.Opc.Client.Interop.Da
{
    [ComConversionLoss]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct OPCITEMPROPERTIES
    {
        public HRESULT hrErrorID;
        public int dwNumProperties;
        [ComConversionLoss] public IntPtr pItemProperties;
        public int dwReserved;
    }
}