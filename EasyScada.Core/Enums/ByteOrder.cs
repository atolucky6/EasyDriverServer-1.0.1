using System;

namespace EasyScada.Core
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
