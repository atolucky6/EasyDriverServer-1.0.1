using System;

namespace EasyDriverPlugin
{
    [Serializable]
    public class Char : DataTypeBase
    {
        #region Constructors

        public Char() : base("Char", 8) { RequireByteLength = 1; }

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
            return ByteHelper.GetCharsAt(buffer, pos, 1);
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
            if (char.TryParse(value.ToString(), out char result))
            {
                ByteHelper.SetCharsAt(buffer, 0, result.ToString());
                return true;
            }
            return false;
        }

        #endregion
    }
}
