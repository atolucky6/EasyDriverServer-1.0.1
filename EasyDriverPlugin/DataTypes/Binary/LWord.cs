using System;

namespace EasyDriverPlugin
{
    [Serializable]
    public class LWord : DataTypeBase
    {
        #region Constructors

        public LWord() : base("LWord", 64) { RequireByteLength = 8; }

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
            return ByteHelper.GetLWordAt(buffer, pos, byteOrder).ToString();
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
            if (ulong.TryParse(value.ToString(), out ulong result))
            {
                ByteHelper.SetLWordAt(buffer, 0, result, byteOrder);
                return true;
            }
            return false;
        }

        #endregion
    }
}
