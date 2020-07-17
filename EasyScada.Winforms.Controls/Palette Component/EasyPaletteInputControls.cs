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
    /// Storage for input control palette settings.
    /// </summary>
    public class EasyPaletteInputControls : Storage
    {
        #region Instance Fields
        private EasyPaletteInputControl _inputControlCommon;
        private EasyPaletteInputControl _inputControlStandalone;
        private EasyPaletteInputControl _inputControlRibbon;
        private EasyPaletteInputControl _inputControlCustom1;
        #endregion

        #region Identity
        /// <summary>
        /// Initialize a new instance of the EasyPaletteInputControls class.
        /// </summary>
        /// <param name="redirector">Palette redirector for sourcing inherited values.</param>
        /// <param name="needPaint">Delegate for notifying paint requests.</param>
        internal EasyPaletteInputControls(PaletteRedirect redirector,
                                             NeedPaintHandler needPaint)
        {
            Debug.Assert(redirector != null);

            // Create the input control style specific and common palettes
            _inputControlCommon = new EasyPaletteInputControl(redirector, PaletteBackStyle.InputControlStandalone, PaletteBorderStyle.InputControlStandalone, PaletteContentStyle.InputControlStandalone, needPaint);
            _inputControlStandalone = new EasyPaletteInputControl(redirector, PaletteBackStyle.InputControlStandalone, PaletteBorderStyle.InputControlStandalone, PaletteContentStyle.InputControlStandalone, needPaint);
            _inputControlRibbon = new EasyPaletteInputControl(redirector, PaletteBackStyle.InputControlRibbon, PaletteBorderStyle.InputControlRibbon, PaletteContentStyle.InputControlRibbon, needPaint);
            _inputControlCustom1 = new EasyPaletteInputControl(redirector, PaletteBackStyle.InputControlCustom1, PaletteBorderStyle.InputControlCustom1, PaletteContentStyle.InputControlCustom1, needPaint);

            // Create redirectors for inheriting from style specific to style common
            PaletteRedirectTriple redirectCommon = new PaletteRedirectTriple(redirector, _inputControlCommon.StateDisabled, _inputControlCommon.StateNormal, _inputControlCommon.StateActive);

            // Inform the input control style to use the new redirector
            _inputControlStandalone.SetRedirector(redirectCommon);
            _inputControlRibbon.SetRedirector(redirectCommon);
            _inputControlCustom1.SetRedirector(redirectCommon);
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
                return _inputControlCommon.IsDefault &&
                       _inputControlStandalone.IsDefault &&
                       _inputControlRibbon.IsDefault &&
                       _inputControlCustom1.IsDefault;
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
            common.StateCommon.BackStyle = PaletteBackStyle.InputControlStandalone;
            common.StateCommon.BorderStyle = PaletteBorderStyle.InputControlStandalone;
            common.StateCommon.ContentStyle = PaletteContentStyle.InputControlStandalone;
            _inputControlStandalone.PopulateFromBase();
            common.StateCommon.BackStyle = PaletteBackStyle.InputControlRibbon;
            common.StateCommon.BorderStyle = PaletteBorderStyle.InputControlRibbon;
            common.StateCommon.ContentStyle = PaletteContentStyle.InputControlRibbon;
            _inputControlRibbon.PopulateFromBase();
        }
        #endregion

        #region InputControlCommon
        /// <summary>
        /// Gets access to the common input control appearance.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining common input control appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteInputControl InputControlCommon
        {
            get { return _inputControlCommon; }
        }

        private bool ShouldSerializeInputControlCommon()
        {
            return !_inputControlCommon.IsDefault;
        }
        #endregion

        #region InputControlStandalone
        /// <summary>
        /// Gets access to the standalone input control appearance.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining standalone input control appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteInputControl InputControlStandalone
        {
            get { return _inputControlStandalone; }
        }

        private bool ShouldSerializeInputControlStandalone()
        {
            return !_inputControlStandalone.IsDefault;
        }
        #endregion

        #region InputControlRibbon
        /// <summary>
        /// Gets access to the input control ribbon style appearance.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining input control ribbon style appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteInputControl InputControlRibbon
        {
            get { return _inputControlRibbon; }
        }

        private bool ShouldSerializeInputControlRibbon()
        {
            return !_inputControlRibbon.IsDefault;
        }
        #endregion

        #region InputControlCustom1
        /// <summary>
        /// Gets access to the custom input control appearance.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining the custom input control appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteInputControl InputControlCustom1
        {
            get { return _inputControlCustom1; }
        }

        private bool ShouldSerializeInputControlCustom1()
        {
            return !_inputControlCustom1.IsDefault;
        }
        #endregion
    }
}
