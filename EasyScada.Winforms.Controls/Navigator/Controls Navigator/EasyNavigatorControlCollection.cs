﻿// *****************************************************************************
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
using System.Diagnostics;
using EasyScada.Winforms.Controls;

namespace EasyScada.Winforms.Controls.Navigator
{
	/// <summary>
	/// Represents a collection of child controls for the navigator.
	/// </summary>
    public class EasyNavigatorControlCollection : EasyControlCollection
	{
		#region Instance Fields
		private EasyNavigator _owner;
		#endregion

		#region Identity
		/// <summary>
		/// Initialize a new instance of the EasyNavigatorControlCollection class.
		/// </summary>
		/// <param name="owner">Control containing this collection.</param>
		public EasyNavigatorControlCollection(EasyNavigator owner)
			: base(owner)
		{
			Debug.Assert(owner != null);

			// Remember the collection owner
			_owner = owner;
		}
		#endregion

		#region Public Overrides
		/// <summary>
		/// Adds the specified control to the control collection.
		/// </summary>
		/// <param name="value">The EasyPage to add to the control collection.</param>
		public override void Add(Control value)
		{
			Debug.Assert(value != null);

			// Cast to correct type
			EasyPage page = (EasyPage)value;

			// We only allow EasyPage controls to be added
			if (page == null)
				throw new ArgumentException("Only EasyPage controls can be added.", "value");

			// Let base class perform actual add
			base.Add(value);
		}
		#endregion
	}
}
