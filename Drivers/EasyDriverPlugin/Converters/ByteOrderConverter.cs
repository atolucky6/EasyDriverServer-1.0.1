using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
