using System;

namespace EasyDriverPlugin
{
    [Serializable]
    public class UDInt : DataTypeBase
    {
        #region Constructors

        public UDInt() : base("UDInt", 32) { RequireByteLength = 4; }

        #endregion

        #region Methods

        /// <summary>
        /// Hàm chuyển đổi chuỗi byte thành giá trị
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="pos"></param>
        /// <param name="bit"></param>
        /// <returns></returns>
        public override string ConvertToValue(byte[] buffer, int pos = 0, int bit = 0, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            return ByteHelper.GetUDIntAt(buffer, pos, byteOrder).ToString();
        }

        /// <summary>
        /// Hàm chuyển giá trị thành chuỗi byte
        /// </summary>
        /// <param name="value"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public override bool TryParseToByteArray(object value, out byte[] buffer, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            buffer = new byte[RequireByteLength];
            if (value == null)
                return false;
            if (uint.TryParse(value.ToString(), out uint result))
            {
                ByteHelper.SetUDIntAt(buffer, 0, result, byteOrder);
                return true;
            }
            return false;
        }

        #endregion
    }
}
