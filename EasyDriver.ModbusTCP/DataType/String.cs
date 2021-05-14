using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDriver.ModbusTCP
{
    public class String : EasyDriverPlugin.String
    {
        public override bool TryParseToByteArray(object value, double gain, double offset, out byte[] buffer, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            buffer = null;
            if (value == null)
                return false;
            buffer = new byte[value.ToString().Length];
            ByteHelper.SetStringAt(buffer, 0, buffer.Length, value.ToString());
            return true;
        }

        public override string ConvertToValue(byte[] buffer, double gain, double offset, int pos = 0, int bit = 0, ByteOrder byteOrder = ByteOrder.ABCD)
        {
            int size = buffer.Length;
            return Encoding.ASCII.GetString(buffer, 0, size);
        }
    }
}
