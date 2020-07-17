using System;

namespace EasyScada.Winforms.Connector
{
    /// <summary>
    /// Quyền truy cập đến Tag
    /// </summary>
    [Serializable]
    public enum AccessPermission
    {
        /// <summary>
        /// Cho phép đọc và ghi giá trị của Tag
        /// </summary>
        ReadAndWrite,

        /// <summary>
        /// Chỉ cho phép đọc giá trị của Tag
        /// </summary>
        ReadOnly,
    }
}
