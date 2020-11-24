using System;

namespace EasyDriverPlugin
{
    /// <summary>
    /// Đối tượng channel dùng để xác định driver mà các child của nó sẽ xử lý
    /// </summary>
    public interface IChannelCore : IGroupItem, ISupportParameters, ISupportSynchronization
    {
        /// <summary>
        /// Trạng thái kết nối
        /// </summary>
        ConnectionStatus ConnectionStatus { get; set; }

        /// <summary>
        /// Tên hoặc đường dẫn đến driver
        /// </summary>
        string DriverPath { get; set; }

        /// <summary>
        /// Thời gian cập nhật lần cuối
        /// </summary>
        DateTime LastRefreshTime { get; set; }
    }
}
