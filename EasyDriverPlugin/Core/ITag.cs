using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyDriverPlugin
{
    public interface ITag : IGroupItem, ISupportParameters, ISupportSynchronization
    {
        /// <summary>
        /// Tên hiển thị của <see cref="ITag"/>
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// Địa chỉ của <see cref="ITag"/>
        /// </summary>
        string Address { get; set; }

        /// <summary>
        /// Giá trị của <see cref="ITag"/>
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// Trạng thái của <see cref="ITag"/>
        /// </summary>
        Quality Quality { get; set; }

        /// <summary>
        /// Thời gian làm mới giá trị của <see cref="ITag"/>
        /// </summary>
        int RefreshRate { get; set; }

        /// <summary>
        /// Quyền truy cập đến <see cref="ITag"/>
        /// </summary>
        AccessPermission AccessPermission { get; set; }

        /// <summary>
        /// Thứ tự sắp xếp byte
        /// </summary>
        ByteOrder ByteOrder { get; set; }

        /// <summary>
        /// Thời gian lần cuối cập nhật đối tượng
        /// </summary>
        DateTime TimeStamp { get; set; }

        /// <summary>
        /// Thời gian giữa 2 lần cập nhật giá trị gần nhất
        /// </summary>
        TimeSpan RefreshInterval { get; set; }  

        double Gain { get; set; }

        double Offset { get; set; }

        IDataType DataType { get; set; }

        /// <summary>
        /// Kiểu dữ liệu 
        /// </summary>
        string DataTypeName { get; }

        /// <summary>
        /// Hàm lấy tất cả các <see cref="ITag"/> bên trong Tag này
        /// </summary>
        /// <returns></returns>
        IEnumerable<ITag> GetAllChildTag();

        /// <summary>
        /// Sự kiện giá trị của <see cref="ITag"/> thay đổi
        /// </summary>
        event EventHandler<TagValueChangedEventArgs> ValueChanged;

        /// <summary>
        /// Sự kiện trạng thái của <see cref="ITag"/> thay đổi
        /// </summary>
        event EventHandler<TagQualityChangedEventArgs> QualityChanged;

        Indexer<ITag> Tags { get; }
    }

    /// <summary>
    /// Quyền truy cập đến <see cref="ITag"/>
    /// </summary>
    [Serializable]
    public enum AccessPermission
    {
        /// <summary>
        /// Cho phép đọc và ghi dữ liệu <see cref="ITag"/>
        /// </summary>
        [Display(Name = "Read & Write")]
        ReadAndWrite,

        /// <summary>
        /// Chỉ cho phép đọc dữ liệu <see cref="ITag"/>
        /// </summary>
        [Display(Name = "Read Only")]
        ReadOnly,
    }

    /// <summary>
    /// Trạng thái kết nối của <see cref="IDevice"/> hoặc trạng thái của <see cref="ITag"/>
    /// </summary>
    [Serializable]
    public enum Quality
    {
        Uncertain,
        Bad,
        Good
    }

    /// <summary>
    /// Tham số của sự kiện trạng thái <see cref="ITag"/> thay đổi
    /// </summary>
    public class TagQualityChangedEventArgs : EventArgs
    {
        #region Members

        /// <summary>
        /// Trạng thái cũ
        /// </summary>
        public Quality OldQuality { get; private set; }

        /// <summary>
        /// Trạng thái mới
        /// </summary>
        public Quality NewQuality { get; private set; }

        #endregion

        #region Constructors

        public TagQualityChangedEventArgs(Quality oldQuality, Quality newQuality)
        {
            OldQuality = oldQuality;
            NewQuality = newQuality;
        }

        #endregion
    }

    /// <summary>
    /// Thàm số của sự kiện giá trị <see cref="ITag"/> thay đổi
    /// </summary>
    public class TagValueChangedEventArgs : EventArgs
    {
        #region Members

        /// <summary>
        /// Giá trị cũ
        /// </summary>
        public string OldValue { get; private set; }

        /// <summary>
        /// Giá trị mới
        /// </summary>
        public string NewValue { get; private set; }

        #endregion

        #region Constructors

        public TagValueChangedEventArgs(ITag tag, string oldValue, string newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        #endregion
    }
}
