using System;

namespace EasyDriverPlugin
{
    /// <summary>
    /// Đối tượng đại diện cho thiết bị mà ta kết nối tới
    /// </summary>
    public interface IDeviceCore : IGroupItem, ISupportParameters, ISupportSynchronization, IHaveTag
    {
        /// <summary>
        /// Trạng thái kết nối
        /// </summary>
        ConnectionStatus ConnectionStatus { get; set; }
        
        /// <summary>
        /// Kiểu sắp xếp dữ liệu mà device này sử dụng
        /// </summary>
        ByteOrder ByteOrder { get; set; }

        /// <summary>
        /// Thời gian lần cuối cập nhật
        /// </summary>
        DateTime LastRefreshTime { get; set; }
    }
}
