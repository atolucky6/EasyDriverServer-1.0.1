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
	/// Details for context menu related events that have a requested relative position.
	/// </summary>
    public class ContextPositionMenuArgs : ContextMenuArgs
	{
		#region Instance Fields
        private EasyContextMenuPositionH _positionH;
        private EasyContextMenuPositionV _positionV;
        #endregion

		#region Identity
        /// <summary>
        /// Initialize a new instance of the ContextMenuArgs class.
        /// </summary>
        public ContextPositionMenuArgs()
            : this(null, null, EasyContextMenuPositionH.Left, EasyContextMenuPositionV.Below)
        {
        }

        /// <summary>
        /// Initialize a new instance of the ContextMenuArgs class.
		/// </summary>
        /// <param name="cms">Context menu strip that can be customized.</param>
        public ContextPositionMenuArgs(ContextMenuStrip cms)
            : this(cms, null, EasyContextMenuPositionH.Left, EasyContextMenuPositionV.Below)
		{
		}

        /// <summary>
        /// Initialize a new instance of the ContextMenuArgs class.
        /// </summary>
        /// <param name="kcm">EasyContextMenu that can be customized.</param>
        /// <param name="positionH">Relative horizontal position of the EasyContextMenu.</param>
        /// <param name="positionV">Relative vertical position of the EasyContextMenu.</param>
        public ContextPositionMenuArgs(EasyContextMenu kcm,
                                       EasyContextMenuPositionH positionH,
                                       EasyContextMenuPositionV positionV)
            : this(null, kcm, positionH, positionV)
        {
        }

        /// <summary>
        /// Initialize a new instance of the ContextMenuArgs class.
        /// </summary>
        /// <param name="cms">Context menu strip that can be customized.</param>
        /// <param name="kcm">EasyContextMenu that can be customized.</param>
        /// <param name="positionH">Relative horizontal position of the EasyContextMenu.</param>
        /// <param name="positionV">Relative vertical position of the EasyContextMenu.</param>
        public ContextPositionMenuArgs(ContextMenuStrip cms,
                                       EasyContextMenu kcm,
                                       EasyContextMenuPositionH positionH,
                                       EasyContextMenuPositionV positionV)
            : base(cms, kcm)
        {
            _positionH = positionH;
            _positionV = positionV;
        }
        #endregion

		#region Public
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
