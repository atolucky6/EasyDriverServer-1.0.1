using System.Runtime.InteropServices;
using EasyDriver.Opc.DA.Client.Common;

namespace EasyDriver.Opc.DA.Client.Interop.Da
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct OPCITEMPROPERTY
    {
        public short vtDataType;
        public short wReserved;
        public int dwPropertyID;
        [MarshalAs(UnmanagedType.LPWStr)] public string szItemID;
        [MarshalAs(UnmanagedType.LPWStr)] public string szDescription;
        [MarshalAs(UnmanagedType.Struct)] public object vValue;
        public HRESULT hrErrorID;
        public int dwReserved;
    }
}