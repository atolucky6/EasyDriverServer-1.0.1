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
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;

namespace EasyScada.Winforms.Controls
{
    /// <summary>
    /// Storage for separator palette settings.
    /// </summary>
    public class EasyPaletteSeparators : Storage
    {
        #region Instance Fields
        private EasyPaletteSeparator _separatorCommon;
        private EasyPaletteSeparator _separatorLowProfile;
        private EasyPaletteSeparator _separatorHighProfile;
        private EasyPaletteSeparator _separatorHighInternalProfile;
        private EasyPaletteSeparator _separatorCustom1;
        #endregion

        #region Identity
        /// <summary>
        /// Initialize a new instance of the EasyPaletteSeparators class.
        /// </summary>
        /// <param name="redirector">Palette redirector for sourcing inherited values.</param>
        /// <param name="needPaint">Delegate for notifying paint requests.</param>
        internal EasyPaletteSeparators(PaletteRedirect redirector,
                                          NeedPaintHandler needPaint)
        {
            Debug.Assert(redirector != null);

            // Create the button style specific and common palettes
            _separatorCommon = new EasyPaletteSeparator(redirector, PaletteBackStyle.SeparatorLowProfile, PaletteBorderStyle.SeparatorLowProfile, needPaint);
            _separatorLowProfile = new EasyPaletteSeparator(redirector, PaletteBackStyle.SeparatorLowProfile, PaletteBorderStyle.SeparatorLowProfile, needPaint);
            _separatorHighProfile = new EasyPaletteSeparator(redirector, PaletteBackStyle.SeparatorHighProfile, PaletteBorderStyle.SeparatorHighProfile, needPaint);
            _separatorHighInternalProfile = new EasyPaletteSeparator(redirector, PaletteBackStyle.SeparatorHighInternalProfile, PaletteBorderStyle.SeparatorHighInternalProfile, needPaint);
            _separatorCustom1 = new EasyPaletteSeparator(redirector, PaletteBackStyle.SeparatorCustom1, PaletteBorderStyle.SeparatorCustom1, needPaint);

            // Create redirectors for inheriting from style specific to style common
            PaletteRedirectDouble redirectCommon = new PaletteRedirectDouble(redirector, 
                                                                             _separatorCommon.StateDisabled, _separatorCommon.StateNormal,
                                                                             _separatorCommon.StatePressed, _separatorCommon.StateTracking);

            // Inform the button style to use the new redirector
            _separatorLowProfile.SetRedirector(redirectCommon);
            _separatorHighProfile.SetRedirector(redirectCommon);
            _separatorHighInternalProfile.SetRedirector(redirectCommon);
            _separatorCustom1.SetRedirector(redirectCommon);
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
                return _separatorCommon.IsDefault &&
                       _separatorLowProfile.IsDefault &&
                       _separatorHighProfile.IsDefault &&
                       _separatorHighInternalProfile.IsDefault &&
                       _separatorCustom1.IsDefault;
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
            common.StateCommon.BackStyle = PaletteBackStyle.SeparatorLowProfile;
            common.StateCommon.BorderStyle = PaletteBorderStyle.SeparatorLowProfile;
            _separatorLowProfile.PopulateFromBase(PaletteMetricPadding.SeparatorPaddingLowProfile);
            common.StateCommon.BackStyle = PaletteBackStyle.SeparatorHighProfile;
            common.StateCommon.BorderStyle = PaletteBorderStyle.SeparatorHighProfile;
            _separatorHighProfile.PopulateFromBase(PaletteMetricPadding.SeparatorPaddingHighProfile);
            common.StateCommon.BackStyle = PaletteBackStyle.SeparatorHighInternalProfile;
            common.StateCommon.BorderStyle = PaletteBorderStyle.SeparatorHighInternalProfile;
            _separatorHighInternalProfile.PopulateFromBase(PaletteMetricPadding.SeparatorPaddingHighInternalProfile);
        }
        #endregion

        #region SeparatorCommon
        /// <summary>
        /// Gets access to the common separator appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining common separator appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteSeparator SeparatorCommon
        {
            get { return _separatorCommon; }
        }

        private bool ShouldSerializeSeparatorCommon()
        {
            return !_separatorCommon.IsDefault;
        }
        #endregion

        #region SeparatorLowProfile
        /// <summary>
        /// Gets access to the low profile separator appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining low profile separator appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteSeparator SeparatorLowProfile
        {
            get { return _separatorLowProfile; }
        }

        private bool ShouldSerializeSeparatorLowProfile()
        {
            return !_separatorLowProfile.IsDefault;
        }
        #endregion

        #region SeparatorHighProfile
        /// <summary>
        /// Gets access to the high profile separator appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining high profile separator appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteSeparator SeparatorHighProfile
        {
            get { return _separatorHighProfile; }
        }

        private bool ShouldSerializeSeparatorHighProfile()
        {
            return !_separatorHighProfile.IsDefault;
        }
        #endregion

        #region SeparatorHighInternalProfile
        /// <summary>
        /// Gets access to the high profile for internal separator appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining high profile for internal separator appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteSeparator SeparatorHighInternalProfile
        {
            get { return _separatorHighInternalProfile; }
        }

        private bool ShouldSerializeSeparatorHighInternalProfile()
        {
            return !_separatorHighInternalProfile.IsDefault;
        }
        #endregion

        #region SeparatorCustom1
        /// <summary>
        /// Gets access to the first custom separator appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining first custom separator appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteSeparator SeparatorCustom1
        {
            get { return _separatorCustom1; }
        }

        private bool ShouldSerializeSeparatorCustom1()
        {
            return !_separatorCustom1.IsDefault;
        }
        #endregion
    }
}
