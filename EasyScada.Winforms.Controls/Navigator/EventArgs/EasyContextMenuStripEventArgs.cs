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
using System.Windows.Forms;
using EasyScada.Winforms.Controls;

namespace EasyScada.Winforms.Controls.Navigator
{
	/// <summary>
	/// Details providing a EasyContextMenu instance.
	/// </summary>
    public class EasyContextMenuEventArgs : EasyPageEventArgs
	{
		#region Instance Fields
        private EasyContextMenu _contextMenu;
		#endregion

		#region Identity
		/// <summary>
        /// Initialize a new instance of the EasyContextMenuEventArgs class.
		/// </summary>
		/// <param name="page">Page effected by event.</param>
		/// <param name="index">Index of page in the owning collection.</param>
        /// <param name="contextMenu">Prepopulated context menu ready for display.</param>
        public EasyContextMenuEventArgs(EasyPage page, 
                                           int index,
                                           EasyContextMenu contextMenu)
			: base(page, index)
		{
            _contextMenu = contextMenu;
		}
		#endregion

        #region EasyContextMenu
        /// <summary>
        /// Gets access to the EasyContextMenu that is to be shown.
        /// </summary>
        public EasyContextMenu EasyContextMenu
        {
            get { return _contextMenu; }
        }
        #endregion
    }
}
