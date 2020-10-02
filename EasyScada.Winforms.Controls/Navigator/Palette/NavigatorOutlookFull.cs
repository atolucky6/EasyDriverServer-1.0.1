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
	/// Storage for outlook full mode related properties.
	/// </summary>
    public class NavigatorOutlookFull : Storage
    {
        #region Instance Fields
        private EasyNavigator _navigator;
        private MapEasyPageText _overflowMapText;
        private MapEasyPageText _overflowMapExtraText;
        private MapEasyPageImage _overflowMapImage;
        private MapEasyPageText _stackMapText;
        private MapEasyPageText _stackMapExtraText;
        private MapEasyPageImage _stackMapImage;
        #endregion

        #region Identity
        /// <summary>
        /// Initialize a new instance of the NavigatorOutlookFull class.
		/// </summary>
        /// <param name="navigator">Reference to owning navigator instance.</param>
        /// <param name="needPaint">Delegate for notifying paint requests.</param>
        public NavigatorOutlookFull(EasyNavigator navigator,
                                    NeedPaintHandler needPaint)
		{
            Debug.Assert(navigator != null);
            
            // Remember back reference
            _navigator = navigator;

            // Store the provided paint notification delegate
            NeedPaint = needPaint;

            // Default values
            _overflowMapImage = MapEasyPageImage.Small;
            _overflowMapText = MapEasyPageText.None;
            _overflowMapExtraText = MapEasyPageText.None;
            _stackMapImage = MapEasyPageImage.MediumSmall;
            _stackMapText = MapEasyPageText.TextTitle;
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
                return ((OverflowMapImage == MapEasyPageImage.Small) &&
                        (OverflowMapText == MapEasyPageText.None) &&
                        (OverflowMapExtraText == MapEasyPageText.None) &&
                        (StackMapImage == MapEasyPageImage.MediumSmall) &&
                        (StackMapText == MapEasyPageText.TextTitle) &&
                        (StackMapExtraText == MapEasyPageText.None));
                        
            }
        }
        #endregion

        #region OverflowMapImage
        /// <summary>
        /// Gets and sets the mapping used for the overflow item image.
        /// </summary>
        [Localizable(true)]
        [Category("Visuals")]
        [Description("Mapping used for the overflow item image.")]
        [RefreshPropertiesAttribute(RefreshProperties.All)]
        [DefaultValue(typeof(MapEasyPageImage), "Small")]
        public virtual MapEasyPageImage OverflowMapImage
        {
            get { return _overflowMapImage; }

            set
            {
                if (_overflowMapImage != value)
                {
                    _overflowMapImage = value;
                    PerformNeedPaint(true);
                }
            }
        }

        /// <summary>
        /// Resets the OverflowMapImage property to its default value.
        /// </summary>
        public void ResetOverflowMapImage()
        {
            OverflowMapImage = MapEasyPageImage.Small;
        }
        #endregion

        #region OverflowMapText
        /// <summary>
        /// Gets and sets the mapping used for the overflow item text.
        /// </summary>
        [Category("Visuals")]
        [Description("Mapping used for the overflow item text.")]
        [RefreshPropertiesAttribute(RefreshProperties.All)]
        [DefaultValue(typeof(MapEasyPageText), "None (Empty string)")]
        public MapEasyPageText OverflowMapText
        {
            get { return _overflowMapText; }

            set
            {
                if (_overflowMapText != value)
                {
                    _overflowMapText = value;
                    PerformNeedPaint(true);
                }
            }
        }

        /// <summary>
        /// Resets the OverflowMapText property to its default value.
        /// </summary>
        public void ResetOverflowMapText()
        {
            OverflowMapText = MapEasyPageText.None;
        }
        #endregion

        #region OverflowMapExtraText
        /// <summary>
        /// Gets and sets the mapping used for the overflow item description.
        /// </summary>
        [Category("Visuals")]
        [Description("Mapping used for the overflow item description.")]
        [RefreshPropertiesAttribute(RefreshProperties.All)]
        [DefaultValue(typeof(MapEasyPageText), "None (Empty string)")]
        public MapEasyPageText OverflowMapExtraText
        {
            get { return _overflowMapExtraText; }

            set
            {
                if (_overflowMapExtraText != value)
                {
                    _overflowMapExtraText = value;
                    PerformNeedPaint(true);
                }
            }
        }

        /// <summary>
        /// Resets the OverflowMapExtraText property to its default value.
        /// </summary>
        public void ResetOverflowMapExtraText()
        {
            OverflowMapExtraText = MapEasyPageText.None;
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
        [DefaultValue(typeof(MapEasyPageText), "Text - Title")]
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
            StackMapText = MapEasyPageText.TextTitle;
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
