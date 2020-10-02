// *****************************************************************************
// 
//  © Component Factory Pty Ltd 2017. All rights reserved.
//	The software and associated documentation supplied hereunder are the 
//  proprietary information of Component Factory Pty Ltd, 13 Swallows Close, 
//  Mornington, Vic 3931, Australia and are supplied subject to licence terms.
// 
//  Version 4.6.0.0 	www.ComponentFactory.com
// *****************************************************************************

using System;
using System.ComponentModel;

using EasyScada.Winforms.Controls;

namespace EasyScada.Winforms.Controls.Navigator
{
    /// <summary>
    /// Custom type converter so that MapEasyPageImage values appear as neat text at design time.
    /// </summary>
    public class MapEasyPageImageConverter : StringLookupConverter
    {
        #region Static Fields
        private Pair[] _pairs = new Pair[] { new Pair(MapEasyPageImage.None,             "None (Null image)"),
                                             new Pair(MapEasyPageImage.Small,            "Small"),
                                             new Pair(MapEasyPageImage.SmallMedium,      "Small - Medium"), 
                                             new Pair(MapEasyPageImage.SmallMediumLarge, "Small - Medium - Large"),
                                             new Pair(MapEasyPageImage.Medium,           "Medium"), 
                                             new Pair(MapEasyPageImage.MediumSmall,      "Medium - Small"), 
                                             new Pair(MapEasyPageImage.MediumLarge,      "Medium - Large"),
                                             new Pair(MapEasyPageImage.Large,            "Large"),
                                             new Pair(MapEasyPageImage.LargeMedium,      "Large - Medium"),
                                             new Pair(MapEasyPageImage.LargeMediumSmall, "Large - Medium - Small"),
                                             new Pair(MapEasyPageImage.ToolTip,          "ToolTip") };
        #endregion
                                             
        #region Identity
        /// <summary>
        /// Initialize a new instance of the MapEasyPageImageConverter clas.
        /// </summary>
        public MapEasyPageImageConverter()
            : base(typeof(MapEasyPageImage))
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
