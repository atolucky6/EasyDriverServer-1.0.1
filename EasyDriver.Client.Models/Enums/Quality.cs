using System;

namespace EasyDriver.Client.Models
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
