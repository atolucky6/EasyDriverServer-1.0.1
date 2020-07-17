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
    /// Storage for form palette settings.
    /// </summary>
    public class EasyPaletteForms : Storage
    {
        #region Instance Fields
        private EasyPaletteForm _formCommon;
        private EasyPaletteForm _formMain;
        private EasyPaletteForm _formCustom1;
        #endregion

        #region Identity
        /// <summary>
        /// Initialize a new instance of the EasyPaletteForms class.
        /// </summary>
        /// <param name="redirector">Palette redirector for sourcing inherited values.</param>
        /// <param name="needPaint">Delegate for notifying paint requests.</param>
        internal EasyPaletteForms(PaletteRedirect redirector,
                                     NeedPaintHandler needPaint)
        {
            Debug.Assert(redirector != null);

            // Create the form style specific and common palettes
            _formCommon = new EasyPaletteForm(redirector, PaletteBackStyle.FormMain, PaletteBorderStyle.FormMain, needPaint);
            _formMain = new EasyPaletteForm(redirector, PaletteBackStyle.FormMain, PaletteBorderStyle.FormMain, needPaint);
            _formCustom1 = new EasyPaletteForm(redirector, PaletteBackStyle.FormCustom1, PaletteBorderStyle.FormCustom1, needPaint);

            // Create redirectors for inheriting from style specific to style common
            PaletteRedirectDouble redirectCommon = new PaletteRedirectDouble(redirector, _formCommon.StateInactive, _formCommon.StateActive);

            // Inform the form style to use the new redirector
            _formMain.SetRedirector(redirectCommon);
            _formCustom1.SetRedirector(redirectCommon);
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
                return _formCommon.IsDefault &&
                       _formMain.IsDefault &&
                       _formCustom1.IsDefault;
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
            common.StateCommon.BackStyle = PaletteBackStyle.FormMain;
            common.StateCommon.BorderStyle = PaletteBorderStyle.FormMain;
            _formMain.PopulateFromBase();
        }
        #endregion

        #region FormCommon
        /// <summary>
        /// Gets access to the common form appearance.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining common form appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteForm FormCommon
        {
            get { return _formCommon; }
        }

        private bool ShouldSerializeFormCommon()
        {
            return !_formCommon.IsDefault;
        }
        #endregion

        #region FormMain
        /// <summary>
        /// Gets access to the main form appearance.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining main form appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteForm FormMain
        {
            get { return _formMain; }
        }

        private bool ShouldSerializeFormMain()
        {
            return !_formMain.IsDefault;
        }
        #endregion

        #region FormCustom1
        /// <summary>
        /// Gets access to the first custom form appearance.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining the first custom form appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteForm FormCustom1
        {
            get { return _formCustom1; }
        }

        private bool ShouldSerializeFormCustom1()
        {
            return !_formCustom1.IsDefault;
        }
        #endregion
    }
}
