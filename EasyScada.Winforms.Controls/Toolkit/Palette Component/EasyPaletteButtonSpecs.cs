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
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;

namespace EasyScada.Winforms.Controls
{
    /// <summary>
    /// Overrides for defining button specifications.
    /// </summary>
    public class EasyPaletteButtonSpecs : Storage
    {
        #region Instance Fields
        private EasyPaletteButtonSpecTyped _common;
        private EasyPaletteButtonSpecTyped _close;
        private EasyPaletteButtonSpecTyped _context;
        private EasyPaletteButtonSpecTyped _next;
        private EasyPaletteButtonSpecTyped _previous;
        private EasyPaletteButtonSpecTyped _generic;
        private EasyPaletteButtonSpecTyped _arrowLeft;
        private EasyPaletteButtonSpecTyped _arrowRight;
        private EasyPaletteButtonSpecTyped _arrowUp;
        private EasyPaletteButtonSpecTyped _arrowDown;
        private EasyPaletteButtonSpecTyped _dropDown;
        private EasyPaletteButtonSpecTyped _pinVertical;
        private EasyPaletteButtonSpecTyped _pinHorizontal;
        private EasyPaletteButtonSpecTyped _formClose;
        private EasyPaletteButtonSpecTyped _formMax;
        private EasyPaletteButtonSpecTyped _formMin;
        private EasyPaletteButtonSpecTyped _formRestore;
        private EasyPaletteButtonSpecTyped _pendantClose;
        private EasyPaletteButtonSpecTyped _pendantMin;
        private EasyPaletteButtonSpecTyped _pendantRestore;
        private EasyPaletteButtonSpecTyped _workspaceMaximize;
        private EasyPaletteButtonSpecTyped _workspaceRestore;
        private EasyPaletteButtonSpecTyped _ribbonMinimize;
        private EasyPaletteButtonSpecTyped _ribbonExpand;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when a button spec change occurs.
        /// </summary>
        public event EventHandler ButtonSpecChanged;
        #endregion

