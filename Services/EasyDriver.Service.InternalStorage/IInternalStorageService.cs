using EasyDriver.ServicePlugin;
using System;

namespace EasyDriver.Service.InternalStorage
{
    /// <summary>
    /// Đối tượng hỗ trợ việc lưu dữ liệu vào internal lite database
    /// </summary>
    public interface IInternalStorageService : IEasyServicePlugin
    {
        /// <summary>
        /// Thêm mới hoặc lưu giá trị vào database 
        /// </summary>
        /// <param name="guidID">GUID sẽ làm khóa chính</param>
        /// <param name="value">Giá trị cần lưu</param>
        /// <returns></returns>
        bool AddOrUpdateStoreValue(string guidID, string value);

        /// <summary>
        /// Xóa giá trị khỏi database
        /// </summary>
        /// <param name="guid">GUID id của đối tượng cần xóa</param>
        /// <returns></returns>
        bool RemoveStoreValue(string guid);

        /// <summary>
        /// Lấy giá trị đã lưu trự trong database
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        StoreValue GetStoreValue(string guid);
    }
}
