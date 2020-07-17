using System;

namespace EasyDriverPlugin
{
    [Serializable]
    public class DInt : DataTypeBase
    {
        #region Constructors

        public DInt() : base("DInt", 32) { RequireByteLength = 4; }

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
            return (ByteHelper.GetDIntAt(buffer, pos, byteOrder) * gain + offset).ToString();
        }

        public override bool TryParseToByteArray(object value, double gain, double offset, out byte[] buffer, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            buffer = new byte[RequireByteLength];
            if (value == null)
                return false;
            if (double.TryParse(value.ToString(), out double dResult))
            {
                dResult = (dResult - offset) / gain;
                if (int.TryParse(dResult.ToString(), out int result))
                {
                    ByteHelper.SetDIntAt(buffer, 0, result, byteOrder);
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}
