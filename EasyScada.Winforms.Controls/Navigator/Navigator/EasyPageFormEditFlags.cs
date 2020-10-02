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
using System.Text;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;

namespace EasyScada.Winforms.Controls.Navigator
{
    internal partial class EasyPageFormEditFlags : Form
    {
        #region Instance Fields
        private EasyPage _page;
        #endregion

        #region Identity
        /// <summary>
        /// Initialize a new instance of the EasyPageFormEditFlags class.
        /// </summary>
        public EasyPageFormEditFlags()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialize a new instance of the EasyPageFormEditFlags class.
        /// </summary>
        /// <param name="page">Reference to page to display flags for.</param>
        public EasyPageFormEditFlags(EasyPage page)
        {
            _page = page;
            InitializeComponent();
        }
        #endregion

        #region Implementation
        private void OnLoad(object sender, EventArgs e)
        {
            checkBoxPageInOverflowBarForOutlookMode.Checked = _page.AreFlagsSet(EasyPageFlags.PageInOverflowBarForOutlookMode);
            checkBoxAllowPageDrag.Checked = _page.AreFlagsSet(EasyPageFlags.AllowPageDrag);
            checkBoxAllowPageReorder.Checked = _page.AreFlagsSet(EasyPageFlags.AllowPageReorder);
            checkBoxAllowConfigSave.Checked = _page.AreFlagsSet(EasyPageFlags.AllowConfigSave);
            checkBoxDockingAllowClose.Checked = _page.AreFlagsSet(EasyPageFlags.DockingAllowClose);
            checkBoxDockingAllowDropDown.Checked = _page.AreFlagsSet(EasyPageFlags.DockingAllowDropDown);
            checkBoxDockingAllowAutoHidden.Checked = _page.AreFlagsSet(EasyPageFlags.DockingAllowAutoHidden);
            checkBoxDockingAllowDocked.Checked = _page.AreFlagsSet(EasyPageFlags.DockingAllowDocked);
            checkBoxDockingAllowFloating.Checked = _page.AreFlagsSet(EasyPageFlags.DockingAllowFloating);
            checkBoxDockingAllowWorkspace.Checked = _page.AreFlagsSet(EasyPageFlags.DockingAllowWorkspace);
            checkBoxDockingAllowNavigator.Checked = _page.AreFlagsSet(EasyPageFlags.DockingAllowNavigator);
        }
        
        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (checkBoxPageInOverflowBarForOutlookMode.Checked)
                _page.SetFlags(EasyPageFlags.PageInOverflowBarForOutlookMode);
            else
                _page.ClearFlags(EasyPageFlags.PageInOverflowBarForOutlookMode);

            if (checkBoxAllowPageDrag.Checked)
                _page.SetFlags(EasyPageFlags.AllowPageDrag);
            else
                _page.ClearFlags(EasyPageFlags.AllowPageDrag);

            if (checkBoxAllowPageReorder.Checked)
                _page.SetFlags(EasyPageFlags.AllowPageReorder);
            else
                _page.ClearFlags(EasyPageFlags.AllowPageReorder);

            if (checkBoxAllowConfigSave.Checked)
                _page.SetFlags(EasyPageFlags.AllowConfigSave);
            else
                _page.ClearFlags(EasyPageFlags.AllowConfigSave);

            if (checkBoxDockingAllowClose.Checked)
                _page.SetFlags(EasyPageFlags.DockingAllowClose);
            else
                _page.ClearFlags(EasyPageFlags.DockingAllowClose);

            if (checkBoxDockingAllowDropDown.Checked)
                _page.SetFlags(EasyPageFlags.DockingAllowDropDown);
            else
                _page.ClearFlags(EasyPageFlags.DockingAllowDropDown);

            if (checkBoxDockingAllowAutoHidden.Checked)
                _page.SetFlags(EasyPageFlags.DockingAllowAutoHidden);
            else
                _page.ClearFlags(EasyPageFlags.DockingAllowAutoHidden);

            if (checkBoxDockingAllowDocked.Checked)
                _page.SetFlags(EasyPageFlags.DockingAllowDocked);
            else
                _page.ClearFlags(EasyPageFlags.DockingAllowDocked);

            if (checkBoxDockingAllowFloating.Checked)
                _page.SetFlags(EasyPageFlags.DockingAllowFloating);
            else
                _page.ClearFlags(EasyPageFlags.DockingAllowFloating);

            if (checkBoxDockingAllowWorkspace.Checked)
                _page.SetFlags(EasyPageFlags.DockingAllowWorkspace);
            else
                _page.ClearFlags(EasyPageFlags.DockingAllowWorkspace);

            if (checkBoxDockingAllowNavigator.Checked)
                _page.SetFlags(EasyPageFlags.DockingAllowNavigator);
            else
                _page.ClearFlags(EasyPageFlags.DockingAllowNavigator);
        }
        #endregion
    }
}
