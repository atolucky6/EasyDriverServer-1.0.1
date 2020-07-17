using System;

namespace EasyScada.Winforms.Connector
{
    [Serializable]
    public enum ByteOrder
    {
        ABCD,
        CDAB,
        BADC,
        DCBA,
    }
}
