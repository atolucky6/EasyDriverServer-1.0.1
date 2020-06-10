using System;

namespace EasyDriverPlugin
{
    /// <summary>
    /// Kiểu lỗi của <see cref="IErrorInfo"/>
    /// </summary>
    [Serializable]
    public enum ErrorMessageType
    {
        /// <summary>
        /// Không có gì
        /// </summary>
        None,
        /// <summary>
        /// Thông tin
        /// </summary>
        Information,
        /// <summary>
        /// Cảnh báo
        /// </summary>
        Warning,
        /// <summary>
        /// Lỗi
        /// </summary>
        Error
    }
}
