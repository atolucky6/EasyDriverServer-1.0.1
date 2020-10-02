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
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace EasyScada.Winforms.Controls
{
    internal class EasyCommandActionList : DesignerActionList
    {
        #region Instance Fields
        private EasyCommand _command;
        #endregion

        #region Identity
        /// <summary>
        /// Initialize a new instance of the EasyCommandActionList class.
        /// </summary>
        /// <param name="owner">Designer that owns this action list instance.</param>
        public EasyCommandActionList(EasyCommandDesigner owner)
            : base(owner.Component)
        {
            // Remember the panel instance
            _command = owner.Component as EasyCommand;
        }
        #endregion

        #region Public Override
        /// <summary>
        /// Returns the collection of DesignerActionItem objects contained in the list.
        /// </summary>
        /// <returns>A DesignerActionItem array that contains the items in this list.</returns>
        public override DesignerActionItemCollection GetSortedActionItems()
        {
            // Create a new collection for holding the single item we want to create
            DesignerActionItemCollection actions = new DesignerActionItemCollection();

            // This can be null when deleting a component instance at design time
            if (_command != null)
            {
            }

            return actions;
        }
        #endregion
    }
}
