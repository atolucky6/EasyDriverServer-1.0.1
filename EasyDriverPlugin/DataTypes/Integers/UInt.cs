using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace EasyDriverPlugin
{
    [Serializable]
    public class UInt : DataTypeBase
    {
        #region Constructors

        public UInt() : base("UInt", 16) { RequireByteLength = 2; }

        #endregion

        #region Methods

        /// <summary>
        /// Hàm chuyển đổi chuỗi byte thành giá trị
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="pos"></param>
        /// <param name="bit"></param>
        /// <returns></returns>
        public override string ConvertToValue(byte[] buffer, double gain, double offset, int pos = 0, int bit = 0, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            return ByteHelper.GetUIntAt(buffer, pos, byteOrder).ToString();
        }

        /// <summary>
        /// Hàm chuyển giá trị thành chuỗi byte
        /// </summary>
        /// <param name="value"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public override bool TryParseToByteArray(object value, double gain, double offset, out byte[] buffer, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            buffer = new byte[RequireByteLength];
            if (value == null)
                return false;
            if (ushort.TryParse(value.ToString(), out ushort result))
            {
                ByteHelper.SetUIntAt(buffer, 0, result, byteOrder);
                return true;
            }
            return false;
        }

        #endregion
    }
}
