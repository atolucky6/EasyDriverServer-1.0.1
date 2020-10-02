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
using EasyScada.Winforms.Controls;

namespace EasyScada.Winforms.Controls.Navigator
{
	/// <summary>
    /// Details for an event that provides pages and cell associated with a page dragging event.
	/// </summary>
    public class PageDragEndData
	{
		#region Instance Fields
        private object _source;
        private EasyNavigator _navigator;
        private EasyPageCollection _pages;
        #endregion

		#region Identity
		/// <summary>
        /// Initialize a new instance of the PageDragEndData class.
		/// </summary>
        /// <param name="source">Source object for the drag data..</param>
        /// <param name="pages">Collection of pages.</param>
        public PageDragEndData(object source,
                               EasyPageCollection pages)
            : this(source, null, pages)
		{
		}

        /// <summary>
        /// Initialize a new instance of the PageDragEndData class.
        /// </summary>
        /// <param name="source">Source object for the drag data..</param>
        /// <param name="navigator">Navigator associated with pages.</param>
        /// <param name="pages">Collection of pages.</param>
        public PageDragEndData(object source,
                               EasyNavigator navigator,
                               EasyPageCollection pages)
        {
            _source = source;
            _navigator = navigator;
            _pages = pages;
        }
        #endregion

        #region Public
        /// <summary>
        /// Gets access to the source of the drag data.
        /// </summary>
        public object Source
        {
            get { return _source; }
        }

        /// <summary>
        /// Gets access to any associated EasyNavigator instance.
        /// </summary>
        public EasyNavigator Navigator
        {
            get { return _navigator; }
        }

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
