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
    /// Storage for palette image settings.
    /// </summary>
    public class EasyPaletteImages : Storage
    {
        #region Instance Fields
        private EasyPaletteImagesCheckBox _imagesCheckBox;
        private EasyPaletteImagesContextMenu _imagesContextMenu;
        private EasyPaletteImagesDropDownButton _imagesDropDownButton;
        private EasyPaletteImagesGalleryButtons _imagesGalleryButtons;
        private EasyPaletteImagesRadioButton _imagesRadioButton;
        private EasyPaletteImagesTreeView _imagesTreeView;
        #endregion

        #region Identity
        /// <summary>
        /// Initialize a new instance of the EasyPaletteImages class.
        /// </summary>
        /// <param name="redirector">Palette redirector for sourcing inherited values.</param>
        /// <param name="needPaint">Delegate for notifying paint requests.</param>
        internal EasyPaletteImages(PaletteRedirect redirector,
                                      NeedPaintHandler needPaint)
        {
            Debug.Assert(redirector != null);

            // Create the different image sets
            _imagesCheckBox = new EasyPaletteImagesCheckBox(redirector, needPaint);
            _imagesContextMenu = new EasyPaletteImagesContextMenu(redirector, needPaint);
            _imagesDropDownButton = new EasyPaletteImagesDropDownButton(redirector, needPaint);
            _imagesGalleryButtons = new EasyPaletteImagesGalleryButtons(redirector, needPaint);
            _imagesRadioButton = new EasyPaletteImagesRadioButton(redirector, needPaint);
            _imagesTreeView = new EasyPaletteImagesTreeView(redirector, needPaint);
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
                return _imagesCheckBox.IsDefault &&
                       _imagesContextMenu.IsDefault &&
                       _imagesDropDownButton.IsDefault &&
                       _imagesGalleryButtons.IsDefault &&
                       _imagesRadioButton.IsDefault &&
                       _imagesTreeView.IsDefault;
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
            _imagesCheckBox.PopulateFromBase();
            _imagesContextMenu.PopulateFromBase();
            _imagesDropDownButton.PopulateFromBase();
            _imagesGalleryButtons.PopulateFromBase();
            _imagesRadioButton.PopulateFromBase();
            _imagesTreeView.PopulateFromBase();
        }
        #endregion

        #region CheckBox
        /// <summary>
        /// Gets access to the check box set of images.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining check box images.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteImagesCheckBox CheckBox
        {
            get { return _imagesCheckBox; }
        }

        private bool ShouldSerializeCheckBox()
        {
            return !_imagesCheckBox.IsDefault;
        }
        #endregion

        #region ContextMenu
        /// <summary>
        /// Gets access to the context menu set of images.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining context menu images.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteImagesContextMenu ContextMenu
        {
            get { return _imagesContextMenu; }
        }

        private bool ShouldSerializeContextMenu()
        {
            return !_imagesContextMenu.IsDefault;
        }
        #endregion

        #region DropDownButton
        /// <summary>
        /// Gets access to the drop down button set of images.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining drop down button images.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteImagesDropDownButton DropDownButton
        {
            get { return _imagesDropDownButton; }
        }

        private bool ShouldSerializeDropDownButton()
        {
            return !_imagesDropDownButton.IsDefault;
        }
        #endregion

        #region CheckBox
        /// <summary>
        /// Gets access to the gallery button images.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining gallery button images.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteImagesGalleryButtons GalleryButtons
        {
            get { return _imagesGalleryButtons; }
        }

        private bool ShouldSerializeGalleryButtons()
        {
            return !_imagesGalleryButtons.IsDefault;
        }
        #endregion

        #region RadioButton
        /// <summary>
        /// Gets access to the radio button set of images.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining radio button images.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteImagesRadioButton RadioButton
        {
            get { return _imagesRadioButton; }
        }

        private bool ShouldSerializeRadioButton()
        {
            return !_imagesRadioButton.IsDefault;
        }
        #endregion

        #region TreeView
        /// <summary>
        /// Gets access to the tree view set of images.
        /// </summary>
        [EasyPersist]
        [Category("Visuals")]
        [Description("Overrides for defining tree view images.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EasyPaletteImagesTreeView TreeView
        {
            get { return _imagesTreeView; }
        }

        private bool ShouldSerializeTreeView()
        {
            return !_imagesTreeView.IsDefault;
        }
        #endregion
    }
}
