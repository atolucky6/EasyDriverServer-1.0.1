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
	/// Details about the context menu about to be shown when clicking the drop down button on a EasyDateTimePicker.
	/// </summary>
	public class DateTimePickerDropArgs : CancelEventArgs
	{
		#region Instance Fields
        private EasyContextMenu _kcm;
        private EasyContextMenuPositionH _positionH;
        private EasyContextMenuPositionV _positionV;
        #endregion

		#region Identity
        /// <summary>
        /// Initialize a new instance of the DateTimePickerDropArgs class.
        /// </summary>
        /// <param name="kcm">EasyContextMenu that can be customized.</param>
        /// <param name="positionH">Relative horizontal position of the EasyContextMenu.</param>
        /// <param name="positionV">Relative vertical position of the EasyContextMenu.</param>
        public DateTimePickerDropArgs(EasyContextMenu kcm,
                                     EasyContextMenuPositionH positionH,
                                     EasyContextMenuPositionV positionV)
        {
            _kcm = kcm;
            _positionH = positionH;
            _positionV = positionV;
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

        /// <summary>
        /// Gets and sets the relative horizontal position of the EasyContextMenu.
        /// </summary>
        public EasyContextMenuPositionH PositionH
        {
            get { return _positionH; }
            set { _positionH = value; }
        }

        /// <summary>
        /// Gets and sets the relative vertical position of the EasyContextMenu.
        /// </summary>
        public EasyContextMenuPositionV PositionV
        {
            get { return _positionV; }
            set { _positionV = value; }
        }
        #endregion
	}
}
