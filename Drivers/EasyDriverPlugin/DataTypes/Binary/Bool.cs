using System;

namespace EasyDriverPlugin
{
    [Serializable]
    public class Bool : DataTypeBase
    {
        #region Constructors

        public Bool() : base("Bool", 1) { RequireByteLength = 1; }

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
            return ByteHelper.GetBitAt(buffer, pos, bit) ? (gain + offset).ToString() : offset.ToString();
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

                switch (dResult)
                {
                    case 1:
                        ByteHelper.SetBitAt(ref buffer, 0, 0, true);
                        return true;
                    case 0:
                        ByteHelper.SetBitAt(ref buffer, 0, 0, false);
                        return true;
                    default:
                        break;
                }
            }
            return false;
        }

        #endregion
    }
}
