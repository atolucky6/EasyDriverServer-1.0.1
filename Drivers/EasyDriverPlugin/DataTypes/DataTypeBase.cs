using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace EasyDriverPlugin
{
    /// <summary>
    /// Kiểu dữ liệu cơ bản
    /// </summary>
    [Serializable]
    public abstract class DataTypeBase : IDataType
    {
        public DataTypeBase(string name, int bitLength)
        {
            Name = name;
            BitLength = bitLength;
        }

        [Category("General"), DisplayName("Name")]
        public virtual string Name { get; set; }

        [Category("General"), DisplayName("Description")]
        public virtual string Description { get; set; }

        [Category("General"), DisplayName("Bit length")]
        public virtual int BitLength { get; protected set; }

        [Category("General"), DisplayName("Byte length")]
        public virtual int RequireByteLength { get; set; }

        [Browsable(false)]
        public virtual bool IsUserDataType { get; protected set; }

        IDataTypeIndexer indexs;
        public virtual IDataTypeIndexer Indexs
        {
            get
            {
                if (indexs == null)
                    indexs = new DataTypeIndexer();
                indexs.UpdateSource(GetIndexs().ToList());
                return indexs;
            }
        }

        public virtual IEnumerable<IDataTypeIndex> GetIndexs() => null;
        public abstract string ConvertToValue(byte[] buffer, double gain, double offset, int pos = 0, int bit = 0, ByteOrder byteOrder = ByteOrder.ABCD);
        public abstract bool TryParseToByteArray(object value, double gain, double offset, out byte[] buffer, ByteOrder byteOrder = ByteOrder.ABCD);
    }
}
