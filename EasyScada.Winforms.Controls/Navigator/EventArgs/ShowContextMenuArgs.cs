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
	/// Details for a close button action event.
	/// </summary>
    public class ShowContextMenuArgs : EasyPageCancelEventArgs
	{
		#region Instance Fields
        private ContextMenuStrip _cms;
        private EasyContextMenu _kcm; 
		#endregion

		#region Identity
		/// <summary>
        /// Initialize a new instance of the ShowContextMenuArgs class.
		/// </summary>
        /// <param name="page">Page effected by event.</param>
        /// <param name="index">Index of page in the owning collection.</param>
        public ShowContextMenuArgs(EasyPage page, int index)
			: base(page, index)
		{
            _cms = page.ContextMenuStrip;
            _kcm = page.EasyContextMenu;
		}
		#endregion

        #region ContextMenuStrip
        /// <summary>
		/// Gets and sets the context menu strip.
		/// </summary>
        public ContextMenuStrip ContextMenuStrip
		{
            get { return _cms; }
            set { _cms = value; }
		}
		#endregion

        #region EasyContextMenu
        /// <summary>
        /// Gets and sets the context menu strip.
        /// </summary>
        public EasyContextMenu EasyContextMenu
        {
            get { return _kcm; }
            set { _kcm = value; }
        }
        #endregion
    }
}
