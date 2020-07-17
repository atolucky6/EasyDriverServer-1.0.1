using System;

namespace EasyScada.Winforms.Connector
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
