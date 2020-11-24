using System.ComponentModel;

namespace EasyDriver.ServicePlugin
{
    /// <summary>
    /// Đối tượng cơ bản của một dịch vụ
    /// </summary>
    public interface IEasyServicePlugin : ISupportInitialize
    {
        /// <summary>
        /// Đối tượng dùng để chứa tất cả các dịch vụ bao gồm cả dịch vụ này và
        /// cung cấp tính năng lấy các dịch vụ khác chứa trong nó
        /// </summary>
        ServiceContainer ServiceContainer { get; }
    }
}
