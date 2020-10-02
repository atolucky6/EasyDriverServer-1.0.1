using System.Collections.Generic;

namespace EasyDriverPlugin
{
    /// <summary>
    /// Interface định nghĩa các tham số cần thiết
    /// </summary>
    public interface IParameterContainer
    {
        /// <summary>
        /// Tên hiển thị
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// Hiển thị sơ lược tham số
        /// </summary>
        string DisplayParameters { get; set; }

        /// <summary>
        /// Dictionary lưuu trữ các thông số
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> Parameters { get; set; }
    }
}
