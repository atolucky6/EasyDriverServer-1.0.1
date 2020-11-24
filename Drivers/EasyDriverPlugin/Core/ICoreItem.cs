using System;
using System.ComponentModel;

namespace EasyDriverPlugin
{
    /// <summary>
    /// Đối tượng cơ bản nhất trong EasyDriver
    /// </summary>
    public interface ICoreItem : IDataErrorInfo, INotifyPropertyChanged, IChangeTracking
    {
        /// <summary>
        /// Tên của đối tượng
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Đường dẫn đến đối tượng
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Cấp độ của đối tượng trong cây
        /// </summary>
        int Level { get; }

        /// <summary>
        /// Thông tin hiển thị trên view
        /// </summary>
        string DisplayInformation { get; set; }

        /// <summary>
        /// Thời gian tạo đối tượng
        /// </summary>
        DateTime CreatedDate { get; set; }

        /// <summary>
        /// Thời gian lần cuối cùng chỉnh sữa đối tượng
        /// </summary>
        DateTime ModifiedDate { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Đối tượng cha chứa đối tượng này
        /// </summary>
        IGroupItem Parent { get; set; }

        /// <summary>
        /// Bit cho biết đối tượng chỉ được đọc hay không
        /// </summary>
        bool IsReadOnly { get; set; }

        /// <summary>
        /// Bit cho phép đối tượng hoạt động
        /// </summary>
        bool Enabled { get; set; }
     
        /// <summary>
        /// Xác định đối tượng này đang được check trên view
        /// </summary>
        bool? IsChecked { get; set; }

        /// <summary>
        /// Kiểu đối tượng
        /// </summary>
        ItemType ItemType { get; set; }

        /// <summary>
        /// Clone đối tượng và trả về kết quả. Hàm chỉ clone lại các kiểu dữ liệu nguyên thủy
        /// </summary>
        /// <returns></returns>
        ICoreItem ShallowCopy();

        /// <summary>
        /// Clone đối tượng và trả về kết quả. Hàm sẽ clone tất cả các thuộc tính
        /// </summary>
        /// <returns></returns>
        ICoreItem DeepCopy();

        /// <summary>
        /// Hàm kiểm tra đối tượng đã bị thay đổi hay chưa
        /// </summary>
        /// <returns></returns>
        bool HasChanges();

        /// <summary>
        /// Hàm lấy thông tin lỗi của thuộc tính
        /// </summary>
        /// <param name="propertyName">Tên thuộc tính cần lấy lỗi</param>
        /// <returns></returns>
        string GetErrorOfProperty(string propertyName);

        /// <summary>
        /// Cập nhật lại đường dẫn
        /// </summary>
        void RefreshPath();

        /// <summary>
        /// Khởi động sự kiện <see cref="Added"/>
        /// </summary>
        void RaiseAddedEvent();

        /// <summary>
        /// Khởi động sự kiện <see cref="Removed"/>
        /// </summary>
        void RaiseRemovedEvent();

        /// <summary>
        /// Sự kiện giá trị của <see cref="ITagCore"/> thay đổi
        /// </summary>
        event EventHandler<TagValueChangedEventArgs> ValueChanged;

        /// <summary>
        /// Sự kiện trạng thái của <see cref="ITagCore"/> thay đổi
        /// </summary>
        event EventHandler<TagQualityChangedEventArgs> QualityChanged;

        /// <summary>
        /// Sự kiện khi tên thay đổi
        /// </summary>
        event EventHandler<NameChangedEventArgs> NameChanged;

        /// <summary>
        /// Sự kiện khi đối tượng bị xóa khỏi parent
        /// </summary>
        event EventHandler Removed;

        /// <summary>
        /// Sự kiện khi đối tượng được thêm vào parent
        /// </summary>
        event EventHandler Added;

        /// <summary>
        /// Lấy trạng thái enabled thực tế của đối tượng
        /// </summary>
        /// <returns></returns>
        bool GetActualEnabledProperty();
    }
}
