namespace EasyDriverPlugin
{
    /// <summary>
    /// Chỉ mục của <see cref="IDataType"/>
    /// </summary>
    public interface IDataTypeIndex
    {
        string Name { get; set; }

        string Description { get; set; }

        /// <summary>
        /// Kiểu dữ liệu của <see cref="IDataTypeIndex"/>
        /// </summary>
        IDataType DataType { get; set; }

        /// <summary>
        /// Địa chỉ của <see cref="IDataTypeIndex"/> trong <see cref="IDataType"/> chứa nó
        /// </summary>
        string Address { get; set; }
    }
}
