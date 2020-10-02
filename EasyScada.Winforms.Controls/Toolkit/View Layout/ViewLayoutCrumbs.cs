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
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace EasyScada.Winforms.Controls
{
	/// <summary>
	/// Creates and layout individual crumbs.
	/// </summary>
    public class ViewLayoutCrumbs : ViewComposite, IContentValues
    {
        #region Type Definitions
        private class CrumbToButton : Dictionary<EasyBreadCrumbItem, ViewDrawButton> { };
        private class ButtonToCrumb : Dictionary<ViewDrawButton, EasyBreadCrumbItem> { };
        private class MenuItemToCrumb : Dictionary<EasyContextMenuItem, EasyBreadCrumbItem> { };
        #endregion

        #region Instance Fields
        private EasyBreadCrumb _easyBreadCrumb;
        private NeedPaintHandler _needPaintDelegate;
        private ButtonController _pressedButtonController;
        private CrumbToButton _crumbToButton;
        private ButtonToCrumb _buttonToCrumb;
        private MenuItemToCrumb _menuItemToCrumb;
        private ViewDrawButton _overflowButton;
        private bool _showingContextMenu;
        #endregion

        #region Identity
        /// <summary>
        /// Initialize a new instance of the ViewLayoutCrumbs class.
		/// </summary>
        /// <param name="easyBreadCrumb">Reference to owning bread crumb control.</param>
        /// <param name="needPaintDelegate">Delegate used to request repainting..</param>
        public ViewLayoutCrumbs(EasyBreadCrumb easyBreadCrumb,
                                NeedPaintHandler needPaintDelegate)
        {
            _easyBreadCrumb = easyBreadCrumb;
            _needPaintDelegate = needPaintDelegate;
            _crumbToButton = new CrumbToButton();
            _buttonToCrumb = new ButtonToCrumb();
            _showingContextMenu = false;

            CreateOverflowButton();
        }

        /// <summary>
        /// Release unmanaged and optionally managed resources.
        /// </summary>
        /// <param name="disposing">Called from Dispose method.</param>
        protected override void Dispose(bool disposing)
        {
            // Remove actual children to prevent disposal happening twice
            Clear();

            // Release each cached child control just once
            foreach (ViewBase child in _crumbToButton.Values)
                child.Dispose();

            // Prevent another call to dispose from trying to dispose them again
            _crumbToButton.Clear();
            _buttonToCrumb.Clear();
        }

        /// <summary>
		/// Obtains the String representation of this instance.
		/// </summary>
		/// <returns>User readable name of the instance.</returns>
		public override string ToString()
		{
			// Return the class name and instance identifier
            return "ViewLayoutCrumbs:" + Id;
		}
		#endregion

        #region Layout
        /// <summary>
		/// Discover the preferred size of the element.
		/// </summary>
		/// <param name="context">Layout context.</param>
		public override Size GetPreferredSize(ViewLayoutContext context)
		{
            // Create/delete child elements to match the current selected bread crumb path
            SyncBreadCrumbs();

            // We need to update the redirector for drawing each crumb
            PaletteRedirectBreadCrumb redirect = _easyBreadCrumb.GetRedirector() as PaletteRedirectBreadCrumb;

            Size preferredSize = Size.Empty;
            for(int i=1; i<Count; i++)
            {
                // Do not show the left border on the first crumb
                redirect.Left = (i == 0);

                // Find size of the child
                Size childSize = this[i].GetPreferredSize(context);

                // Accumulate in width
                preferredSize.Width += childSize.Width;

                // Find maximum height
                preferredSize.Height = Math.Max(preferredSize.Height, childSize.Height);
            }

            // Add on the control level padding
            preferredSize.Width += _easyBreadCrumb.Padding.Horizontal;
            preferredSize.Height += _easyBreadCrumb.Padding.Vertical;

            return preferredSize;
		}

		/// <summary>
		/// Perform a layout of the elements.
		/// </summary>
		/// <param name="context">Layout context.</param>
		public override void Layout(ViewLayoutContext context)
		{
			Debug.Assert(context != null);

            // We need to update the redirector for drawing each crumb
            PaletteRedirectBreadCrumb redirect = _easyBreadCrumb.GetRedirector() as PaletteRedirectBreadCrumb;

            // We take on all the available display area
			ClientRectangle = context.DisplayRectangle;

            // Create/delete child elements to match the current selected bread crumb path
            SyncBreadCrumbs();

            // Positioning rectangle is our client rectangle reduced by control padding
            Rectangle layoutRect = new Rectangle(ClientLocation.X + _easyBreadCrumb.Padding.Left,
                                                 ClientLocation.Y + _easyBreadCrumb.Padding.Top,
                                                 ClientWidth - _easyBreadCrumb.Padding.Horizontal,
                                                 ClientHeight - _easyBreadCrumb.Padding.Vertical);

            // Position from left to right all items except the overflow button
            int offset = layoutRect.X;
            for(int i=1; i<Count; i++)
            {
                // Do not show the left border on the first crumb
                redirect.Left = (i == 1);

                // Find size of the child
                Size childSize = this[i].GetPreferredSize(context);
                context.DisplayRectangle = new Rectangle(offset, layoutRect.Y, childSize.Width, layoutRect.Height);

                // Position the child
                this[i].Layout(context);
                this[i].Visible = true;

                // Move across
                offset += childSize.Width;
            }

            // If we overflowed then we need to use the overflow button
            if (offset > ClientWidth)
            {
                // Overflow button must be visible
                this[0].Visible = true;

                // How much space do we need to save?
                int overflowed = offset - ClientWidth;

                // Position overflow button and only the last items to fit space
                offset = layoutRect.X;
                for (int i = 0; i < Count; i++)
                {
                    // Decide if the crumb (not the overflow button) can be visible
                    if (i > 0)
                        this[i].Visible = (overflowed <= 0);

                    if (this[i].Visible)
                    {
                        // Do not show the left border on the first crumb
                        redirect.Left = (i == 0);

                        // Recover the already calculated size
                        Size childSize = this[i].GetPreferredSize(context);
                        context.DisplayRectangle = new Rectangle(offset, layoutRect.Y, childSize.Width, layoutRect.Height);

                        // Position the child
                        this[i].Layout(context);

                        // Move across
                        offset += childSize.Width;
                    }

                    // Adjust overflow space depending on if we are positioning crumb or overflow
                    if (i != 0)
                        overflowed -= this[i].ClientWidth;
                    else
                        overflowed += this[i].ClientWidth;
                }
            }
            else
            {
                // No overflow then no need for the overflow button
                this[0].Visible = false;
            }

            // Must restore the display rectangle to the same size as when it entered
            context.DisplayRectangle = ClientRectangle;
        }
		#endregion

        #region Paint
        /// <summary>
        /// Perform rendering before child elements are rendered.
        /// </summary>
        /// <param name="context">Rendering context.</param>
        public override void RenderBefore(RenderContext context)
        {
            foreach (ViewBase child in this)
            {
                // Only interested in updating view buttons
                if (child is ViewDrawButton)
                {
                    // Cast to correct type
                    ViewDrawButton crumbButton = child as ViewDrawButton;

                    // That are associated with crumb items
                    EasyBreadCrumbItem crumbItem;
                    if (_buttonToCrumb.TryGetValue(crumbButton, out crumbItem))
                    {
                        // If the button is pressed then point button downwards, 
                        // otherwise we point in the direction the buttons layed out.
                        if (crumbButton.ElementState == PaletteState.Pressed)
                            crumbButton.DropDownOrientation = VisualOrientation.Top;
                        else
                            crumbButton.DropDownOrientation = VisualOrientation.Left;
                    }
                }
            }

            base.RenderBefore(context);
        }

        /// <summary>
        /// Perform a render of the elements.
        /// </summary>
        /// <param name="context">Rendering context.</param>
        public override void Render(RenderContext context)
        {
            Debug.Assert(context != null);

            // Perform rendering before any children
            RenderBefore(context);

            // We need to update the redirector for drawing each crumb
            PaletteRedirectBreadCrumb redirect = _easyBreadCrumb.GetRedirector() as PaletteRedirectBreadCrumb;

            bool first = true;
            for(int i=0; i<Count; i++)
            {
                if (this[i].Visible)
                {
                    // Do not show the left border on the first crumb
                    if (first)
                    {
                        redirect.Left = true;
                        first = false;
                    }
                    else
                        redirect.Left = false;

                    this[i].Render(context);
                }
            }

            // Perform rendering after that of children
            RenderAfter(context);
        }
        #endregion

        #region IContentValues
        /// <summary>
        /// Gets the content image.
        /// </summary>
        /// <param name="state">The state for which the image is needed.</param>
        /// <returns>Image value.</returns>
        public Image GetImage(PaletteState state)
        {
            return _easyBreadCrumb.GetRedirector().GetButtonSpecImage(PaletteButtonSpecStyle.ArrowLeft, state);
        }

        /// <summary>
        /// Gets the image color that should be transparent.
        /// </summary>
        /// <param name="state">The state for which the image is needed.</param>
        /// <returns>Color value.</returns>
        public Color GetImageTransparentColor(PaletteState state)
        {
            return _easyBreadCrumb.GetRedirector().GetButtonSpecImageTransparentColor(PaletteButtonSpecStyle.ArrowLeft);
        }

        /// <summary>
        /// Gets the content short text.
        /// </summary>
        /// <returns>String value.</returns>
        public string GetShortText()
        {
            return string.Empty;
        }

        /// <summary>
        /// Gets the content long text.
        /// </summary>
        /// <returns>String value.</returns>
        public string GetLongText()
        {
            return string.Empty;
        }
        #endregion

        #region Private
        private void CreateOverflowButton()
        {
            // Create a button used when we overflow the available area
            _overflowButton = new ViewDrawButton(_easyBreadCrumb.StateDisabled.BreadCrumb,
                                                 _easyBreadCrumb.StateNormal.BreadCrumb,
                                                 _easyBreadCrumb.StateTracking.BreadCrumb,
                                                 _easyBreadCrumb.StatePressed.BreadCrumb,
                                                 _easyBreadCrumb.GetStateCommon(),
                                                 this, VisualOrientation.Top, false);

            _overflowButton.Splitter = true;
            _overflowButton.TestForFocusCues = true;
            _overflowButton.DropDownPalette = _easyBreadCrumb.GetRedirector();

            // Create controller for operating the button
            ButtonController crumbButtonController = new ButtonController(_overflowButton, _needPaintDelegate);
            crumbButtonController.Tag = this;
            crumbButtonController.BecomesFixed = true;
            crumbButtonController.Click += new MouseEventHandler(OnOverflowButtonClick);
            _overflowButton.MouseController = crumbButtonController;
        }

        private void SyncBreadCrumbs()
        {
            // Remove all existing children
            Clear();

            // Walk up the bread crumb trail
            EasyBreadCrumbItem item = _easyBreadCrumb.SelectedItem;
            while (item != null)
            {
                ViewDrawButton crumbButton;
  
                // If we do not have a button to represent this crumb...
                if (!_crumbToButton.TryGetValue(item, out crumbButton))
                {
                    // Setup the button for drawing as a drop down button if required
                    crumbButton = new ViewDrawButton(_easyBreadCrumb.StateDisabled.BreadCrumb,
                                                     _easyBreadCrumb.StateNormal.BreadCrumb,
                                                     _easyBreadCrumb.StateTracking.BreadCrumb,
                                                     _easyBreadCrumb.StatePressed.BreadCrumb,
                                                     _easyBreadCrumb.GetStateCommon(), 
                                                     item, VisualOrientation.Top, false);

                    crumbButton.Splitter = true;
                    crumbButton.TestForFocusCues = true;
                    crumbButton.DropDownPalette = _easyBreadCrumb.GetRedirector();

                    // Create controller for operating the button
                    ButtonController crumbButtonController = new ButtonController(crumbButton, _needPaintDelegate);
                    crumbButtonController.Tag = item;
                    crumbButtonController.BecomesFixed = true;
                    crumbButtonController.Click += new MouseEventHandler(OnButtonClick);
                    crumbButton.MouseController = crumbButtonController;
                   
                    // Add to cache for future use
                    _crumbToButton.Add(item, crumbButton);
                    _buttonToCrumb.Add(crumbButton, item);
                }
                
                // Only show a drop down button if we have some children to choose from
                crumbButton.DropDown = _easyBreadCrumb.DropDownNavigation && (item.Items.Count > 0);

                // Add crumb to end of child collection
                Insert(0, crumbButton);

                // Move up another level
                item = item.Parent;
            }

            // Always add the overflow to the first item
            Insert(0, _overflowButton);
        }

        private void OnButtonClick(object sender, MouseEventArgs e)
        {
            // Only allow a single context menu at a time
            if (!_showingContextMenu)
            {
                // Get access to the controller, view and crumb item
                ViewDrawButton viewButton = sender as ViewDrawButton;
                ButtonController controller = viewButton.MouseController as ButtonController;
                EasyBreadCrumbItem breadCrumb = controller.Tag as EasyBreadCrumbItem;

                // Do we need to show a drop down menu?
                if (viewButton.DropDown && viewButton.SplitRectangle.Contains(e.Location))
                {
                    // Create a context menu with a items collection
                    EasyContextMenu kcm = new EasyContextMenu();
                    
                    // Use same palette settings for context menu as the main control
                    kcm.Palette = _easyBreadCrumb.Palette;
                    if (kcm.Palette == null)
                        kcm.PaletteMode = _easyBreadCrumb.PaletteMode;

                    // Add an items collection as the root item of the context menu
                    EasyContextMenuItems items = new EasyContextMenuItems();
                    kcm.Items.Add(items);

                    // Store lookup between each menu item and the crumb it represents. Prevents
                    // needing to use the menu item tag for remembering association. Leaving the
                    // tag free for use by the user.
                    _menuItemToCrumb = new MenuItemToCrumb();

                    // Create a new menu item to represent each child crumb
                    foreach (EasyBreadCrumbItem childCrumb in breadCrumb.Items)
                    {
                        EasyContextMenuItem childMenu = new EasyContextMenuItem();

                        // Store 1-to-1 association
                        _menuItemToCrumb.Add(childMenu, childCrumb);

                        // Copy across the display details of the child crumb item
                        childMenu.Text = childCrumb.ShortText;
                        childMenu.ExtraText = childCrumb.LongText;
                        childMenu.Image = childCrumb.Image;
                        childMenu.ImageTransparentColor = childCrumb.ImageTransparentColor;
                        childMenu.Click += new EventHandler(OnChildCrumbClick);

                        items.Items.Add(childMenu);
                    }

                    // Allow the user a chance to alter the menu contents or cancel it entirely
                    BreadCrumbMenuArgs bcma = new BreadCrumbMenuArgs(breadCrumb, kcm, EasyContextMenuPositionH.Left, EasyContextMenuPositionV.Below);
                    _easyBreadCrumb.OnCrumbDropDown(bcma);

                    // Is there still the need to show a menu that is not empty?
                    if (!bcma.Cancel &&
                        (bcma.EasyContextMenu != null) &&
                        CommonHelper.ValidEasyContextMenu(bcma.EasyContextMenu))
                    {
                        // Cache the controller for use in menu close processing, prevents the need to 
                        // store anything in the EasyContextMenu tag and so free up its use to the user.
                        _pressedButtonController = controller;

                        // Show the context menu so user can select the next item for selection
                        bcma.EasyContextMenu.Closed += new ToolStripDropDownClosedEventHandler(OnEasyContextMenuClosed);
                        bcma.EasyContextMenu.Show(_easyBreadCrumb, _easyBreadCrumb.RectangleToScreen(new Rectangle(viewButton.SplitRectangle.X - viewButton.SplitRectangle.Width,
                                                                                                                            viewButton.SplitRectangle.Y,
                                                                                                                            viewButton.SplitRectangle.Width * 2,
                                                                                                                            viewButton.SplitRectangle.Height)),
                                                     bcma.PositionH,
                                                     bcma.PositionV);

                        // do not show another context menu whilst this one is visible
                        _showingContextMenu = true;
                    }
                    else
                    {
                        // Button gives a fixed appearance when pressed, without a context menu that is not necessary
                        controller.RemoveFixed();
                    }
                }
                else
                {
                    // Button gives a fixed appearance when pressed, without a context menu that is not necessary
                    controller.RemoveFixed();

                    // Clicking item makes it become the selected crumb
                    _easyBreadCrumb.SelectedItem = breadCrumb;
                }
            }
        }

        private void OnEasyContextMenuClosed(object sender, EventArgs e)
        {
            // Cast to correct type
            EasyContextMenu kcm = (EasyContextMenu)sender;

            // Unhook from context menu and dipose of it, we only use each menu instance once
            kcm.Closed -= new ToolStripDropDownClosedEventHandler(OnEasyContextMenuClosed);
            kcm.Dispose();

            // Remove the fixed appearnce of the view button
            _pressedButtonController.RemoveFixed();
            _pressedButtonController = null;

            // No longer showing context menu, so safe to show another one
            _showingContextMenu = false;
        }

        private void OnChildCrumbClick(object sender, EventArgs e)
        {
            // Make the clicked child crumb the newly selected item
            EasyContextMenuItem childItem = sender as EasyContextMenuItem;
            _easyBreadCrumb.SelectedItem = _menuItemToCrumb[childItem];
        }

        private void OnOverflowButtonClick(object sender, MouseEventArgs e)
        {
            // Only allow a single context menu at a time
            if (!_showingContextMenu)
            {
                // Get access to the controller, view and crumb item
                ViewDrawButton viewButton = sender as ViewDrawButton;
                ButtonController controller = viewButton.MouseController as ButtonController;

                // Create a context menu with a items collection
                EasyContextMenu kcm = new EasyContextMenu();

                // Use same palette settings for context menu as the main control
                kcm.Palette = _easyBreadCrumb.Palette;
                if (kcm.Palette == null)
                    kcm.PaletteMode = _easyBreadCrumb.PaletteMode;

                // Add an items collection as the root item of the context menu
                EasyContextMenuItems items = new EasyContextMenuItems();
                kcm.Items.Add(items);

                // Store lookup between each menu item and the crumb it represents. Prevents
                // needing to use the menu item tag for remembering association. Leaving the
                // tag free for use by the user.
                _menuItemToCrumb = new MenuItemToCrumb();

                // Create a new menu item to represent each of the invisible crumbs not children of the root
                // (item 0=overflow button, 1=root; 2=child of root, so we start at index 3)
                for(int i=3; i<Count; i++)
                {
                    if (!this[i].Visible)
                    {
                        EasyBreadCrumbItem childCrumb = _buttonToCrumb[(ViewDrawButton)this[i]];
                        EasyContextMenuItem childMenu = new EasyContextMenuItem();

                        // Store 1-to-1 association
                        _menuItemToCrumb.Add(childMenu, childCrumb);

                        // Copy across the display details of the child crumb item
                        childMenu.Text = childCrumb.ShortText;
                        childMenu.ExtraText = childCrumb.LongText;
                        childMenu.Image = childCrumb.Image;
                        childMenu.ImageTransparentColor = childCrumb.ImageTransparentColor;
                        childMenu.Click += new EventHandler(OnChildCrumbClick);

                        items.Items.Add(childMenu);
                    }
                }

                // Create a new menu item to represent each of the roots children
                bool firstRoot = true;
                foreach (EasyBreadCrumbItem childCrumb in _easyBreadCrumb.RootItem.Items)
                {
                    // The first time we add an entry
                    if (firstRoot)
                    {
                        // Add a separator if entries already exist
                        if (items.Items.Count > 0)
                            items.Items.Add(new EasyContextMenuSeparator());

                        firstRoot = false;
                    }

                    EasyContextMenuItem childMenu = new EasyContextMenuItem();

                    // Store 1-to-1 association
                    _menuItemToCrumb.Add(childMenu, childCrumb);

                    // Copy across the display details of the child crumb item
                    childMenu.Text = childCrumb.ShortText;
                    childMenu.ExtraText = childCrumb.LongText;
                    childMenu.Image = childCrumb.Image;
                    childMenu.ImageTransparentColor = childCrumb.ImageTransparentColor;
                    childMenu.Click += new EventHandler(OnChildCrumbClick);

                    items.Items.Add(childMenu);
                }

                // Allow the user a chance to alter the menu contents or cancel it entirely
                ContextPositionMenuArgs cpma = new ContextPositionMenuArgs(kcm, EasyContextMenuPositionH.Left, EasyContextMenuPositionV.Below);
                _easyBreadCrumb.OnOverflowDropDown(cpma);

                // Is there still the need to show a menu that is not empty?
                if (!cpma.Cancel &&
                    (cpma.EasyContextMenu != null) &&
                    CommonHelper.ValidEasyContextMenu(cpma.EasyContextMenu))
                {
                    // Cache the controller for use in menu close processing, prevents the need to 
                    // store anything in the EasyContextMenu tag and so free up its use to the user.
                    _pressedButtonController = controller;

                    // Show the context menu so user can select the next item for selection
                    cpma.EasyContextMenu.Closed += new ToolStripDropDownClosedEventHandler(OnEasyContextMenuClosed);
                    cpma.EasyContextMenu.Show(_easyBreadCrumb, _easyBreadCrumb.RectangleToScreen(new Rectangle(viewButton.ClientRectangle.X,
                                                                                                                        viewButton.ClientRectangle.Y,
                                                                                                                        viewButton.ClientRectangle.Width * 2,
                                                                                                                        viewButton.ClientRectangle.Height)),
                                                 cpma.PositionH,
                                                 cpma.PositionV);

                    // do not show another context menu whilst this one is visible
                    _showingContextMenu = true;
                }
                else
                {
                    // Button gives a fixed appearance when pressed, without a context menu that is not necessary
                    controller.RemoveFixed();

                    // Clicking item makes it become the selected crumb
                    _easyBreadCrumb.SelectedItem = _easyBreadCrumb.RootItem;
                }
            }
        }
        #endregion
    }
}
