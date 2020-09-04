using System;

namespace EasyScada.Core
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
