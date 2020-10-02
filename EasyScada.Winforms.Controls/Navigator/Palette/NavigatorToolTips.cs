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
	/// Storage for tooltip related properties.
	/// </summary>
    public class NavigatorToolTips : Storage
    {
        #region Instance Fields
        private EasyNavigator _navigator;
        private bool _allowPageToolTips;
        private bool _allowButtonSpecToolTips;
        private MapEasyPageText _mapText;
        private MapEasyPageText _mapExtraText;
        private MapEasyPageImage _mapImage;
        #endregion

        #region Identity
        /// <summary>
        /// Initialize a new instance of the NavigatorPopupPage class.
		/// </summary>
        /// <param name="navigator">Reference to owning navigator instance.</param>
        /// <param name="needPaint">Delegate for notifying paint requests.</param>
        public NavigatorToolTips(EasyNavigator navigator,
                                NeedPaintHandler needPaint)
		{
            Debug.Assert(navigator != null);
            Debug.Assert(needPaint != null);
            
            // Remember back reference
            _navigator = navigator;

            // Store the provided paint notification delegate
            NeedPaint = needPaint;

            // Default values
            _allowPageToolTips = false;
            _allowButtonSpecToolTips = false;
            _mapImage = MapEasyPageImage.ToolTip;
            _mapText = MapEasyPageText.ToolTipTitle;
            _mapExtraText = MapEasyPageText.ToolTipBody;
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
                return (!AllowPageToolTips &&
                        !AllowButtonSpecToolTips &&
                        (MapImage == MapEasyPageImage.ToolTip) &&
                        (MapText == MapEasyPageText.ToolTipTitle) &&
                        (MapExtraText == MapEasyPageText.ToolTipBody));
            }
        }
        #endregion

        #region AllowPageToolTips
        /// <summary>
        /// Gets and sets a value indicating if tooltips should be displayed for page headers.
        /// </summary>
        [Category("Visuals")]
        [Description("Should tooltips be displayed for page headers.")]
        [DefaultValue(false)]
        public bool AllowPageToolTips
        {
            get { return _allowPageToolTips; }
            set { _allowPageToolTips = value; }
        }
        #endregion

        #region AllowButtonSpecToolTips
        /// <summary>
        /// Gets and sets a value indicating if tooltips should be displayed for button specs.
        /// </summary>
        [Category("Visuals")]
        [Description("Should tooltips be displayed for button specs.")]
        [DefaultValue(false)]
        public bool AllowButtonSpecToolTips
        {
            get { return _allowButtonSpecToolTips; }
            set { _allowButtonSpecToolTips = value; }
        }
        #endregion

        #region MapImage
        /// <summary>
        /// Gets and sets the mapping used for the tooltip image.
        /// </summary>
        [Localizable(true)]
        [Category("Visuals")]
        [Description("Mapping used for the tooltip image.")]
        [RefreshPropertiesAttribute(RefreshProperties.All)]
        [DefaultValue(typeof(MapEasyPageImage), "ToolTip")]
        public virtual MapEasyPageImage MapImage
        {
            get { return _mapImage; }
            set { _mapImage = value; }
        }

        /// <summary>
        /// Resets the MapImage property to its default value.
        /// </summary>
        public void ResetMapImage()
        {
            MapImage = MapEasyPageImage.ToolTip;
        }
        #endregion

        #region MapText
        /// <summary>
        /// Gets and sets the mapping used for the tooltip text.
        /// </summary>
        [Category("Visuals")]
        [Description("Mapping used for the tooltip text.")]
        [RefreshPropertiesAttribute(RefreshProperties.All)]
        [DefaultValue(typeof(MapEasyPageText), "ToolTipTitle")]
        public MapEasyPageText MapText
        {
            get { return _mapText; }
            set { _mapText = value; }
        }

        /// <summary>
        /// Resets the MapText property to its default value.
        /// </summary>
        public void ResetMapText()
        {
            MapText = MapEasyPageText.ToolTipTitle;
        }
        #endregion

        #region MapExtraText
        /// <summary>
        /// Gets and sets the mapping used for the tooltip description.
        /// </summary>
        [Category("Visuals")]
        [Description("Mapping used for the tooltip description.")]
        [RefreshPropertiesAttribute(RefreshProperties.All)]
        [DefaultValue(typeof(MapEasyPageText), "ToolTipBody")]
        public MapEasyPageText MapExtraText
        {
            get { return _mapExtraText; }
            set { _mapExtraText = value; }
        }

        /// <summary>
        /// Resets the MapExtraText property to its default value.
        /// </summary>
        public void ResetMapExtraText()
        {
            MapExtraText = MapEasyPageText.ToolTipBody;
        }
        #endregion
    }
}
