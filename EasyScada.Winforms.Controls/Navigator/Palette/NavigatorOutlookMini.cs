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
using System.Drawing;
using System.Drawing.Text;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using EasyScada.Winforms.Controls;

namespace EasyScada.Winforms.Controls.Navigator
{
	/// <summary>
	/// Storage for outlook mini mode related properties.
	/// </summary>
    public class NavigatorOutlookMini : Storage
    {
        #region Instance Fields
        private EasyNavigator _navigator;
        private ButtonStyle _miniButtonStyle;
        private MapEasyPageText _miniMapText;
        private MapEasyPageText _miniMapExtraText;
        private MapEasyPageImage _miniMapImage;
        private MapEasyPageText _stackMapText;
        private MapEasyPageText _stackMapExtraText;
        private MapEasyPageImage _stackMapImage;
        #endregion

        #region Identity
        /// <summary>
        /// Initialize a new instance of the NavigatorOutlookMini class.
		/// </summary>
        /// <param name="navigator">Reference to owning navigator instance.</param>
        /// <param name="needPaint">Delegate for notifying paint requests.</param>
        public NavigatorOutlookMini(EasyNavigator navigator,
                                    NeedPaintHandler needPaint)
		{
            Debug.Assert(navigator != null);
            
            // Remember back reference
            _navigator = navigator;

            // Store the provided paint notification delegate
            NeedPaint = needPaint;

            // Default values
            _miniButtonStyle = ButtonStyle.NavigatorMini;
            _miniMapImage = MapEasyPageImage.None;
            _miniMapText = MapEasyPageText.TextTitle;
            _miniMapExtraText = MapEasyPageText.None;
            _stackMapImage = MapEasyPageImage.MediumSmall;
            _stackMapText = MapEasyPageText.None;
            _stackMapExtraText = MapEasyPageText.None;
        }
		#endregion

        #region IsDefault
        /// <summary>
        /// Gets a value indicating if all values are default.
        /// </summary>
        [Browsable(false)]
        public override bool IsDefault
        {
            get
            {
                return ((MiniButtonStyle == ButtonStyle.NavigatorMini) &&
                        (MiniMapImage == MapEasyPageImage.None) &&
                        (MiniMapText == MapEasyPageText.TextTitle) &&
                        (MiniMapExtraText == MapEasyPageText.None) &&
                        (StackMapImage == MapEasyPageImage.MediumSmall) &&
                        (StackMapText == MapEasyPageText.None) &&
                        (StackMapExtraText == MapEasyPageText.None));
                        
            }
        }
        #endregion

        #region MiniButtonStyle
        /// <summary>
        /// Gets and sets the mini button style.
        /// </summary>
        [Category("Visuals")]
        [Description("Mini button style.")]
        [DefaultValue(typeof(ButtonStyle), "NavigatorMini")]
        public ButtonStyle MiniButtonStyle
        {
            get { return _miniButtonStyle; }

            set
            {
                if (_miniButtonStyle != value)
                {
                    _miniButtonStyle = value;
                    _navigator.OnViewBuilderPropertyChanged("MiniButtonStyleOutlook");
                }
            }
        }
        #endregion

        #region MiniMapImage
        /// <summary>
        /// Gets and sets the mapping used for the mini button item image.
        /// </summary>
        [Localizable(true)]
        [Category("Visuals")]
        [Description("Mapping used for the mini button item image.")]
        [RefreshPropertiesAttribute(RefreshProperties.All)]
        [DefaultValue(typeof(MapEasyPageImage), "None (Null image)")]
        public virtual MapEasyPageImage MiniMapImage
        {
            get { return _miniMapImage; }

            set
            {
                if (_miniMapImage != value)
                {
                    _miniMapImage = value;
                    PerformNeedPaint(true);
                }
            }
        }

        /// <summary>
        /// Resets the MiniMapImage property to its default value.
        /// </summary>
        public void ResetMiniMapImage()
        {
            MiniMapImage = MapEasyPageImage.None;
        }
        #endregion