        #region Identity
        /// <summary>
        /// Initialize a new instance of the EasyPaletteButtonSpecs class.
        /// </summary>
        /// <param name="redirector">Palette redirector for sourcing inherited values.</param>
        internal EasyPaletteButtonSpecs(PaletteRedirect redirector)
        {
            Debug.Assert(redirector != null);

            // Create exposed button specifications
            _common = new EasyPaletteButtonSpecTyped(redirector);
            _generic = new EasyPaletteButtonSpecTyped(redirector);
            _close = new EasyPaletteButtonSpecTyped(redirector);
            _context = new EasyPaletteButtonSpecTyped(redirector);
            _next = new EasyPaletteButtonSpecTyped(redirector);
            _previous = new EasyPaletteButtonSpecTyped(redirector);
            _arrowLeft = new EasyPaletteButtonSpecTyped(redirector);
            _arrowRight = new EasyPaletteButtonSpecTyped(redirector);
            _arrowUp = new EasyPaletteButtonSpecTyped(redirector);
            _arrowDown = new EasyPaletteButtonSpecTyped(redirector);
            _dropDown = new EasyPaletteButtonSpecTyped(redirector);
            _pinVertical = new EasyPaletteButtonSpecTyped(redirector);
            _pinHorizontal = new EasyPaletteButtonSpecTyped(redirector);
            _formClose = new EasyPaletteButtonSpecTyped(redirector);
            _formMax = new EasyPaletteButtonSpecTyped(redirector);
            _formMin = new EasyPaletteButtonSpecTyped(redirector);
            _formRestore = new EasyPaletteButtonSpecTyped(redirector);
            _pendantClose = new EasyPaletteButtonSpecTyped(redirector);
            _pendantMin = new EasyPaletteButtonSpecTyped(redirector);
            _pendantRestore = new EasyPaletteButtonSpecTyped(redirector);
            _workspaceMaximize = new EasyPaletteButtonSpecTyped(redirector);
            _workspaceRestore = new EasyPaletteButtonSpecTyped(redirector);
            _ribbonMinimize = new EasyPaletteButtonSpecTyped(redirector);
            _ribbonExpand = new EasyPaletteButtonSpecTyped(redirector);

            // Create redirector for inheriting from style specific to style common
            PaletteRedirectButtonSpec redirectCommon = new PaletteRedirectButtonSpec(redirector, _common);

            // Inform the button spec to use the new redirector
            _generic.SetRedirector(redirectCommon);
            _close.SetRedirector(redirectCommon);
            _context.SetRedirector(redirectCommon);
            _next.SetRedirector(redirectCommon);
            _previous.SetRedirector(redirectCommon);
            _arrowLeft.SetRedirector(redirectCommon);
            _arrowRight.SetRedirector(redirectCommon);
            _arrowUp.SetRedirector(redirectCommon);
            _arrowDown.SetRedirector(redirectCommon);
            _dropDown.SetRedirector(redirectCommon);
            _pinVertical.SetRedirector(redirectCommon);
            _pinHorizontal.SetRedirector(redirectCommon);
            _formClose.SetRedirector(redirectCommon);
            _formMax.SetRedirector(redirectCommon);
            _formMin.SetRedirector(redirectCommon);
            _formRestore.SetRedirector(redirectCommon);
            _pendantClose.SetRedirector(redirectCommon);
            _pendantMin.SetRedirector(redirectCommon);
            _pendantRestore.SetRedirector(redirectCommon);
            _workspaceMaximize.SetRedirector(redirectCommon);
            _workspaceRestore.SetRedirector(redirectCommon);
            _ribbonMinimize.SetRedirector(redirectCommon);
            _ribbonExpand.SetRedirector(redirectCommon);

            // Hook into the storage change events
            _common.ButtonSpecChanged += new EventHandler(OnButtonSpecChanged);
            _generic.ButtonSpecChanged += new EventHandler(OnButtonSpecChanged);
            _close.ButtonSpecChanged += new EventHandler(OnButtonSpecChanged);
            _context.ButtonSpecChanged += new EventHandler(OnButtonSpecChanged);
            _next.ButtonSpecChanged += new EventHandler(OnButtonSpecChanged);
            _previous.ButtonSpecChanged += new EventHandler(OnButtonSpecChanged);
            _arrowLeft.ButtonSpecChanged += new EventHandler(OnButtonSpecChanged);
            _arrowRight.ButtonSpecChanged += new EventHandler(OnButtonSpecChanged);
            _arrowUp.ButtonSpecChanged += new EventHandler(OnButtonSpecChanged);
            _arrowDown.ButtonSpecChanged += new EventHandler(OnButtonSpecChanged);
            _dropDown.ButtonSpecChanged += new EventHandler(OnButtonSpecChanged);
            _pinVertical.ButtonSpecChanged += new EventHandler(OnButtonSpecChanged);
            _pinHorizontal.ButtonSpecChanged += new EventHandler(OnButtonSpecChanged);
            _formClose.ButtonSpecChanged += new EventHandler(OnButtonSpecChanged);
            _formMax.ButtonSpecChanged += new EventHandler(OnButtonSpecChanged);
            _formMin.ButtonSpecChanged += new EventHandler(OnButtonSpecChanged);
            _formRestore.ButtonSpecChanged += new EventHandler(OnButtonSpecChanged);
            _pendantClose.ButtonSpecChanged += new EventHandler(OnButtonSpecChanged);
            _pendantMin.ButtonSpecChanged += new EventHandler(OnButtonSpecChanged);
            _pendantRestore.ButtonSpecChanged += new EventHandler(OnButtonSpecChanged);
            _workspaceMaximize.ButtonSpecChanged += new EventHandler(OnButtonSpecChanged);
            _workspaceRestore.ButtonSpecChanged += new EventHandler(OnButtonSpecChanged);
            _ribbonMinimize.ButtonSpecChanged += new EventHandler(OnButtonSpecChanged);
            _ribbonExpand.ButtonSpecChanged += new EventHandler(OnButtonSpecChanged);
        }
        #endregion

