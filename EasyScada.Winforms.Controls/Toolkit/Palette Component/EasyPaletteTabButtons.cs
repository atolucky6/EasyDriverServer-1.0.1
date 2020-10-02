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
    /// Storage for tab button palette settings.
    /// </summary>
    public class EasyPaletteTabButtons : Storage
    {
        #region Instance Fields
        private EasyPaletteTabButton _tabCommon;
        private EasyPaletteTabButton _tabHighProfile;
        private EasyPaletteTabButton _tabStandardProfile;
        private EasyPaletteTabButton _tabLowProfile;
        private EasyPaletteTabButton _tabDock;
        private EasyPaletteTabButton _tabDockAutoHidden;
        private EasyPaletteTabButton _tabOneNote;
        private EasyPaletteTabButton _tabCustom1;
        private EasyPaletteTabButton _tabCustom2;
        private EasyPaletteTabButton _tabCustom3;
        #endregion

        #region Identity
        /// <summary>
        /// Initialize a new instance of the EasyPaletteTabButtons class.
        /// </summary>
        /// <param name="redirector">Palette redirector for sourcing inherited values.</param>
        /// <param name="needPaint">Delegate for notifying paint requests.</param>
        internal EasyPaletteTabButtons(PaletteRedirect redirector,
                                       NeedPaintHandler needPaint)
        {
            Debug.Assert(redirector != null);

            // Create the button style specific and common palettes
            _tabCommon = new EasyPaletteTabButton(redirector, PaletteBackStyle.TabHighProfile, PaletteBorderStyle.TabHighProfile, PaletteContentStyle.TabHighProfile, needPaint);
            _tabHighProfile = new EasyPaletteTabButton(redirector, PaletteBackStyle.TabHighProfile, PaletteBorderStyle.TabHighProfile, PaletteContentStyle.TabHighProfile, needPaint);
            _tabStandardProfile = new EasyPaletteTabButton(redirector, PaletteBackStyle.TabStandardProfile, PaletteBorderStyle.TabStandardProfile, PaletteContentStyle.TabStandardProfile, needPaint);
            _tabLowProfile = new EasyPaletteTabButton(redirector, PaletteBackStyle.TabLowProfile, PaletteBorderStyle.TabLowProfile, PaletteContentStyle.TabLowProfile, needPaint);
            _tabDock = new EasyPaletteTabButton(redirector, PaletteBackStyle.TabDock, PaletteBorderStyle.TabDock, PaletteContentStyle.TabDock, needPaint);
            _tabDockAutoHidden = new EasyPaletteTabButton(redirector, PaletteBackStyle.TabDockAutoHidden, PaletteBorderStyle.TabDockAutoHidden, PaletteContentStyle.TabDockAutoHidden, needPaint);
            _tabOneNote = new EasyPaletteTabButton(redirector, PaletteBackStyle.TabOneNote, PaletteBorderStyle.TabOneNote, PaletteContentStyle.TabOneNote, needPaint);
            _tabCustom1 = new EasyPaletteTabButton(redirector, PaletteBackStyle.TabCustom1, PaletteBorderStyle.TabCustom1, PaletteContentStyle.TabCustom1, needPaint);
            _tabCustom2 = new EasyPaletteTabButton(redirector, PaletteBackStyle.TabCustom2, PaletteBorderStyle.TabCustom2, PaletteContentStyle.TabCustom2, needPaint);
            _tabCustom3 = new EasyPaletteTabButton(redirector, PaletteBackStyle.TabCustom3, PaletteBorderStyle.TabCustom3, PaletteContentStyle.TabCustom3, needPaint);

            // Create redirectors for inheriting from style specific to style common
            PaletteRedirectTriple redirectCommon = new PaletteRedirectTriple(redirector, 
                                                                             _tabCommon.StateDisabled, _tabCommon.StateNormal,
                                                                             _tabCommon.StatePressed, _tabCommon.StateTracking,
                                                                             _tabCommon.StateSelected,_tabCommon.OverrideFocus);
            // Inform the button style to use the new redirector
            _tabHighProfile.SetRedirector(redirectCommon);
            _tabStandardProfile.SetRedirector(redirectCommon);
            _tabLowProfile.SetRedirector(redirectCommon);
            _tabDock.SetRedirector(redirectCommon);
            _tabDockAutoHidden.SetRedirector(redirectCommon);
            _tabOneNote.SetRedirector(redirectCommon);
            _tabCustom1.SetRedirector(redirectCommon);
            _tabCustom2.SetRedirector(redirectCommon);
            _tabCustom3.SetRedirector(redirectCommon);
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
                return _tabCommon.IsDefault &&
                       _tabHighProfile.IsDefault &&
                       _tabStandardProfile.IsDefault &&
                       _tabLowProfile.IsDefault &&
                       _tabDock.IsDefault &&
                       _tabDockAutoHidden.IsDefault &&
                       _tabOneNote.IsDefault &&
                       _tabCustom1.IsDefault &&
                       _tabCustom2.IsDefault &&
                       _tabCustom3.IsDefault;
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
            common.StateCommon.BackStyle = PaletteBackStyle.TabHighProfile;
            common.StateCommon.BorderStyle = PaletteBorderStyle.TabHighProfile;
            common.StateCommon.ContentStyle = PaletteContentStyle.TabHighProfile;
            _tabHighProfile.PopulateFromBase();
            common.StateCommon.BackStyle = PaletteBackStyle.TabStandardProfile;
            common.StateCommon.BorderStyle = PaletteBorderStyle.TabStandardProfile;
            common.StateCommon.ContentStyle = PaletteContentStyle.TabStandardProfile;
            _tabStandardProfile.PopulateFromBase();
            common.StateCommon.BackStyle = PaletteBackStyle.TabLowProfile;
            common.StateCommon.BorderStyle = PaletteBorderStyle.TabLowProfile;
            common.StateCommon.ContentStyle = PaletteContentStyle.TabLowProfile;
            _tabLowProfile.PopulateFromBase();
            common.StateCommon.BackStyle = PaletteBackStyle.TabDock;
            common.StateCommon.BorderStyle = PaletteBorderStyle.TabDock;
            common.StateCommon.ContentStyle = PaletteContentStyle.TabDock;
            _tabDock.PopulateFromBase();
            common.StateCommon.BackStyle = PaletteBackStyle.TabDockAutoHidden;
            common.StateCommon.BorderStyle = PaletteBorderStyle.TabDockAutoHidden;
            common.StateCommon.ContentStyle = PaletteContentStyle.TabDockAutoHidden;
            _tabDockAutoHidden.PopulateFromBase();
            common.StateCommon.BackStyle = PaletteBackStyle.TabOneNote;
            common.StateCommon.BorderStyle = PaletteBorderStyle.TabOneNote;
            common.StateCommon.ContentStyle = PaletteContentStyle.TabOneNote;
            _tabOneNote.PopulateFromBase();
        }
        #endregion

        #region TabCommon
        /// <summary>
        /// Gets access to the common appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining common appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteTabButton TabCommon
        {
            get { return _tabCommon; }
        }

        private bool ShouldSerializeTabCommon()
        {
            return !_tabCommon.IsDefault;
        }
        #endregion

        #region TabHighProfile
        /// <summary>
        /// Gets access to the High Profile appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining High Profile appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteTabButton TabHighProfile
        {
            get { return _tabHighProfile; }
        }

        private bool ShouldSerializeTabHighProfile()
        {
            return !_tabHighProfile.IsDefault;
        }
        #endregion

        #region TabStandardProfile
        /// <summary>
        /// Gets access to the Standard Profile appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining Standard Profile appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteTabButton TabStandardProfile
        {
            get { return _tabStandardProfile; }
        }

        private bool ShouldSerializeTabStandardProfile()
        {
            return !_tabStandardProfile.IsDefault;
        }
        #endregion

        #region TabLowProfile
        /// <summary>
        /// Gets access to the LowProfile appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining LowProfile appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteTabButton TabLowProfile
        {
            get { return _tabLowProfile; }
        }

        private bool ShouldSerializeTabLowProfile()
        {
            return !_tabLowProfile.IsDefault;
        }
        #endregion

        #region TabDock
        /// <summary>
        /// Gets access to the Dock appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining Dock appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteTabButton TabDock
        {
            get { return _tabDock; }
        }

        private bool ShouldSerializeTabDock()
        {
            return !_tabDock.IsDefault;
        }
        #endregion

        #region TabDockAutoHidden
        /// <summary>
        /// Gets access to the Dock AutoHidden appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining Dock AutoHidden appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteTabButton TabDockAutoHidden
        {
            get { return _tabDockAutoHidden; }
        }

        private bool ShouldSerializeTabDockAutoHidden()
        {
            return !_tabDockAutoHidden.IsDefault;
        }
        #endregion

        #region TabOneNote
        /// <summary>
        /// Gets access to the OneNote appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining OneNote appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteTabButton TabOneNote
        {
            get { return _tabOneNote; }
        }

        private bool ShouldSerializeTabOneNote()
        {
            return !_tabOneNote.IsDefault;
        }
        #endregion

        #region TabCustom1
        /// <summary>
        /// Gets access to the Custom1 appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining Custom1 appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteTabButton TabCustom1
        {
            get { return _tabCustom1; }
        }

        private bool ShouldSerializeTabCustom1()
        {
            return !_tabCustom1.IsDefault;
        }
        #endregion

        #region TabCustom2
        /// <summary>
        /// Gets access to the Custom2 appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining Custom2 appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteTabButton TabCustom2
        {
            get { return _tabCustom2; }
        }

        private bool ShouldSerializeTabCustom2()
        {
            return !_tabCustom2.IsDefault;
        }
        #endregion

        #region TabCustom3
        /// <summary>
        /// Gets access to the Custom3 appearance entries.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining Custom3 appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteTabButton TabCustom3
        {
            get { return _tabCustom3; }
        }

        private bool ShouldSerializeTabCustom3()
        {
            return !_tabCustom3.IsDefault;
        }
        #endregion
    }
}
