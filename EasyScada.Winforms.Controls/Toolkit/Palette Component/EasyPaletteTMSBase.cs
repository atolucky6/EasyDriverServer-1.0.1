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
	/// Base class for the palette TMS storage classes to derive from.
	/// </summary>
	public abstract class EasyPaletteTMSBase : Storage
    {
        #region Instance Fields
        private EasyInternalKCT _internalKCT;
        #endregion
        
        #region Identity
        /// <summary>
        /// Initialize a new instance of the EasyPaletteKCTBase class.
		/// </summary>
        /// <param name="internalKCT">Reference to inherited values.</param>
        /// <param name="needPaint">Delegate for notifying paint requests.</param>
        internal EasyPaletteTMSBase(EasyInternalKCT internalKCT,
                                       NeedPaintHandler needPaint)
		{
            Debug.Assert(internalKCT != null);

            _internalKCT = internalKCT;

            // Store the provided paint notification delegate
            NeedPaint = needPaint;
        }
        #endregion

        #region Protected
        /// <summary>
        /// Gets access to the internal class used to inherit values.
        /// </summary>
        internal EasyInternalKCT InternalKCT
        {
            get { return _internalKCT; }
        }
        #endregion
    }
}
