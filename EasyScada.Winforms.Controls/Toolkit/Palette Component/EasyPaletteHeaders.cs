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
    /// Storage for header palette settings.
    /// </summary>
    public class EasyPaletteHeaders : Storage
    {
        #region Instance Fields
        private EasyPaletteHeader _headerCommon;
        private EasyPaletteHeader _headerPrimary;
        private EasyPaletteHeader _headerSecondary;
        private EasyPaletteHeader _headerDockInactive;
        private EasyPaletteHeader _headerDockActive;
        private EasyPaletteHeader _headerCalendar;
        private EasyPaletteHeader _headerForm;
        private EasyPaletteHeader _headerCustom1;
        private EasyPaletteHeader _headerCustom2;
        #endregion

        #region Identity
        /// <summary>
        /// Initialize a new instance of the EasyPaletteHeaders class.
        /// </summary>
        /// <param name="redirector">Palette redirector for sourcing inherited values.</param>
        /// <param name="needPaint">Delegate for notifying paint requests.</param>
        internal EasyPaletteHeaders(PaletteRedirect redirector,
                                       NeedPaintHandler needPaint)
        {
            Debug.Assert(redirector != null);
            // Create the button style specific and common palettes
            _headerCommon = new EasyPaletteHeader(redirector, PaletteBackStyle.HeaderPrimary, PaletteBorderStyle.HeaderPrimary, PaletteContentStyle.HeaderPrimary, needPaint);
            _headerPrimary = new EasyPaletteHeader(redirector, PaletteBackStyle.HeaderPrimary, PaletteBorderStyle.HeaderPrimary, PaletteContentStyle.HeaderPrimary, needPaint);
            _headerSecondary = new EasyPaletteHeader(redirector, PaletteBackStyle.HeaderSecondary, PaletteBorderStyle.HeaderSecondary, PaletteContentStyle.HeaderSecondary, needPaint);
            _headerDockInactive = new EasyPaletteHeader(redirector, PaletteBackStyle.HeaderDockInactive, PaletteBorderStyle.HeaderDockInactive, PaletteContentStyle.HeaderDockInactive, needPaint);
            _headerDockActive = new EasyPaletteHeader(redirector, PaletteBackStyle.HeaderDockActive, PaletteBorderStyle.HeaderDockActive, PaletteContentStyle.HeaderDockActive, needPaint);
            _headerCalendar = new EasyPaletteHeader(redirector, PaletteBackStyle.HeaderCalendar, PaletteBorderStyle.HeaderCalendar, PaletteContentStyle.HeaderCalendar, needPaint);
            _headerForm = new EasyPaletteHeader(redirector, PaletteBackStyle.HeaderForm, PaletteBorderStyle.HeaderForm, PaletteContentStyle.HeaderForm, needPaint);
            _headerCustom1 = new EasyPaletteHeader(redirector, PaletteBackStyle.HeaderCustom1, PaletteBorderStyle.HeaderCustom1, PaletteContentStyle.HeaderCustom1, needPaint);
            _headerCustom2 = new EasyPaletteHeader(redirector, PaletteBackStyle.HeaderCustom2, PaletteBorderStyle.HeaderCustom2, PaletteContentStyle.HeaderCustom2, needPaint);

            // Create redirectors for inheriting from style specific to style common
            PaletteRedirectTripleMetric redirectCommon = new PaletteRedirectTripleMetric(redirector, 
                                                                                         _headerCommon.StateDisabled, _headerCommon.StateDisabled,
                                                                                         _headerCommon.StateNormal, _headerCommon.StateNormal);

            // Inform the button style to use the new redirector
            _headerPrimary.SetRedirector(redirectCommon);
            _headerSecondary.SetRedirector(redirectCommon);
            _headerDockInactive.SetRedirector(redirectCommon);
            _headerDockActive.SetRedirector(redirectCommon);
            _headerCalendar.SetRedirector(redirectCommon);
            _headerForm.SetRedirector(redirectCommon);
            _headerCustom1.SetRedirector(redirectCommon);
            _headerCustom2.SetRedirector(redirectCommon);
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
                return _headerCommon.IsDefault &&
                       _headerPrimary.IsDefault &&
                       _headerSecondary.IsDefault &&
                       _headerDockInactive.IsDefault &&
                       _headerDockActive.IsDefault &&
                       _headerCalendar.IsDefault &&
                       _headerForm.IsDefault &&
                       _headerCustom1.IsDefault &&
                       _headerCustom2.IsDefault;
            }
        }
        #endregion

        #region PopulateFromBase
        /// <summary>
        /// Populate values from the base palette.
        /// </summary>
        /// <param name="common">Reference to common settings.</param>
        public void PopulateFromBase(EasyPaletteCommon common)
        {
            // Populate only the designated styles
            common.StateCommon.BackStyle = PaletteBackStyle.HeaderPrimary;
            common.StateCommon.BorderStyle = PaletteBorderStyle.HeaderPrimary;
            common.StateCommon.ContentStyle = PaletteContentStyle.HeaderPrimary;
            _headerPrimary.PopulateFromBase();
            common.StateCommon.BackStyle = PaletteBackStyle.HeaderSecondary;
            common.StateCommon.BorderStyle = PaletteBorderStyle.HeaderSecondary;
            common.StateCommon.ContentStyle = PaletteContentStyle.HeaderSecondary;
            _headerSecondary.PopulateFromBase();
            common.StateCommon.BackStyle = PaletteBackStyle.HeaderDockInactive;
            common.StateCommon.BorderStyle = PaletteBorderStyle.HeaderDockInactive;
            common.StateCommon.ContentStyle = PaletteContentStyle.HeaderDockInactive;
            _headerDockInactive.PopulateFromBase();
            common.StateCommon.BackStyle = PaletteBackStyle.HeaderDockActive;
            common.StateCommon.BorderStyle = PaletteBorderStyle.HeaderDockActive;
            common.StateCommon.ContentStyle = PaletteContentStyle.HeaderDockActive;
            _headerDockActive.PopulateFromBase();
            common.StateCommon.BackStyle = PaletteBackStyle.HeaderCalendar;
            common.StateCommon.BorderStyle = PaletteBorderStyle.HeaderCalendar;
            common.StateCommon.ContentStyle = PaletteContentStyle.HeaderCalendar;
            _headerCalendar.PopulateFromBase();
            common.StateCommon.BackStyle = PaletteBackStyle.HeaderForm;
            common.StateCommon.BorderStyle = PaletteBorderStyle.HeaderForm;
            common.StateCommon.ContentStyle = PaletteContentStyle.HeaderForm;
            _headerForm.PopulateFromBase();
        }
        #endregion

        #region HeaderCommon
        /// <summary>
        /// Gets access to the common header appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining common header appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteHeader HeaderCommon
        {
            get { return _headerCommon; }
        }

        private bool ShouldSerializeHeaderCommon()
        {
            return !_headerCommon.IsDefault;
        }
        #endregion

        #region HeaderPrimary
        /// <summary>
        /// Gets access to the primary header appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining primary header appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteHeader HeaderPrimary
        {
            get { return _headerPrimary; }
        }

        private bool ShouldSerializeHeaderPrimary()
        {
            return !_headerPrimary.IsDefault;
        }
        #endregion

        #region HeaderSecondary
        /// <summary>
        /// Gets access to the secondary header appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining secondary header appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteHeader HeaderSecondary
        {
            get { return _headerSecondary; }
        }

        private bool ShouldSerializeHeaderSecondary()
        {
            return !_headerSecondary.IsDefault;
        }
        #endregion

        #region HeaderDockInactive
        /// <summary>
        /// Gets access to the inactive dock header appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining inactive dock header appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteHeader HeaderDockInactive
        {
            get { return _headerDockInactive; }
        }

        private bool ShouldSerializeHeaderDockInactive()
        {
            return !_headerDockInactive.IsDefault;
        }
        #endregion

        #region HeaderDockActive
        /// <summary>
        /// Gets access to the active dock header appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining active dock header appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteHeader HeaderDockActive
        {
            get { return _headerDockActive; }
        }

        private bool ShouldSerializeHeaderDockActive()
        {
            return !_headerDockActive.IsDefault;
        }
        #endregion

        #region HeaderCalendar
        /// <summary>
        /// Gets access to the calendar header appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining calendar header appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteHeader HeaderCalendar
        {
            get { return _headerCalendar; }
        }

        private bool ShouldSerializeHeaderCalendar()
        {
            return !_headerCalendar.IsDefault;
        }
        #endregion

        #region HeaderForm
        /// <summary>
        /// Gets access to the main form header appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining main form header appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteHeader HeaderForm
        {
            get { return _headerForm; }
        }

        private bool ShouldSerializeHeaderForm()
        {
            return !_headerForm.IsDefault;
        }
        #endregion

        #region HeaderCustom1
        /// <summary>
        /// Gets access to the first custom header appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining the first custom header appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteHeader HeaderCustom1
        {
            get { return _headerCustom1; }
        }

        private bool ShouldSerializeHeaderCustom1()
        {
            return !_headerCustom1.IsDefault;
        }
        #endregion

        #region HeaderCustom2
        /// <summary>
        /// Gets access to the second custom header appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining the second custom header appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteHeader HeaderCustom2
        {
            get { return _headerCustom2; }
        }

        private bool ShouldSerializeHeaderCustom2()
        {
            return !_headerCustom2.IsDefault;
        }
        #endregion
    }
}
