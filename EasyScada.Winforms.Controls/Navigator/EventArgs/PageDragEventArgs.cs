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
using System.Drawing;

namespace EasyScada.Winforms.Controls.Navigator
{
	/// <summary>
    /// Details for an event that provides pages associated with a page dragging event.
	/// </summary>
	public class PageDragEventArgs : EventArgs
	{
		#region Instance Fields
        private Point _screenPoint;
        private EasyPageCollection _pages;
		#endregion

		#region Identity
		/// <summary>
        /// Initialize a new instance of the EasyPageDragEventArgs class.
		/// </summary>
        /// <param name="screenPoint">Screen point of the mouse.</param>
        /// <param name="pages">Array of event associated pages.</param>
        public PageDragEventArgs(Point screenPoint,
                                 EasyPage[] pages)
		{
            _screenPoint = screenPoint;
            _pages = new EasyPageCollection();

            if (pages != null)
                _pages.AddRange(pages);
		}

        /// <summary>
        /// Initialize a new instance of the EasyPageDragEventArgs class.
        /// </summary>
        /// <param name="screenPoint">Screen point of the mouse.</param>
        /// <param name="pages">Collection of event associated pages.</param>
        public PageDragEventArgs(Point screenPoint,
                                 EasyPageCollection pages)
        {
            _screenPoint = screenPoint;
            _pages = pages;
        }
        #endregion

        #region ScreenPoint
        /// <summary>
        /// Gets access to the associated screen point.
        /// </summary>
        public Point ScreenPoint
        {
            get { return _screenPoint; }
        }
        #endregion

        #region Pages
        /// <summary>
        /// Gets access to the collection of pages.
        /// </summary>
        public EasyPageCollection Pages
        {
            get { return _pages; }
        }
        #endregion
    }
}