        #region IsDefault
        /// <summary>
        /// Gets a value indicating if all values are default.
        /// </summary>
        public override bool IsDefault
        {
            get
            {
                return _common.IsDefault &&
                       _generic.IsDefault &&
                       _close.IsDefault &&
                       _context.IsDefault &&
                       _next.IsDefault &&
                       _previous.IsDefault &&
                       _arrowLeft.IsDefault &&
                       _arrowRight.IsDefault &&
                       _arrowUp.IsDefault &&
                       _arrowDown.IsDefault &&
                       _dropDown.IsDefault &&
                       _pinVertical.IsDefault &&
                       _pinHorizontal.IsDefault &&
                       _formClose.IsDefault &&
                       _formMax.IsDefault &&
                       _formMin.IsDefault &&
                       _formRestore.IsDefault &&
                       _pendantClose.IsDefault &&
                       _pendantMin.IsDefault &&
                       _pendantRestore.IsDefault &&
                       _workspaceMaximize.IsDefault &&
                       _workspaceRestore.IsDefault &&
                       _ribbonMinimize.IsDefault &&
                       _ribbonExpand.IsDefault;
            }
        }
        #endregion

        #region PopulateFromBase
        /// <summary>
        /// Populate values from the base palette.
        /// </summary>
        public void PopulateFromBase()
        {
            // Populate only the designated styles
            _generic.PopulateFromBase(PaletteButtonSpecStyle.Generic);
            _close.PopulateFromBase(PaletteButtonSpecStyle.Close);
            _context.PopulateFromBase(PaletteButtonSpecStyle.Context);
            _next.PopulateFromBase(PaletteButtonSpecStyle.Next);
            _previous.PopulateFromBase(PaletteButtonSpecStyle.Previous);
            _arrowLeft.PopulateFromBase(PaletteButtonSpecStyle.ArrowLeft);
            _arrowRight.PopulateFromBase(PaletteButtonSpecStyle.ArrowRight);
            _arrowUp.PopulateFromBase(PaletteButtonSpecStyle.ArrowUp);
            _arrowDown.PopulateFromBase(PaletteButtonSpecStyle.ArrowDown);
            _dropDown.PopulateFromBase(PaletteButtonSpecStyle.DropDown);
            _pinVertical.PopulateFromBase(PaletteButtonSpecStyle.PinVertical);
            _pinHorizontal.PopulateFromBase(PaletteButtonSpecStyle.PinHorizontal);
            _formClose.PopulateFromBase(PaletteButtonSpecStyle.FormClose);
            _formMax.PopulateFromBase(PaletteButtonSpecStyle.FormMax);
            _formMin.PopulateFromBase(PaletteButtonSpecStyle.FormMin);
            _formRestore.PopulateFromBase(PaletteButtonSpecStyle.FormRestore);
            _pendantClose.PopulateFromBase(PaletteButtonSpecStyle.PendantClose);
            _pendantRestore.PopulateFromBase(PaletteButtonSpecStyle.PendantRestore);
            _pendantMin.PopulateFromBase(PaletteButtonSpecStyle.PendantMin);
            _pendantRestore.PopulateFromBase(PaletteButtonSpecStyle.PendantRestore);
            _workspaceMaximize.PopulateFromBase(PaletteButtonSpecStyle.WorkspaceMaximize);
            _workspaceRestore.PopulateFromBase(PaletteButtonSpecStyle.WorkspaceRestore);
            _ribbonMinimize.PopulateFromBase(PaletteButtonSpecStyle.RibbonMinimize);
            _ribbonExpand.PopulateFromBase(PaletteButtonSpecStyle.RibbonExpand);
        }
        #endregion

        #region Common
        /// <summary>
        /// Gets access to the common button specification.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining common button specifications.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteButtonSpecTyped Common
        {
            get { return _common; }
        }

