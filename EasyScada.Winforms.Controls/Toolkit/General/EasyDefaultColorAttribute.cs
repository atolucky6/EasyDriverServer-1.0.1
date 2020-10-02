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
using System.Drawing;

namespace EasyScada.Winforms.Controls
{
    /// <summary>
    /// Create a default value attribute for color property.
    /// </summary>
    public sealed class EasyDefaultColorAttribute : DefaultValueAttribute
    {
        #region Identity
        /// <summary>
        /// Initialize a new instance of the EasyDefaultColorAttribute class.
        /// </summary>
        public EasyDefaultColorAttribute()
            : base(Color.Empty)
        {
        }
        #endregion
    }
}
