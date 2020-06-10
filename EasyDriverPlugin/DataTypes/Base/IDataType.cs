using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyDriverPlugin
{
    public interface IDataType 
    {

        string Name { get; set; }

        string Description { get; set; }

        /// <summary>
        /// Bit cho biết đây là kiểu dữ liệu tự định nghĩa hay là kiểu dữ liệu cơ bản
        /// </summary>
        bool IsUserDataType { get; }

        /// <summary>
        /// Độ dài dữ liệu tính theo bit của <see cref="IDataType"/>
        /// </summary>
        int BitLength { get; }

        /// <summary>
        /// Độ dài của chuỗi byte dùng đễ lưu trữ giá trị
        /// </summary>
        int RequireByteLength { get; set; }

        /// <summary>
        /// Hàm lấy tất cả các <see cref="IDataTypeIndex"/> trong <see cref="IDataType"/>
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDataTypeIndex> GetIndexs();

        /// <summary>
        /// Hàm chuyển giá trị thành chuỗi byte
        /// </summary>
        /// <param name="value"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        bool TryParseToByteArray(object value, double gaine, double offset, out byte[] buffer, ByteOrder byteOrder = ByteOrder.ABCD);

        /// <summary>
        /// Hàm chuyển đổi chuỗi byte thành giá trị
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="pos"></param>
        /// <param name="bit"></param>
        /// <returns></returns>
        string ConvertToValue(byte[] buffer, double gain, double offset, int pos = 0, int bit = 0, ByteOrder byteOrder = ByteOrder.ABCD);

        /// <summary>
        /// Chỉ mục đến <see cref="IDataTypeIndex"/>
        /// </summary>
        IDataTypeIndexer Indexs { get; }
    }

    /// <summary>
    /// Interface cung cấp các chỉ mục đến <see cref="IDataTypeIndex"/>
    /// </summary>
    public interface IDataTypeIndexer
    {
        IDataTypeIndex this[string name] { get; }
        IDataTypeIndex this[Func<IDataTypeIndex, bool> predicate] { get; }

        /// <summary>
        /// Cập nhật lại danh sách <see cref="ITag"/>
        /// </summary>
        /// <param name="tags"></param>
        void UpdateSource(List<IDataTypeIndex> tags);
    }

    /// <summary>
    /// Chỉ mục đến <see cref="IDataTypeIndex"/>
    /// </summary>
    public class DataTypeIndexer : IDataTypeIndexer
    {
        #region Members

        List<IDataTypeIndex> indexs;

        #endregion

        #region Indexers

        public IDataTypeIndex this[Func<IDataTypeIndex, bool> predicate] => indexs.FirstOrDefault(predicate);

        public IDataTypeIndex this[string name] => indexs.FirstOrDefault(x => x.Name == name);

        #endregion

        #region Constructors

        public DataTypeIndexer(List<IDataTypeIndex> tags) => indexs = tags;

        public DataTypeIndexer() { }

        #endregion

        #region Methods

        /// <summary>
        /// Cập nhật lại danh sách <see cref="IDataTypeIndex"/>
        /// </summary>
        /// <param name="indexs"></param>
        public void UpdateSource(List<IDataTypeIndex> indexs)
        {
            this.indexs = indexs;
        }

        #endregion
    }
}