        private bool ShouldSerializeCommon()
        {
            return !_common.IsDefault;
        }
        #endregion

        #region Generic
        /// <summary>
        /// Gets access to the generic button specification.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining generic button specifications.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteButtonSpecTyped Generic
        {
            get { return _generic; }
        }

        private bool ShouldSerializeGeneric()
        {
            return !_generic.IsDefault;
        }
        #endregion

        #region Close
        /// <summary>
        /// Gets access to the close button specification.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining close button specifications.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteButtonSpecTyped Close
        {
            get { return _close ; }
        }

        private bool ShouldSerializeClose()
        {
            return !_close.IsDefault;
        }
        #endregion

        #region Context
        /// <summary>
        /// Gets access to the context button specification.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining context button specifications.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteButtonSpecTyped Context
        {
            get { return _context; }
        }

        private bool ShouldSerializeContext()
        {
            return !_context.IsDefault;
        }
        #endregion

        #region Next
        /// <summary>
        /// Gets access to the next button specification.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining next button specifications.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteButtonSpecTyped Next
        {
            get { return _next; }
        }

        private bool ShouldSerializeNext()
        {
            return !_next.IsDefault;
        }
        #endregion

        #region Previous
        /// <summary>
        /// Gets access to the previous button specification.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining previous button specifications.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteButtonSpecTyped Previous
        {
            get { return _previous; }
        }

        private bool ShouldSerializePrevious()
        {
            return !_previous.IsDefault;
        }
        #endregion

        #region ArrowLeft
        /// <summary>
        /// Gets access to the left arrow button specification.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining left arrow button specifications.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteButtonSpecTyped ArrowLeft
        {
            get { return _arrowLeft; }
        }

        private bool ShouldSerializeArrowLeft()
        {
            return !_arrowLeft.IsDefault;
        }
        #endregion

        #region ArrowRight
        /// <summary>
        /// Gets access to the right arrow button specification.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining right arrow button specifications.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteButtonSpecTyped ArrowRight
        {
            get { return _arrowRight; }
        }

        private bool ShouldSerializeArrowRight()
        {
            return !_arrowRight.IsDefault;
        }
        #endregion

        #region ArrowUp
        /// <summary>
        /// Gets access to the right up button specification.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining up arrow button specifications.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteButtonSpecTyped ArrowUp
        {
            get { return _arrowUp; }
        }

        private bool ShouldSerializeArrowUp()
        {
            return !_arrowUp.IsDefault;
        }
        #endregion

        #region ArrowDown
        /// <summary>
        /// Gets access to the right up button specification.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining up arrow button specifications.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteButtonSpecTyped ArrowDown
        {
            get { return _arrowDown; }
        }

        private bool ShouldSerializeArrowDown()
        {
            return !_arrowDown.IsDefault;
        }
        #endregion

        #region DropDown
        /// <summary>
        /// Gets access to the drop down button specification.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining drop down button specifications.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteButtonSpecTyped DropDown
        {
            get { return _dropDown; }
        }

        private bool ShouldSerializeDropDown()
        {
            return !_dropDown.IsDefault;
        }
        #endregion

        #region PinVertical
        /// <summary>
        /// Gets access to the pin vertical button specification.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining pin vertical button specifications.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteButtonSpecTyped PinVertical
        {
            get { return _pinVertical; }
        }

        private bool ShouldSerializePinVertical()
        {
            return !_pinVertical.IsDefault;
        }
        #endregion

        #region PinHorizontal
        /// <summary>
        /// Gets access to the pin horizontal button specification.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining pin horizontal button specifications.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteButtonSpecTyped PinHorizontal
        {
            get { return _pinHorizontal; }
        }

        private bool ShouldSerializePinHorizontal()
        {
            return !_pinHorizontal.IsDefault;
        }
        #endregion

        #region FormClose
        /// <summary>
        /// Gets access to the form close button specification.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining form close button specifications.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteButtonSpecTyped FormClose
        {
            get { return _formClose; }
        }

