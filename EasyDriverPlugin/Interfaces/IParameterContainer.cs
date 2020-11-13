using System;
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

        /// <summary>
        /// Hàm lấy giá trị từ parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetValue<T>(string key) where T : IConvertible;

        /// <summary>
        /// Hàm lấy giá trị từ parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGetValue<T>(string key, out T value) where T : IConvertible;

        /// <summary>
        /// Hàm thêm key value pair vào parameters
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetValue(string key, string value);

        /// <summary>
        /// Hàm kiểm tra key có tồn tại trong parameters không
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Contains(string key);
    }
}
