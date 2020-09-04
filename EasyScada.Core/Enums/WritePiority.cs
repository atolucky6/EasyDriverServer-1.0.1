using System;

namespace EasyScada.Core
{
    [Serializable]
    public enum WritePiority
    {
        Default,
        Medium,
        High,
        Highest,
    }
}
