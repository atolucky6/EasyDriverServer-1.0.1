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

namespace EasyScada.Winforms.Controls
{
    /// <summary>
    /// Attribute that marks properties for persistence inside the Easy palette.
    /// </summary>
	[Serializable]
	[AttributeUsage(AttributeTargets.Property)]
    public sealed class EasyPersistAttribute : Attribute
    {
        // Instance fields
        private bool _navigate;
        private bool _populate;

        /// <summary>
        /// Initialize a new instance of the EasyPersistAttribute class.
        /// </summary>
        public EasyPersistAttribute()
            : this(true, true)
        {
        }

        /// <summary>
        /// Initialize a new instance of the EasyPersistAttribute class.
        /// </summary>
        /// <param name="navigate">Should persistence navigate down the property.</param>
        public EasyPersistAttribute(bool navigate)
            : this(navigate, true)
        {
        }

        /// <summary>
        /// Initialize a new instance of the EasyPersistAttribute class.
        /// </summary>
        /// <param name="navigate">Should persistence navigate down the property.</param>
        /// <param name="populate">Should property be reset as part of a populate operation.</param>
        public EasyPersistAttribute(bool navigate, bool populate)
        {
            _navigate = navigate;
            _populate = populate;
        }

        /// <summary>
        /// Gets and sets a value indicating if the persistence should navigate the property.
        /// </summary>
        public bool Navigate
        {
            get { return _navigate; }
            set { _navigate = value; }
        }

        /// <summary>
        /// Gets and sets a value indicating if the property should be reset as part of a populate operation.
        /// </summary>
        public bool Populate
        {
            get { return _populate; }
            set { _populate = value; }
        }
    }
}
