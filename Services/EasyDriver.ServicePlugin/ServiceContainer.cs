using System;

namespace EasyDriver.ServicePlugin
{
    /// <summary>
    /// Dùng để chứa các <see cref="IEasyServicePlugin"/> và cung cấp tính năng lấy <see cref="IEasyServicePlugin"/>
    /// </summary>
    public class ServiceContainer
    {
        #region Singleton
        public static ServiceContainer Default { get; } = new ServiceContainer();
        #endregion

        #region Members
        /// <summary>
        /// Định nghĩa hàm lấy dịch vụ thông qua kiểu dữ liệu
        /// </summary>
        public Func<Type, IEasyServicePlugin> GetServiceFunc { get; set; }
        #endregion

        /// <summary>
        /// Hàm lấy dịch vụ bằng kiểu của dịch vụ đó. Kiểu của dịch vụ phải được kế thừa từ <see cref="IEasyServicePlugin"/>
        /// </summary>
        /// <typeparam name="T">Kiểu của dịch vụ đó. Có thể là interface hoặc là class/></typeparam>
        /// <returns>Trả về dịch vụ nếu có. Nếu không có thì kết quả trả về là null</returns>
        public T GetService<T>() where T : IEasyServicePlugin
        {
            return (T)GetServiceFunc?.Invoke(typeof(T));
        }
    }
}
