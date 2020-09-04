using System;

namespace EasyScada.Core
{
    [Serializable]
    public enum WriteMode
    {
        WriteAllValue,
        WriteLatestValue,
    }
}
