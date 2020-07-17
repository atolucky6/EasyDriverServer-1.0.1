using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Controls
{
    /// <summary>
    /// Custom type converter so that WriteMode values appear as neat text at design time.
    /// </summary>
    internal class WriteModeConverter : StringLookupConverter
    {
        #region Static Fields
        private Pair[] _pairs = new Pair[] { new Pair(WriteMode.OnEnter,         "Write when press enter"),
                                             new Pair(WriteMode.LostFocus,       "Write when lost focus"),
                                             new Pair(WriteMode.ValueChanged,    "Write when value changed")
        };
        #endregion

        #region Identity
        /// <summary>
        /// Initialize a new instance of the WriteModeConverter clas.
        /// </summary>
        public WriteModeConverter()
            : base(typeof(WriteMode))
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