        private bool ShouldSerializeFormClose()
        {
            return !_formClose.IsDefault;
        }
        #endregion

        #region FormMin
        /// <summary>
        /// Gets access to the form minimize button specification.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining form minimize button specifications.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteButtonSpecTyped FormMin
        {
            get { return _formMin; }
        }

        private bool ShouldSerializeFormMin()
        {
            return !_formMin.IsDefault;
        }
        #endregion

        #region FormMax
        /// <summary>
        /// Gets access to the form maximize button specification.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining form maximize button specifications.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteButtonSpecTyped FormMax
        {
            get { return _formMax; }
        }

        private bool ShouldSerializeFormMax()
        {
            return !_formMax.IsDefault;
        }
        #endregion

        #region FormRestore
        /// <summary>
        /// Gets access to the form restore button specification.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining form restore button specifications.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteButtonSpecTyped FormRestore
        {
            get { return _formRestore; }
        }

        private bool ShouldSerializeFormRestore()
        {
            return !_formRestore.IsDefault;
        }
        #endregion

        #region PendantClose
        /// <summary>
        /// Gets access to the pendant close button specification.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining pendant close button specifications.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteButtonSpecTyped PendantClose
        {
            get { return _pendantClose; }
        }

        private bool ShouldSerializePendantClose()
        {
            return !_pendantClose.IsDefault;
        }
        #endregion

        #region PendantMin
        /// <summary>
        /// Gets access to the pendant minimize button specification.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining pendant minimize button specifications.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteButtonSpecTyped PendantMin
        {
            get { return _pendantMin; }
        }

        private bool ShouldSerializePendantMin()
        {
            return !_pendantMin.IsDefault;
        }
        #endregion

        #region PendantRestore
        /// <summary>
        /// Gets access to the pendant restore button specification.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining pendant restore button specifications.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteButtonSpecTyped PendantRestore
        {
            get { return _pendantRestore; }
        }

        private bool ShouldSerializePendantRestore()
        {
            return !_pendantRestore.IsDefault;
        }
        #endregion

        #region WorkspaceMaximize
        /// <summary>
        /// Gets access to the workspace maximize button specification.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining workspace maximize button specifications.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteButtonSpecTyped WorkspaceMaximize
        {
            get { return _workspaceMaximize; }
        }

        private bool ShouldSerializeWorkspaceMaximize()
        {
            return !_workspaceMaximize.IsDefault;
        }
        #endregion

        #region WorkspaceRestore
        /// <summary>
        /// Gets access to the workspace restore button specification.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining workspace restore button specifications.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteButtonSpecTyped WorkspaceRestore
        {
            get { return _workspaceRestore; }
        }

        private bool ShouldSerializeWorkspaceRestore()
        {
            return !_workspaceRestore.IsDefault;
        }
        #endregion

        #region RibbonMinimize
        /// <summary>
        /// Gets access to the ribbon minimize button specification.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining ribbon minimize button specifications.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteButtonSpecTyped RibbonMinimize
        {
            get { return _ribbonMinimize; }
        }

        private bool ShouldSerializeRibbonMinimize()
        {
            return !_ribbonMinimize.IsDefault;
        }
        #endregion

        #region RibbonExpand
        /// <summary>
        /// Gets access to the ribbon expand button specification.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining ribbon expand button specifications.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteButtonSpecTyped RibbonExpand
        {
            get { return _ribbonExpand; }
        }

        private bool ShouldSerializeRibbonExpand()
        {
            return !_ribbonExpand.IsDefault;
        }
        #endregion

        #region OnButtonSpecChanged
        /// <summary>
        /// Raises the ButtonSpecChanged event.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">An EventArgs containing event data.</param>
        protected virtual void OnButtonSpecChanged(object sender, EventArgs e)
        {
            if (ButtonSpecChanged != null)
                ButtonSpecChanged(this, e);
        }
        #endregion
    }
}
