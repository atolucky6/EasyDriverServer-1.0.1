using System;

namespace EasyDriverPlugin
{
    [Serializable]
    public class Real : DataTypeBase
    {
        #region Constructors

        public Real() : base("Real", 32) { RequireByteLength = 4; }

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
            //string abcd = string.Format("{0:0.000}", ByteHelper.GetRealAt(buffer, pos, ByteOrder.ABCD) * gain + offset);
            //string badc = string.Format("{0:0.000}", ByteHelper.GetRealAt(buffer, pos, ByteOrder.BADC) * gain + offset);
            //string cdab = string.Format("{0:0.000}", ByteHelper.GetRealAt(buffer, pos, ByteOrder.CDAB) * gain + offset);
            //string dcba = string.Format("{0:0.000}", ByteHelper.GetRealAt(buffer, pos, ByteOrder.DCBA) * gain + offset);
            return string.Format("{0:0.000}", ByteHelper.GetRealAt(buffer, pos, byteOrder) * gain + offset);
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
                if (float.TryParse(dResult.ToString(), out float result))
                {
                    ByteHelper.SetRealAt(buffer, 0, result, byteOrder);
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}
