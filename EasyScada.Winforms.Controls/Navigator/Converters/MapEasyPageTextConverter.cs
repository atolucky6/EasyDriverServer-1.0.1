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
    /// Custom type converter so that MapEasyPageText values appear as neat text at design time.
    /// </summary>
    public class MapEasyPageTextConverter : StringLookupConverter
    {
        #region Static Fields
        private Pair[] _pairs = new Pair[] { new Pair(MapEasyPageText.None,                          "None (Empty string)"),
                                             new Pair(MapEasyPageText.Text,                          "Text"),
                                             new Pair(MapEasyPageText.TextTitle,                     "Text - Title"), 
                                             new Pair(MapEasyPageText.TextTitleDescription,          "Text - Title - Description"),
                                             new Pair(MapEasyPageText.TextDescription,               "Text - Description"), 
                                             new Pair(MapEasyPageText.Title,                         "Title"), 
                                             new Pair(MapEasyPageText.TitleText,                     "Title - Text"),
                                             new Pair(MapEasyPageText.TitleDescription,              "Title - Description"),
                                             new Pair(MapEasyPageText.Description,                   "Description"),
                                             new Pair(MapEasyPageText.DescriptionText,               "Description - Text"),
                                             new Pair(MapEasyPageText.DescriptionTitle,              "Description - Title"),
                                             new Pair(MapEasyPageText.DescriptionTitleText,          "Description - Title - Text"),
                                             new Pair(MapEasyPageText.ToolTipTitle,                  "ToolTipTitle"),
                                             new Pair(MapEasyPageText.ToolTipBody,                   "ToolTipBody"),
        };
        #endregion
                                             
        #region Identity
        /// <summary>
        /// Initialize a new instance of the MapEasyPageTextConverter clas.
        /// </summary>
        public MapEasyPageTextConverter()
            : base(typeof(MapEasyPageText))
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
