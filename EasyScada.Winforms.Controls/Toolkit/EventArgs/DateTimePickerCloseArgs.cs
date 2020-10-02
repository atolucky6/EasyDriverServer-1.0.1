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
using System.Windows.Forms;
using System.Diagnostics;

namespace EasyScada.Winforms.Controls
{
	/// <summary>
	/// Details about the context menu that has been closed up on a EasyDateTimePicker.
	/// </summary>
	public class DateTimePickerCloseArgs : EventArgs
	{
		#region Instance Fields
        private EasyContextMenu _kcm;
        #endregion

		#region Identity
        /// <summary>
        /// Initialize a new instance of the DateTimePickerCloseArgs class.
        /// </summary>
        /// <param name="kcm">EasyContextMenu that can be examined.</param>
        public DateTimePickerCloseArgs(EasyContextMenu kcm)
        {
            _kcm = kcm;
        }
        #endregion

		#region Public
        /// <summary>
        /// Gets access to the EasyContextMenu instance.
        /// </summary>
        public EasyContextMenu EasyContextMenu
        {
            get { return _kcm; }
        }
        #endregion
	}
}
