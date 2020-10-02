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
    /// Storage for label palette settings.
    /// </summary>
    public class EasyPaletteLabels : Storage
    {
        #region Instance Fields
        private EasyPaletteLabel _labelCommon;
        private EasyPaletteLabel _labelNormalControl;
        private EasyPaletteLabel _labelBoldControl;
        private EasyPaletteLabel _labelItalicControl;
        private EasyPaletteLabel _labelTitleControl;
        private EasyPaletteLabel _labelNormalPanel;
        private EasyPaletteLabel _labelBoldPanel;
        private EasyPaletteLabel _labelItalicPanel;
        private EasyPaletteLabel _labelTitlePanel;
        private EasyPaletteLabel _labelCaptionPanel;
        private EasyPaletteLabel _labelToolTip;
        private EasyPaletteLabel _labelSuperTip;
        private EasyPaletteLabel _labelKeyTip;
        private EasyPaletteLabel _labelCustom1;
        private EasyPaletteLabel _labelCustom2;
        private EasyPaletteLabel _labelCustom3;
        #endregion

        #region Identity
        /// <summary>
        /// Initialize a new instance of the EasyPaletteLabels class.
        /// </summary>
        /// <param name="redirector">Palette redirector for sourcing inherited values.</param>
        /// <param name="needPaint">Delegate for notifying paint requests.</param>
        internal EasyPaletteLabels(PaletteRedirect redirector,
                                      NeedPaintHandler needPaint)
        {
            Debug.Assert(redirector != null);

            // Create the button style specific and common palettes
            _labelCommon = new EasyPaletteLabel(redirector, PaletteContentStyle.LabelNormalControl, needPaint);
            _labelNormalControl = new EasyPaletteLabel(redirector, PaletteContentStyle.LabelNormalControl, needPaint);
            _labelBoldControl = new EasyPaletteLabel(redirector, PaletteContentStyle.LabelBoldControl, needPaint);
            _labelItalicControl = new EasyPaletteLabel(redirector, PaletteContentStyle.LabelItalicControl, needPaint);
            _labelTitleControl = new EasyPaletteLabel(redirector, PaletteContentStyle.LabelTitleControl, needPaint);
            _labelNormalPanel = new EasyPaletteLabel(redirector, PaletteContentStyle.LabelNormalPanel, needPaint);
            _labelBoldPanel = new EasyPaletteLabel(redirector, PaletteContentStyle.LabelBoldPanel, needPaint);
            _labelItalicPanel = new EasyPaletteLabel(redirector, PaletteContentStyle.LabelItalicPanel, needPaint);
            _labelTitlePanel = new EasyPaletteLabel(redirector, PaletteContentStyle.LabelTitlePanel, needPaint);
            _labelCaptionPanel = new EasyPaletteLabel(redirector, PaletteContentStyle.LabelGroupBoxCaption, needPaint);
            _labelToolTip = new EasyPaletteLabel(redirector, PaletteContentStyle.LabelToolTip, needPaint);
            _labelSuperTip = new EasyPaletteLabel(redirector, PaletteContentStyle.LabelSuperTip, needPaint);
            _labelKeyTip = new EasyPaletteLabel(redirector, PaletteContentStyle.LabelKeyTip, needPaint);
            _labelCustom1 = new EasyPaletteLabel(redirector, PaletteContentStyle.LabelCustom1, needPaint);
            _labelCustom2 = new EasyPaletteLabel(redirector, PaletteContentStyle.LabelCustom2, needPaint);
            _labelCustom3 = new EasyPaletteLabel(redirector, PaletteContentStyle.LabelCustom3, needPaint);

            // Create redirectors for inheriting from style specific to style common
            PaletteRedirectContent redirectCommon = new PaletteRedirectContent(redirector, _labelCommon.StateDisabled, _labelCommon.StateNormal);

            // Inform the button style to use the new redirector
            _labelNormalControl.SetRedirector(redirectCommon);
            _labelBoldControl.SetRedirector(redirectCommon);
            _labelItalicControl.SetRedirector(redirectCommon);
            _labelTitleControl.SetRedirector(redirectCommon);
            _labelNormalPanel.SetRedirector(redirectCommon);
            _labelBoldPanel.SetRedirector(redirectCommon);
            _labelItalicPanel.SetRedirector(redirectCommon);
            _labelTitlePanel.SetRedirector(redirectCommon);
            _labelCaptionPanel.SetRedirector(redirectCommon);
            _labelToolTip.SetRedirector(redirectCommon);
            _labelSuperTip.SetRedirector(redirectCommon);
            _labelKeyTip.SetRedirector(redirectCommon);
            _labelCustom1.SetRedirector(redirectCommon);
            _labelCustom2.SetRedirector(redirectCommon);
            _labelCustom3.SetRedirector(redirectCommon);
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
                return _labelCommon.IsDefault &&
                       _labelNormalControl.IsDefault &&
                       _labelBoldControl.IsDefault &&
                       _labelItalicControl.IsDefault &&
                       _labelTitleControl.IsDefault &&
                       _labelNormalPanel.IsDefault &&
                       _labelBoldPanel.IsDefault &&
                       _labelItalicPanel.IsDefault &&
                       _labelTitlePanel.IsDefault &&
                       _labelCaptionPanel.IsDefault &&
                       _labelToolTip.IsDefault &&
                       _labelSuperTip.IsDefault &&
                       _labelKeyTip.IsDefault &&
                       _labelCustom1.IsDefault &&
                       _labelCustom2.IsDefault &&
                       _labelCustom3.IsDefault;
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
            common.StateCommon.ContentStyle = PaletteContentStyle.LabelNormalControl;
            _labelNormalControl.PopulateFromBase();
            common.StateCommon.ContentStyle = PaletteContentStyle.LabelBoldControl;
            _labelNormalControl.PopulateFromBase();
            common.StateCommon.ContentStyle = PaletteContentStyle.LabelItalicControl;
            _labelNormalControl.PopulateFromBase();
            common.StateCommon.ContentStyle = PaletteContentStyle.LabelTitleControl;
            _labelTitleControl.PopulateFromBase();
            common.StateCommon.ContentStyle = PaletteContentStyle.LabelNormalPanel;
            _labelNormalPanel.PopulateFromBase();
            common.StateCommon.ContentStyle = PaletteContentStyle.LabelBoldPanel;
            _labelNormalPanel.PopulateFromBase();
            common.StateCommon.ContentStyle = PaletteContentStyle.LabelItalicPanel;
            _labelNormalPanel.PopulateFromBase();
            common.StateCommon.ContentStyle = PaletteContentStyle.LabelTitlePanel;
            _labelTitlePanel.PopulateFromBase();
            common.StateCommon.ContentStyle = PaletteContentStyle.LabelGroupBoxCaption;
            _labelCaptionPanel.PopulateFromBase();
            common.StateCommon.ContentStyle = PaletteContentStyle.LabelToolTip;
            _labelToolTip.PopulateFromBase();
            common.StateCommon.ContentStyle = PaletteContentStyle.LabelSuperTip;
            _labelSuperTip.PopulateFromBase();
            common.StateCommon.ContentStyle = PaletteContentStyle.LabelKeyTip;
            _labelKeyTip.PopulateFromBase();
        }
        #endregion

        #region LabelCommon
        /// <summary>
        /// Gets access to the common label appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining common label appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteLabel LabelCommon
        {
            get { return _labelCommon; }
        }

        private bool ShouldSerializeLabelCommon()
        {
            return !_labelCommon.IsDefault;
        }
        #endregion

        #region LabelNormalControl
        /// <summary>
        /// Gets access to the normal label used for control style backgrounds.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining normal label appearance for use on control style backgrounds.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteLabel LabelNormalControl
        {
            get { return _labelNormalControl; }
        }

        private bool ShouldSerializeLabelNormalControl()
        {
            return !_labelNormalControl.IsDefault;
        }
        #endregion

        #region LabelBoldControl
        /// <summary>
        /// Gets access to the bold label used for control style backgrounds.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining bold label appearance for use on control style backgrounds.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteLabel LabelBoldControl
        {
            get { return _labelBoldControl; }
        }

        private bool ShouldSerializeLabelBoldControl()
        {
            return !_labelBoldControl.IsDefault;
        }
        #endregion

        #region LabelItalicControl
        /// <summary>
        /// Gets access to the italic label used for control style backgrounds.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining italic label appearance for use on control style backgrounds.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteLabel LabelItalicControl
        {
            get { return _labelItalicControl; }
        }

        private bool ShouldSerializeLabelItalicControl()
        {
            return !_labelItalicControl.IsDefault;
        }
        #endregion

        #region LabelTitleControl
        /// <summary>
        /// Gets access to the title label used for control style backgrounds.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining title label appearance for use on control style backgrounds.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteLabel LabelTitleControl
        {
            get { return _labelTitleControl; }
        }

        private bool ShouldSerializeLabelTitleControl()
        {
            return !_labelTitleControl.IsDefault;
        }
        #endregion

        #region LabelNormalPanel
        /// <summary>
        /// Gets access to the normal label used for panel style backgrounds.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining normal label appearance for use on panel style backgrounds.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteLabel LabelNormalPanel
        {
            get { return _labelNormalPanel; }
        }

        private bool ShouldSerializeLabelNormalPanel()
        {
            return !_labelNormalPanel.IsDefault;
        }
        #endregion

        #region LabelBoldPanel
        /// <summary>
        /// Gets access to the bold label used for panel style backgrounds.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining bold label appearance for use on panel style backgrounds.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteLabel LabelBoldPanel
        {
            get { return _labelBoldPanel; }
        }

        private bool ShouldSerializeLabelBoldPanel()
        {
            return !_labelBoldPanel.IsDefault;
        }
        #endregion

        #region LabelItalicPanel
        /// <summary>
        /// Gets access to the italic label used for panel style backgrounds.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining italic label appearance for use on panel style backgrounds.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteLabel LabelItalicPanel
        {
            get { return _labelItalicPanel; }
        }

        private bool ShouldSerializeLabelItalicPanel()
        {
            return !_labelItalicPanel.IsDefault;
        }
        #endregion

        #region LabelTitlePanel
        /// <summary>
        /// Gets access to the title label used for panel style backgrounds.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining title label appearance for use on panel style backgrounds.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteLabel LabelTitlePanel
        {
            get { return _labelTitlePanel; }
        }

        private bool ShouldSerializeLabelTitlePanel()
        {
            return !_labelTitlePanel.IsDefault;
        }
        #endregion

        #region LabelCaptionPanel
        /// <summary>
        /// Gets access to the caption label used for group box style backgrounds.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining caption label appearance for use on group box style backgrounds.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteLabel LabelCaptionPanel
        {
            get { return _labelCaptionPanel; }
        }

        private bool ShouldSerializeLabelCaptionPanel()
        {
            return !_labelCaptionPanel.IsDefault;
        }
        #endregion

        #region LabelToolTip
        /// <summary>
        /// Gets access to the tooltip label appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining the tooltip label appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteLabel LabelToolTip
        {
            get { return _labelToolTip; }
        }

        private bool ShouldSerializeLabelToolTip()
        {
            return !_labelToolTip.IsDefault;
        }
        #endregion

        #region LabelSuperTip
        /// <summary>
        /// Gets access to the super tooltip label appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining the super tooltip label appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteLabel LabelSuperTip
        {
            get { return _labelSuperTip; }
        }

        private bool ShouldSerializeLabelSuperTip()
        {
            return !_labelSuperTip.IsDefault;
        }
        #endregion

        #region LabelKeyTip
        /// <summary>
        /// Gets access to the keytip label appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining the keytip label appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteLabel LabelKeyTip
        {
            get { return _labelKeyTip; }
        }

        private bool ShouldSerializeLabelKeyTip()
        {
            return !_labelKeyTip.IsDefault;
        }
        #endregion

        #region LabelCustom1
        /// <summary>
        /// Gets access to the first custom label appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining the first custom label appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteLabel LabelCustom1
        {
            get { return _labelCustom1; }
        }

        private bool ShouldSerializeLabelCustom1()
        {
            return !_labelCustom1.IsDefault;
        }
        #endregion

        #region LabelCustom2
        /// <summary>
        /// Gets access to the second custom label appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining the first second label appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteLabel LabelCustom2
        {
            get { return _labelCustom2; }
        }

        private bool ShouldSerializeLabelCustom2()
        {
            return !_labelCustom2.IsDefault;
        }
        #endregion

        #region LabelCustom3
        /// <summary>
        /// Gets access to the third custom label appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining the third second label appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteLabel LabelCustom3
        {
            get { return _labelCustom3; }
        }

        private bool ShouldSerializeLabelCustom3()
        {
            return !_labelCustom3.IsDefault;
        }
        #endregion
    }
}
