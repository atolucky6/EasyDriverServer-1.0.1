﻿using System;

namespace EasyDriverPlugin
{
    [Serializable]
    public class ULInt : DataTypeBase
    {
        #region Constructors

        public ULInt() : base("ULInt", 64) { RequireByteLength = 8; }

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
            return (ByteHelper.GetLIntAt(buffer, pos, byteOrder) * gain + offset).ToString();
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
                if (ulong.TryParse(dResult.ToString(), out ulong result))
                {
                    ByteHelper.SetULIntAt(buffer, 0, result, byteOrder);
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}
