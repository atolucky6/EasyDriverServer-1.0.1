using EasyDriverPlugin.Converters;
using System;
using System.ComponentModel;

namespace EasyDriverPlugin
{
    [Serializable]
    [TypeConverter(typeof(ByteOrderConverter))]
    public enum ByteOrder
    {
        ABCD,
        CDAB,
        BADC,
        DCBA,
    }
}
