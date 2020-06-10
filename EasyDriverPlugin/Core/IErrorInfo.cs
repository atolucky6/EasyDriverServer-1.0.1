using System.Collections.ObjectModel;

namespace EasyDriverPlugin
{
    /// <summary>
    /// Interface đại diện cho thông tin lỗi của đối tượng
    /// </summary>
    public interface IErrorInfo
    {
        /// <summary>
        /// Tên của thuộc tính chứa lỗi này
        /// </summary>
        string PropertyName { get; set; }

        /// <summary>
        /// Danh sách các lỗi con
        /// </summary>
        ObservableCollection<IErrorInfo> Childs { get; set; }

        /// <summary>
        /// Mã lỗi
        /// </summary>
        int ErrorCode { get; set; }

        /// <summary>
        /// Tin của lỗi
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// Kiểu lỗi
        /// </summary>
        ErrorMessageType MessageType { get; set; }

        /// <summary>
        /// Đối tượng sở hữu lỗi này
        /// </summary>
        object Owner { get; set; }

        /// <summary>
        /// Clone đối tượng này
        /// </summary>
        /// <returns></returns>
        IErrorInfo Clone();

        /// <summary>
        /// Đếm số lỗi có trong danh sách lỗi con
        /// </summary>
        /// <returns></returns>
        int CountErrors();

        /// <summary>
        /// Đếm số cảnh báo có trong danh sách lỗi con
        /// </summary>
        /// <returns></returns>
        int CountWarning();

        /// <summary>
        /// Cập nhật các thuộc tính của lỗi này bằng 1 <see cref="IErrorInfo"/> khác
        /// </summary>
        /// <param name="errorInfo"></param>
        void Replace(IErrorInfo errorInfo);

        /// <summary>
        /// Tạo <see cref="IErrorInfo"/> mới và thêm vào danh sách con
        /// </summary>
        /// <returns></returns>
        IErrorInfo CreateAndAddToChilds(object owner = null);

        /// <summary>
        /// Xóa tất cả thông tin
        /// </summary>
        void Clear();
    }
}
