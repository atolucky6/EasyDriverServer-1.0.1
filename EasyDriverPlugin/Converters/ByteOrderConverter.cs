using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EasyDriverPlugin.Converters
{
    /// <summary>
    /// Custom type converter so that ButtonStyle values appear as neat text at design time.
    /// </summary>
    internal class ByteOrderConverter : StringLookupConverter
    {
        #region Static Fields
        private Pair[] _pairs = new Pair[] { new Pair(ByteOrder.ABCD,           "High Byte First / High Word First"),
                                             new Pair(ByteOrder.CDAB,           "High Byte First / Low Word First"),
                                             new Pair(ByteOrder.BADC,           "Low Byte First / High Word First"),
                                             new Pair(ByteOrder.DCBA,           "Low Byte First / Low Word First"),
        };
        #endregion

        #region Identity
        /// <summary>
        /// Initialize a new instance of the ButtonStyleConverter clas.
        /// </summary>
        public ByteOrderConverter()
            : base(typeof(ByteOrder))
        {
        }
        #endregion

        #region Protected
        /// <summary>
        /// Gets an array of lookup pairs.
        /// </summary>
        protected override Pair[] Pairs
        {
            get { return _pairs; }
        }
        #endregion
    }

    public class ByteOrderDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ByteOrder byteOrder)
            {
                switch (byteOrder)
                {
                    case ByteOrder.ABCD:
                        return "High Byte First / High Word First";
                    case ByteOrder.CDAB:
                        return "High Byte First / Low Word First";
                    case ByteOrder.BADC:
                        return "Low Byte First / High Word First";
                    case ByteOrder.DCBA:
                        return "Low Byte First / Low Word First";
                    default:
                        break;
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
