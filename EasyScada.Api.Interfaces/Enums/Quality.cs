using System;

namespace EasyScada.Api.Interfaces
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
