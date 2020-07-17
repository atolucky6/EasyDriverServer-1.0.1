using System;

namespace EasyScada.Winforms.Connector
{
    /// <summary>
    /// Trạng thái kết nối của Tag
    /// </summary>
    [Serializable]
    public enum Quality
    {
        Uncertain,
        Bad,
        Good
    }
}
