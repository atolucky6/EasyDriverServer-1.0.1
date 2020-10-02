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
using System.Diagnostics;

namespace EasyScada.Winforms.Controls.Navigator
{
	/// <summary>
	/// Provide a EasyPageFlags enumeration with event details.
	/// </summary>
	public class EasyPageFlagsEventArgs : EventArgs
	{
		#region Instance Fields
		private EasyPageFlags _flags;
		#endregion

		#region Identity
		/// <summary>
        /// Initialize a new instance of the EasyPageFlagsEventArgs class.
		/// </summary>
        /// <param name="flags">EasyPageFlags enumeration.</param>
        public EasyPageFlagsEventArgs(EasyPageFlags flags)
		{
			// Remember parameter details
            _flags = flags;
		}
		#endregion

		#region Public
		/// <summary>
        /// Gets the EasyPageFlags enumeration value.
		/// </summary>
        public EasyPageFlags Flags
		{
			get { return _flags; }
		}
		#endregion
	}
}
