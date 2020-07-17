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
    /// Settings associated with ribbon control.
    /// </summary>
    public class EasyPaletteRibbon : Storage
    {
        #region Instance Fields
        private PaletteRedirect _redirect;
        private PaletteRibbonBackInheritRedirect _ribbonAppMenuOuterInherit;
        private PaletteRibbonBackInheritRedirect _ribbonAppMenuInnerInherit;
        private PaletteRibbonBackInheritRedirect _ribbonAppMenuDocsInherit;
        private PaletteRibbonTextInheritRedirect _ribbonAppMenuDocsTitleInherit;
        private PaletteRibbonTextInheritRedirect _ribbonAppMenuDocsEntryInherit;
        private PaletteRibbonGeneralInheritRedirect _ribbonGeneralRedirect;
        private PaletteRibbonBackInheritRedirect _ribbonQATFullRedirect;
        private PaletteRibbonBackInheritRedirect _ribbonQATOverRedirect;
        private PaletteRibbonBackInheritRedirect _ribbonGalleryBackRedirect;
        private PaletteRibbonBackInheritRedirect _ribbonGalleryBorderRedirect;

        private PaletteRibbonGeneral _ribbonGeneral;
        private EasyPaletteRibbonAppButton _ribbonAppButton;
        private EasyPaletteRibbonGroupArea _ribbonGroupArea;
        private EasyPaletteRibbonGroupButtonText _ribbonGroupButtonText;
        private EasyPaletteRibbonGroupCheckBoxText _ribbonGroupCheckBoxText;
        private EasyPaletteRibbonGroupNormalBorder _ribbonGroupNormalBorder;
        private EasyPaletteRibbonGroupNormalTitle _ribbonGroupNormalTitle;
        private EasyPaletteRibbonGroupCollapsedBorder _ribbonGroupCollapsedBorder;
        private EasyPaletteRibbonGroupCollapsedBack _ribbonGroupCollapsedBack;
        private EasyPaletteRibbonGroupCollapsedFrameBorder _ribbonGroupCollapsedFrameBorder;
        private EasyPaletteRibbonGroupCollapsedFrameBack _ribbonGroupCollapsedFrameBack;
        private EasyPaletteRibbonGroupCollapsedText _ribbonGroupCollapsedText;
        private EasyPaletteRibbonGroupRadioButtonText _ribbonGroupRadioButtonText;
        private EasyPaletteRibbonGroupLabelText _ribbonGroupLabelText;
        private PaletteRibbonBack _ribbonQATFullbar;
        private EasyPaletteRibbonQATMinibar _ribbonQATMinibar;
        private PaletteRibbonBack _ribbonQATOverflow;
        private EasyPaletteRibbonTab _ribbonTab;
        private PaletteRibbonBack _ribbonAppMenuInner;
        private PaletteRibbonBack _ribbonAppMenuOuter;
        private PaletteRibbonBack _ribbonAppMenuDocs;
        private PaletteRibbonText _ribbonAppMenuDocsTitle;
        private PaletteRibbonText _ribbonAppMenuDocsEntry;
        private PaletteRibbonBack _ribbonGalleryBack;
        private PaletteRibbonBack _ribbonGalleryBorder;
        #endregion

        #region Identity
        /// <summary>
        /// Initialize a new instance of the EasyPaletteRibbon class.
        /// </summary>
        /// <param name="redirect">Redirector to inherit values from.</param>
        /// <param name="needPaint">Delegate for notifying paint requests.</param>
        internal EasyPaletteRibbon(PaletteRedirect redirect,
                                      NeedPaintHandler needPaint)
        {
            Debug.Assert(redirect != null);

            // Store incoming reference
            _redirect = redirect;

            // Create redirectors
            _ribbonGeneralRedirect = new PaletteRibbonGeneralInheritRedirect(redirect);
            _ribbonAppMenuInnerInherit = new PaletteRibbonBackInheritRedirect(redirect, PaletteRibbonBackStyle.RibbonAppMenuInner);
            _ribbonAppMenuOuterInherit = new PaletteRibbonBackInheritRedirect(redirect, PaletteRibbonBackStyle.RibbonAppMenuOuter);
            _ribbonAppMenuDocsInherit = new PaletteRibbonBackInheritRedirect(redirect, PaletteRibbonBackStyle.RibbonAppMenuDocs);
            _ribbonAppMenuDocsTitleInherit = new PaletteRibbonTextInheritRedirect(redirect, PaletteRibbonTextStyle.RibbonAppMenuDocsTitle);
            _ribbonAppMenuDocsEntryInherit = new PaletteRibbonTextInheritRedirect(redirect, PaletteRibbonTextStyle.RibbonAppMenuDocsEntry);
            _ribbonQATFullRedirect = new PaletteRibbonBackInheritRedirect(redirect, PaletteRibbonBackStyle.RibbonQATFullbar);
            _ribbonQATOverRedirect = new PaletteRibbonBackInheritRedirect(redirect, PaletteRibbonBackStyle.RibbonQATOverflow);
            _ribbonGalleryBackRedirect = new PaletteRibbonBackInheritRedirect(redirect, PaletteRibbonBackStyle.RibbonGalleryBack);
            _ribbonGalleryBorderRedirect = new PaletteRibbonBackInheritRedirect(redirect, PaletteRibbonBackStyle.RibbonGalleryBorder);

            // Create palettes
            _ribbonGeneral = new PaletteRibbonGeneral(_ribbonGeneralRedirect, needPaint);
            _ribbonAppButton = new EasyPaletteRibbonAppButton(redirect, needPaint);
            _ribbonAppMenuInner = new PaletteRibbonBack(_ribbonAppMenuInnerInherit, needPaint);
            _ribbonAppMenuOuter = new PaletteRibbonBack(_ribbonAppMenuOuterInherit, needPaint);
            _ribbonAppMenuDocs = new PaletteRibbonBack(_ribbonAppMenuDocsInherit, needPaint);
            _ribbonAppMenuDocsTitle = new PaletteRibbonText(_ribbonAppMenuDocsTitleInherit, needPaint);
            _ribbonAppMenuDocsEntry = new PaletteRibbonText(_ribbonAppMenuDocsEntryInherit, needPaint);
            _ribbonGroupArea = new EasyPaletteRibbonGroupArea(redirect, needPaint);
            _ribbonGroupButtonText = new EasyPaletteRibbonGroupButtonText(redirect, needPaint);
            _ribbonGroupCheckBoxText = new EasyPaletteRibbonGroupCheckBoxText(redirect, needPaint);
            _ribbonGroupNormalBorder = new EasyPaletteRibbonGroupNormalBorder(redirect, needPaint);
            _ribbonGroupNormalTitle = new EasyPaletteRibbonGroupNormalTitle(redirect, needPaint);
            _ribbonGroupCollapsedBorder = new EasyPaletteRibbonGroupCollapsedBorder(redirect, needPaint);
            _ribbonGroupCollapsedBack = new EasyPaletteRibbonGroupCollapsedBack(redirect, needPaint);
            _ribbonGroupCollapsedFrameBorder = new EasyPaletteRibbonGroupCollapsedFrameBorder(redirect, needPaint);
            _ribbonGroupCollapsedFrameBack = new EasyPaletteRibbonGroupCollapsedFrameBack(redirect, needPaint);
            _ribbonGroupCollapsedText = new EasyPaletteRibbonGroupCollapsedText(redirect, needPaint);
            _ribbonGroupRadioButtonText = new EasyPaletteRibbonGroupRadioButtonText(redirect, needPaint);
            _ribbonGroupLabelText = new EasyPaletteRibbonGroupLabelText(redirect, needPaint);
            _ribbonQATFullbar = new PaletteRibbonBack(_ribbonQATFullRedirect, needPaint);
            _ribbonQATMinibar = new EasyPaletteRibbonQATMinibar(redirect, needPaint);
            _ribbonQATOverflow = new PaletteRibbonBack(_ribbonQATOverRedirect, needPaint);
            _ribbonTab = new EasyPaletteRibbonTab(redirect, needPaint);
            _ribbonGalleryBack = new PaletteRibbonBack(_ribbonGalleryBackRedirect, needPaint);
            _ribbonGalleryBorder = new PaletteRibbonBack(_ribbonGalleryBorderRedirect, needPaint);
        }

        /// <summary>
        /// Gets a value indicating if all values are default.
        /// </summary>
        public override bool IsDefault
        {
            get
            {
                return RibbonAppButton.IsDefault &&
                       RibbonAppMenuOuter.IsDefault &&
                       RibbonAppMenuInner.IsDefault &&
                       RibbonAppMenuDocs.IsDefault &&
                       RibbonAppMenuDocsTitle.IsDefault &&
                       RibbonAppMenuDocsEntry.IsDefault &&
                       RibbonGeneral.IsDefault &&
                       RibbonGroupArea.IsDefault &&
                       RibbonGroupButtonText.IsDefault &&
                       RibbonGroupCheckBoxText.IsDefault &&
                       RibbonGroupNormalBorder.IsDefault &&
                       RibbonGroupNormalTitle.IsDefault &&
                       RibbonGroupCollapsedBorder.IsDefault &&
                       RibbonGroupCollapsedBack.IsDefault &&
                       RibbonGroupCollapsedFrameBorder.IsDefault &&
                       RibbonGroupCollapsedFrameBack.IsDefault &&
                       RibbonGroupCollapsedText.IsDefault &&
                       RibbonGroupLabelText.IsDefault &&
                       RibbonGroupRadioButtonText.IsDefault &&
                       RibbonQATFullbar.IsDefault &&
                       RibbonQATMinibar.IsDefault &&
                       RibbonTab.IsDefault &&
                       RibbonGalleryBack.IsDefault &&
                       RibbonGalleryBorder.IsDefault;
            }
        }
        #endregion

        #region PopulateFromBase
        /// <summary>
        /// Populate values from the base palette.
        /// </summary>
        public void PopulateFromBase()
        {
            RibbonAppButton.PopulateFromBase();
            RibbonAppMenuOuter.PopulateFromBase(PaletteState.Normal);
            RibbonAppMenuInner.PopulateFromBase(PaletteState.Normal);
            RibbonAppMenuDocs.PopulateFromBase(PaletteState.Normal);
            RibbonAppMenuDocsTitle.PopulateFromBase(PaletteState.Normal);
            RibbonAppMenuDocsEntry.PopulateFromBase(PaletteState.Normal);
            RibbonGeneral.PopulateFromBase();
            RibbonGroupArea.PopulateFromBase();
            RibbonGroupButtonText.PopulateFromBase();
            RibbonGroupCheckBoxText.PopulateFromBase();
            RibbonGroupNormalBorder.PopulateFromBase();
            RibbonGroupNormalTitle.PopulateFromBase();
            RibbonGroupCollapsedBack.PopulateFromBase();
            RibbonGroupCollapsedBorder.PopulateFromBase();
            RibbonGroupCollapsedFrameBorder.PopulateFromBase();
            RibbonGroupCollapsedFrameBack.PopulateFromBase();
            RibbonGroupCollapsedText.PopulateFromBase();
            RibbonGroupRadioButtonText.PopulateFromBase();
            RibbonGroupLabelText.PopulateFromBase();
            RibbonQATFullbar.PopulateFromBase(PaletteState.Normal);
            RibbonQATMinibar.PopulateFromBase();
            RibbonQATOverflow.PopulateFromBase(PaletteState.Normal);
            RibbonTab.PopulateFromBase();
            RibbonGalleryBack.PopulateFromBase(PaletteState.Normal);
            RibbonGalleryBorder.PopulateFromBase(PaletteState.Normal);
        }
        #endregion

        #region RibbonAppButton
        /// <summary>
        /// Get access to the application button tab settings.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Ribbon application button specific settings.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteRibbonAppButton RibbonAppButton
        {
            get { return _ribbonAppButton; }
        }

        private bool ShouldSerializeRibbonAppButton()
        {
            return !_ribbonAppButton.IsDefault;
        }
        #endregion

        #region RibbonAppMenuOuter
        /// <summary>
        /// Gets access to the application button menu outer palette details.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining application button menu outer appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public virtual PaletteRibbonBack RibbonAppMenuOuter
        {
            get { return _ribbonAppMenuOuter; }
        }

        private bool ShouldSerializeRibbonAppMenuOuter()
        {
            return !_ribbonAppMenuOuter.IsDefault;
        }
        #endregion

        #region RibbonAppMenuInner
        /// <summary>
        /// Gets access to the application button menu inner palette details.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining application button menu inner appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public virtual PaletteRibbonBack RibbonAppMenuInner
        {
            get { return _ribbonAppMenuInner; }
        }

        private bool ShouldSerializeRibbonAppMenuInner()
        {
            return !_ribbonAppMenuInner.IsDefault;
        }
        #endregion

        #region RibbonAppMenuDocs
        /// <summary>
        /// Gets access to the application button menu recent docs palette details.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining application button menu recent docs appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public virtual PaletteRibbonBack RibbonAppMenuDocs
        {
            get { return _ribbonAppMenuDocs; }
        }

        private bool ShouldSerializeRibbonAppMenuDocs()
        {
            return !_ribbonAppMenuDocs.IsDefault;
        }
        #endregion

        #region RibbonAppMenuDocsTitle
        /// <summary>
        /// Gets access to the application button menu recent documents title.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining application button menu recent documents title.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public virtual PaletteRibbonText RibbonAppMenuDocsTitle
        {
            get { return _ribbonAppMenuDocsTitle; }
        }

        private bool ShouldSerializeRibbonAppMenuDocsTitle()
        {
            return !_ribbonAppMenuDocsTitle.IsDefault;
        }
        #endregion

        #region RibbonAppMenuDocsEntry
        /// <summary>
        /// Gets access to the application button menu recent documents entry.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining application button menu recent documents entry.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public virtual PaletteRibbonText RibbonAppMenuDocsEntry
        {
            get { return _ribbonAppMenuDocsEntry; }
        }

        private bool ShouldSerializeRibbonAppMenuDocsEntry()
        {
            return !_ribbonAppMenuDocsEntry.IsDefault;
        }
        #endregion

        #region RibbonGeneral
        /// <summary>
        /// Get access to the general ribbon settings.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Ribbon general settings.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public PaletteRibbonGeneral RibbonGeneral
        {
            get { return _ribbonGeneral; }
        }

        private bool ShouldSerializeRibbonGeneral()
        {
            return !_ribbonGeneral.IsDefault;
        }
        #endregion

        #region RibbonGroupArea
        /// <summary>
        /// Get access to the ribbon group area settings.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Ribbon group area specific settings.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteRibbonGroupArea RibbonGroupArea
        {
            get { return _ribbonGroupArea; }
        }

        private bool ShouldSerializeRibbonGroupArea()
        {
            return !_ribbonGroupArea.IsDefault;
        }
        #endregion

        #region RibbonGroupButtonText
        /// <summary>
        /// Get access to the ribbon group button text settings.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Ribbon group button text specific settings.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteRibbonGroupButtonText RibbonGroupButtonText
        {
            get { return _ribbonGroupButtonText; }
        }

        private bool ShouldSerializeRibbonGroupButtonText()
        {
            return !_ribbonGroupButtonText.IsDefault;
        }
        #endregion

        #region RibbonGroupCheckBoxText
        /// <summary>
        /// Get access to the ribbon group check box text settings.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Ribbon group check box text specific settings.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteRibbonGroupCheckBoxText RibbonGroupCheckBoxText
        {
            get { return _ribbonGroupCheckBoxText; }
        }

        private bool ShouldSerializeRibbonGroupCheckBoxText()
        {
            return !_ribbonGroupCheckBoxText.IsDefault;
        }
        #endregion

        #region RibbonGroupNormalBorder
        /// <summary>
        /// Get access to the ribbon group normal border settings.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Ribbon group normal border specific settings.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteRibbonGroupNormalBorder RibbonGroupNormalBorder
        {
            get { return _ribbonGroupNormalBorder; }
        }

        private bool ShouldSerializeRibbonGroupNormalBorder()
        {
            return !_ribbonGroupNormalBorder.IsDefault;
        }
        #endregion

        #region RibbonGroupNormalTitle
        /// <summary>
        /// Get access to the ribbon group normal title settings.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Ribbon group normal title specific settings.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteRibbonGroupNormalTitle RibbonGroupNormalTitle
        {
            get { return _ribbonGroupNormalTitle; }
        }

        private bool ShouldSerializeRibbonGroupNormalTitle()
        {
            return !_ribbonGroupNormalTitle.IsDefault;
        }
        #endregion

        #region RibbonGroupCollapsedBorder
        /// <summary>
        /// Get access to the ribbon group collapsed border settings.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Ribbon group collapsed border specific settings.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteRibbonGroupCollapsedBorder RibbonGroupCollapsedBorder
        {
            get { return _ribbonGroupCollapsedBorder; }
        }

        private bool ShouldSerializeRibbonGroupCollapsedBorder()
        {
            return !_ribbonGroupCollapsedBorder.IsDefault;
        }
        #endregion

        #region RibbonGroupCollapsedBack
        /// <summary>
        /// Get access to the ribbon group collapsed background settings.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Ribbon group collapsed background specific settings.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteRibbonGroupCollapsedBack RibbonGroupCollapsedBack
        {
            get { return _ribbonGroupCollapsedBack; }
        }

        private bool ShouldSerializeRibbonGroupCollapsedBack()
        {
            return !_ribbonGroupCollapsedBack.IsDefault;
        }
        #endregion

        #region RibbonGroupCollapsedFrameBorder
        /// <summary>
        /// Get access to the ribbon group collapsed frame border settings.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Ribbon group collapsed frame border specific settings.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteRibbonGroupCollapsedFrameBorder RibbonGroupCollapsedFrameBorder
        {
            get { return _ribbonGroupCollapsedFrameBorder; }
        }

        private bool ShouldSerializeRibbonGroupCollapsedFrameBorder()
        {
            return !_ribbonGroupCollapsedFrameBorder.IsDefault;
        }
        #endregion

        #region RibbonGroupCollapsedFrameBack
        /// <summary>
        /// Get access to the ribbon group collapsed frame background settings.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Ribbon group collapsed frame background specific settings.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteRibbonGroupCollapsedFrameBack RibbonGroupCollapsedFrameBack
        {
            get { return _ribbonGroupCollapsedFrameBack; }
        }

        private bool ShouldSerializeRibbonGroupCollapsedFrameBack()
        {
            return !_ribbonGroupCollapsedFrameBack.IsDefault;
        }
        #endregion

        #region RibbonGroupCollapsedText
        /// <summary>
        /// Get access to the ribbon group collapsed text settings.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Ribbon group collapsed text specific settings.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteRibbonGroupCollapsedText RibbonGroupCollapsedText
        {
            get { return _ribbonGroupCollapsedText; }
        }

        private bool ShouldSerializeRibbonGroupCollapsedText()
        {
            return !_ribbonGroupCollapsedText.IsDefault;
        }
        #endregion

        #region RibbonGroupLabelText
        /// <summary>
        /// Get access to the ribbon group label text settings.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Ribbon group label text specific settings.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteRibbonGroupLabelText RibbonGroupLabelText
        {
            get { return _ribbonGroupLabelText; }
        }

        private bool ShouldSerializeRibbonGroupLabelText()
        {
            return !_ribbonGroupLabelText.IsDefault;
        }
        #endregion

        #region RibbonGroupRadioButtonText
        /// <summary>
        /// Get access to the ribbon radio button box text settings.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Ribbon group radio button text specific settings.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteRibbonGroupRadioButtonText RibbonGroupRadioButtonText
        {
            get { return _ribbonGroupRadioButtonText; }
        }

        private bool ShouldSerializeRibbonGroupRadioButtonText()
        {
            return !_ribbonGroupRadioButtonText.IsDefault;
        }
        #endregion

        #region RibbonQATFullbar
        /// <summary>
        /// Get access to the quick access toolbar full settings.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Ribbon quick access toolbar full settings.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public PaletteRibbonBack RibbonQATFullbar
        {
            get { return _ribbonQATFullbar; }
        }

        private bool ShouldSerializeRibbonQATFullbar()
        {
            return !_ribbonQATFullbar.IsDefault;
        }
        #endregion

        #region RibbonQATMinibar
        /// <summary>
        /// Get access to the quick access toolbar mini settings.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Ribbon quick access toolbar mini settings.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteRibbonQATMinibar RibbonQATMinibar
        {
            get { return _ribbonQATMinibar; }
        }

        private bool ShouldSerializeRibbonQATMinibar()
        {
            return !_ribbonQATMinibar.IsDefault;
        }
        #endregion

        #region RibbonQATOverflow
        /// <summary>
        /// Get access to the quick access toolbar overflow settings.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Ribbon quick access toolbar overflow settings.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public PaletteRibbonBack RibbonQATOverflow
        {
            get { return _ribbonQATOverflow; }
        }

        private bool ShouldSerializeRibbonQATOverflow()
        {
            return !_ribbonQATOverflow.IsDefault;
        }
        #endregion

        #region RibbonTab
        /// <summary>
        /// Get access to the ribbon tab settings.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Ribbon tab specific settings.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteRibbonTab RibbonTab
        {
            get { return _ribbonTab; }
        }

        private bool ShouldSerializeRibbonTab()
        {
            return !_ribbonTab.IsDefault;
        }
        #endregion

        #region RibbonGalleryBack
        /// <summary>
        /// Gets access to the ribbon gallery background palette details.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining ribbon gallery background appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public virtual PaletteRibbonBack RibbonGalleryBack
        {
            get { return _ribbonGalleryBack; }
        }

        private bool ShouldSerializeRibbonGalleryBack()
        {
            return !_ribbonGalleryBack.IsDefault;
        }
        #endregion

        #region RibbonGalleryBorder
        /// <summary>
        /// Gets access to the ribbon gallery border palette details.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining ribbon gallery border appearance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public virtual PaletteRibbonBack RibbonGalleryBorder
        {
            get { return _ribbonGalleryBorder; }
        }

        private bool ShouldSerializeRibbonGalleryBorder()
        {
            return !_ribbonGalleryBorder.IsDefault;
        }
        #endregion
    }
}
