using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core
{
    /// <summary>
    /// Custom type converter so that WriteMode values appear as neat text at design time.
    /// </summary>
    public class WriteTriggerConverter : StringLookupConverter
    {
        #region Static Fields
        private Pair[] _pairs = new Pair[] { new Pair(WriteTrigger.OnEnter,         "On Press Enter"),
                                             new Pair(WriteTrigger.LostFocus,       "On Lost Focus"),
                                             new Pair(WriteTrigger.ValueChanged,    "On Value Changed"),
        };
        #endregion

        #region Identity
        /// <summary>
        /// Initialize a new instance of the WriteModeConverter clas.
        /// </summary>
        public WriteTriggerConverter()
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
