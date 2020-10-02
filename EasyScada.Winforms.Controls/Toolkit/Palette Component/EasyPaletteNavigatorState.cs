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
using System.Text;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections.Generic;
using System.Diagnostics;

namespace EasyScada.Winforms.Controls
{
	/// <summary>
	/// Storage for an individual navigator states.
	/// </summary>
    public class EasyPaletteNavigatorState : Storage
    {
        #region Instance Fields
        private EasyPaletteNavigatorStateBar _bar;
        #endregion

        #region Identity
        /// <summary>
        /// Initialize a new instance of the EasyPaletteNavigatorState class.
        /// </summary>
        /// <param name="redirect">Inheritence redirection instance.</param>
        /// <param name="needPaint">Delegate for notifying paint requests.</param>
        public EasyPaletteNavigatorState(PaletteRedirect redirect,
                                            NeedPaintHandler needPaint)
        {
            Debug.Assert(redirect != null);
            
            // Create the storage objects
            _bar = new EasyPaletteNavigatorStateBar(redirect, needPaint);
        }
        #endregion

        #region IsDefault
        /// <summary>
        /// Gets a value indicating if all values are default.
        /// </summary>
        [Browsable(false)]
        public override bool IsDefault
        {
            get
            {
                return _bar.IsDefault;
            }
        }
        #endregion

        #region PopulateFromBase
        /// <summary>
        /// Populate values from the base palette.
        /// </summary>
        public void PopulateFromBase()
        {
            _bar.PopulateFromBase();
        }
        #endregion

        #region Bar
        /// <summary>
        /// Gets access to the navigator bar appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining navigator bar appearance entries.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteNavigatorStateBar Bar
        {
            get { return _bar; }
        }

        private bool ShouldSerializeStateCommon()
        {
            return !_bar.IsDefault;
        }
        #endregion
    }
}
