using System;
using System.Runtime.InteropServices;
using EasyDriver.Opc.DA.Client.Common;

namespace EasyDriver.Opc.DA.Client.Interop.Da
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