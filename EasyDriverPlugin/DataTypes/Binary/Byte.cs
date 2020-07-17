using System;

namespace EasyDriverPlugin
{
    [Serializable]
    public class Byte : DataTypeBase
    {
        #region Constructors

        public Byte() : base("Byte", 8) { RequireByteLength = 1; }

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
            return (ByteHelper.GetByteAt(buffer, pos) * gain + offset).ToString();
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
            if (double.TryParse(value.ToString(), out double dResult))
            {
                dResult = (dResult - offset) / gain;
                if (byte.TryParse(dResult.ToString(), out byte result))
                {
                    ByteHelper.SetByteAt(buffer, 0, result);
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}