        #region MiniMapText
        /// <summary>
        /// Gets and sets the mapping used for the mini button item text.
        /// </summary>
        [Category("Visuals")]
        [Description("Mapping used for the mini button item text.")]
        [RefreshPropertiesAttribute(RefreshProperties.All)]
        [DefaultValue(typeof(MapEasyPageText), "Text - Title")]
        public MapEasyPageText MiniMapText
        {
            get { return _miniMapText; }

            set
            {
                if (_miniMapText != value)
                {
                    _miniMapText = value;
                    PerformNeedPaint(true);
                }
            }
        }

        /// <summary>
        /// Resets the MiniMapText property to its default value.
        /// </summary>
        public void ResetMiniMapText()
        {
            MiniMapText = MapEasyPageText.TextTitle;
        }
        #endregion

        #region MiniMapExtraText
        /// <summary>
        /// Gets and sets the mapping used for the mini button item description.
        /// </summary>
        [Category("Visuals")]
        [Description("Mapping used for the mini button item description.")]
        [RefreshPropertiesAttribute(RefreshProperties.All)]
        [DefaultValue(typeof(MapEasyPageText), "None (Empty string)")]
        public MapEasyPageText MiniMapExtraText
        {
            get { return _miniMapExtraText; }

            set
            {
                if (_miniMapExtraText != value)
                {
                    _miniMapExtraText = value;
                    PerformNeedPaint(true);
                }
            }
        }

        /// <summary>
        /// Resets the MiniMapExtraText property to its default value.
        /// </summary>
        public void ResetMiniMapExtraText()
        {
            MiniMapExtraText = MapEasyPageText.None;
        }
        #endregion

        #region StackMapImage
        /// <summary>
        /// Gets and sets the mapping used for the stack item image.
        /// </summary>
        [Localizable(true)]
        [Category("Visuals")]
        [Description("Mapping used for the stack item image.")]
        [RefreshPropertiesAttribute(RefreshProperties.All)]
        [DefaultValue(typeof(MapEasyPageImage), "MediumSmall")]
        public virtual MapEasyPageImage StackMapImage
        {
            get { return _stackMapImage; }

            set
            {
                if (_stackMapImage != value)
                {
                    _stackMapImage = value;
                    PerformNeedPaint(true);
                }
            }
        }

        /// <summary>
        /// Resets the StackMapImage property to its default value.
        /// </summary>
        public void ResetStackMapImage()
        {
            StackMapImage = MapEasyPageImage.MediumSmall;
        }
        #endregion

        #region StackMapText
        /// <summary>
        /// Gets and sets the mapping used for the stack item text.
        /// </summary>
        [Category("Visuals")]
        [Description("Mapping used for the stack item text.")]
        [RefreshPropertiesAttribute(RefreshProperties.All)]
        [DefaultValue(typeof(MapEasyPageText), "None (Empty string)")]
        public MapEasyPageText StackMapText
        {
            get { return _stackMapText; }

            set
            {
                if (_stackMapText != value)
                {
                    _stackMapText = value;
                    PerformNeedPaint(true);
                }
            }
        }

        /// <summary>
        /// Resets the StackMapText property to its default value.
        /// </summary>
        public void ResetStackMapText()
        {
            StackMapText = MapEasyPageText.None;
        }
        #endregion

        #region StackMapExtraText
        /// <summary>
        /// Gets and sets the mapping used for the stack item description.
        /// </summary>
        [Category("Visuals")]
        [Description("Mapping used for the stack item description.")]
        [RefreshPropertiesAttribute(RefreshProperties.All)]
        [DefaultValue(typeof(MapEasyPageText), "None (Empty string)")]
        public MapEasyPageText StackMapExtraText
        {
            get { return _stackMapExtraText; }

            set
            {
                if (_stackMapExtraText != value)
                {
                    _stackMapExtraText = value;
                    PerformNeedPaint(true);
                }
            }
        }

        /// <summary>
        /// Resets the StackMapExtraText property to its default value.
        /// </summary>
        public void ResetStackMapExtraText()
        {
            StackMapExtraText = MapEasyPageText.None;
        }
        #endregion
    }
}
